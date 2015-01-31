using System;
using System.Collections.Generic;

namespace n9.core
{
    public class ProgramModel
    {
        // ===========================================================================

        public Module Root = new Module();
        public List<StructDeclaration> StructDecls = new List<StructDeclaration>();
        public List<VariableDeclaration> GlobalVariables = new List<VariableDeclaration>();
        public List<FuncDeclaration> FuncDecls = new List<FuncDeclaration>();

        Module CurrentModule;

        // ===========================================================================

        public static ProgramModel Generate(N9Context ctx)
        {
            var model = new ProgramModel();
            BuiltinTypes.RegisterBuiltins(model.Root);

            foreach (var s in ctx.SourceFiles)
                model.Analyze(s);

            return model;
        }

        void Analyze(SourceFile s)
        {
            CurrentModule = Root; // reset Current module to Root at the start of each source file.

            foreach (var stmt in s.Statements)
                AnalyzeToplevelStatement(stmt);
        }

        void AnalyzeToplevelStatement(Statement stmt)
        {
            if (stmt is ModuleStatement)
            {
                var s = stmt as ModuleStatement;
                CurrentModule = Root.FindOrCreateToModule(s.Module);
            }

            else if (stmt is ImportStatement)
                return; // just ignore import statement at this phase

            else if (stmt is StructDeclaration)
            {
                var s = stmt as StructDeclaration;
                CurrentModule.RegisterSymbol(s.Name, s);
                StructDecls.Add(s);
            }

            else if (stmt is VariableDeclaration)
            {
                var s = stmt as VariableDeclaration;
                CurrentModule.RegisterSymbol(s.Name, s);
                GlobalVariables.Add(s);
            }

            else if (stmt is FuncDeclaration)
            {
                var s = stmt as FuncDeclaration;
                CurrentModule.RegisterSymbol(s.Name, s);
                FuncDecls.Add(s);
            }

            else throw new Exception("Statement was not expected at top level.");
        }
    }
}