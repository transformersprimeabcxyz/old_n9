using System.Collections.Generic;
using System.Text;

namespace n9.core
{
    public abstract class Statement
    {
    }

    public class TypeDeclaration
    {
        public string Name;
        // TODO.. other stuff, arrays, pointers, other modifiers or annotations

        public override string ToString()
        {
            return Name;
        }

        public static TypeDeclaration Auto = new TypeDeclaration { Name = "(auto)" };
    }

    public class VariableDeclaration : Statement
    {
        public string Name;
        public TypeDeclaration Type;
        public Expression InitializationExpression;

        public override string ToString()
        {
            if (InitializationExpression == null)
                return Name + " : " + Type;
            return Name + " : " + Type + " = " + InitializationExpression;
        }
    }

    public class StructDeclaration : Statement
    {
        public string Name;
        public List<VariableDeclaration> Members = new List<VariableDeclaration>();

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
}