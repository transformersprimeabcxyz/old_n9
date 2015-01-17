using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class LexerTests
	{
		[TestMethod]
		public void Lexer_IntegerLiterals()
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
        public void Lexer_FloatLiterals()
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
        public void Lexer_BadFloat()
        {
            // TODO, we want to change this from a CompilationExecption to a Diagnostic later.
            var tokens = Tokenize("1.0.0");
        }

        [TestMethod]
        public void Lexer_Comments()
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

        [TestMethod]
        public void Lexer_Symbols()
        {
            var tokens = Tokenize("!.,(){}[]=+-*/:;");
            Assert.IsTrue(tokens.Count == 16);

            tokens = Tokenize("== != > < >= <= && & || |");
            Assert.IsTrue(tokens.Count == 10);
            Assert.IsTrue(tokens[0].Type == TokenType.Equality);
            Assert.IsTrue(tokens[1].Type == TokenType.Inequality);
            Assert.IsTrue(tokens[2].Type == TokenType.GreaterThan);
            Assert.IsTrue(tokens[3].Type == TokenType.LessThan);
            Assert.IsTrue(tokens[4].Type == TokenType.GreaterThanEqual);
            Assert.IsTrue(tokens[5].Type == TokenType.LessThanEqual);
            Assert.IsTrue(tokens[6].Type == TokenType.LogicalAnd);
            Assert.IsTrue(tokens[7].Type == TokenType.BitAnd);
            Assert.IsTrue(tokens[8].Type == TokenType.LogicalOr);
            Assert.IsTrue(tokens[9].Type == TokenType.BitOr);
        }

        [TestMethod]
        public void Lexer_StringLiterals()
        {
            var tokens = Tokenize("\"Hello World\"");  // tokenize "Hello World"
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Type == TokenType.StringLiteral);
            Assert.IsTrue(tokens[0].StringLiteral == "Hello World");

            tokens = Tokenize("\"Hello \nWorld\"");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].StringLiteral.Contains("\n"));

            tokens = Tokenize("\"\\t\"");
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].StringLiteral == "\t");
        }

        [TestMethod]
        [ExpectedException(typeof(CompilationException))]
        public void Lexer_StringBadControlCodes()
        {
            var tokens = Tokenize("\"\\m is not a valid control code\"");  // tokenizing "\m"
        }

        [TestMethod]
        [ExpectedException(typeof(CompilationException))]
        public void Lexer_UnterminatedStringLiteral()
        {
            var tokens = Tokenize("\"This string does not have a terminating quotation mark");
        }

        [TestMethod]
        public void Lexer_StringLiteralLinefeeds()
        {
            var tokens = Tokenize("\"Test \r\n String'\""); // Tokenize "Test <CR><LF> String" -- \r should get stripped out
            Assert.IsTrue(tokens[0].StringLiteral.Contains("\n"));
            Assert.IsTrue(tokens[0].StringLiteral.Contains("\r") == false);
            
            tokens = Tokenize("\"Test \\r\\n String'\""); // Tokenize "Test \r\n String" -- \r should be left intact
            Assert.IsTrue(tokens[0].StringLiteral.Contains("\r"));

            tokens = Tokenize("\"Test \\\\r String'\""); // Tokenize "Test \\r String" -- string @"\r" should be present in literal
            Assert.IsTrue(tokens[0].StringLiteral.Contains(@"\r"));
        }

        [TestMethod]
        public void Lexer_Keywords()
        {
            var tokens = Tokenize("struct func if else while return defer version");
            Assert.IsTrue(tokens.Count == 8);
            Assert.IsTrue(tokens[0].Type == TokenType.Struct);
            Assert.IsTrue(tokens[1].Type == TokenType.Func);
            Assert.IsTrue(tokens[2].Type == TokenType.If);
            Assert.IsTrue(tokens[3].Type == TokenType.Else);
            Assert.IsTrue(tokens[4].Type == TokenType.While);
            Assert.IsTrue(tokens[5].Type == TokenType.Return);
            Assert.IsTrue(tokens[6].Type == TokenType.Defer);
            Assert.IsTrue(tokens[7].Type == TokenType.Version);
        }

        // ====================================================

        List<Token> Tokenize(string pgm)
        {
            var tokens = new List<Token>();
            var fr = FileReader.FromString(pgm);
            var lx = new Lexer(fr);
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

