﻿using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace antlr
{
    public class SymbolTable
    {
        Dictionary<string, (Type Type, object Value)> memory = new Dictionary<string, (Type Type, object Value)>();

        public void Add(IToken variable, Type type)
        {
            var name = variable.Text.Trim();
            if (memory.ContainsKey(name))
            {
                Errors.ReportError(variable, $"Variable {name} was already declared.");
            }
            else
            {
                if (type == Type.Int) memory.Add(name, (type, 0));
                else memory.Add(name, (type, (float)0));
            }
        }
        public (Type Type, object Value) this[IToken variable]
        {
            get
            {
                var name = variable.Text.Trim();
                if (memory.ContainsKey(name))
                {
                    return memory[name];
                }
                else
                {
                    Errors.ReportError(variable, $"Variable {name} was NOT declared.");
                    return (Type.Error, 0);
                }
            }
            set
            {
                var name = variable.Text.Trim();
                memory[name] = value;
            }
        }
    }
}
