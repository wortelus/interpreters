using antlr.Processing;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace antlr
{
    public class EvalListener : grammarProjABaseListener
    {
        ParseTreeProperty<(Type type, object value)> values = new ParseTreeProperty<(Type type, object value)>();
        ParseTreeProperty<string> code = new ParseTreeProperty<string>();
        SymbolTable SymbolTable = new SymbolTable();

        public ParseTreeProperty<string> GetCode()
        {
            return code;
        }

        private float? ToFloat(object value)
        {
            if (value is int x) return (float)x;
            float outval;

            if (float.TryParse(value.ToString(), out outval))
            {
                return outval;
            }
            return null;
        }

        public override void ExitExprParentheses([NotNull] grammarProjAParser.ExprParenthesesContext context)
        {
            values.Put(context, values.Get(context.expression()));
            code.Put(context, code.Get(context.expression()));
        }

        public override void ExitPrefixSub([NotNull] grammarProjAParser.PrefixSubContext context)
        {
            var val = values.Get(context.expression());
            var valCode = code.Get(context.expression());

            try
            {
                switch (val.type)
                {
                    case Type.Int:
                        {
                            int a = -(int)val.value;
                            values.Put(context, (Type.Int, a));
                            code.Put(context, valCode + "uminus\n");
                            break;
                        }
                    case Type.Float:
                        {
                            float a = -(float)val.value;
                            values.Put(context, (Type.Float, a));
                            code.Put(context, valCode + "uminus\n");
                            break;
                        }
                    default:
                        //errors.Add("Cannot convert to numerical value.");
                        Errors.ReportError(context.Start, "Cannot convert to numerical value.");
                        break;
                }
            }
            catch (Exception e)
            {
                //errors.Add($"Cannot convert to numerical value: {e.Message}");
                Errors.ReportError(context.Start, $"Cannot convert to numerical value: {e.Message}");
            }
        }

        public override void ExitPrefixNeg([NotNull] grammarProjAParser.PrefixNegContext context)
        {
            var val = values.Get(context.expression());
            var valCode = code.Get(context.expression());

            try
            {
                switch (val.type)
                {
                    case Type.Bool:
                        {
                            bool a = !(bool)val.value;
                            values.Put(context, (Type.Bool, a));

                            code.Put(context, valCode + "not\n");
                            break;
                        }
                    default:
                        //errors.Add("Cannot negate non bool type.");
                        Errors.ReportError(context.Start, "Cannot negate non bool type.");
                        break;
                }
            }
            catch (Exception e)
            {
                //errors.Add($"Cannot negate non bool type: {e.Message}");
                Errors.ReportError(context.Start, $"Cannot negate non bool type: {e.Message}");
            }
        }

        public override void ExitMulDivOp([NotNull] grammarProjAParser.MulDivOpContext context)
        {
            var leftValue = values.Get(context.expression()[0]);
            var rightValue = values.Get(context.expression()[1]);

            switch (context.op.Type)
            {
                case grammarProjAParser.MUL:
                    {
                        // both ints
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Int, (int)a * b));
                            code.Put(context, string.Concat(
                                    $"{leftCode}\n",
                                    $"{rightCode}\n",
                                    $"mul I\n"
                                ));
                        }
                        // both floats
                        else if (leftValue.type == rightValue.type && leftValue.type == Type.Float)
                        {
                            float a = (float)leftValue.value;
                            float b = (float)rightValue.value;

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Float, a * b));
                            code.Put(context, string.Concat(
                                    $"{leftCode}\n",
                                    $"{rightCode}\n",
                                    $"mul F\n"
                                ));
                        }
                        // one float, one int
                        else if ((leftValue.type == Type.Int && rightValue.type == Type.Float) ||
                                 (leftValue.type == Type.Float && rightValue.type == Type.Int))
                        {
                            float? a = ToFloat(leftValue.value);
                            float? b = ToFloat(rightValue.value);

                            if (a is null || b is null)
                            {
                                //errors.Add("Unsupported multiplication");
                                Errors.ReportError(context.Start, "Either a or b cannot be converted to float during multiplication.");
                                return;
                            }

                            // mul code
                            var left = code.Get(context.expression()[0]);
                            var right = code.Get(context.expression()[1]);
                            if (leftValue.type == Type.Int)
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{left}\n",
                                    $"itof\n",
                                    $"{right}\n",
                                    "mul F\n"
                                    ));
                            }
                            else
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{left}\n",
                                    $"{right}\n",
                                    $"itof\n",
                                    "mul F\n"
                                    ));
                            }

                            values.Put(context, (Type.Float, (float)a * b));
                        }
                        else
                        {
                            //errors.Add("Unsupported multiplication");
                            Errors.ReportError(context.Start, "Unsupported multiplication");
                        }
                    }
                    break;
                case grammarProjAParser.DIV:
                    {
                        // both ints
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Int, (int)a / b));
                            code.Put(context, string.Concat(
                                    $"{leftCode}\n",
                                    $"{rightCode}\n",
                                    $"div I\n"
                                ));
                        }
                        // both floats
                                                else if (leftValue.type == rightValue.type && leftValue.type == Type.Float)
                        {
                            float a = (float)leftValue.value;
                            float b = (float)rightValue.value;
                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);
                            values.Put(context, (Type.Float, a / b));
                            code.Put(context, string.Concat(
                                $"{leftCode}\n",
                                $"{rightCode}\n",
                                $"div F\n"
                                ));
                        }

                        // one float, one int
                        else if ((leftValue.type == Type.Int && leftValue.type == Type.Float) ||
                            (rightValue.type == Type.Int && rightValue.type == Type.Float))
                        {
                            float a = (float)leftValue.value;
                            float b = (float)rightValue.value;

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Int, (float)a / b));

                            if (leftValue.type == Type.Int)
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{leftCode}\n",
                                    $"itof\n",
                                    $"{rightCode}\n",
                                    "div F\n"
                                    ));
                            }
                            else
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{leftCode}\n",
                                    $"{rightCode}\n",
                                    $"itof\n",
                                    "div F\n"
                                    ));
                            }
                        }
                        else
                        {
                            //errors.Add("Unsupported division");
                            Errors.ReportError(context.Start, "Unsupported division");
                        }
                    }
                    break;
                case grammarProjAParser.MOD:
                    {
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Int, (int)a % b));
                            code.Put(context, string.Concat(
                                $"{leftCode}\n",
                                $"{rightCode}\n",
                                $"mod\n"
                                ));
                        }
                        else if (rightValue.type == Type.Float)
                        {
                            Errors.ReportError(context.Start, "During mod you have to have int as second parameter.");
                        }
                        else if ((leftValue.type == Type.Int || leftValue.type == Type.Float) &&
                            (rightValue.type == Type.Int || rightValue.type == Type.Float))
                        {
                            float? a = ToFloat(leftValue.value);
                            float? b = ToFloat(rightValue.value);

                            if (a is null || b is null)
                            {
                                //errors.Add("Unsupported addition");
                                Errors.ReportError(context.Start, "Either a or b cannot be converted to float during mod.");
                                return;
                            }

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            if (leftValue.type == Type.Int)
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{leftCode}\n",
                                    $"itof\n",
                                    $"{rightCode}\n",
                                    "mod\n"
                                    ));
                            }
                            else
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{leftCode}\n",
                                    $"{rightCode}\n",
                                    $"itof\n",
                                    "mod\n"
                                    ));
                            }

                            values.Put(context, (Type.Int, (float)a % b));
                        }
                        else
                        {
                            //errors.Add("Unsupported division");
                            Errors.ReportError(context.Start, "Unsupported division");
                        }
                    }
                    break;
                default:
                    //errors.Add("unknown op in muldivop");
                    Errors.ReportError(context.Start, "unknown op in muldivop");
                    break;
            }
        }

        public override void ExitAddSubOp([NotNull] grammarProjAParser.AddSubOpContext context)
        {
            var leftValue = values.Get(context.expression()[0]);
            var rightValue = values.Get(context.expression()[1]);

            switch (context.op.Type)
            {
                case grammarProjAParser.ADD:
                    {
                        // Both ints
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Int, (int)a + b));
                            code.Put(context, string.Concat(
                            $"{leftCode}\n",
                            $"{rightCode}\n",
                            "add I \n"
                         ));
                        }
                        // Both floats
                        else if (leftValue.type == Type.Float && rightValue.type == Type.Float)
                        {
                            float a = (float)leftValue.value;
                            float b = (float)rightValue.value;

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Float, (float)a + b));
                            code.Put(context, string.Concat(
                                                    $"{leftCode}\n",
                                                    $"{rightCode}\n",
                                                    "add F\n"
                                                ));
                        }
                        // One float, one int
                        else if (
                            (leftValue.type == Type.Int && rightValue.type == Type.Float) ||
                            (leftValue.type == Type.Float && rightValue.type == Type.Int)
                            )
                        {
                            float? a = ToFloat(leftValue.value);
                            float? b = ToFloat(rightValue.value);


                            // add code
                            var left = code.Get(context.expression()[0]);
                            var right = code.Get(context.expression()[1]);
                            if (leftValue.type == Type.Int)
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{left}\n",
                                    $"itof\n",
                                    $"{right}\n",
                                    "add F\n"
                                    ));
                            }
                            else
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{left}\n",
                                    $"{right}\n",
                                    $"itof\n",
                                    "add F\n"
                                    ));
                            }

                            if (a is null || b is null)
                            {
                                //errors.Add("Unsupported addition");
                                Errors.ReportError(context.Start, "Either a or b cannot be converted to float during addition.");
                                return;
                            }

                            values.Put(context, (Type.Int, (float)a + b));
                        }
                        else
                        {
                            //errors.Add("Unsupported addition");
                            Errors.ReportError(context.Start, "Unsupported addition");
                        }
                    }
                    break;
                case grammarProjAParser.SUB:
                    {
                        // Both ints
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            var left = code.Get(context.expression()[0]);
                            var right = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Int, (int)a - b));
                            code.Put(context, string.Format(
                                $"{left}\n",
                                $"{right}\n",
                                "sub I\n"
                                ));
                        }
                        // Both floats
                        else if (leftValue.type == Type.Float && rightValue.type == Type.Float)
                        {
                            float a = (float)leftValue.value;
                            float b = (float)rightValue.value;

                            var leftCode = code.Get(context.expression()[0]);
                            var rightCode = code.Get(context.expression()[1]);

                            values.Put(context, (Type.Float, (float)a - b));
                            code.Put(context, string.Format(
                                $"{leftCode}\n",
                                $"{rightCode}\n",
                                "sub F\n"
                                ));
                        }
                        // One float, one int
                        else if ((leftValue.type == Type.Int && rightValue.type == Type.Float) &&
                                 (leftValue.type == Type.Float && rightValue.type == Type.Int))
                        {
                            float? a = ToFloat(leftValue.value);
                            float? b = ToFloat(rightValue.value);

                            // add code
                            var left = code.Get(context.expression()[0]);
                            var right = code.Get(context.expression()[1]);
                            if (leftValue.type == Type.Int)
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{left}\n",
                                    $"itof\n",
                                    $"{right}\n",
                                    "sub F\n"
                                    ));
                            }
                            else
                            {
                                code.Put(
                                    context, string.Concat(
                                    $"{left}\n",
                                    $"{right}\n",
                                    $"itof\n",
                                    "sub F\n"
                                    ));
                            }

                            if (a is null || b is null)
                            {
                                //errors.Add("Unsupported addition");
                                Errors.ReportError(context.Start, "Either a or b cannot be converted to float during substraction.");
                                return;
                            }

                            values.Put(context, (Type.Int, (float)a - b));
                        }
                        else
                        {
                            //errors.Add("Unsupported minusing");
                            Errors.ReportError(context.Start, "Unsupported substraction.");
                        }
                    }
                    break;
                case grammarProjAParser.CONCAT:
                    {
                        if (leftValue.type == Type.String && rightValue.type == Type.String)
                        {
                            values.Put(context, (Type.String, String.Concat((string)leftValue.value, (string)rightValue.value)));
                            code.Put(context, string.Concat(
                                           code.Get(context.expression()[0]),
                                           code.Get(context.expression()[1]),
                                           "concat\n"
                                       ));
                        }
                        else
                        {
                            //errors.Add("Unsupported concating");
                            Errors.ReportError(context.Start, "Unsupported concating");
                        }
                    }
                    break;
                default:
                    {
                        //errors.Add("Error in add sub concat.");
                        Errors.ReportError(context.Start, "Error in add sub concat.");
                    }
                    break;
            }
        }

        public override void ExitArithmeticComp([NotNull] grammarProjAParser.ArithmeticCompContext context)
        {
            var leftValue = values.Get(context.expression()[0]);
            var rightValue = values.Get(context.expression()[1]);

            var leftCode = code.Get(context.expression()[0]);
            var rightCode = code.Get(context.expression()[1]);

            if (context.op.Type == grammarProjAParser.LT || context.op.Type == grammarProjAParser.GT)
            {
                try
                {
                    // check if both are floats
                    bool comp = ToFloat(leftValue.value) > ToFloat(rightValue.value);
                    values.Put(context, (Type.Bool,
                    context.op.Type == grammarProjAParser.LT ? comp : !comp
                    ));
                    // add code
                    if (leftValue.type != rightValue.type)
                    {
                        if (leftValue.type == Type.Float)
                        {
                            code.Put(
                               context, string.Concat(
                               $"{leftCode}\n",
                               $"{rightCode}\n",
                               $"itof\n",
                               context.op.Type == grammarProjAParser.LT ? "lt\n" : "gt\n"
                           ));
                        }
                        else
                        {
                            code.Put(
                               context, string.Concat(
                               $"{leftCode}\n",
                               $"itof\n",
                               $"{rightCode}\n",
                               context.op.Type == grammarProjAParser.LT ? "lt\n" : "gt\n"
                            ));
                        }
                    }
                    else
                    {
                        code.Put(context, string.Concat(
                            $"{leftCode}\n", 
                            $"{rightCode}\n",
                            context.op.Type == grammarProjAParser.LT ? "lt\n" : "gt\n"
                        ));
                    }
                }
                catch
                {
                    //errors.Add("couldn't arithmetic comp, check types.");
                    Errors.ReportError(context.Start, "couldn't arithmetic comp, check types.");
                }
            }
            else
            {
                //errors.Add("error arithmetic comp.");
                Errors.ReportError(context.Start, "error arithmetic comp.");
            }
        }

        public override void ExitArithmeticEq([NotNull] grammarProjAParser.ArithmeticEqContext context)
        {
            var leftValue = values.Get(context.expression()[0]);
            var rightValue = values.Get(context.expression()[1]);

            var leftCode = code.Get(context.expression()[0]);
            var rightCode = code.Get(context.expression()[1]);

            if (context.op.Type == grammarProjAParser.EQ || context.op.Type == grammarProjAParser.NEQ)
            {
                try
                {
                    // check if both are floats
                    bool comp = ToFloat(leftValue.value) == ToFloat(rightValue.value);
                    values.Put(context, (Type.Bool,
                    context.op.Type == grammarProjAParser.EQ ? comp : !comp
                    ));
                    // add code
                    if (leftValue.type != rightValue.type)
                    {
                        if (leftValue.type == Type.Float)
                        {
                            code.Put(
                               context, string.Concat(
                               $"{leftCode}\n",
                               $"itof\n",
                               $"{rightCode}\n",
                               context.op.Type == grammarProjAParser.EQ ? "eq\n" : "eq\nnot\n"
                           ));
                        }
                        else
                        {
                            code.Put(
                               context, string.Concat(
                               $"{leftCode}\n",
                               $"{rightCode}\n",
                               $"itof\n",
                               context.op.Type == grammarProjAParser.EQ ? "eq\n" : "eq\nnot\n"
                            ));
                        }
                    }
                    else
                    {
                        code.Put(
                               context, string.Concat(
                               $"{leftCode}\n",
                               $"{rightCode}\n",
                               context.op.Type == grammarProjAParser.EQ ? "eq\n" : "eq\nnot\n"
                            ));
                    }
                }
                catch
                {
                    //errors.Add("couldn't arithmetic eq, check types.");
                    Errors.ReportError(context.Start, "couldn't arithmetic eq, check types.");
                }
            }
            else
            {
                //errors.Add("error arithmetic eq.");
                Errors.ReportError(context.Start, "error arithmetic eq.");
            }
        }

        public override void ExitLogicalAnd([NotNull] grammarProjAParser.LogicalAndContext context)
        {
            var leftValue = values.Get(context.expression()[0]);
            var rightValue = values.Get(context.expression()[1]);

            if (context.op.Type == grammarProjAParser.AND)
            {
                try
                {
                    bool comp = (bool)leftValue.value && (bool)rightValue.value;
                    values.Put(context, (Type.Bool, comp));
                    code.Put(context, string.Concat(
                                               code.Get(context.expression()[0]),
                                               code.Get(context.expression()[1]),
                                               "and\n"
                    ));
                }
                catch
                {
                    //errors.Add("couldn't logical and, check types.");
                    Errors.ReportError(context.Start, "couldn't logical and, check types.");
                }
            }
            else
            {
                //errors.Add("error logical and.");
                Errors.ReportError(context.Start, "error logical and.");
            }
        }

        public override void ExitLogicalOr([NotNull] grammarProjAParser.LogicalOrContext context)
        {
            var leftValue = values.Get(context.expression()[0]);
            var rightValue = values.Get(context.expression()[1]);

            if (context.op.Type == grammarProjAParser.OR)
            {
                try
                {
                    bool comp = (bool)leftValue.value || (bool)rightValue.value;
                    values.Put(context, (Type.Bool, comp));
                    code.Put(context, string.Concat(
                           code.Get(context.expression()[0]),
                           code.Get(context.expression()[1]),
                           "or\n"
                        ));
                }
                catch
                {
                    //errors.Add("couldn't logical or, check types.");
                    Errors.ReportError(context.Start, "couldn't logical or, check types.");
                }
            }
            else
            {
                //errors.Add("error logical or.");
                Errors.ReportError(context.Start, "error logical or.");
            }
        }


        bool first = true;
        string firstContent = "";
        public override void EnterAssignment([NotNull] grammarProjAParser.AssignmentContext context)
        {
            base.EnterAssignment(context);

            if (first)
            {
                first = false;
                firstContent = context.expression().GetText();
            }
        }

        public override void ExitAssignment([NotNull] grammarProjAParser.AssignmentContext context)
        {
            base.ExitAssignment(context);

            var rightCode = code.Get(context.expression());
            var rightValue = values.Get(context.expression());
            var storedValue = SymbolTable[context.IDENTIFIER().Symbol];
            string variable = context.IDENTIFIER().GetText();

            if (storedValue.Type == rightValue.type)
            {
                SymbolTable[context.IDENTIFIER().Symbol] = rightValue;
                values.Put(context, rightValue);
                code.Put(context, string.Concat(
                          rightCode,
                          $"save {variable}\n",
                          $"load {variable}\n"
                          ));
            }
            else if (rightValue.type == Type.Float)
            {
                try
                {
                    var value = (Type.Float, ToFloat(rightValue.value));
                    SymbolTable[context.IDENTIFIER().Symbol] = value;
                    values.Put(context, value);
                    code.Put(context, string.Concat(
                            rightCode,
                            $"itof\n",
                            $"save {variable}\n",
                            $"load {variable}\n"
                          ));
                }
                catch
                {
                    //errors.Add("Couldnt assign non float variable. ");
                    Errors.ReportError(context.Start, "Couldnt assign non float variable. ");
                }
            }
            else if (rightValue.type == Type.Int)
            {
                try
                {
                    int v = (int)rightValue.value;
                    var value = (Type.Int, v);
                    SymbolTable[context.IDENTIFIER().Symbol] = value;
                    values.Put(context, value);
                    code.Put(context, string.Concat(
                           rightCode,
                           $"save {variable}\n",
                           $"load {variable}\n"
                         ));
                }
                catch
                {
                    //errors.Add("Couldnt assign non int variable. ");
                    Errors.ReportError(context.Start, "Couldnt assign non int variable. ");
                }
            }
            else if (rightValue.type == Type.Bool)
            {
                try
                {
                    bool v = (bool)rightValue.value;
                    var value = (Type.Bool, v);
                    SymbolTable[context.IDENTIFIER().Symbol] = value;
                    values.Put(context, value);
                    code.Put(context, string.Concat(
                           rightCode,
                           $"save {variable}\n",
                           $"load {variable}\n"
                         ));
                }
                catch
                {
                    //errors.Add("Couldnt assign non bool variable. ");
                    Errors.ReportError(context.Start, "Couldnt assign non bool variable. ");
                }
            }
            else if (rightValue.type == Type.String)
            {
                try
                {
                    string v = (string)rightValue.value;
                    var value = (Type.String, v);
                    SymbolTable[context.IDENTIFIER().Symbol] = value;
                    values.Put(context, value);
                    code.Put(context, string.Concat(
                           rightCode,
                           $"save {variable}\n",
                           $"load {variable}\n"
                         ));
                }
                catch
                {
                    //errors.Add("Couldnt assign non string variable. ");
                    Errors.ReportError(context.Start, "Couldnt assign non string variable. ");
                }
            }
            else
            {
                //errors.Add("Error in assignment.");
                Errors.ReportError(context.Start, "Error in assignment.");
            }

            if (context.expression().GetText() == firstContent && !first)
            {
                first = true;
                rightCode = code.Get(context);
                code.Put(context, rightCode 
                    // + "pop\n"
                    );
            }
        }

        public override void ExitInt([NotNull] grammarProjAParser.IntContext context)
        {
            int value;
            if (int.TryParse(context.INT().GetText(), out value))
            {
                values.Put(context, (Type.Int, value));
                code.Put(context, $"push I {value}\n");
            }
            else
            {
                //errors.Add("Couldnt convert to integer during int expression. ");
                Errors.ReportError(context.Start, "Couldnt convert to integer during int expression. ");
            }
        }

        public override void ExitFloat([NotNull] grammarProjAParser.FloatContext context)
        {
            float value;
            if (float.TryParse(context.FLOAT().GetText(), CultureInfo.InvariantCulture, out value))
            {
                values.Put(context, (Type.Float, value));
                code.Put(context, $"push F {value}\n");
            }
            else
            {
                //errors.Add("Couldnt convert to float during int expression. ");
                Errors.ReportError(context.Start, "Couldnt convert to float during int expression. ");
            }
        }

        public override void ExitBool([NotNull] grammarProjAParser.BoolContext context)
        {
            try
            {
                string value = context.BOOL().GetText();
                if (value.ToLower().Trim() == "true")
                {
                    values.Put(context, (Type.Bool, true));
                    code.Put(context, $"push B {value}\n");
                }
                else if (value.ToLower().Trim() == "false")
                {
                    values.Put(context, (Type.Bool, false));
                    code.Put(context, $"push B {value}\n");
                }
                else
                {
                    //errors.Add("Couldnt convert to boolean during int expression. Not boolean value.");
                    Errors.ReportError(context.Start, "Couldnt convert to boolean during int expression. Not boolean value.");
                }
            }
            catch
            {
                //errors.Add("Couldnt convert to boolean during int expression.");
                Errors.ReportError(context.Start, "Couldnt convert to boolean during int expression.");
            }
        }

        public override void ExitString([NotNull] grammarProjAParser.StringContext context)
        {
            try
            {
                string value = context.STRING().GetText();
                values.Put(context, (Type.String, value));
                code.Put(context, $"push S {value}\n");
            }
            catch
            {
                //errors.Add("Couldnt convert to string.");
                Errors.ReportError(context.Start, "Couldnt convert to string.");
            }
        }

        public override void ExitIdentifier([NotNull] grammarProjAParser.IdentifierContext context)
        {
            string varName = context.IDENTIFIER().GetText();

            var storedVar = SymbolTable[context.IDENTIFIER().Symbol];
            if (storedVar.Type == Type.Error)
            {
                return;
            }

            values.Put(context, storedVar);
            code.Put(context, $"load {varName}\n");
        }

        //
        //
        // STATEMENTS
        //
        //

        public override void ExitEmptyStatement([NotNull] grammarProjAParser.EmptyStatementContext context)
        {
            base.ExitEmptyStatement(context);
        }

        public override void ExitDeclarations([NotNull] grammarProjAParser.DeclarationsContext context)
        {
            var typeValue = context.TYPE_KEYWORD().GetText();

            Type t;
            try
            {
                t = typeValue.FromString();
            }
            catch
            {
                //errors.Add("Couldn't declare var.");
                Errors.ReportError(context.Start, $"Unknown type {typeValue}.");
                return;
            }

            string code = string.Empty;
            foreach (var identifier in context.IDENTIFIER())
            {
                var identifierName = identifier.GetText();
                if (identifierName != null)
                {
                    SymbolTable.Add(identifier.Symbol, t);
                    SymbolTable[identifier.Symbol] = (t, TypeExtensions.DefaultValue(t));

                    code += $"push {TypeExtensions.ToChar(t)} {t.Represent(t.DefaultValue())}\n";
                    code += $"save {identifierName}\n";
                }
                else
                {
                    //errors.Add("Couldn't declare var.");
                    Errors.ReportError(context.Start, "Couldn't declare var.");
                }
            }
            this.code.Put(context, code);
        }

        public override void ExitRead([NotNull] grammarProjAParser.ReadContext context)
        {
            string code = string.Empty;
            foreach (var identifier in context.IDENTIFIER())
            {
                var identifierName = identifier.GetText();

                char datatype = 'U';
                switch (SymbolTable[identifier.Symbol].Type)
                {
                    case Type.Int:
                        datatype = 'I';
                        break;
                    case Type.Float:
                        datatype = 'F';
                        break;
                    case Type.Bool:
                        datatype = 'B';
                        break;
                    case Type.String:
                        datatype = 'S';
                        break;
                    default:
                        Errors.ReportError(context.Start, "Unknown type.");
                        break;
                }

                if (identifierName != null)
                {
                    code += $"read {datatype}\n";
                    code += $"save {identifierName}\n";
                }
                else
                {
                    //errors.Add("Couldn't read var.");
                    Errors.ReportError(context.Start, "Couldn't read var.");
                }
            }
            this.code.Put(context, code);
        }

        public override void ExitWrite([NotNull] grammarProjAParser.WriteContext context)
        {
            string writeCode = string.Empty;
            var exprs = context.expression();
            int count = exprs.Length;
            foreach (var expression in exprs)
            {
                writeCode += code.Get(expression);
            }
            writeCode += $"print {count}\n";
            code.Put(context, writeCode);
        }

        public override void ExitBlock([NotNull] grammarProjAParser.BlockContext context)
        {
            var statements = context.statement();
            code.Put(context, string.Concat(statements.Select(s => code.Get(s))));
        }

        public override void ExitIfElse([NotNull] grammarProjAParser.IfElseContext context)
        {
            var condition = values.Get(context.expression());
            if (condition.type != Type.Bool)
            {
                //errors.Add("If condition must be boolean.");
                Errors.ReportError(context.Start, "If condition must be boolean.");
                return;
            }

            var conditionCode = code.Get(context.expression());

            int negativeLabel = Label.GetNextLabel();
            int positiveEndLabel = Label.GetNextLabel();

            string ifBranchCode = code.Get(context.iftrue);
            string ifElseBranchCode = context.ifelse == null ? string.Empty : code.Get(context.ifelse);

            code.Put(context, string.Concat(conditionCode,
                "fjmp ",
                $"{negativeLabel}\n",
                $"{ifBranchCode}\n",
                $"jmp {positiveEndLabel}\n",
                $"label {negativeLabel}\n",
                $"{ifElseBranchCode}\n",
                $"label {positiveEndLabel}\n"));
        }

        public override void ExitWhile([NotNull] grammarProjAParser.WhileContext context)
        {
            var condition = values.Get(context.expression());
            if (condition.type != Type.Bool)
            {
                //errors.Add("If condition must be boolean.");
                Errors.ReportError(context.Start, "While condition must be boolean.");
                return;
            }

            var conditionCode = code.Get(context.expression());

            var endLabel = Label.GetNextLabel();
            var startLabel = Label.GetNextLabel();

            string statementCode =
                $"{code.Get(context.statement())}\n";

            code.Put(context, $"label {startLabel}\n" +
                $"{conditionCode}\n" +
                $"fjmp {endLabel}\n" +
                $"{statementCode}\n" +
                $"jmp {startLabel}\n" +
                $"label {endLabel}\n");
        }

        public override void ExitDoWhile([NotNull] grammarProjAParser.DoWhileContext context)
        {
            var condition = values.Get(context.expression());
            if (condition.type != Type.Bool)
            {
                //errors.Add("If condition must be boolean.");
                Errors.ReportError(context.Start, "Do while condition must be boolean.");
                return;
            }

            var startLabel = Label.GetNextLabel();
            var endLabel = Label.GetNextLabel();

            string statement =
                $"{code.Get(context.statement())}\n";

            code.Put(context, $"label {startLabel}\n" +
                $"{statement}\n" +
                $"{condition}\n" +
                $"fjmp {endLabel}\n" +
                $"jmp {startLabel}\n" +
                $"label {endLabel}\n");
        }

        public override void ExitEval([NotNull] grammarProjAParser.EvalContext context)
        {
            base.ExitEval(context);
            code.Put(context, 
                $"{code.Get(context.expression())}\n" + 
                $"pop\n"
                );
        }

        public override void ExitProgram([NotNull] grammarProjAParser.ProgramContext context)
        {
            string code = string.Empty;
            foreach (var statement in context.statement())
            {
                code += this.code.Get(statement);
            }
            Console.Write(code);
            File.WriteAllText("output.txt", code);
        }
    }
}
