namespace PolicyManager.Lexer.Models
{
    public class PolicyVisitorResult
    {
        public string StringResult { get; set; }

        public double? NumberResult { get; set; }

        public bool? BooleanResult { get; set; }

        public override int GetHashCode()
        {
            if (!string.IsNullOrWhiteSpace(StringResult))
            {
                return StringResult.GetHashCode();
            }

            if (NumberResult.HasValue)
            {
                return NumberResult.Value.GetHashCode();
            }

            if (BooleanResult.HasValue)
            {
                return BooleanResult.Value.GetHashCode();
            }

            return 0;
        }

        public override bool Equals(object obj)
        {
            var policyVisitorResult = obj as PolicyVisitorResult;
            if (!string.IsNullOrWhiteSpace(StringResult))
            {
                return StringResult.Equals(policyVisitorResult.StringResult);
            }

            if (NumberResult.HasValue)
            {
                return NumberResult.Equals(policyVisitorResult.NumberResult);
            }

            if (BooleanResult.HasValue)
            {
                return BooleanResult.Equals(policyVisitorResult.BooleanResult);
            }

            return false;
        }
    }
}
