using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class InvalidTokenException : Exception
    {
        static public InvalidTokenException Create(SyntaxNode node, string expected = null) 
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

            return new InvalidTokenException(message);
        }

        private InvalidTokenException() : base() { }

        private InvalidTokenException(string message) : base(message) { }

    }
}
