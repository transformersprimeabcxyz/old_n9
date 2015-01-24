using System.Collections.Generic;
using System.Text;

namespace n9.core
{
    public abstract class Statement
    {
        public abstract void Print(StringBuilder buffer, int indentLevel = 0);
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            Print(sb);
            return sb.ToString();
        }
    }

    static class BufferExt
    {
        public static void Append(this StringBuilder buffer, string str, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++)
                buffer.Append("   ");
            buffer.Append(str);
        }

        public static void Append(this StringBuilder buffer, object obj, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++)
                buffer.Append("   ");
            buffer.Append(obj.ToString());
        }
    }
    
    public class TypeDeclaration
    {
        public string Name;
        public bool UnsizedArray;
        public bool SizedArray;
        public bool Pointer;
        public int ArraySize;

        // TODO.. pointer-to-pointers, non-literal-sized arrays, other modifiers or annotations

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            if (UnsizedArray) 
                sb.Append("[]");
            if (SizedArray)
            {
                sb.Append("[");
                sb.Append(ArraySize);
                sb.Append("]");
            }
            if (Pointer)
                sb.Append("*");
            return sb.ToString();
        }

        public static TypeDeclaration Auto = new TypeDeclaration { Name = "(auto)" };
    }

    public class VariableDeclaration : Statement
    {
        public string Name;
        public TypeDeclaration Type;
        public Expression InitializationExpression;

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append(Name, indentLevel);
            if (Type.Name == "(auto)")
            {
                buffer.Append(" := ");
                buffer.Append(InitializationExpression);
                buffer.Append(";\n");
            } else {
                buffer.Append(" : ");
                buffer.Append(Type);
                if (InitializationExpression != null) 
                { 
                    buffer.Append(" = ");
                    buffer.Append(InitializationExpression);
                }
                buffer.Append(";\n");
            }
        }
    }

    public class MethodParameterDeclaration : Statement
    {
        public string Name;
        public TypeDeclaration Type;

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append(Name);
            buffer.Append(": ");
            buffer.Append(Type);
        }
    }

    public class StructDeclaration : Statement
    {
        public string Name;
        public List<VariableDeclaration> Members = new List<VariableDeclaration>();

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("struct ", indentLevel);
            buffer.Append(Name);
            buffer.Append("\n");
            buffer.Append("{\n", indentLevel); 
            foreach (var member in Members)
            {
                buffer.Append(member, indentLevel+1);
            }
            buffer.Append("}\n", indentLevel);
        }
    }

    public class FuncDeclaration : Statement
    {
        public string Name;
        public List<MethodParameterDeclaration> Parameters = new List<MethodParameterDeclaration>();
        public TypeDeclaration ReturnType;
        public List<Statement> Body = new List<Statement>();

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("func ", indentLevel);
            buffer.Append(Name);

            buffer.Append("(");
            bool first = true;
            foreach (var p in Parameters)
            {
                if (!first)
                    buffer.Append(", ");
                buffer.Append(p);
                first = false;
            }

            buffer.Append(")");
            if (ReturnType != null)
            {
                buffer.Append(" : ");
                buffer.Append(ReturnType);
            }
            buffer.Append("\n");
            buffer.Append("{\n", indentLevel);
            foreach (var stmt in Body)
                stmt.Print(buffer, indentLevel + 1);
            buffer.Append("}\n", indentLevel);
        }
    }

    public class ReturnStatement : Statement
    {
        public Expression Expr;

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("return ", indentLevel);
            buffer.Append(Expr);
            buffer.Append(";\n");
        }
    }

    public class DeferStatement : Statement
    {
        public List<Statement> Body = new List<Statement>();

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("defer ", indentLevel);
            if (Body.Count == 1) {
                Body[0].Print(buffer);
            } else {
                buffer.Append("\n");
                buffer.Append("{\n", indentLevel);
                foreach (var stmt in Body)
                    stmt.Print(buffer, indentLevel+1);
                buffer.Append("}\n", indentLevel);
            }
        }
    }

    public class AssignStatement : Statement
    {
        public AssignExpr AssignExpr;

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append(AssignExpr, indentLevel);
            buffer.Append(";\n");
        }
    }

    public class CallStatement : Statement
    {
        public CallExpr CallExpr;

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append(CallExpr, indentLevel);
            buffer.Append(";\n");
        }
    }

    public class IfStatement : Statement
    {
        public Expression IfExpr;
        public List<Statement> ThenBody = new List<Statement>();
        public List<Statement> ElseBody = new List<Statement>();

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("if (", indentLevel);
            buffer.Append(IfExpr);
            buffer.Append(")\n");
            buffer.Append("{\n", indentLevel);

            foreach (var stmt in ThenBody)
                stmt.Print(buffer, indentLevel + 1);
            buffer.Append("}", indentLevel);
            if (ElseBody.Count > 0)
            {
                buffer.Append(" else {\n");
                foreach (var stmt in ElseBody)
                    stmt.Print(buffer, indentLevel + 1);
                buffer.Append("}", indentLevel);
            }
            buffer.Append("\n");
        }
    }

    public class WhileStatement : Statement
    {
        public Expression ConditionalExpr;
        public List<Statement> Body = new List<Statement>();

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("while (", indentLevel);
            buffer.Append(ConditionalExpr);
            buffer.Append(")\n");
            buffer.Append("{\n", indentLevel);

            foreach (var stmt in Body)
                stmt.Print(buffer, indentLevel + 1);

            buffer.Append("}\n", indentLevel);
        }
    }

    public class PragmaStatement : Statement
    {
        public List<Token> Body = new List<Token>();

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("pragma ", indentLevel);
            bool first = true;
            foreach (var token in Body)
            {
                if (!first)
                    buffer.Append(" ");
                buffer.Append(token.Text);
                first = false;
            }
            buffer.Append(";\n");
        }
    }

    public class VersionStatement : Statement
    {
        public Expression ConditionalExpr;
        public List<Statement> Body = new List<Statement>();
        public List<Statement> ElseBody = new List<Statement>();

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("version (", indentLevel);
            buffer.Append(ConditionalExpr);
            buffer.Append(")\n");
            buffer.Append("{\n", indentLevel);

            foreach (var stmt in Body)
                stmt.Print(buffer, indentLevel + 1);
            
            buffer.Append("}\n", indentLevel);

            if (ElseBody.Count > 0)
            {
                buffer.Append("else\n", indentLevel);
                buffer.Append("{\n", indentLevel);
                foreach (var stmt in ElseBody)
                    stmt.Print(buffer, indentLevel + 1);
                buffer.Append("}\n", indentLevel);
            }
        }
    }

    public class ModuleStatement : Statement
    {
        public string Module;

        public override void Print(StringBuilder buffer, int indentLevel = 0)
        {
            buffer.Append("module ", indentLevel);
            buffer.Append(Module);
            buffer.Append(";\n");
        }
    }
}