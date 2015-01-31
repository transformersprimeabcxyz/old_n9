using System.Collections.Generic;

namespace n9.core
{
    public class oldBinder
    {
        // =====================================================================
        //  Binder state
        // =====================================================================

        Parser parser;
        public List<FuncDeclaration> Funcs = new List<FuncDeclaration>();

        // =====================================================================
        //  Constructor 
        // =====================================================================

        public static oldBinder FromParser(Parser p)
        {
            return new oldBinder { parser = p };
        }

        public static oldBinder FromFile(string filename)
        {
            return new oldBinder { parser = Parser.FromFile(filename) };
        }

        public static oldBinder FromString(string pgm, string filename = "default.n9")
        {
            return new oldBinder { parser = Parser.FromString(pgm, filename) };
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
