using System;
using System.Collections.Generic;

namespace n9.core
{
    public class Module
    {
        // =====================================================

        public string CurrentPath = "";

        Module parent = null;
        Dictionary<string, object> symbols = new Dictionary<string, object>();

        // =====================================================

        public void RegisterSymbol(string symbol, object info)
        {
            if (info is Module)
                throw new Exception("Register child modules through RegisterModule");
            if (symbols.ContainsKey(symbol))
                throw new Exception("Key "+symbol+" already exists in module <"+CurrentPath+">");
            symbols[symbol] = info;
        }

        public Module RegisterModule(string module)
        {
            if (symbols.ContainsKey(module))
                throw new Exception("Key " + module + " already exists in module <" + CurrentPath + ">");

            var subpath = CurrentPath;
            if (CurrentPath.Length > 0)
                subpath += ".";
            subpath += module;

            var ns = new Module { parent = this, CurrentPath = subpath };
            symbols[module] = ns;
            return ns;
        }

        public bool Exists(string symbol)
        {
            return symbols.ContainsKey(symbol);
        }

        public object Get(string symbol)
        {
            if (symbols.ContainsKey(symbol))
                return symbols[symbol];
            if (parent == null)
                return null;
            return parent.Get(symbol);
        }

        Module GetOrCreateModule(string s)
        {
            if (symbols.ContainsKey(s))
            {
                var dir = symbols[s] as Module;
                if (dir != null)
                    return dir;
                throw new Exception("The symbol " + s + " already existed but is not a namespace");
            }
            return RegisterModule(s);
        }

        public Module FindOrCreateToModule(string path)
        {
            var names = path.Split('.');
            var node = this;
            foreach (var name in names)
                node = node.GetOrCreateModule(name);

            return node;
        }

        public void PrintDirectory(bool recurse = true)
        {
            var keys = new List<string>(symbols.Keys);
            keys.Sort();

            foreach (var name in keys)
            {
                var printableName = CurrentPath;
                if (printableName.Length > 0)
                    printableName += ".";
                printableName += name;

                var obj = symbols[name];
                var dir = obj as Module;

                Console.WriteLine(printableName + "\t[" + obj.GetType().Name+"]");

                if (recurse && dir != null)
                    dir.PrintDirectory();
            }
        }
    }
}