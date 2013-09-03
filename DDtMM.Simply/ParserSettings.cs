using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class ParserSettings
    {
        public enum SelectionCriteria
        {
            First,
            Longest
        }

        /// <summary>
        /// What to accept when processing alternates.
        /// </summary>
        public SelectionCriteria AlternateCritera;

        /// <summary>
        /// The Productions that are processable at the root of the tree.  If empty all are considered ok.
        /// </summary>
        public List<string> RootProductionNames;

        /// <summary>
        /// If a zero length result is returned, do we accept it or keep processing further rules?
        /// </summary>
        public bool ZeroLengthRulesOK;
        public ParserSettings()
        {
            AlternateCritera = SelectionCriteria.First;
            RootProductionNames = new List<string>();
            ZeroLengthRulesOK = true;
        }
    }
}
