using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class ParserTests
	{
        [TestMethod]
        public void Parser_ExpressionsBasic()
        {
            ExprParseTest("2", "2");
            ExprParseTest("a", "a");

            ExprParseTest("5+2", "(5 Plus 2)");
            ExprParseTest("(5+2)", "(5 Plus 2)");
            ExprParseTest("2+5*3", "(2 Plus (5 Asterisk 3))");
            ExprParseTest("2*5+3", "((2 Asterisk 5) Plus 3)");

            ExprParseTest("2+5+3+7", "(((2 Plus 5) Plus 3) Plus 7)");
            ExprParseTest("2+(5+3)+7", "((2 Plus (5 Plus 3)) Plus 7)");

            ExprParseTest("-2", "-2");
            ExprParseTest("5+-2", "(5 Plus -2)");

            ExprParseTest("foo()", "foo()");
            ExprParseTest("foo(bar)", "foo(bar)");
            ExprParseTest("foo(bar,1,1+2,(a/b))", "foo(bar,1,(1 Plus 2),(a Divslash b))");
            ExprParseTest("a()()", "a()()");
        }

        [TestMethod]
        public void Parser_VariableDeclarations()
        {
            var decl = Parser.FromString("i := 5;").ParseStatement() as VariableDeclaration;
            Assert.IsTrue(decl is VariableDeclaration);
            Assert.IsTrue(decl.Name == "i");
            Assert.IsTrue(decl.Type.Name == "(auto)");
            Assert.IsTrue(decl.InitializationExpression is IntLiteralExpr);
            Assert.IsTrue((decl.InitializationExpression as IntLiteralExpr).Literal.IntegerLiteral == 5);

            decl = Parser.FromString("i : int;").ParseStatement() as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.InitializationExpression == null);

            decl = Parser.FromString("i : int = a*3;").ParseStatement() as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.InitializationExpression is BinaryOperatorExpr);

            AssertException(() => Parser.FromString("i : int").ParseStatement()); // Missing terminating semicolon
            AssertException(() => Parser.FromString("i := ;").ParseStatement()); // Missing initialization expression in inference syntax
            AssertException(() => Parser.FromString("i : ;").ParseStatement()); // missing type specifier

            StmtParseTest("i : int;", "i: int");
            StmtParseTest("i : int = 5;", "i: int = 5");
            StmtParseTest("i := 5;", "i: (auto) = 5");
            StmtParseTest("i := (2*5);", "i: (auto) = (2 Asterisk 5)");
            StmtParseTest("i : int = (2*5);", "i: int = (2 Asterisk 5)");
        }

        [TestMethod]
        public void Parser_StructDeclarations()
        {
            var decl = Parser.FromString("struct foo { }").ParseStatement() as StructDeclaration;
            Assert.IsTrue(decl is StructDeclaration);
            Assert.IsTrue(decl.Name == "foo");
            Assert.IsTrue(decl.Members.Count == 0);

            decl = Parser.FromString("struct foo { i : int; }").ParseStatement() as StructDeclaration;
            Assert.IsTrue(decl.Members.Count == 1);
            Assert.IsTrue(decl.Members[0].Name == "i");
            Assert.IsTrue(decl.Members[0].Type.Name == "int");
            Assert.IsTrue(decl.Members[0].InitializationExpression == null);

            decl = Parser.FromString("struct foo { i := 5; }").ParseStatement() as StructDeclaration;
            Assert.IsTrue(decl.Members.Count == 1);
            Assert.IsTrue(decl.Members[0].Type.Name == "(auto)");
            Assert.IsTrue(decl.Members[0].InitializationExpression is IntLiteralExpr);

            decl = Parser.FromString("struct foo { i : int = 5; str : string; }").ParseStatement() as StructDeclaration;
            Assert.IsTrue(decl.Members.Count == 2);
            Assert.IsTrue(decl.Members[0].Type.Name == "int");
            Assert.IsTrue(decl.Members[0].InitializationExpression is IntLiteralExpr);
            Assert.IsTrue(decl.Members[1].Name == "str");
            Assert.IsTrue(decl.Members[1].Type.Name == "string");
            Assert.IsTrue(decl.Members[1].InitializationExpression == null);
        }

        [TestMethod]
        public void Parser_FuncDeclarations()
        {
            var decl = Parser.FromString("func foo() {}").ParseStatement() as FuncDeclaration;
            Assert.IsTrue(decl is FuncDeclaration);
            Assert.IsTrue(decl.Name == "foo");
            Assert.IsTrue(decl.Parameters.Count == 0);
            Assert.IsTrue(decl.Body.Count == 0);
            Assert.IsTrue(decl.ReturnType == null);

            decl = Parser.FromString("func foo(i:int) {}").ParseStatement() as FuncDeclaration;
            Assert.IsTrue(decl.Parameters.Count == 1);
            Assert.IsTrue(decl.Parameters[0].Name == "i");
            Assert.IsTrue(decl.Parameters[0].Type.Name == "int");

            decl = Parser.FromString("func foo(i:int, s:string) {}").ParseStatement() as FuncDeclaration;
            Assert.IsTrue(decl.Parameters.Count == 2);
            Assert.IsTrue(decl.Parameters[1].Name == "s");
            Assert.IsTrue(decl.Parameters[1].Type.Name == "string");

            decl = Parser.FromString("func foo(i:int, s:string) : bool {}").ParseStatement() as FuncDeclaration;
            Assert.IsTrue(decl.Parameters.Count == 2);
            Assert.IsTrue(decl.ReturnType.Name == "bool");

            decl = Parser.FromString(@"

                func foo(i:int, s:string) : string
                {
                    a:=5;
                    return ""Hello"";
                }

            ").ParseStatement() as FuncDeclaration;
            Assert.IsTrue(decl.Body.Count == 2);
            Assert.IsTrue(decl.Body[0] is VariableDeclaration);
            Assert.IsTrue(decl.Body[1] is ReturnStatement);
        }

        [TestMethod]
        public void Parser_CodeStatements()
        {
            var stmt = Parser.FromString("return 42;").ParseStatement() as ReturnStatement;
            Assert.IsTrue(stmt is ReturnStatement);
            Assert.IsTrue(stmt.Expr is IntLiteralExpr);

            stmt = Parser.FromString("return a+b;").ParseStatement() as ReturnStatement;
            Assert.IsTrue(stmt.Expr is BinaryOperatorExpr);

        }

        static void ExprParseTest(string source, string output)
        {
            var expr = Parser.FromString(source).ParseExpression().ToString();
            Assert.IsTrue(output == expr);
        }

        static void StmtParseTest(string source, string output)
        {
            var expr = Parser.FromString(source).ParseStatement().ToString();
            Assert.IsTrue(output == expr);
        }

        void AssertException(Action e)
        {
            bool failed = false;
            try
            {
                e();
            }
            catch
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }
	}
}