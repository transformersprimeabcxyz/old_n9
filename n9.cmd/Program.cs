using n9.core;
using System;

namespace n9.cmd
{
	class Program
	{
		static void Main(string[] args)
		{
            var binder = Binder.FromString(@"

            func sum_to_int(max:int) : int
            {
                i := 1;
                defer quit();
                sum := 0;
                while (i < max)
                {
                    sum = sum + i;
                    i = i + 1;
                }
                return sum;
            }

            "); binder.Bind();
            Console.WriteLine(binder.Funcs[0]);

/*            Console.Write(Parser.FromString("x : string;").ParseStatement());
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
