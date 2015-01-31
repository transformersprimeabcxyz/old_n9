using System;

namespace n9.core
{
    public class UnboundModel
    {
        // ===========================================================================

        public Module PgmRoot = new Module();
        Module CurrentModule;

        // ===========================================================================

        public static UnboundModel Generate(N9Context ctx)
        {
            var model = new UnboundModel();

            foreach (var s in ctx.SourceFiles)
                model.Analyze(s);

            return model;
        }

        void Analyze(SourceFile s)
        {
            CurrentModule = PgmRoot; // reset Current module to Root at the start of each source file.

            foreach (var stmt in s.Statements)
                AnalyzeToplevelStatement(stmt);
        }

        void AnalyzeToplevelStatement(Statement stmt)
        {
            if (stmt is ModuleStatement)
            {
                var s = stmt as ModuleStatement;
                CurrentModule = PgmRoot.FindOrCreateToModule(s.Module);
            }

            else if (stmt is ImportStatement)
                return; // just ignore import statement at this phase

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
            }

            else throw new Exception("Statement was not expected at top level.");
        }
    }
}