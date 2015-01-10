using n9.core;
using System;

namespace n9.cmd
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.WriteLine(Parser.FromString("2+5*3").ParseExpression());
            Console.WriteLine(Parser.FromString("2*5+3").ParseExpression());

            Console.WriteLine(Parser.FromString("5+3*2").ParseExpression());
            Console.WriteLine(Parser.FromString("(5+-3)*2").ParseExpression());

            Console.WriteLine(Parser.FromString("1/2.").ParseExpression());
            Console.WriteLine(Parser.FromString("a()()").ParseExpression());

            Console.WriteLine(Parser.FromString("i : int;").ParseStatement());
            Console.WriteLine(Parser.FromString("x : string;").ParseStatement());
            Console.WriteLine(Parser.FromString("x : int = 2;").ParseStatement());
            Console.WriteLine(Parser.FromString("x : int = 2/1;").ParseStatement());
            Console.WriteLine(Parser.FromString("x := 2;").ParseStatement());

            Console.WriteLine(Parser.FromString(@"struct test {}").ParseStatement());
            Console.WriteLine(Parser.FromString(@"struct test { i:=5; }").ParseStatement());
            Console.WriteLine(Parser.FromString(@"struct test { i:int=5; }").ParseStatement());
            Console.WriteLine(Parser.FromString(@"struct test { i:int=5; f:=1.0f; test:=""Hello""; }").ParseStatement());

            Console.WriteLine(Parser.FromString(@"func a() { a:=5; b:=""hello""; } ").ParseStatement());

            Console.WriteLine(Parser.FromString(@"return a+b;").ParseStatement());

            Console.WriteLine(Parser.FromString(@"i = 0;").ParseStatement());

            Console.WriteLine(Parser.FromString(@"while(true) a();").ParseStatement());
            Console.WriteLine(Parser.FromString(@"while(true) { a(); b = 1+2; }").ParseStatement());

            Console.WriteLine(Parser.FromString(@"i == 1 + x >= 2").ParseExpression());
            Console.WriteLine(Parser.FromString(@"i == 1 && x >= 2").ParseExpression());
		}
	}
}
