using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antlr
{
    public class Interpreter
    {
        Dictionary<int, int> labels = new Dictionary<int, int>();
        Stack<(Type type, object value)> stack = new Stack<(Type type, object value)>();
        Dictionary<string, (Type type, object value)> memory = new Dictionary<string, (Type type, object value)> ();

        string code;
        private int programLength;
        private int instructionPointer = 0;

        public Interpreter(string code)
        {
            this.code = code;
            programLength = code.Split('\n').Length;

            // goto's preprocessing
            int ip = 0;
            foreach (string line in code.Split('\n'))
            {
                string[] args = line.Trim().Split(' ');
                string cmd = args[0];

                if (cmd == "label")
                {
                    if (args.Length != 2)
                    {
                        throw new Exception("During label preprocessing, label did not have 1 argument.");
                    }

                    int jumpIP;
                    if (!int.TryParse(args[1], out jumpIP))
                    {
                        throw (new Exception("Label is not int"));
                    }

                    labels.Add(jumpIP, ip);
                }
                ip++;
            }
        }

        public void Run()
        {
            instructionPointer = 0;
            string[] lines = code.Split('\n');

            do
            {
                string line = lines[instructionPointer];
                Line(line.Trim());
                instructionPointer++;
            } while (instructionPointer < programLength);
        }

        private void Line(string line)
        {
            string[] cmdArgs = line.Split(' ');
            string command = cmdArgs[0];

            string[] args = cmdArgs.Skip(1).ToArray();
            switch (command)
            {
                case "add":
                    Add(args);
                    break;
                case "sub":
                    Sub(args);
                    break;
                case "mul":
                    Mul(args);
                    break;
                case "div":
                    Div(args);
                    break;
                case "mod":
                    Mod(args);
                    break;
                case "eq":
                    Equal(args);
                    break;
                case "lt":
                    Less(args);
                    break;
                case "gt":
                    Greater(args);
                    break;
                case "or":
                    Or(args);
                    break;
                case "and":
                    And(args);
                    break;
                case "concat":
                    Concat(args);
                    break;
                case "uminus":
                    PrefixSub(args);
                    break;
                case "not":
                    PrefixNeg(args);
                    break;
                case "itof":
                    Itof(args);
                    break;
                case "push":
                    Push(args);
                    break;
                case "pop":
                    Pop(args);
                    break;
                case "load":
                    Load(args);
                    break;
                case "save":
                    Save(args);
                    break;
                case "label":
                    Label(args); 
                    break;
                case "jmp":
                    Jmp(args);
                    break;
                case "fjmp":
                    Fjmp(args);
                    break;
                case "print":
                    Print(args); 
                    break;
                case "read":
                    Read(args); 
                    break;
                case "":
                    break;

                default:
                    throw new Exception($"Invalid command {command}.");
            }
        }

        private void Label(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Invalid number of arguments for label");
            }
            // labels.Add(int.Parse(args[0]), int.Parse(args[1]));
        }
        
        private (Type, object, object) ArithmeticCheck(string[] args, string operation)
        {
            if (args.Length != 1)
            {
                throw new Exception($"Invalid number of arguments for {operation}");
            }
            (Type type, object value) rightValue = stack.Pop();
            (Type type, object value) leftValue = stack.Pop();
            if (leftValue.type != rightValue.type ||
                leftValue.type.ToChar().ToString() != args[0])
            {
                throw new Exception($"Type mismatch for {operation}");
            }

            return (leftValue.type, leftValue.value, rightValue.value);
        }

        private void Add(string[] args)
        {
            (Type type, object leftValue, object rightValue) = ArithmeticCheck(args, "add");

            switch (type)
            {
                case Type.Int:
                    stack.Push((Type.Int, (int)leftValue + (int)rightValue));
                    break;
                case Type.Float:
                    stack.Push((Type.Float, (float)leftValue + (float)rightValue));
                    break;
                default:
                    throw new Exception("Invalid type for add");
            }
        }

        private void Sub(string[] args)
        {
            (Type type, object leftValue, object rightValue) = ArithmeticCheck(args, "sub");
            switch (type)
            {
                case Type.Int:
                    stack.Push((Type.Int, (int)leftValue - (int)rightValue));
                    break;
                case Type.Float:
                    stack.Push((Type.Float, (float)leftValue - (float)rightValue));
                    break;
                default:
                    throw new Exception("Invalid type for sub");
            }
        }

        private void Mul(string[] args)
        {
            (Type type, object leftValue, object rightValue) = ArithmeticCheck(args, "mul");
            switch (type)
            {
                case Type.Int:
                    stack.Push((Type.Int, (int)rightValue * (int)leftValue));
                    break;
                case Type.Float:
                    stack.Push((Type.Float, (float)rightValue * (float)leftValue));
                    break;
                default:
                    throw new Exception("Invalid type for mul");
            }
        }

        private void Div(string[] args)
        {
            (Type type, object leftValue, object rightValue) = ArithmeticCheck(args, "div");
            switch (type)
            {
                case Type.Int:
                    stack.Push((Type.Int, (int)leftValue / (int)rightValue));
                    break;
                case Type.Float:
                    stack.Push((Type.Float, (float)leftValue / (float)rightValue));
                    break;
                default:
                    throw new Exception("Invalid type for div");
            }
        }

        private void Mod(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for mod");
            }

            (Type type, object value) rightValue = stack.Pop();
            (Type type, object value) leftValue = stack.Pop();
            if (leftValue.type != rightValue.type)
            {
                throw new Exception($"Type mismatch for mod");
            }
            
            switch (leftValue.type)
            {
                case Type.Int:
                    stack.Push((Type.Int, (int)leftValue.value % (int)rightValue.value));
                    break;
                default:
                    throw new Exception("Invalid type for mod. Only integers accepted.");
            }
        }

        private void PrefixSub(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for prefix sub");
            }
            (Type type, object value) = stack.Pop();
            switch (type)
            {
                case Type.Int:
                    stack.Push((Type.Int, -(int)value));
                    break;
                case Type.Float:
                    stack.Push((Type.Float, -(float)value));
                    break;
                default:
                    throw new Exception("Invalid type for prefix sub");
            }
        }

        private void PrefixNeg(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for prefix neg");
            }
            (Type type, object value) = stack.Pop();
            switch (type)
            {
                case Type.Bool:
                    stack.Push((Type.Bool, !(bool)value));
                    break;
                default:
                    throw new Exception("Invalid type for prefix neg. Only booleans accepted.");
            }
        }

        private void Concat(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for concat");
            }
            (Type type, object value) right = stack.Pop();
            (Type type, object value) left = stack.Pop();
            if (left.type != Type.String || right.type != Type.String)
            {
                throw new Exception("Invalid type for concat. Only strings accepted.");
            }
            stack.Push((Type.String, ((string)left.value).Trim('\"') + ((string)right.value).Trim('\"')));
        }

        private void And(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for and");
            }
            (Type type, object value) left = stack.Pop();
            (Type type, object value) right = stack.Pop();
            if (left.type != Type.Bool || right.type != Type.Bool)
            {
                throw new Exception("Invalid type for and. Only booleans accepted.");
            }
            stack.Push((Type.Bool, (bool)left.value && (bool)right.value));
        }

        private void Or(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for or");
            }
            (Type type, object value) left = stack.Pop();
            (Type type, object value) right = stack.Pop();
            if (left.type != Type.Bool || right.type != Type.Bool)
            {
                throw new Exception("Invalid type for or. Only booleans accepted.");
            }
            stack.Push((Type.Bool, (bool)left.value || (bool)right.value));
        }

        private void Not(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for not");
            }
            (Type type, object value) = stack.Pop();
            if (type != Type.Bool)
            {
                throw new Exception("Invalid type for not. Only booleans accepted.");
            }
            stack.Push((Type.Bool, !(bool)value));
        }

        private void Equal(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for equal");
            }
            (Type type, object value) left = stack.Pop();
            (Type type, object value) right = stack.Pop();

            if (left.type != right.type)
            {
                throw new Exception("Type mismatch for equal");
            }
            stack.Push((Type.Bool, left.value.Equals(right.value)));
        }

        private void Less(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for less");
            }
            (Type type, object value) right = stack.Pop();
            (Type type, object value) left = stack.Pop();
            if (left.type != right.type)
            {
                throw new Exception("Type mismatch for less");
            }
            switch (left.type)
            {
                case Type.Int:
                    stack.Push((Type.Bool, (int)left.value < (int)right.value));
                    break;
                case Type.Float:
                    stack.Push((Type.Bool, (float)left.value < (float)right.value));
                    break;
                default:
                    throw new Exception("Invalid type for less");
            }
        }

        private void Greater(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for greater");
            }
            (Type type, object value) right = stack.Pop();
            (Type type, object value) left = stack.Pop();
            if (left.type != right.type)
            {
                throw new Exception("Type mismatch for greater");
            }
            switch (left.type)
            {
                case Type.Int:
                    stack.Push((Type.Bool, (int)left.value > (int)right.value));
                    break;
                case Type.Float:
                    stack.Push((Type.Bool, (float)left.value > (float)right.value));
                    break;
                default:
                    throw new Exception("Invalid type for greater");
            }
        }

        private void Itof(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("Invalid number of arguments for itof");
            }
            (Type type, object value) = stack.Pop();
            if (type != Type.Int)
            {
                throw new Exception("Invalid type for itof. Only integers accepted.");
            }
            stack.Push((Type.Float, (float)(int)value));
        }

        private void Push(string[] args)
        {
            if (args.Length < 2)
                throw new Exception("Too few arguments for push");

            if (args.Length > 2 && (args[0][0].FromChar() != Type.String))
            {
                throw new Exception("Invalid number of arguments for push");
            }
            string arg = args[0];
            if (arg.Length != 1)
            {
                throw new Exception("Invalid argument for push. Arg not 1 char.");
            }
            Type t = arg[0].FromChar();
            if (t == Type.Unknown)
            {
                throw new Exception("Invalid argument for push. Unknown type.");
            }

            string val = args[1];

            switch (t)
            {
                case Type.Int:
                    {
                        int output;
                        if (int.TryParse(val, out output))
                            stack.Push((Type.Int, output));
                        else
                            throw new Exception("Invalid int for push.");
                        break;
                    }
                case Type.Float:
                    {
                        float output;
                        if (float.TryParse(val, out output))
                            stack.Push((Type.Float, output));
                        else
                            throw new Exception("Invalid float for push.");
                        break;
                    }
                case Type.Bool:
                    {
                        bool output;
                        if (bool.TryParse(val, out output))
                            stack.Push((Type.Bool, output));
                        else
                            throw new Exception("Invalid bool for push.");
                        break;
                    }
                case Type.String:
                    {
                        string output = string.Join(' ', args.Skip(1));
                        stack.Push((Type.String, output.Trim('\"')));
                        break;
                    }
                default:
                    throw new Exception("Unknown push type.");
            }
        }

        private void Read(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Wrong number of arguments for read.");
            }

            string read = Console.ReadLine();
            Type t = args[0][0].FromChar();
            if (t == Type.Unknown) 
            {
                throw new Exception("Unknown read type.");
            }
            Push(new string[] { t.ToChar().ToString(), read } );
        }

        private void Print(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("You must specify n for write.");
            }

            List<object> items = new List<object>();
            int count;
            if (!int.TryParse(args[0], out count))
                throw new Exception("Couldn't parse n for write.");

            for (int i = 0; i < count; i++)
            {
                items.Add(stack.Pop().value);
            }
            items.Reverse();

            foreach (var item in items)
            {
                Console.Write(item);
            }
            Console.Write(Environment.NewLine);
        }

        private void Pop(string[] args)
        {
            if (args.Length != 0)
            {
                throw new Exception("No args for pop needed.");
            }
            stack.Pop();
        }

        private void Save(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Argument for var id needed for save.");
            }
            string varname = args[0];
            (Type type, object value) top = stack.Pop();
            memory[varname] = top;
        }

        private void Load(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Argument for var id needed for load");
            }
            string varname = args[0];
            if (!memory.ContainsKey(varname))
            {
                throw new Exception($"Var name of {varname} not in memory.");
            }
            stack.Push(memory[varname]);
        }

        private void Jmp(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Jmp takes label as its only argument.");
            }

            int jumpIP;
            if (!int.TryParse(args[0], out jumpIP))
            {
                throw new Exception("Couldn't parse label to instruction pointer.");
            }

            if (labels.ContainsKey(jumpIP))
            {
                // offset by one for IP increment later
                instructionPointer = labels[jumpIP] - 1;
            }
            else
            {
                throw new Exception($"Label {jumpIP} not found it labels memory.");
            }
        }

        private void Fjmp(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Fjmp takes its conditional jump IP as its only parameter.");
            }

            int jumpIP;
            if (!int.TryParse(args[0],out jumpIP))
            {
                throw new Exception("Couldn't parse IP to int in fjmp.");
            }

            (Type type, object value) top = stack.Pop();
            if (top.type != Type.Bool)
            {
                throw new Exception("Condition for fjmp must be boolean.");
            }

            if ((bool)top.value)
            {
                // nothing
                //instructionPointer++;
            } 
            else
            {
                // offset by one for IP increment later
                instructionPointer = labels[jumpIP] - 1;
            }
        }
    }
}
