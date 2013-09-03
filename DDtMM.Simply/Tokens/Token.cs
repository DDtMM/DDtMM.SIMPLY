using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Tokens
{
    public class Token
    {
        public TokenType TokenType { get; set; }

        /// <summary>
        /// Content of Token.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The token after this
        /// </summary>
        public Token Next {
            get { return _next; }
            internal set
            {
                _next = value;
                if (_next != null)
                {
                    _next.Index = this.Index + 1;
                }
            }
        }
        private Token _next;

        /// <summary>
        /// Charater position in source document
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Index of list of tokens
        /// </summary>
        public int Index { get; private set; }

        public Token(TokenType tokenType, string text, int position)
        {
            TokenType = tokenType;
            Text = text;
            Position = position;
            Index = -1;
            Next = null;
            
        }

        public override string ToString()
        {
            return String.Format("(Token) {0}: \"{1}\"", TokenType.Name, Text.Replace("\"", "\\\""));
        }
    }
}
