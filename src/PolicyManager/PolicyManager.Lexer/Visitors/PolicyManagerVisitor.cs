using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using PolicyManager.Lexer.Grammar;
using PolicyManager.Lexer.Models;
using System;
using System.Collections.Generic;

namespace PolicyManager.Lexer.Visitors
{
    public class PolicyManagerVisitor
        : PolicyManagerBaseVisitor<ReturnValue>
    {
        private readonly IDictionary<string, string> initialState = new Dictionary<string, string>();
        private IDictionary<string, ReturnValue> memory = new Dictionary<string, ReturnValue>();

        public PolicyManagerVisitor(Dictionary<string, string> state)
        {
            initialState = state;
            foreach (var kvp in initialState)
            {
                memory.Add(kvp.Key, new ReturnValue(kvp.Value));
            }
        }

        public override ReturnValue VisitTerminal([NotNull] ITerminalNode node)
        {
            if (memory.ContainsKey("return"))
            {
                return memory["return"];
            }

            return base.VisitTerminal(node);
        }

        public override ReturnValue VisitAssignment([NotNull] PolicyManagerParser.AssignmentContext context)
        {
            var id = context.ID().GetText();
            var value = Visit(context.expr());
            if (memory.ContainsKey(id))
            {
                memory[id] = value;
            }
            else
            {
                memory.Add(id, value);
            }

            return memory[id];
        }

        public override ReturnValue VisitIdAtom([NotNull] PolicyManagerParser.IdAtomContext context)
        {
            var id = context.GetText();
            if (memory.ContainsKey(id))
            {
                return memory[id];
            }

            throw new InvalidOperationException($"No such variable: {id}");
        }

        public override ReturnValue VisitStringAtom([NotNull] PolicyManagerParser.StringAtomContext context)
        {
            var str = context.GetText();
            str = str.Replace("\"", string.Empty);
            return new ReturnValue(str);
        }

        public override ReturnValue VisitNumberAtom([NotNull] PolicyManagerParser.NumberAtomContext context)
        {
            var str = context.GetText();
            var value = double.Parse(str);
            return new ReturnValue(value);
        }

        public override ReturnValue VisitBooleanAtom([NotNull] PolicyManagerParser.BooleanAtomContext context)
        {
            var str = context.GetText();
            var value = bool.Parse(str);
            return new ReturnValue(value);
        }

        public override ReturnValue VisitNilAtom([NotNull] PolicyManagerParser.NilAtomContext context)
        {
            return default(ReturnValue);
        }

        public override ReturnValue VisitParExpr([NotNull] PolicyManagerParser.ParExprContext context)
        {
            return Visit(context.expr());
        }

        public override ReturnValue VisitPowExpr([NotNull] PolicyManagerParser.PowExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));
            return new ReturnValue(Math.Pow(left.ToDouble(), right.ToDouble()));
        }

        public override ReturnValue VisitUnaryMinusExpr([NotNull] PolicyManagerParser.UnaryMinusExprContext context)
        {
            var value = Visit(context.expr());
            return new ReturnValue(value.ToDouble());
        }

        public override ReturnValue VisitNotExpr([NotNull] PolicyManagerParser.NotExprContext context)
        {
            var value = Visit(context.expr());
            return new ReturnValue(!value.ToBoolean());
        }

        public override ReturnValue VisitMultiplicationExpr([NotNull] PolicyManagerParser.MultiplicationExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            var returnValue = default(ReturnValue);
            switch (context.op.Type)
            {
                case PolicyManagerParser.MULT:
                    returnValue = new ReturnValue(left.ToDouble() * right.ToDouble());
                    break;

                case PolicyManagerParser.DIV:
                    returnValue = new ReturnValue(left.ToDouble() / right.ToDouble());
                    break;

                case PolicyManagerParser.MOD:
                    returnValue = new ReturnValue(left.ToDouble() % right.ToDouble());
                    break;

                default:
                    throw new InvalidOperationException($"Unknown Operator: {PolicyManagerParser.DefaultVocabulary.GetDisplayName(context.op.Type)}");
            }

            return returnValue;
        }

        public override ReturnValue VisitAdditiveExpr([NotNull] PolicyManagerParser.AdditiveExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            var returnValue = default(ReturnValue);
            switch (context.op.Type)
            {
                case PolicyManagerParser.PLUS:
                    if (left.IsDouble() && right.IsDouble())
                    {
                        returnValue = new ReturnValue(left.ToDouble() + right.ToDouble());
                    }
                    else
                    {
                        returnValue = new ReturnValue(left.ToString() + right.ToString());
                    }
                    break;

                case PolicyManagerParser.MINUS:
                    returnValue = new ReturnValue(left.ToDouble() - right.ToDouble());
                    break;

                default:
                    throw new InvalidOperationException($"Unknown Operator: {PolicyManagerParser.DefaultVocabulary.GetDisplayName(context.op.Type)}");
            }

            return returnValue;
        }

        public override ReturnValue VisitRelationalExpr([NotNull] PolicyManagerParser.RelationalExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            var returnValue = default(ReturnValue);
            switch (context.op.Type)
            {
                case PolicyManagerParser.LT:
                    returnValue = new ReturnValue(left.ToDouble() < right.ToDouble());
                    break;

                case PolicyManagerParser.LTEQ:
                    returnValue = new ReturnValue(left.ToDouble() <= right.ToDouble());
                    break;

                case PolicyManagerParser.GT:
                    returnValue = new ReturnValue(left.ToDouble() > right.ToDouble());
                    break;

                case PolicyManagerParser.GTEQ:
                    returnValue = new ReturnValue(left.ToDouble() >= right.ToDouble());
                    break;

                default:
                    throw new InvalidOperationException($"Unknown Operator: {PolicyManagerParser.DefaultVocabulary.GetDisplayName(context.op.Type)}");
            }

            return returnValue;
        }

        public override ReturnValue VisitEqualityExpr([NotNull] PolicyManagerParser.EqualityExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            var returnValue = default(ReturnValue);
            switch (context.op.Type)
            {
                case PolicyManagerParser.EQ:
                    if (left.IsDouble() && right.IsDouble())
                    {
                        returnValue = new ReturnValue(Math.Abs(left.ToDouble() - right.ToDouble()) < 0);
                    }
                    else
                    {
                        returnValue = new ReturnValue(left.Equals(right));
                    }
                    break;

                case PolicyManagerParser.NEQ:
                    if (left.IsDouble() && right.IsDouble())
                    {
                        returnValue = new ReturnValue(Math.Abs(left.ToDouble() - right.ToDouble()) >= 0);
                    }
                    else
                    {
                        returnValue = new ReturnValue(!left.Equals(right));
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown Operator: {PolicyManagerParser.DefaultVocabulary.GetDisplayName(context.op.Type)}");
            }

            return returnValue;
        }

        public override ReturnValue VisitAndExpr([NotNull] PolicyManagerParser.AndExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            return new ReturnValue(left.ToBoolean() && right.ToBoolean());
        }

        public override ReturnValue VisitOrExpr([NotNull] PolicyManagerParser.OrExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            return new ReturnValue(left.ToBoolean() || right.ToBoolean());
        }

        public override ReturnValue VisitLog([NotNull] PolicyManagerParser.LogContext context)
        {
            var value = Visit(context.expr());
            // TODO: switch to ILogger...
            Console.WriteLine(value);
            return value;
        }

        public override ReturnValue VisitIf_stat([NotNull] PolicyManagerParser.If_statContext context)
        {
            var conditions = context.condition_block();
            var evaluatedBlock = false;

            foreach (var condition in conditions)
            {
                var evaluated = Visit(condition.expr());
                if (evaluated.IsBoolean() && evaluated.ToBoolean())
                {
                    evaluatedBlock = true;
                    Visit(condition.stat_block());
                    break;
                }
            }

            if (!evaluatedBlock && context.stat_block() != null)
            {
                Visit(context.stat_block());
            }

            return default(ReturnValue);
        }

        public override ReturnValue VisitWhile_stat([NotNull] PolicyManagerParser.While_statContext context)
        {
            var value = Visit(context.expr());

            while (value.ToBoolean())
            {
                Visit(context.stat_block());

                value = Visit(context.expr());
            }

            return default(ReturnValue);
        }
    }
}
