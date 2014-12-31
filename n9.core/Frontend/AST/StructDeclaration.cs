using System.Collections.Generic;
using System.Text;

namespace n9.core
{
    public class StructDeclaration : Statement
    {
        public string Name;
        public List<StructMember> Members = new List<StructMember>();

        // ===================================================

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("struct ");
            sb.Append(Name);
            sb.Append("\n");
            sb.Append("{\n");
            foreach (var member in Members)
            {
                sb.Append("    ");
                sb.Append(member.ToString());
                sb.Append("\n");
            }

            sb.Append("}\n");
            return sb.ToString();
        }
    }

    public class StructMember
    {
        public string MemberName;
        public string Type; // TODO... created like an.. UnboundType object
        public bool Pointer;
        public bool Array;
        public bool Owned;
        public Expression InitializationExpression;

        // ===================================================

        public override string ToString()
        {
            return MemberName + " : " + Type;
        }
    }
}