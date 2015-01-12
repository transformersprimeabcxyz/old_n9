using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n9.core
{
    public class Scope
    {
        // =====================================================
        Scope parent = null;
        Dictionary<string, object> symbols = new Dictionary<string, object>();

        // TODO: list of imports
        // TODO: actions to do at scope end

        // =====================================================

        public Scope NewScope()
        {
            return new Scope { parent = this };
        }

        public void Register(string symbol, object info)
        {
            symbols[symbol] = info;
        }

        public object Get(string symbol)
        {
            if (symbols.ContainsKey(symbol))
                return symbols[symbol];
            if (parent == null)
                return null;
            return parent.Get(symbol);
        }
    }
}