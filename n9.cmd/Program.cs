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

            var expr = Parser.FromString("2(foo)").ParseExpression();
            Console.WriteLine("---");

		}
	}
}
