namespace n9.core
{
    // All this stuff should be treated as immutable.
    // Normally we would use 'readonly' to denote immutableness, but then we can't use object initializer syntax, which makes us sad.
    
    public struct CursorPosition
    {
        public int row;
        public int col;
    }

    public struct FilePosition
    {
        public string Filename;
        public int row;
        public int col;
        
        public FilePosition(string filename, int row, int col)
        {
            Filename = filename;
            this.col = col;
            this.row = row;
        }

        public FilePosition(string filename, CursorPosition pos)
        {
            Filename = filename;
            col = pos.col;
            row = pos.row;
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
        public CursorPosition CursorPosition;
        
        // This is where unions would be good.
        public long IntegerLiteral;
        public double FloatLiteral;
        public char CharLiteral;
        public string StringLiteral;
        public NumberLiteralClass NumberLiteralClass;

        public static Token EOF(CursorPosition pos) { return new Token { Type=TokenType.EOF, CursorPosition=pos, Text = "<EOF>" }; }

        public override string ToString()
        {
            return string.Format("[{0}:{1}] : {2} {3}",CursorPosition.row, CursorPosition.col, Type, Text);
        }
    }
}
