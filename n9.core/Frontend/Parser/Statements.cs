using System.Collections.Generic;
using System.Text;

namespace n9.core
{
    public abstract class Statement
    {
        //public abstract void Print(StringBuilder buffer, int indentLevel = 0);
        //public override string ToString()
        //{
        //    var sb = new StringBuilder();
        //    Print(sb);
        //    return sb.ToString();
        //}
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
                return Name + ": " + Type;
            return Name + ": " + Type + " = " + InitializationExpression;
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
                sb.Append(member);
                sb.Append("\n");
            }

            sb.Append("}\n");
            return sb.ToString();
        }
    }

    public class FuncDeclaration : Statement
    {
        public string Name;
        public List<VariableDeclaration> Parameters = new List<VariableDeclaration>();
        public TypeDeclaration ReturnType;
        public List<Statement> Body = new List<Statement>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("func ");
            sb.Append(Name);
            
            sb.Append("(");
            bool first = true;
            foreach (var p in Parameters)
            {
                if (!first) 
                    sb.Append(", ");
                sb.Append(p);
                first = false;
            }

            sb.Append(")");
            if (ReturnType != null)
            {
                sb.Append(" : ");
                sb.Append(ReturnType);
                sb.Append("\n");
            }

            sb.Append("{\n");
            foreach (var stmt in Body)
            {
                sb.Append("    ");
                sb.Append(stmt);
                sb.Append("\n");
            }
            sb.Append("}\n");
            return sb.ToString();
        }
    }

    public class ReturnStatement : Statement
    {
        public Expression Expr;
        
        public override string ToString()
        {
            return "return " + Expr + ";";
        }
    }

    public class AssignStatement : Statement
    {
        public AssignExpr AssignExpr;

        public override string ToString()
        {
            return AssignExpr + ";";
        }
    }

    public class CallStatement : Statement
    {
        public CallExpr CallExpr;

        public override string ToString()
        {
            return CallExpr + ";";
        }
    }

    public class IfStatement : Statement
    {
        public Expression IfExpr;
        public List<Statement> ThenBody = new List<Statement>();
        public List<Statement> ElseBody = new List<Statement>();
    }

    public class WhileStatement : Statement
    {
        public Expression ConditionalExpr;
        public List<Statement> Body = new List<Statement>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("while (");
            sb.Append(ConditionalExpr);
            sb.Append(")\n");
            sb.Append("{\n");

            foreach (var stmt in Body)
            {
                sb.Append("    ");
                sb.Append(stmt);
                sb.Append("\n");
            }

            sb.Append("}\n");
            return sb.ToString();
        }
    }
}