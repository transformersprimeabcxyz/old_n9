using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class TokenizationTests
	{
		[TestMethod]
		public void TestIntegerLiterals()
		{
            var tokens = Tokenize("0");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.IntLiteral);
            Assert.IsTrue(tokens[0].IntegerLiteral == 0);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Int);

            tokens = Tokenize("12345678");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.IntLiteral);
            Assert.IsTrue(tokens[0].IntegerLiteral == 12345678);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Int);

            tokens = Tokenize("0xDEADBEEF 0x7F7F");
            Assert.IsTrue(tokens.Count == 2);
            Assert.IsTrue(tokens[0].Type == TokenType.IntLiteral);
            Assert.IsTrue(tokens[0].IntegerLiteral == 3735928559);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Int);
            Assert.IsTrue(tokens[1].Type == TokenType.IntLiteral);
            Assert.IsTrue(tokens[1].IntegerLiteral == 32639);
            Assert.IsTrue(tokens[1].NumberLiteralClass == NumberLiteralClass.Int);

            tokens = Tokenize("0xDEADBEEF 0x7F7F");
            Assert.IsTrue(tokens.Count == 2);
            Assert.IsTrue(tokens[0].Type == TokenType.IntLiteral);
            Assert.IsTrue(tokens[0].IntegerLiteral == 3735928559);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Int);
            Assert.IsTrue(tokens[1].Type == TokenType.IntLiteral);
            Assert.IsTrue(tokens[1].IntegerLiteral == 32639);
            Assert.IsTrue(tokens[1].NumberLiteralClass == NumberLiteralClass.Int);

            tokens = Tokenize("0b11110000");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.IntLiteral);
            Assert.IsTrue(tokens[0].IntegerLiteral == 0xF0);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Int);

            tokens = Tokenize("0o777");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.IntLiteral);
            Assert.IsTrue(tokens[0].IntegerLiteral == 511);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Int);
		}

        [TestMethod]
        public void TestFloatLiterals()
        {
            var tokens = Tokenize("1.0");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.FloatLiteral);
            Assert.IsTrue(tokens[0].FloatLiteral == 1.0);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Float64);

            tokens = Tokenize("1.0f");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.FloatLiteral);
            Assert.IsTrue(tokens[0].FloatLiteral == 1.0);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Float32);

            tokens = Tokenize("1.0d");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.FloatLiteral);
            Assert.IsTrue(tokens[0].FloatLiteral == 1.0);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Float64);

            tokens = Tokenize("1.0m");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.FloatLiteral);
            Assert.IsTrue(tokens[0].FloatLiteral == 1.0);
            Assert.IsTrue(tokens[0].NumberLiteralClass == NumberLiteralClass.Decimal);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilationException))]
        public void TestBadFloat()
        {
            // TODO, we want to change this from a CompilationExecption to a Diagnostic later.
            var tokens = Tokenize("1.0.0");
        }

        [TestMethod]
        public void TestComments()
        {
            var tokens = Tokenize(@"
                // line comment 1
                identifier
                //line comment 2");
            Assert.IsTrue(tokens.Count == 1);

            tokens = Tokenize(@"
                /* comment */ real_token_1 /* commment */
                /* multi
                   line
                   comment */
                real_token_2
                /* nested /* comments /* don't /* matter */

                // we might want to make nested comments work, but for now they do what C does
                /*/ still_in_a_comment */
                ");
            Assert.IsTrue(tokens.Count == 2);
        }

        // ====================================================

        List<Token> Tokenize(string pgm)
        {
            var tokens = new List<Token>();
            var fr = FileReader.FromString(pgm);
            var lx = new Lexer(fr, "default");
            while (true)
            {
                var t = lx.Next();
                if (t.Type == TokenType.EOF)
                    return tokens;
                tokens.Add(t);
            }
        }
	}
}

