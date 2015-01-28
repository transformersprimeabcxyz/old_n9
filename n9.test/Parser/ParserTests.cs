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

            ExprParseTest("!a", "Bang(a)");
        }

        [TestMethod]
        public void Parser_TypeDeclarations()
        {
            var decl = N9Util.ParseStatement("i : int;") as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.Type.Pointer == false);
            Assert.IsTrue(decl.Type.UnsizedArray == false);
            Assert.IsTrue(decl.Type.SizedArray == false);

            decl = N9Util.ParseStatement("i : int*;") as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.Type.Pointer == true);
            Assert.IsTrue(decl.Type.UnsizedArray == false);
            Assert.IsTrue(decl.Type.SizedArray == false);

            decl = N9Util.ParseStatement("i : int[];") as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.Type.Pointer == false);
            Assert.IsTrue(decl.Type.UnsizedArray == true);
            Assert.IsTrue(decl.Type.SizedArray == false);

            decl = N9Util.ParseStatement("i : int[20];") as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.Type.Pointer == false);
            Assert.IsTrue(decl.Type.UnsizedArray == false);
            Assert.IsTrue(decl.Type.SizedArray == true);
            Assert.IsTrue(decl.Type.ArraySize == 20);

            decl = N9Util.ParseStatement("i : int[]*;") as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.Type.Pointer == true);
            Assert.IsTrue(decl.Type.UnsizedArray == true);
            Assert.IsTrue(decl.Type.SizedArray == false);

            decl = N9Util.ParseStatement("i : int[20]*;") as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.Type.Pointer == true);
            Assert.IsTrue(decl.Type.UnsizedArray == false);
            Assert.IsTrue(decl.Type.SizedArray == true);
        }

        [TestMethod]
        public void Parser_VariableDeclarations()
        {
            var decl = N9Util.ParseStatement("i := 5;") as VariableDeclaration;
            Assert.IsTrue(decl is VariableDeclaration);
            Assert.IsTrue(decl.Name == "i");
            Assert.IsTrue(decl.Type.Name == "(auto)");
            Assert.IsTrue(decl.InitializationExpression is IntLiteralExpr);
            Assert.IsTrue((decl.InitializationExpression as IntLiteralExpr).Literal.IntegerLiteral == 5);

            decl = N9Util.ParseStatement("i : int;") as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.InitializationExpression == null);

            decl = N9Util.ParseStatement("i : int = a*3;") as VariableDeclaration;
            Assert.IsTrue(decl.Type.Name == "int");
            Assert.IsTrue(decl.InitializationExpression is BinaryOperatorExpr);

            N9Util.AssertException(() => N9Util.ParseStatement("i : int")); // Missing terminating semicolon
            N9Util.AssertException(() => N9Util.ParseStatement("i := ;")); // Missing initialization expression in inference syntax
            N9Util.AssertException(() => N9Util.ParseStatement("i : ;")); // missing type specifier

            StmtParseTest("i : int;", "i : int");
            StmtParseTest("i : int = 5;", "i : int = 5");
            StmtParseTest("i := 5;", "i := 5");
            StmtParseTest("i := (2*5);", "i := (2 Asterisk 5)");
            StmtParseTest("i : int = (2*5);", "i : int = (2 Asterisk 5)");
        }

        [TestMethod]
        public void Parser_StructDeclarations()
        {
            var decl = N9Util.ParseStatement("struct foo { }") as StructDeclaration;
            Assert.IsTrue(decl is StructDeclaration);
            Assert.IsTrue(decl.Name == "foo");
            Assert.IsTrue(decl.Members.Count == 0);

            decl = N9Util.ParseStatement("struct foo { i : int; }") as StructDeclaration;
            Assert.IsTrue(decl.Members.Count == 1);
            Assert.IsTrue(decl.Members[0].Name == "i");
            Assert.IsTrue(decl.Members[0].Type.Name == "int");
            Assert.IsTrue(decl.Members[0].InitializationExpression == null);

            decl = N9Util.ParseStatement("struct foo { i := 5; }") as StructDeclaration;
            Assert.IsTrue(decl.Members.Count == 1);
            Assert.IsTrue(decl.Members[0].Type.Name == "(auto)");
            Assert.IsTrue(decl.Members[0].InitializationExpression is IntLiteralExpr);

            decl = N9Util.ParseStatement("struct foo { i : int = 5; str : string; }") as StructDeclaration;
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
            var decl = N9Util.ParseStatement("func foo() {}") as FuncDeclaration;
            Assert.IsTrue(decl is FuncDeclaration);
            Assert.IsTrue(decl.Name == "foo");
            Assert.IsTrue(decl.Parameters.Count == 0);
            Assert.IsTrue(decl.Body.Count == 0);
            Assert.IsTrue(decl.ReturnType == null);

            decl = N9Util.ParseStatement("func foo(i:int) {}") as FuncDeclaration;
            Assert.IsTrue(decl.Parameters.Count == 1);
            Assert.IsTrue(decl.Parameters[0].Name == "i");
            Assert.IsTrue(decl.Parameters[0].Type.Name == "int");

            decl = N9Util.ParseStatement("func foo(i:int, s:string) {}") as FuncDeclaration;
            Assert.IsTrue(decl.Parameters.Count == 2);
            Assert.IsTrue(decl.Parameters[1].Name == "s");
            Assert.IsTrue(decl.Parameters[1].Type.Name == "string");

            decl = N9Util.ParseStatement("func foo(i:int, s:string) : bool {}") as FuncDeclaration;
            Assert.IsTrue(decl.Parameters.Count == 2);
            Assert.IsTrue(decl.ReturnType.Name == "bool");

            decl = N9Util.ParseStatement(@"

                func foo(i:int, s:string) : string
                {
                    a:=5;
                    return ""Hello"";
                }

            ") as FuncDeclaration;
            Assert.IsTrue(decl.Body.Count == 2);
            Assert.IsTrue(decl.Body[0] is VariableDeclaration);
            Assert.IsTrue(decl.Body[1] is ReturnStatement);
        }

        [TestMethod]
        public void Parser_CodeStatements()
        {
            var s1 = N9Util.ParseStatement("return 42;") as ReturnStatement;
            Assert.IsTrue(s1 is ReturnStatement);
            Assert.IsTrue(s1.Expr is IntLiteralExpr);

            s1 = N9Util.ParseStatement("return a+b;") as ReturnStatement;
            Assert.IsTrue(s1.Expr is BinaryOperatorExpr);

            s1 = N9Util.ParseStatement("return;") as ReturnStatement;
            Assert.IsTrue(s1 != null);

            var s2 = N9Util.ParseStatement("i = 0;") as AssignStatement;
            Assert.IsTrue(s2 is AssignStatement);
            Assert.IsTrue(s2.AssignExpr.Left is NameExpr);
            Assert.IsTrue(s2.AssignExpr.Right is IntLiteralExpr);

            s2 = N9Util.ParseStatement("i = a + (x * y);") as AssignStatement;
            Assert.IsTrue(s2 is AssignStatement);
            Assert.IsTrue(s2.AssignExpr.Left is NameExpr);
            Assert.IsTrue(s2.AssignExpr.Right is BinaryOperatorExpr);

            var s3 = N9Util.ParseStatement("defer i = 42;") as DeferStatement;
            Assert.IsTrue(s3 is DeferStatement);
            Assert.IsTrue(s3.Body.Count == 1);
            Assert.IsTrue(s3.Body[0] is AssignStatement);

            s3 = N9Util.ParseStatement("defer { i = 0; close(foo); }") as DeferStatement;
            Assert.IsTrue(s3 is DeferStatement);
            Assert.IsTrue(s3.Body.Count == 2);
            Assert.IsTrue(s3.Body[0] is AssignStatement);
            Assert.IsTrue(s3.Body[1] is CallStatement);
        }

        [TestMethod]
        public void Parser_FlowControlStatements()
        {
            var _while = N9Util.ParseStatement("while (1) foo();") as WhileStatement;
            Assert.IsTrue(_while is WhileStatement);
            Assert.IsTrue(_while.ConditionalExpr is IntLiteralExpr);
            Assert.IsTrue(_while.Body.Count == 1);
            Assert.IsTrue(_while.Body[0] is CallStatement);

            _while = N9Util.ParseStatement("while (1);") as WhileStatement;
            Assert.IsTrue(_while.Body.Count == 0);

            _while = N9Util.ParseStatement(@"

            while (i < Count)
            { 
                processFoo(i);
                i = i + 1;
            }

            ") as WhileStatement;
            Assert.IsTrue(_while is WhileStatement);
            Assert.IsTrue(_while.ConditionalExpr is BinaryOperatorExpr);
            Assert.IsTrue(_while.Body.Count == 2);
            Assert.IsTrue(_while.Body[0] is CallStatement);
            Assert.IsTrue(_while.Body[1] is AssignStatement);

            var _if = N9Util.ParseStatement("if (true) foo();") as IfStatement;
            Assert.IsTrue(_if is IfStatement);
            Assert.IsTrue(_if.IfExpr is NameExpr); // TODO 'true' is currently a name, not a bool literal yet
            Assert.IsTrue(_if.ThenBody.Count == 1);
            Assert.IsTrue(_if.ThenBody[0] is CallStatement);
            Assert.IsTrue(_if.ElseBody.Count == 0);

            _if = N9Util.ParseStatement("if (i == 1) { foo(i); close(); }") as IfStatement;
            Assert.IsTrue(_if.IfExpr is BinaryOperatorExpr);
            Assert.IsTrue(_if.ThenBody.Count == 2);

            _if = N9Util.ParseStatement(@"if (a) foo(); else bar();") as IfStatement;
            Assert.IsTrue(_if.ThenBody.Count == 1);
            Assert.IsTrue(_if.ElseBody.Count == 1);
            Assert.IsTrue(_if.ElseBody[0] is CallStatement);

            _if = N9Util.ParseStatement(@"

            if (a) 
                foo(); 
            else if (b) 
                bar();
            else 
                baz();

            ") as IfStatement;
            Assert.IsTrue(_if.ThenBody.Count == 1);
            Assert.IsTrue(_if.ElseBody.Count == 1);
            Assert.IsTrue(_if.ElseBody[0] is IfStatement);
            var el1 = _if.ElseBody[0] as IfStatement;
            Assert.IsTrue(el1.IfExpr is NameExpr);
            Assert.IsTrue(el1.ThenBody.Count == 1);
            Assert.IsTrue(el1.ElseBody.Count == 1);

            var _ver = N9Util.ParseStatement("version(debug) assert();") as VersionStatement;
            Assert.IsTrue(_ver is VersionStatement);
            Assert.IsTrue(_ver.ConditionalExpr is NameExpr);
            Assert.IsTrue(_ver.Body.Count == 1);
            Assert.IsTrue(_ver.Body[0] is CallStatement);

            var _mod = N9Util.ParseStatement("module;") as ModuleStatement;
            Assert.IsTrue(_mod is ModuleStatement);
            Assert.IsTrue(_mod.Module == "");

            _mod = N9Util.ParseStatement("module foo;") as ModuleStatement;
            Assert.IsTrue(_mod.Module == "foo");

            _mod = N9Util.ParseStatement("module foo.bar;") as ModuleStatement;
            Assert.IsTrue(_mod.Module == "foo.bar");

            N9Util.AssertException(() => N9Util.ParseStatement("module foo 1;")); //expect dot or semi after name
            N9Util.AssertException(() => N9Util.ParseStatement("module foo.;")); //expect name after dot
        }

        static void ExprParseTest(string source, string output)
        {
            var expr = N9Util.ParseExpression(source).ToString();
            Assert.IsTrue(output == expr);
        }

        static void StmtParseTest(string source, string output)
        {
            var expr = N9Util.ParseStatement(source).ToString();
            Assert.IsTrue(expr.ToString().StartsWith(output));
        }
	}
}