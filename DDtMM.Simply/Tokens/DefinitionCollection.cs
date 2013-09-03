using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Tokens
{
    public class DefinitionCollection : List<TokenType>
    {
        static private readonly Regex definitionRegex = new Regex(@"((?:\\\\|\\;|\\|[^;\\])+);");
        static private readonly Regex findReferencesRegex = new Regex(@"\{([a-z]\w*)\}", RegexOptions.IgnoreCase);

        static public DefinitionCollection Parse(string text)
        {
            DefinitionCollection col = new DefinitionCollection();

            text = Common.StripComments(text);
            foreach (Match match in definitionRegex.Matches(text))
            {
                TokenType def = TokenType.Parse(match.Groups[1].Value);
                TokenType existingDef = col.FirstOrDefault(d => d.Name == def.Name);
                if (existingDef == null)
                {
                    col.Add(def);
                }
                else
                {
                    existingDef.Regex += "|" + def.Regex;
                }
            }

            return col;
        }

        /// <summary>
        /// Gets TokenType by name
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TokenType this[string key] 
        { 
            get { return customKeys.GetValue(key); } 
        }

        public DefinitionsRegex RegexInfo;
        private ListCustomKeys<string, TokenType> customKeys;

        public DefinitionCollection()
        {
            customKeys = new ListCustomKeys<string,TokenType>(this, delegate(TokenType tt) {
                return tt.Name;
            });
        }


        /// <summary>
        /// Runs CompileToken on all regexes, causing all instances {variblename} to be replaced
        /// if a matching variable is found.
        /// </summary>
        /// <param name="replacementSource">
        /// Definition collection that has replacement values
        /// If replacementSource is null, then we use the values already added to compiled
        /// to update future values.  
        /// </param>
        /// <returns></returns>
        public DefinitionCollection Compile(RegexOptions regexOptions, DefinitionCollection replacementSource = null)
        {
            DefinitionCollection compiled = new DefinitionCollection();

            // self replacement, sort by dependencies
            if (replacementSource == null)
            {
                // add to dependencies all regexes that have {a\w} where a\w exists in this.
                // those without dependencies will be added as empty items.
                Dictionary<TokenType, List<string>> dependencies = this.ToDictionary(t => t, t =>
                    findReferencesRegex.Matches(t.Regex).OfType<Match>()
                        .Select(m => m.Groups[1].Value)
                        .Where(k => this.Exists(t2 => t2.Name == k)).ToList());

                Dictionary<TokenType, List<string>> resolvedDependencies = new Dictionary<TokenType, List<string>>();

                /*
                 * ResolvedDependencies are added in order of which they can be replaced.
                 * If nothing has been resolved but some still remain then there is a recursive relationship
                 */
                int startingCount;
                do
                {
                    startingCount = dependencies.Count;
                    IEnumerable<TokenType> depedencyKeys = dependencies.Keys.ToList();
                    foreach (TokenType t in depedencyKeys)
                    {
                        // if no instances exists where a dependency does not exist
                        //if (!dependencies[t].Exists(
                        //    d => !replacementSource.Exists(r => r.Name == d)))
                        if (!dependencies[t].Exists(
                            d => resolvedDependencies.Keys.FirstOrDefault(r => r.Name == d) == null )) 
                        {
                            resolvedDependencies.Add(t, dependencies[t]);
                            dependencies.Remove(t);
                        }
                    }
                } while (dependencies.Count > 0 && dependencies.Count != startingCount);

                if (dependencies.Count > 0) throw new Exception("Recursive dependency error");

  
                foreach (var dependencyInfo in resolvedDependencies)
                {
                    compiled.Add(CompileToken(dependencyInfo.Key, compiled, dependencyInfo.Value));
                }
            }
            else
            {
                foreach (TokenType token in this)
                {
                    IEnumerable<string> dependencies =
                        findReferencesRegex.Matches(token.Regex).OfType<Match>()
                        .Select(m => m.Groups[1].Value)
                        .Where(k => replacementSource.Exists(t2 => t2.Name == k));

                    compiled.Add(CompileToken(token, replacementSource, dependencies));
                }
                compiled = compiled.Compile(regexOptions);
            }
            compiled.RegexInfo = new DefinitionsRegex(compiled, regexOptions);
            return compiled;
        }

        /// <summary>
        /// Updates regexes in collection from the source where the key of the source 
        /// is present in the regex and between braces.
        /// </summary>
        /// <returns></returns>
        private TokenType CompileToken(TokenType token,
            DefinitionCollection replacementSource, IEnumerable<string> dependencies)
        {
            string regex = token.Regex;
            foreach (string replacementKey in dependencies)
            {
                regex = regex.Replace(string.Format("{{{0}}}", replacementKey),
                    string.Format("(?:{0})", replacementSource[replacementKey].Regex));
            }
            return new TokenType(token.Name, regex, token.IsIgnored);
        }

        public bool ContainsKey(string key) 
        {
            return customKeys.ContainsKey(key);
        }

        public int IndexOfKey(string key)
        {
            return customKeys.GetIndex(key);
        }

    }
}
