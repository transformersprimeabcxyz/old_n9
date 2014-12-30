namespace n9.core
{
    // All this stuff should be treated as immutable.
    // Normally we would use 'readonly' to denote immutableness, but then we can't use object initializer syntax, which makes us sad.

    public struct FilePosition
    {
        public string Filename;
        public int row;
        public int col;
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

        Bang,
        Dot,
        Comma,
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

        EOF
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
        
        // This is where unions would be good.
        public long IntegerLiteral;
        public double FloatLiteral;
        public char CharLiteral;
        public string StringLiteral;
        public NumberLiteralClass NumberLiteralClass;

        public static Token EOF(FilePosition pos) { return new Token { Type=TokenType.EOF, Position=pos, Text = "<EOF>" }; }

        public override string ToString()
        {
            return string.Format("[{0}:{1}] : {2} {3}", Position.row, Position.col, Type, Text);
        }
    }
}
