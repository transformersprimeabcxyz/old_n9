using System.Collections.Generic;

namespace n9.core
{
    public class Binder
    {
        // =====================================================================
        //  Binder state
        // =====================================================================

        Parser parser;
        Scope GlobalScope = new Scope();
        public List<FuncDeclaration> Funcs = new List<FuncDeclaration>();

        // =====================================================================
        //  Constructor 
        // =====================================================================

        public static Binder FromParser(Parser p)
        {
            return new Binder { parser = p };
        }

        public static Binder FromFile(string filename)
        {
            return new Binder { parser = Parser.FromFile(filename) };
        }

        public static Binder FromString(string pgm, string filename = "default.n9")
        {
            return new Binder { parser = Parser.FromString(pgm, filename) };
        }

        // =====================================================================
        //  Binder
        // =====================================================================

        public void Bind()
        {
            while (true)
            {
                var stmt = parser.ParseStatement();
                if (stmt == null)
                    break;

                if (stmt is FuncDeclaration)
                    Funcs.Add(stmt as FuncDeclaration);
            }
        }

    }
}
