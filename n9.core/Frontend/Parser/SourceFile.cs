using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n9.core
{
    public class SourceFile
    {
        // =====================================================================

        public List<Statement> Statements = new List<Statement>();
        public List<string> Imports = new List<string>();
        N9Context ctx;

        // =====================================================================

        public static SourceFile FromString(N9Context ctx, string pgm, string filename="default.n9")
        {
            var parser = Parser.FromString(pgm, filename);
            return FromParser(ctx, parser);
        }

        public static SourceFile FromFile(N9Context ctx, string filename)
        {
            var parser = Parser.FromFile(filename);
            return FromParser(ctx, parser);
        }

        static SourceFile FromParser(N9Context ctx, Parser p)
        {
            var s = new SourceFile();
            s.ctx = ctx;

            while (true)
            {
                var stmt = p.ParseStatement();
                if (stmt == null)
                    break;
                s.Statements.Add(stmt);
            }
            return s;
        }

        public void Analyze()
        {
            var unprocessed = Statements;
            Statements = new List<Statement>();
            foreach (var stmt in unprocessed)
            {
                AnalyzeToplevelStatement(stmt);
            }
        }

        void AnalyzeToplevelStatement(Statement stmt)
        {
            if (stmt is ModuleStatement)
                Statements.Add(stmt);

            else if (stmt is StructDeclaration)
                Statements.Add(stmt);

            else if (stmt is VariableDeclaration)
                Statements.Add(stmt);

            else if (stmt is VersionStatement)
            {
                var s = stmt as VersionStatement;
                bool satisfied = EvaluateVersionConditional(s.ConditionalExpr);

                if (satisfied)
                    foreach (var innerStmt in s.Body) // parse contents as top-level statements
                        AnalyzeToplevelStatement(innerStmt);
                else
                    foreach (var innerStmt in s.ElseBody)
                        AnalyzeToplevelStatement(innerStmt);
            }

            else if (stmt is FuncDeclaration)
            {
                var s = stmt as FuncDeclaration;
                s.Body = AnalyzeInnerBody(s.Body);
                Statements.Add(stmt);
            }

            else throw new Exception("Statement was not expected at top level.");
        }
        
        // TODO, basically I'm not really thrilled with the way version statements are culled from the AST.
        // We do an entire pass through the AST doing only this, and we generate a lot of garbage doing it,
        // and the code feels like it could be more concise.
        // A possible resolution is figuring out how to do it in Parser.ParseStatementOrBlock()
        // But anyway, this works and for our immediate "research compiler" purposes we have bigger fish to fry.
        List<Statement> AnalyzeInnerBody(List<Statement> body)
        {
            var newBody = new List<Statement>();
            foreach (var stmt in body)
            {
                if (stmt is VersionStatement)
                {
                    var s = stmt as VersionStatement;
                    bool satisfied = EvaluateVersionConditional(s.ConditionalExpr);
                    if (satisfied)
                    {
                        var b = AnalyzeInnerBody(s.Body);
                        newBody.AddRange(b);
                    }
                    else
                    {
                        var b = AnalyzeInnerBody(s.ElseBody);
                        newBody.AddRange(b);
                    }
                }
                else if (stmt is WhileStatement)
                {
                    var s = stmt as WhileStatement;
                    s.Body = AnalyzeInnerBody(s.Body);
                    newBody.Add(s);
                }
                else if (stmt is IfStatement)
                {
                    var s = stmt as IfStatement;
                    s.ThenBody = AnalyzeInnerBody(s.ThenBody);
                    s.ElseBody = AnalyzeInnerBody(s.ElseBody);
                    newBody.Add(s);
                }
                else
                    newBody.Add(stmt);
            }
            return newBody;
        }
        
        bool EvaluateVersionConditional(Expression expr)
        {
            if (expr is NameExpr)
            {
                var e = expr as NameExpr;
                return ctx.VersionTags.Contains(e.Name);
            }

            if (expr is BinaryOperatorExpr)
            {
                var e = expr as BinaryOperatorExpr;
                bool left = EvaluateVersionConditional(e.Left);
                bool right = EvaluateVersionConditional(e.Right);
                if (e.Op == TokenType.LogicalAnd)
                    return left && right;
                if (e.Op == TokenType.LogicalOr)
                    return left || right;
                throw new Exception("Unsupported operator in version condition");
            }

            if (expr is UnaryOperatorExpr)
            {
                var e = expr as UnaryOperatorExpr;
                bool right = EvaluateVersionConditional(e.Right);
                if (e.Op == TokenType.Bang)
                    return !right;
                throw new Exception("Unsupported operator in version condition");
            }

            throw new Exception("Syntax error.");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var stmt in Statements)
                stmt.Print(sb, 0);
            return sb.ToString();
        }
    }
}