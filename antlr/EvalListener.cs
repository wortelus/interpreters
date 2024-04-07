using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antlr
{
    internal class EvalListener : grammarProjABaseListener
    {
        ParseTreeProperty<(Type type, object value)> values = new ParseTreeProperty<(Type type, object value)>();
    }
}
