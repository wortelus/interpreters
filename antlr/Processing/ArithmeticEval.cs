using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static antlr.grammarProjAParser;

namespace antlr.Processing
{
    public class ArithmeticEval
    {
        public static void Eval(Tuple<Type, object> left, Tuple<Type, object> right,
            ref ParseTreeProperty<(Type type, object value)> values,
            ref ParseTreeProperty<string> code,
            ref ExpressionContext context
            )
        {


        }
    }
}
