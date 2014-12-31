using System.Collections.Generic;
using System.Text;

namespace n9.core
{
    public class VariableDeclaration : Statement
    {
        public string Name;
        public string Type; // TODO... created like an.. UnboundType object
        public bool Pointer;
        public bool Array;
        public bool Owned;
        public Expression InitializationExpression;

        // ===================================================

        public override string ToString()
        {
            return Name + " : " + Type + ";";
        }
    }
}