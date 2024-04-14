using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antlr.Processing
{
    public static class Label
    {
        private static int labelCounter = 0;
        public static int LabelCounter { get { return labelCounter; } }

        public static int GetNextLabel()
        {
            return labelCounter++;
        }
    }
}
