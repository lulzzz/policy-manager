using System;

namespace PolicyManager.Lexer.Models
{
    public class ReturnValue
    {
        private object value;

        public ReturnValue(object value)
        {
            this.value = value;
        }

        public bool IsBoolean()
        {
            return bool.TryParse(value?.ToString(), out bool result);
        }

        public bool ToBoolean()
        {
            return bool.Parse(value?.ToString());
        }

        public double ToDouble()
        {
            return double.Parse(value?.ToString());
        }

        public bool IsDouble()
        {
            return value is double;
        }

        public override int GetHashCode()
        {
            if (value == null) return 0;

            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (value == obj) return true;

            if (value == null || obj == null) return false;

            var otherValue = obj as ReturnValue;
            return value.Equals(otherValue.value);
        }

        public override string ToString()
        {
            return Convert.ToString(value);
        }
    }
}
