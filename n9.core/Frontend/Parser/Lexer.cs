using System;
using System.Collections.Generic;
using System.Text;

namespace n9.core
{
    public class Lexer
    {
        // =====================================================================
        //  Lexer state
        // =====================================================================

        FileReader reader;
        StringBuilder strbuf = new StringBuilder();
        //DiagnosticREceiver

        // =====================================================================
        //  Constructor, helpers
        // =====================================================================

        public Lexer(FileReader reader)
        {
            this.reader = reader;
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

        // TODO: I thoroughly dislike the mismatched use of 'char' and 'int'.
        // Things to fix eventually.

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

                    ch2 = Peek();
                    switch (ch)
                    {
                        case '=':
                            if (ch2 == '=') { Read(); return new Token { Type = TokenType.Equality, Position = pos, Text = "==" }; }
                            return new Token { Type = TokenType.Equals,   Position = pos, Text = "=" };
                        case '!':
                            if (ch2 == '=') { Read(); return new Token { Type = TokenType.Inequality, Position = pos, Text = "!=" }; }
                            return new Token { Type = TokenType.Bang, Position = pos, Text = "!" };
                        case '<':
                            if (ch2 == '=') { Read(); return new Token { Type = TokenType.LessThanEqual, Position = pos, Text = "<=" }; }
                            return new Token { Type = TokenType.LessThan, Position = pos, Text = "<" };
                        case '>':
                            if (ch2 == '=') { Read(); return new Token { Type = TokenType.GreaterThanEqual, Position = pos, Text = ">=" }; }
                            return new Token { Type = TokenType.GreaterThan, Position = pos, Text = ">" };
                        case '&':
                            if (ch2 == '&') { Read(); return new Token { Type = TokenType.LogicalAnd, Position = pos, Text = "&&" }; }
                            return new Token { Type = TokenType.BitAnd, Position = pos, Text = "&" };
                        case '|':
                            if (ch2 == '|') { Read(); return new Token { Type = TokenType.LogicalOr, Position = pos, Text = "||" }; }
                            return new Token { Type = TokenType.BitOr, Position = pos, Text = "|" };

                        case '+': return new Token { Type = TokenType.Plus,     Position = pos, Text = "+" };
                        case '-': return new Token { Type = TokenType.Minus,    Position = pos, Text = "-" };
                        case '*': return new Token { Type = TokenType.Asterisk, Position = pos, Text = "*" };
                        
                        case '.': return new Token { Type = TokenType.Dot,      Position = pos, Text = "." };
                        case ',': return new Token { Type = TokenType.Comma,    Position = pos, Text = "," };
                        case ':': return new Token { Type = TokenType.Colon,    Position = pos, Text = ":" };
                        case ';': return new Token { Type = TokenType.Semi,     Position = pos, Text = ";" };
                        case '(': return new Token { Type = TokenType.LParen,   Position = pos, Text = "(" };
                        case ')': return new Token { Type = TokenType.RParen,   Position = pos, Text = ")" };
                        case '{': return new Token { Type = TokenType.LCurly,   Position = pos, Text = "{" };
                        case '}': return new Token { Type = TokenType.RCurly,   Position = pos, Text = "}" };
                        case '[': return new Token { Type = TokenType.LBracket, Position = pos, Text = "[" };
                        case ']': return new Token { Type = TokenType.RBracket, Position = pos, Text = "]" };
                    }

                    if (ch == '"') // string literals
                    {
                        strbuf.Clear();

                        while (true)
                        {
                            ch2 = Peek();
                            if (ch2 == '"' || ch2 == -1) break;
                            if (ch2 == '\r')
                            {
                                // strip /r's from multi-line literals. you can use "\r" explicitly if needed.
                                // This should make builds be the same regardless of source line endings.
                                Read();
                                continue;
                            }
                            char c = ReadStringLiteralChar();
                            strbuf.Append(c);
                        }
                        if (ch2 == -1)
                            throw new CompilationException(Diagnostic.Error("Unterminated string constant"));
                        Read(); //consume trailing "
                        string literal = strbuf.ToString();
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
            keywords["if"] = TokenType.If;
            keywords["else"] = TokenType.Else;
            keywords["while"] = TokenType.While;
            keywords["return"] = TokenType.Return;
            keywords["defer"] = TokenType.Defer;
            keywords["pragma"] = TokenType.Pragma;
		}
    }

    // =====================================================================
    //  Token definitions
    // =====================================================================

    public struct FilePosition
    {
        public string Filename;
        public int row;
        public int col;
    }

    public enum NumberLiteralClass
    {
        Int,
        Float32,
        Float64,
        Decimal
    }

    public class Token
    {
        public string Text;
        public TokenType Type;
        public FilePosition Position;

        public long IntegerLiteral;
        public double FloatLiteral;
        public char CharLiteral;
        public string StringLiteral;
        public NumberLiteralClass NumberLiteralClass;

        public static Token EOF(FilePosition pos) { return new Token { Type = TokenType.EOF, Position = pos, Text = "<EOF>" }; }

        public override string ToString()
        {
            return string.Format("[{0}:{1}] : {2} {3}", Position.row, Position.col, Type, Text);
        }

        public Token Clone()
        {
            return new Token {
                Text = this.Text,
                Type = this.Type,
                Position = this.Position,
                IntegerLiteral = this.IntegerLiteral,
                FloatLiteral = this.FloatLiteral,
                CharLiteral = this.CharLiteral,
                StringLiteral = this.StringLiteral,
                NumberLiteralClass = this.NumberLiteralClass
            };
        }
    }

    public enum TokenType
    {
        Id,

        IntLiteral,
        CharLiteral,
        StringLiteral,
        FloatLiteral,

        Func,
        Struct,

        If,
        Else,
        While,
        Return,
        Defer,
        Pragma,

        Bang,
        Dot,
        Comma,
        Colon,
        Semi,
        LParen,
        RParen,
        LCurly,
        RCurly,
        LBracket,
        RBracket,

        Equals,
        Plus,
        Minus,
        Asterisk,
        Divslash,

        Equality,
        Inequality,
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,
        LogicalAnd,
        LogicalOr,

        BitAnd,
        BitOr,

        EOF
    }
}