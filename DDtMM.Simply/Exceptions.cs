using DDtMM.SIMPLY.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public static class Exceptions
    {
        static public Exception InvalidSyntaxException(SyntaxNode node, string expected = null) 
        {
            string message;
            if (node.StartToken != null) 
            {
                message = string.Format("Invalid token \"{0}\" at position {1}.", node.StartToken.Text, node.StartToken.Position);
            }
            else 
            {
                message = string.Format("Unexpected Empty Token.");
            }

            if (!String.IsNullOrWhiteSpace(expected)) 
            {
                message += string.Format(" Expected rule {0}, but token matched rule {1}.", expected, node.Rule);
            }

            return new Exception(message);
        }

        static public Exception UnresolvedReferenceException(Referrer referrer)
        {
            return new Exception(string.Format("Unresolved reference for {0}, refer's to {1}",
                referrer.ProductionName ?? referrer, referrer.Symbol));
        }

    }
}
