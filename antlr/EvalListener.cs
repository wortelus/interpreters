using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antlr
{
    public class EvalListener : grammarProjABaseListener
    {
        ParseTreeProperty<(Type type, object value)> values = new ParseTreeProperty<(Type type, object value)>();
        SymbolTable SymbolTable = new SymbolTable();
        private float? ToFloat(object value)
        {
            if (value is int x) return (float)x;
            float outval;
            if (float.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out outval))
            {
                return outval;
            }
            return null;
        }

        public override void ExitExprParentheses([NotNull] grammarProjAParser.ExprParenthesesContext context)
        {
            values.Put(context, values.Get(context.expression()));
        }

        public override void ExitPrefixSub([NotNull] grammarProjAParser.PrefixSubContext context)
        {
            var val = values.Get(context.expression());

            try
            {
                switch (val.type)
                {
                    case Type.Int:
                        {
                            int a = -(int)val.value;
                            values.Put(context, (Type.Int, a));
                            break;
                        }
                    case Type.Float:
                        {
                            float a = -(float)val.value;
                            values.Put(context, (Type.Float, a));
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

            try
            {
                switch (val.type)
                {
                    case Type.Bool:
                        {
                            bool a = !(bool)val.value;
                            values.Put(context, (Type.Bool, a));
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
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            values.Put(context, (Type.Int, (int)a * b));
                        }
                        else if ((leftValue.type == Type.Int || leftValue.type == Type.Float) &&
                            (rightValue.type == Type.Int || rightValue.type == Type.Float))
                        {
                            float? a = ToFloat(leftValue.value);
                            float? b = ToFloat(rightValue.value);

                            if (a is null || b is null)
                            {
                                //errors.Add("Unsupported multiplication");
                                Errors.ReportError(context.Start, "Either a or b cannot be converted to float during multiplication.");
                                return;
                            }

                            values.Put(context, (Type.Int,  (float)a * b));
                        } else
                        {
                            //errors.Add("Unsupported multiplication");
                            Errors.ReportError(context.Start, "Unsupported multiplication");
                        }
                    }
                    break;
                case grammarProjAParser.DIV:
                    {
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            values.Put(context, (Type.Int, (int)a / b));
                        }
                        else if ((leftValue.type == Type.Int || leftValue.type == Type.Float) &&
                            (rightValue.type == Type.Int || rightValue.type == Type.Float))
                        {
                            float a = (float)leftValue.value;
                            float b = (float)rightValue.value;

                            values.Put(context, (Type.Int, (float)a / b));
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

                            values.Put(context, (Type.Int, (int)a % b));
                        }
                        else if ((leftValue.type == Type.Int || leftValue.type == Type.Float) &&
                            (rightValue.type == Type.Int || rightValue.type == Type.Float))
                        {
                            float a = (float)leftValue.value;
                            float b = (float)rightValue.value;

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
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            values.Put(context, (Type.Int, (int)a + b));
                        }
                        else if ((leftValue.type == Type.Int || leftValue.type == Type.Float) &&
                            (rightValue.type == Type.Int || rightValue.type == Type.Float))
                        {
                            float? a = ToFloat(leftValue.value);
                            float? b = ToFloat(rightValue.value);

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
                        if (leftValue.type == rightValue.type && leftValue.type == Type.Int)
                        {
                            int a = (int)leftValue.value;
                            int b = (int)rightValue.value;

                            values.Put(context, (Type.Int, (int)a - b));
                        }
                        else if ((leftValue.type == Type.Int || leftValue.type == Type.Float) &&
                            (rightValue.type == Type.Int || rightValue.type == Type.Float))
                        {
                            float? a = ToFloat(leftValue.value);
                            float? b = ToFloat(rightValue.value);

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

            if (context.op.Type == grammarProjAParser.LT || context.op.Type == grammarProjAParser.GT)
            {
                try
                {
                    bool comp = ToFloat(leftValue.value) > ToFloat(rightValue.value);
                    values.Put(context, (Type.Bool,
                    context.op.Type == grammarProjAParser.LT ? comp : !comp
                    ));
                }
                catch 
                {
                    //errors.Add("couldn't arithmetic comp, check types.");
                    Errors.ReportError(context.Start, "couldn't arithmetic comp, check types.");
                }
            } else
            {
                //errors.Add("error arithmetic comp.");
                Errors.ReportError(context.Start, "error arithmetic comp.");
            }
        }

        public override void ExitArithmeticEq([NotNull] grammarProjAParser.ArithmeticEqContext context)
        {
            var leftValue = values.Get(context.expression()[0]);
            var rightValue = values.Get(context.expression()[1]);

            if (context.op.Type == grammarProjAParser.EQ || context.op.Type == grammarProjAParser.NEQ)
            {
                try
                {
                    bool comp = ToFloat(leftValue.value) == ToFloat(rightValue.value);
                    values.Put(context, (Type.Bool,
                    context.op.Type == grammarProjAParser.EQ ? comp : !comp
                    ));
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

        public override void ExitAssignment([NotNull] grammarProjAParser.AssignmentContext context)
        {
            var rightValue = values.Get(context.expression());
            var storedValue = SymbolTable[context.IDENTIFIER().Symbol];
            string variable = context.IDENTIFIER().GetText();

            if (storedValue.Type == rightValue.type)
            {
                SymbolTable[context.IDENTIFIER().Symbol] = rightValue;
                values.Put(context, rightValue);
            }
            else if (rightValue.type == Type.Float)
            {
                try
                {
                    var value = (Type.Float, ToFloat(rightValue.value));
                    SymbolTable[context.IDENTIFIER().Symbol] = value;
                    values.Put(context, value);
                }
                catch
                {
                    //errors.Add("Couldnt assign non float variable. ");
                    Errors.ReportError(context.Start, "Couldnt assign non float variable. ");
                }
            } else if (rightValue.type == Type.Int)
            {
                try
                {
                    int v = (int)rightValue.value;
                    var value = (Type.Int, v);
                    SymbolTable[context.IDENTIFIER().Symbol] = value;
                    values.Put(context, value);
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
        }

        public override void ExitInt([NotNull] grammarProjAParser.IntContext context)
        {
            int value;
            if (int.TryParse(context.INT().GetText(), out value)) {
                values.Put(context, (Type.Int, value));
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
                } 
                else if (value.ToLower().Trim() == "false")
                {
                    values.Put(context, (Type.Bool, false));
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

            foreach (var identifier in context.IDENTIFIER())
            {
                var identifierName = identifier.GetText();
                if (identifierName != null)
                {
                    SymbolTable.Add(identifier.Symbol, t);
                    SymbolTable[identifier.Symbol] = (t, TypeExtensions.DefaultValue(t));
                }
                else
                {
                    //errors.Add("Couldn't declare var.");
                    Errors.ReportError(context.Start, "Couldn't declare var.");
                }
            }
        }

        public override void ExitRead([NotNull] grammarProjAParser.ReadContext context)
        {
            base.ExitRead(context);
        }

        public override void ExitWrite([NotNull] grammarProjAParser.WriteContext context)
        {
            base.ExitWrite(context);
        }

        public override void ExitBlock([NotNull] grammarProjAParser.BlockContext context)
        {
            base.ExitBlock(context);
        }

        public override void ExitIfElse([NotNull] grammarProjAParser.IfElseContext context)
        {
            var condition = values.Get(context.expression());
            if (condition.type != Type.Bool)
            {
                //errors.Add("If condition must be boolean.");
                Errors.ReportError(context.Start, "If condition must be boolean.");
            }
            base.ExitIfElse(context);
        }

        public override void ExitWhile([NotNull] grammarProjAParser.WhileContext context)
        {
            var condition = values.Get(context.expression());
            if (condition.type != Type.Bool)
            {
                //errors.Add("If condition must be boolean.");
                Errors.ReportError(context.Start, "While condition must be boolean.");
            }
        }

        public override void ExitEval([NotNull] grammarProjAParser.EvalContext context)
        {
            base.ExitEval(context);
        }
    }
}
