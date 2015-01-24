using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n9.core
{
    public class UnboundProgramModel
    {
        // ===========================================================================

        public Module PgmRoot = new Module();
        Module CurrentModule;

        N9Context ctx;
        
        // ===========================================================================

        public static UnboundProgramModel Generate(N9Context _ctx)
        {
            var model = new UnboundProgramModel { ctx = _ctx };

            foreach (var s in _ctx.SourceFiles)
                model.Analyze(s);

            return model;
        }

        void Analyze(Parser p)
        {
            CurrentModule = PgmRoot; // reset Current module to Root at the start of each source file.

            while (true)
            {
                var stmt = p.ParseStatement();
                if (stmt == null) 
                    return;

                AnalyzeToplevelStatement(p, stmt);
            }
        }

        void AnalyzeToplevelStatement(Parser p, Statement stmt)
        {
            if (stmt is ModuleStatement)
            {
                var s = stmt as ModuleStatement;
                CurrentModule = PgmRoot.FindOrCreateToModule(s.Module);
            }

            else if (stmt is VersionStatement)
            {
                var s = stmt as VersionStatement;
                bool satisfied = EvaluateVersionConditional(s.ConditionalExpr);

                if (satisfied)
                    foreach (var innerStmt in s.Body) // parse contents as top-level statements
                        AnalyzeToplevelStatement(p, innerStmt);

                // else we're just discarding the contents of statement.
            }

            else if (stmt is StructDeclaration)
            {
                var s = stmt as StructDeclaration;
                CurrentModule.RegisterSymbol(s.Name, s);
            }

            else if (stmt is VariableDeclaration)
            {
                var s = stmt as VariableDeclaration;
                CurrentModule.RegisterSymbol(s.Name, s);
            }

            else if (stmt is FuncDeclaration)
            {
                var s = stmt as FuncDeclaration;
                CurrentModule.RegisterSymbol(s.Name, s);
                // TODO should we check non-toplevel version statements?
            }

            else throw new Exception("Statement was not expected at top level.");
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
            throw new Exception("Syntax error.");
        }
    }
}