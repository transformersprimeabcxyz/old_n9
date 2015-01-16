using System;
using System.IO;

namespace n9.core
{
    public class CGen
    {
        // =====================================================================
        //  CGen state
        // =====================================================================

        Binder binder;

        // =====================================================================
        //  Constructor
        // =====================================================================

        public CGen(Binder b) // TODO refactoring related to data flow between steps.
        {
            binder = b;
        }

        // =====================================================================
        //  Code generator
        // =====================================================================

        public void Generate()
        {
            using (TextWriter w = File.CreateText("n9main.c"))
            {
                w.WriteLine("int printf(const char* format, ...);\n");
                codegen(w);
                
                w.WriteLine("int main(int argc, char** argv)");
                w.WriteLine("{");
                w.WriteLine("    int result = n9main();");
                w.WriteLine("    printf(\"%d\\n\",result);");
                w.WriteLine("}");
            }
        }

        public void Compile()
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "tcc.exe";
            proc.StartInfo.Arguments = "n9main.c"; 
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            proc.WaitForExit();
        }

        public int Run()
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "n9main.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            return Int32.Parse(output.Trim());
        }

        void codegen(TextWriter w)
        {
            w.WriteLine("// forward-delcare function prototypes first, so that we can output bodies to C in any order");
            foreach (FuncDeclaration func in binder.Funcs)
                generatePrototype(func, w);

            w.WriteLine();
            w.WriteLine("// declare function bodies");
            foreach(FuncDeclaration func in binder.Funcs)
                codegenFunc(func, w);
        }

        void generatePrototype(FuncDeclaration func, TextWriter w)
        {
            string returnType = "void";
            if (func.ReturnType != null)
                returnType = func.ReturnType.Name;
            w.Write("{0} {1}(", returnType, func.Name);

            bool first = true;
            foreach (var arg in func.Parameters)
            {
                if (!first)
                    w.Write(", ");
                w.Write(arg.Type.Name);
                first = false;
            }
            w.WriteLine(");");
        }

        void codegenFunc(FuncDeclaration func, TextWriter w)
        {
            string returnType = "void";
            if (func.ReturnType != null)
                returnType = func.ReturnType.Name;
            w.Write("{0} {1}(", returnType, func.Name);

            bool first = true;
            foreach (var arg in func.Parameters)
            {
                if (!first)
                    w.Write(", ");
                w.Write("{0} {1}", arg.Type.Name, arg.Name);
                first = false;
            }
            w.WriteLine(")");
            w.WriteLine("{");

            foreach (var stmt in func.Body)
            {
                codegenStatement(stmt, w);
            }

            w.WriteLine("}\n");
        }

        void codegenStatement(Statement s, TextWriter w)
        {
            if (s is ReturnStatement)
            {
                var stmt = s as ReturnStatement;
                w.Write("return ");
                codegenExpression(stmt.Expr, w);
                w.WriteLine(";");
            }

            else if (s is VariableDeclaration)
            {
                var stmt = s as VariableDeclaration;
                w.Write("{0} {1}", stmt.Type.Name, stmt.Name);
                if (stmt.InitializationExpression != null)
                {
                    w.Write(" = ");
                    codegenExpression(stmt.InitializationExpression, w);
                }
                w.WriteLine(";");
            }

            else if (s is AssignStatement)
            {
                var stmt = s as AssignStatement;
                codegenExpression(stmt.AssignExpr, w);
                w.WriteLine(";");
            }

            else if (s is CallStatement)
            {
                var stmt = s as CallStatement;
                codegenExpression(stmt.CallExpr, w);
                w.WriteLine(";");
            }

            else if (s is IfStatement)
            {
                var stmt = s as IfStatement;
                w.Write("if (");
                codegenExpression(stmt.IfExpr, w);
                w.WriteLine(") {");
                foreach (var _stmt in stmt.ThenBody)
                    codegenStatement(_stmt, w);
                w.Write("}");
                if (stmt.ElseBody.Count > 0)
                {
                    w.WriteLine(" else { ");
                    foreach (var _stmt in stmt.ElseBody)
                        codegenStatement(_stmt, w);
                    w.Write("}");
                }
                w.WriteLine();
            }
        }

        void codegenExpression(Expression e, TextWriter w)
        {
            if (e is IntLiteralExpr)
            {
                var ex = e as IntLiteralExpr;
                w.Write(ex.Literal.IntegerLiteral);
            }

            else if (e is NameExpr)
            {
                var ex = e as NameExpr;
                w.Write(ex.Name);
            }

            else if (e is AssignExpr)
            {
                var ex = e as AssignExpr;
                codegenExpression(ex.Left, w);
                w.Write(" = ");
                codegenExpression(ex.Right, w);
            }

            else if (e is BinaryOperatorExpr)
            {
                var ex = e as BinaryOperatorExpr;
                w.Write("(");
                codegenExpression(ex.Left, w);
                switch (ex.Op)
                {
                    case TokenType.Plus:            w.Write("+"); break;
                    case TokenType.Minus:           w.Write("-"); break;
                    case TokenType.Asterisk:        w.Write("*"); break;
                    case TokenType.Divslash:        w.Write("/"); break;

                    case TokenType.Equality:        w.Write(" == "); break;
                    case TokenType.Inequality:      w.Write(" != "); break;
                    case TokenType.LessThan:        w.Write(" < "); break;
                    case TokenType.LessThanEqual:   w.Write(" <= "); break;
                    case TokenType.GreaterThan:     w.Write(" > "); break;
                    case TokenType.GreaterThanEqual:w.Write(" >= "); break;
                    case TokenType.LogicalAnd:      w.Write(" && "); break;
                    case TokenType.LogicalOr:       w.Write(" || "); break;
                }
                codegenExpression(ex.Right, w);
                w.Write(")");
            }

            else if (e is CallExpr)
            {
                var ex = e as CallExpr;
                codegenExpression(ex.Function, w);
                w.Write("(");

                bool first = true;
                foreach (var arg in ex.Args)
                {
                    if (!first) 
                        w.Write(", ");
                    codegenExpression(arg, w);
                    first = false;
                }
                w.Write(")");
            }
        }
    }
}