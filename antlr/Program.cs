using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace antlr
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting ANTLR by wortelus");
            string? inputName;
            do
            {
                Console.Write("Name of the input file: ");
                inputName = Console.ReadLine() + ".txt";
                if (File.Exists(inputName) == false)
                {
                    Console.WriteLine("File not found, try again");
                    inputName = null;
                }                
            } while (inputName == null);

            var inputFile = new StreamReader(inputName);

            AntlrInputStream input = new AntlrInputStream(inputFile);
            var lexer = new grammarProjALexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new grammarProjAParser(tokens);

            parser.AddErrorListener(new VerboseListener());

            IParseTree tree = parser.program();

            if (parser.NumberOfSyntaxErrors == 0)
            {
                ParseTreeWalker walker = new ParseTreeWalker();
                walker.Walk(new EvalListener(), tree);
            }
        }
    }
}