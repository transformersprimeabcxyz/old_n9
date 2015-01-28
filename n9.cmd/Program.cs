using n9.core;
using System;

namespace n9.cmd
{
	class Program
	{
		static void Main(string[] args)
		{
            string pgm;

            pgm = @"

            module module1;

            version(a && b)
                a_and_b:int;
            else version(a)
                a:int;
            else version(b)
                b:int;

            func test()
            {
                version(a)
                    i:int;
                else 
                    x:int;

                while (foo())
                {
                    version (a)
                        i = i + 1;
                    else
                        x = x + 1;
                }

                j:int;
            }
            ";

            var ctx = N9Context.FromString(pgm).Tags("a","b").Construct();
            Console.WriteLine(ctx.SourceFiles[0]);

            return;


            pgm = @"

            func n9main() : int
            {
                sum: int = 0;
                loopctr: int = 0;
                while (loopctr < 10)
                {
                    sum = sum + square(loopctr);
                    loopctr = loopctr + 1;
                }
                return sum;
            }

            func square(i:int) : int
            {
                return i*i;
            }

            ";

            ctx = N9Context.FromString(pgm).Construct();

            var binder = Binder.FromString(pgm); binder.Bind();

            Console.WriteLine(binder.Funcs[0]);

            var cgen = new CGen(binder);
            cgen.Generate();
            cgen.Compile();
            int res = cgen.Run();
            Console.WriteLine(res);

/*         
            Console.Write(Parser.FromString("x : string;").ParseStatement());
            Console.Write(Parser.FromString("x : int = 2;").ParseStatement());
            Console.Write(Parser.FromString("x : int = 2/1;").ParseStatement());
            Console.Write(Parser.FromString("x := 2;").ParseStatement());

            Console.Write(Parser.FromString(@"struct test {}").ParseStatement());
            Console.Write(Parser.FromString(@"struct test { i:=5; }").ParseStatement());
            Console.Write(Parser.FromString(@"struct test { i:int=5; }").ParseStatement());
            Console.Write(Parser.FromString(@"struct test { i:int=5; f:=1.0f; test:=""Hello""; }").ParseStatement());

            Console.Write(Parser.FromString(@"func a() : int { a:=5; b:=""hello""; } ").ParseStatement());

            Console.Write(Parser.FromString(@"return a+b;").ParseStatement());

            Console.WriteLine(Parser.FromString(@"i = 0;").ParseStatement());

            Console.WriteLine(Parser.FromString(@"while(true) a();").ParseStatement());
            Console.WriteLine(Parser.FromString(@"while(true) { a(); b = 1+2; }").ParseStatement());
            Console.WriteLine(Parser.FromString(@"while(true) {} ").ParseStatement());

            var s = Parser.FromString(@"if (i==1) foo();").ParseStatement();
            s = Parser.FromString(@"if (i==1) { foo(); i = i + 1; }").ParseStatement();
            s = Parser.FromString(@"if (i==1) { foo(); i = i + 1; } else bar();").ParseStatement();
            s = Parser.FromString(@"if (i==1) { foo(); i = i + 1; } else { bar(); }").ParseStatement();

            Console.WriteLine(Parser.FromString(@"i == 1 + x >= 2").ParseExpression());
            Console.WriteLine(Parser.FromString(@"i == 1 && x >= 2").ParseExpression());

            Console.Write(Parser.FromString(@"

            func a(a:bool*, b:bool[0xFF]) : int[]
            { 
                if (a)
                    foo();
                else if (b)
                    bar();
                else 
                    while (i < count)
                        foo(i);

                init();
                b:=""hello""; 
                return 5/2;
            } 

            ").ParseStatement());*/
		}
	}
}
