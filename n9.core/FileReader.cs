using System.IO;
using System.Text;

// NOTE: I thoroughly dislike the mismatched use of 'char' and 'int'.
// Things to fix eventually.

namespace n9.core
{
    public class FileReader
    {
        // =============================================================== 
        // TODO if we don't end up using the 'TestMatch' style functions, we should nuke this FileReaderState and just inline it.
        
        struct FileReaderState
        {
            public int Position;
            public int Line, Column;
        }

        // =============================================================== 

        public string Filename { get; private set; }
        char[] buffer;
        FileReaderState state;

        // =============================================================== 

        public static FileReader FromFile(string filename)
        {
            var r = new FileReader();
            r.buffer = File.ReadAllText(filename, Encoding.UTF8).ToCharArray();
            r.state.Position = 0;
            r.state.Line = 1;
            r.state.Column = 1;
            r.Filename = filename;
            return r;
        }

        public static FileReader FromString(string str, string filename="default.n9")
        {
            var r = new FileReader();
            r.buffer = str.ToCharArray();
            r.state.Position = 0;
            r.state.Line = 1;
            r.state.Column = 1;
            r.Filename = filename;
            return r;
        }

        // =============================================================== 

        const int ONE_TRUE_TAB_LENGTH = 4; // if we think we should count 'bytes' instead of columns, we could make this 1 instead.

        public int Read()
        {
            if (state.Position >= buffer.Length) return -1;

            char ch = buffer[state.Position++];

            if (ch == '\t')
            {
                state.Column += ONE_TRUE_TAB_LENGTH;
            }
            else if (ch == '\n')
            {
                state.Line++;
                state.Column = 1;
            }
            else
            {
                state.Column++;
            }

            return ch;
        }

        public int Peek()
        {
            if (state.Position >= buffer.Length) return -1;
            return buffer[state.Position];
        }

        /*public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int ch = Read();
                if (ch == '\r') continue;
                if (ch == '\n' || ch == -1)
                    return sb.ToString();
                sb.Append((char)ch);
            }
        }*/

        public void IgnoreRemainingLine()
        {
            while (true)
            {
                int ch = Read();
                if (ch == '\n' || ch == -1)
                    return;
            }
        }

        public FilePosition Pos
        { get { return new FilePosition { Filename = this.Filename, col = state.Column, row = state.Line }; } }

        /*
        static bool IsIdChar(int ch)
        {
            return (ch == '_') || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9');
        }*/
        /*
        public bool TestAndConsumeSymbol(string symbol)
        {
            var savedState = state;

            for (int i = 0; i < symbol.Length; i++)
            {
                int ch = Read();
                if (ch != symbol[i])
                    goto fail;
            }

            return true;

        fail:
            state = savedState;
            return false;
        }

        public bool TestAndConsumeSymbolM1(string symbol)
        {
            if (state.Position == 0)
                return false;

            var savedState = state;

            if (symbol[0] != buffer[state.Position - 1])
                goto fail;

            for (int i = 1; i < symbol.Length; i++)
            {
                int ch = Read();
                if (ch != symbol[i])
                    goto fail;
            }

            return true;

        fail:
            state = savedState;
            return false;
        }

        public bool TestAndConsumeWholeSymbol(string symbol)
        {
            var savedState = state;

            for (int i = 0; i < symbol.Length; i++)
            {
                int ch = Read();
                if (ch != symbol[i])
                    goto fail;
            }

            if (IsIdChar(Peek())) // if next char is an id-char, fail it
                goto fail;

            return true;

        fail:
            state = savedState;
            return false;
        }

        public bool TestAndConsumeWholeSymbolM1(string symbol)
        {
            if (state.Position == 0)
                return false;

            var savedState = state;

            if (symbol[0] != buffer[state.Position - 1])
                goto fail;

            for (int i = 1; i < symbol.Length; i++)
            {
                int ch = Read();
                if (ch != symbol[i])
                    goto fail;
            }

            if (IsIdChar(Peek())) // if next char is an id-char, fail it
                goto fail;

            return true;

        fail:
            state = savedState;
            return false;
        }*/
    }
}