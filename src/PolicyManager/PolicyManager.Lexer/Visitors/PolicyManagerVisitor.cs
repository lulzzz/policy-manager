using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using PolicyManager.Lexer.Models;
using System;
using System.Collections.Generic;

namespace PolicyManager.Lexer.Visitors
{
    public class PolicyManagerVisitor
        : PolicyManagerBaseVisitor<PolicyVisitorResult>
    {
        private readonly IDictionary<string, string> initialState = new Dictionary<string, string>();
        private IDictionary<string, PolicyVisitorResult> memory = new Dictionary<string, PolicyVisitorResult>();

        public PolicyManagerVisitor(Dictionary<string, string> state)
        {
            initialState = state;
            foreach (var kvp in initialState)
            {
                memory.Add(kvp.Key, new PolicyVisitorResult() { StringResult = kvp.Value });
            }
        }

        public override PolicyVisitorResult VisitTerminal([NotNull] ITerminalNode node)
        {
            if (memory.ContainsKey("return"))
            {
                return memory["return"];
            }

            return base.VisitTerminal(node);
        }

        public override PolicyVisitorResult VisitAssignment([NotNull] PolicyManagerParser.AssignmentContext context)
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

        public override PolicyVisitorResult VisitIdAtom([NotNull] PolicyManagerParser.IdAtomContext context)
        {
            var id = context.GetText();
            if (memory.ContainsKey(id))
            {
                return memory[id];
            }

            throw new InvalidOperationException($"No such variable: {id}");
        }

        public override PolicyVisitorResult VisitStringAtom([NotNull] PolicyManagerParser.StringAtomContext context)
        {
            var str = context.GetText();
            str = str.Replace("\"", string.Empty);
            return new PolicyVisitorResult() { StringResult = str };
        }

        public override PolicyVisitorResult VisitNumberAtom([NotNull] PolicyManagerParser.NumberAtomContext context)
        {
            var str = context.GetText();
            var value = double.Parse(str);
            return new PolicyVisitorResult() { NumberResult = value };
        }

        public override PolicyVisitorResult VisitBooleanAtom([NotNull] PolicyManagerParser.BooleanAtomContext context)
        {
            var str = context.GetText();
            var value = bool.Parse(str);
            return new PolicyVisitorResult() { BooleanResult = value };
        }

        public override PolicyVisitorResult VisitNilAtom([NotNull] PolicyManagerParser.NilAtomContext context)
        {
            return new PolicyVisitorResult();
        }

        public override PolicyVisitorResult VisitParExpr([NotNull] PolicyManagerParser.ParExprContext context)
        {
            return Visit(context.expr());
        }

        public override PolicyVisitorResult VisitPowExpr([NotNull] PolicyManagerParser.PowExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));
            return new PolicyVisitorResult() { NumberResult = Math.Pow(left.NumberResult.Value, right.NumberResult.Value) };
        }

        public override PolicyVisitorResult VisitUnaryMinusExpr([NotNull] PolicyManagerParser.UnaryMinusExprContext context)
        {
            var value = Visit(context.expr());
            return new PolicyVisitorResult() { NumberResult = -1 * value.NumberResult };
        }

        public override PolicyVisitorResult VisitNotExpr([NotNull] PolicyManagerParser.NotExprContext context)
        {
            var value = Visit(context.expr());
            return new PolicyVisitorResult() { BooleanResult = !value.BooleanResult };
        }

        public override PolicyVisitorResult VisitMultiplicationExpr([NotNull] PolicyManagerParser.MultiplicationExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            var policyVisitorResult = new PolicyVisitorResult();
            switch (context.op.Type)
            {
                case PolicyManagerParser.LT:
                    policyVisitorResult = new PolicyVisitorResult() { BooleanResult = left.NumberResult < right.NumberResult };
                    break;

                case PolicyManagerParser.LTEQ:
                    policyVisitorResult = new PolicyVisitorResult() { BooleanResult = left.NumberResult <= right.NumberResult };
                    break;

                case PolicyManagerParser.GT:
                    policyVisitorResult = new PolicyVisitorResult() { BooleanResult = left.NumberResult > right.NumberResult };
                    break;

                case PolicyManagerParser.GTEQ:
                    policyVisitorResult = new PolicyVisitorResult() { BooleanResult = left.NumberResult >= right.NumberResult };
                    break;

                default:
                    throw new InvalidOperationException($"Unknown Operator: {PolicyManagerParser.DefaultVocabulary.GetDisplayName(context.op.Type)}");
            }

            return policyVisitorResult;
        }

        public override PolicyVisitorResult VisitEqualityExpr([NotNull] PolicyManagerParser.EqualityExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            var policyVisitorResult = new PolicyVisitorResult();
            switch (context.op.Type)
            {
                case PolicyManagerParser.EQ:
                    if (left.NumberResult.HasValue && right.NumberResult.HasValue)
                    {
                        policyVisitorResult = new PolicyVisitorResult() { BooleanResult = Math.Abs(left.NumberResult.Value - right.NumberResult.Value) < 0 };
                    }
                    else
                    {
                        policyVisitorResult = new PolicyVisitorResult() { BooleanResult = left.Equals(right) };
                    }
                    break;

                case PolicyManagerParser.NEQ:
                    if (left.NumberResult.HasValue && right.NumberResult.HasValue)
                    {
                        policyVisitorResult = new PolicyVisitorResult() { BooleanResult = Math.Abs(left.NumberResult.Value - right.NumberResult.Value) >= 0 };
                    }
                    else
                    {
                        policyVisitorResult = new PolicyVisitorResult() { BooleanResult = !left.Equals(right) };
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown Operator: {PolicyManagerParser.DefaultVocabulary.GetDisplayName(context.op.Type)}");
            }

            return policyVisitorResult;
        }

        public override PolicyVisitorResult VisitAndExpr([NotNull] PolicyManagerParser.AndExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            return new PolicyVisitorResult() { BooleanResult = left.BooleanResult.Value && right.BooleanResult.Value };
        }

        public override PolicyVisitorResult VisitOrExpr([NotNull] PolicyManagerParser.OrExprContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            return new PolicyVisitorResult() { BooleanResult = left.BooleanResult.Value || right.BooleanResult.Value };
        }

        public override PolicyVisitorResult VisitLog([NotNull] PolicyManagerParser.LogContext context)
        {
            var value = Visit(context.expr());
            // TODO: switch to ILogger...
            Console.WriteLine(value);
            return value;
        }

        public override PolicyVisitorResult VisitIf_stat([NotNull] PolicyManagerParser.If_statContext context)
        {
            var conditions = context.condition_block();
            var evaluatedBlock = false;

            foreach (var condition in conditions)
            {
                var evaluated = Visit(condition.expr());
                if (evaluated.BooleanResult.Value)
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

            return default(PolicyVisitorResult);
        }

        public override PolicyVisitorResult VisitWhile_stat([NotNull] PolicyManagerParser.While_statContext context)
        {
            var value = Visit(context.expr());

            while (value.BooleanResult.Value)
            {
                Visit(context.stat_block());

                value = Visit(context.expr());
            }

            return default(PolicyVisitorResult);
        }
    }
}
