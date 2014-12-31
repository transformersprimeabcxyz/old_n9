using n9.core;
using System;

namespace n9.cmd
{
	class Program
	{
		static void Main(string[] args)
		{
            var pgm = Parser.FromString(@"
struct hello 
{
    test : int; 
    froob : string;
    inferred := ;
}
").Parse();
            foreach (var stmt in pgm)
                Console.WriteLine(stmt);
            
            /*
            var fr = FileReader.FromString(@"1.0m");
            var lx = new Lexer(fr);
            while (true)
            {
                var t = lx.Next();
                Console.WriteLine(t);
                if (t.Type == TokenType.EOF) break;
            }
            */
		}
	}
}
