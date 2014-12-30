using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n9.core
{
    public class Lexer
    {
        // =====================================================================
        //  Lexer state
        // =====================================================================

        FileReader reader;
        string filename; // Did I just needlessly add filename to FileReader?
        StringBuilder strbuf = new StringBuilder();
        //DiagnosticREceiver

        // =====================================================================
        //  Constructor, helpers
        // =====================================================================

        public Lexer(FileReader reader, string filename)
        {
            this.reader = reader;
            this.filename = filename;
        }

        int Read() { return reader.Read(); }
        int Peek() { return reader.Peek(); }

        static bool IsWhitespace(int ch)
        {
            return (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n');
        }

        static bool IsDigit(int ch)
        {
            return (ch >= '0' && ch <= '9');
        }
        
        static bool IsDigitOrDot(int ch)
        {
            return ((ch >= '0' && ch <= '9') || ch == '.');
        }

        static bool IsHexDigit(int ch)
        {
            return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
        }

        static bool IsBinaryDigit(int ch)
        {
            return (ch >= '0' && ch <= '1');
        }

        static bool IsOctalDigit(int ch)
        {
            return (ch >= '0' && ch <= '7');
        }

        static int AsHexDigit(int ch) // TODO unused currently
        {
            if (ch >= '0' && ch <= '9')
                return ch - '0';
            if (ch >= 'A' && ch <= 'F')
                return ch - 'A' + 10;
            if (ch >= 'a' && ch <= 'f')
                return ch - 'a' + 10;
            return -1;
        }

        // At the moment we are making a diliberate choice to avoid use of unicode characters as Identifier characters
        //  - It likely opens doors for some interesting "obfuscated code" attack vectors
        //  - It likely creates ABI issues
        static bool IsFirstIdChar(int ch)
        {
            return (ch == '_') || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }

        static bool IsIdChar(int ch)
        {
            return IsFirstIdChar(ch) || IsDigit(ch);
        }

        static bool IsValidSymbolName(string id) // TODO note unused
        {
            if (id == null || id.Length == 0)
                return false;

            if (IsFirstIdChar(id[0]) == false)
                return false;

            for (int i = 1; i < id.Length; i++)
                if (IsIdChar(id[i]) == false)
                    return false;

            return true;
        }

        char ReadStringLiteralChar()
        {
            int ch = Read();
            if (ch != '\\')
                return (char) ch;

            int ch2 = Read();
            if (ch2 == 'n') return '\n'; 
            if (ch2 == 'r') return '\r';
            if (ch2 == 't') return '\t';
            if (ch2 == '0') return '\0';
            if (ch2 == '"') return '\"';
            if (ch2 == '\\') return '\\'; 
            if (ch2 == '\'') return '\''; 

            // I dont really know what the use of these are but I will keep them for... some reason
            if (ch2 == 'a') return '\a'; 
            if (ch2 == 'b') return '\b';
            if (ch2 == 'f') return '\f'; 
            if (ch2 == 'v') return '\v';

            // TODO /u unicode stuff

            throw new CompilationException(Diagnostic.Error("Invalid character sequence \\" + (char)ch2));
        }

        // =====================================================================
        //  Tokenizing engine
        // =====================================================================

        public Token Next()
        {
            FilePosition pos;
            int ch, ch2;

            //try
            {
                while (true)
                {
                    pos = reader.Pos;
                    ch = Read();

                    if (IsWhitespace(ch))
                        continue;

                    if (ch == -1) // EOF
                        return Token.EOF(pos);

                    if (ch == '/') // line comment, multi-line comment, div
                    {
                        ch2 = Peek();
                        if (ch2 == '/') // line comment;
                        {
                            do
                            {
                                ch = Read();
                                if (ch == -1)
                                    return Token.EOF(reader.Pos);
                            } while (ch != '\n');
                            continue;
                        }
                        if (ch2 == '*') // multi-line comment
                        {
                            Read(); // consume the '*'

                            do
                            {
                                ch = Read();
                                if (ch == -1)
                                    return Token.EOF(reader.Pos);
                            } while ((ch != '*') || (Peek() != '/'));

                            Read(); // consume the '/'
                            continue;
                        }
                        // else, div
                        return new Token { Type = TokenType.Divslash, Position = pos, Text = "/" };
                    }

                    // identifier or keyword
                    if (IsFirstIdChar(ch))
                    {
                        strbuf.Clear();
                        strbuf.Append((char)ch);

                        while (true)
                        {
                            ch = Peek();
                            if (IsIdChar(ch))
                            {
                                strbuf.Append((char)Read());
                                continue;
                            }

                            string id = strbuf.ToString(); 
                            if (keywords.ContainsKey(id)) // Is it a keyword?
                                return new Token { Type = keywords[id], Position = pos, Text = id };

                            // regular identifier
                            return new Token { Type = TokenType.Id, Position = pos, Text = id };
                        }
                        
                    }

                    // literals
                    if (ch == '0') // Check special numeric literals
                    {
                        ch2 = Peek();
                        if (ch2 == 'x') // Hex literal
                        {
                            strbuf.Clear(); 
                            Read(); // consume 'x'

                            while (true)
                            {
                                ch = Peek();
                                if (IsHexDigit(ch))
                                {
                                    strbuf.Append((char)Read());
                                    continue;
                                }

                                string rawnum = strbuf.ToString();
                                return new Token
                                {
                                    Type = TokenType.IntLiteral,
                                    Position = pos,
                                    Text = "0x" + rawnum,
                                    IntegerLiteral = Convert.ToInt64(rawnum, 16),
                                    NumberLiteralClass = NumberLiteralClass.Int
                                };
                            }
                        }

                        if (ch2 == 'b') // Binary literal
                        {
                            strbuf.Clear();
                            Read(); // consume 'b'

                            while (true)
                            {
                                ch = Peek();
                                if (IsBinaryDigit(ch))
                                {
                                    strbuf.Append((char)Read());
                                    continue;
                                }

                                string rawnum = strbuf.ToString();
                                return new Token
                                {
                                    Type = TokenType.IntLiteral,
                                    Position = pos,
                                    Text = "0b" + rawnum,
                                    IntegerLiteral = Convert.ToInt64(rawnum, 2),
                                    NumberLiteralClass = NumberLiteralClass.Int
                                };
                            }
                        }

                        if (ch2 == 'o') // Octal literal
                        {
                            strbuf.Clear();
                            Read(); // consume 'o'

                            while (true)
                            {
                                ch = Peek();
                                if (IsOctalDigit(ch))
                                {
                                    strbuf.Append((char)Read());
                                    continue;
                                }

                                string rawnum = strbuf.ToString();
                                return new Token
                                {
                                    Type = TokenType.IntLiteral,
                                    Position = pos,
                                    Text = "0o" + rawnum,
                                    IntegerLiteral = Convert.ToInt64(rawnum, 8),
                                    NumberLiteralClass = NumberLiteralClass.Int
                                };
                            }
                        }
                    }
                    if (IsDigit(ch)) // unprefixed numeric literals
                    {
                        strbuf.Clear();
                        strbuf.Append((char)ch);
                        int numDots = 0;

                        while (true)
                        {
                            ch = Peek();
                            if (IsDigitOrDot(ch))
                            {
                                strbuf.Append((char)Read());
                                if (ch == '.') numDots++;
                                continue;
                            }

                            string rawnum = strbuf.ToString();
                            if (numDots > 1)
                            {
                                throw new CompilationException(Diagnostic.Error("Cannot validly parse number: " + rawnum));
                            }
                            if (numDots == 1)
                            {
                                ch2 = Peek();
                                if (ch2 == 'm')
                                {
                                    Read(); // consume 'm'
                                    return new Token
                                    {
                                        Type = TokenType.FloatLiteral,
                                        Position = pos,
                                        Text = rawnum,
                                        FloatLiteral = Double.Parse(rawnum),
                                        NumberLiteralClass = NumberLiteralClass.Decimal,
                                    };
                                }
                                if (ch2 == 'f')
                                {
                                    Read(); // consume 'f'
                                    return new Token
                                    {
                                        Type = TokenType.FloatLiteral,
                                        Position = pos,
                                        Text = rawnum,
                                        FloatLiteral = Double.Parse(rawnum),
                                        NumberLiteralClass = NumberLiteralClass.Float32,
                                    };
                                }
                                if (ch2 == 'd')
                                    Read(); // 'd' is optional (it is the default). If its present, consume it.
                                return new Token
                                {
                                    Type = TokenType.FloatLiteral,
                                    Position = pos,
                                    Text = rawnum,
                                    FloatLiteral = Double.Parse(rawnum),
                                    NumberLiteralClass = NumberLiteralClass.Float64,
                                };
                            }
                            // normal integer
                            return new Token
                            {
                                Type = TokenType.IntLiteral,
                                Position = pos,
                                Text = rawnum,
                                IntegerLiteral = Int64.Parse(rawnum),
                                NumberLiteralClass = NumberLiteralClass.Int,
                            };
                        }
                    }
                
                    if (ch == '=') return new Token { Type = TokenType.Equals, Position = pos, Text = "=" };
                    if (ch == '+') return new Token { Type = TokenType.Plus, Position = pos, Text = "+" };
                    if (ch == '-') return new Token { Type = TokenType.Minus, Position = pos, Text = "-" };
                    if (ch == '*') return new Token { Type = TokenType.Asterisk, Position = pos, Text = "*" };

                    if (ch == '!') return new Token { Type = TokenType.Bang, Position = pos, Text = "!" };
                    if (ch == '.') return new Token { Type = TokenType.Dot, Position = pos, Text = "." };
                    if (ch == ',') return new Token { Type = TokenType.Comma, Position = pos, Text = "," };
                    if (ch == '(') return new Token { Type = TokenType.LParen, Position = pos, Text = "(" };
                    if (ch == ')') return new Token { Type = TokenType.RParen, Position = pos, Text = ")" };
                    if (ch == '{') return new Token { Type = TokenType.LCurly, Position = pos, Text = "{" };
                    if (ch == '}') return new Token { Type = TokenType.RCurly, Position = pos, Text = "}" };
                    if (ch == '[') return new Token { Type = TokenType.LBracket, Position = pos, Text = "[" };
                    if (ch == ']') return new Token { Type = TokenType.RBracket, Position = pos, Text = "]" };

                    if (ch == '"') // string literals
                    {
                        strbuf.Clear();

                        while (true)
                        {
                            ch2 = Peek();
                            if (ch2 == '"' || ch2 == -1) break;
                            char c = ReadStringLiteralChar();
                            strbuf.Append(c);
                        }
                        if (ch2 == -1)
                            throw new CompilationException(Diagnostic.Error("Unterminated string constant"));
                        Read(); //consume trailing "
                        string literal = strbuf.ToString();
                        Console.WriteLine(literal);
                        return new Token { Type = TokenType.StringLiteral, Position = pos, StringLiteral = literal, Text = literal };
                        // TODO supporting A"strings" for ascii strings?
                        // TODO support wide strings????????????????? lame.
                        // TODO support..raw strings?
                        // What else?
                    }

                    // TODO, char literals (but what to do with them?
                    // TODO, bool literals........
                }
            }

         //   return null;
        }


        // =====================================================================
        //  Keywords
        // =====================================================================
        
        static Dictionary<string, TokenType> keywords;

		static Lexer()
		{
			keywords = new Dictionary<string, TokenType>();

			keywords["struct"] = TokenType.Struct;
			keywords["func"] = TokenType.Func;
		}
    }
}