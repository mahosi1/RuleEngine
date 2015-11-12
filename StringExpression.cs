using System;
using System.CodeDom;

namespace RuleEngine
{
    /// <summary>
    /// represents a string expression of length or contents
    /// </summary>
    [Serializable]
    class StringExpression : BaseExpression
    {
        static readonly CodeTypeReferenceExpression CodeTypeReferenceExpression =
            new CodeTypeReferenceExpression(typeof (StringExpression).FullName);
        static readonly CodePrimitiveExpression CodePrimitiveExpressionTrue = 
            new CodePrimitiveExpression(true);
        readonly CodeBinaryOperatorExpression _binaryExpression;


        /// <summary>
        /// string length functions
        /// </summary>
        /// <example>
        /// "abc".Length >= 3 ?
        /// </example>
        public StringExpression(
            string stringProperty,
            NumericOperand operand,
            int value,
            Conditional conditional)
            : base(stringProperty, value, operand, conditional)
        {
            this._binaryExpression = new CodeBinaryOperatorExpression
                {
                    Left =
                        new CodeMethodInvokeExpression(
                            CodeTypeReferenceExpression, "LengthFunction",
                            new CodeExpression[]
                                {
                                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
                                                                        stringProperty),
                                    new CodePrimitiveExpression(value),
                                    new CodePrimitiveExpression(operand),
                                }),
                    Operator = CodeBinaryOperatorType.ValueEquality,
                    Right = CodePrimitiveExpressionTrue
                };
        }

        /// <summary>
        /// string content functions
        /// </summary>
        /// <example>
        /// "abcdefg".StartsWith("abc") ?
        /// </example>
        public StringExpression(
            string stringProperty,
            StringOperand operand,
            string value,
            Conditional conditional)
            : base(stringProperty, value, operand, conditional)
        {
            this._binaryExpression = new CodeBinaryOperatorExpression
                {
                    Left =
                        new CodeMethodInvokeExpression(
                            CodeTypeReferenceExpression, "StringFunction",
                            new CodeExpression[]
                                {
                                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
                                                                        stringProperty),
                                    new CodePrimitiveExpression(value),
                                    new CodePrimitiveExpression(operand),
                                }),
                    Operator = CodeBinaryOperatorType.ValueEquality,
                    Right = CodePrimitiveExpressionTrue
                };
        }

        public override CodeBinaryOperatorExpression CodeBinaryOperatorExpression
        {
            get { return _binaryExpression; }
        }

        /// <summary>
        /// instance string methods could bomb due to null, so we just wrap these
        /// </summary>
        /// <remarks>unfortunately this must remain public for the codedom dispatch</remarks>
        public static bool StringFunction(string a, string b, StringOperand operand)
        {
            // a is null ?
            if (operand == StringOperand.Equals && a == null && b == null) return true;

            if (null == a) return false;
            if (operand == StringOperand.Equals)
                return a == b;
            if (operand == StringOperand.DoesNotEqual)
                return a != b;

            if (null == b) return false;

            if (operand == StringOperand.StartsWith)
                return a.StartsWith(b);
            if (operand == StringOperand.DoesNotStartWith)
                return !a.StartsWith(b);
            if (operand == StringOperand.Contains)
                return a.Contains(b);
            if (operand == StringOperand.DoesNotContain)
                return !a.Contains(b);
            if (operand == StringOperand.EndsWith)
                return a.EndsWith(b);
            if (operand == StringOperand.DoesNotEndWith)
                return !a.EndsWith(b);
            throw new ArgumentException("operand " + operand + " not mapped");
        }

        /// <summary>
        /// instance string methods could bomb due to null, so we just wrap these
        /// </summary>
        /// <remarks>unfortunately this must remain public for the codedom dispatch</remarks>
        public static bool LengthFunction(string a, int length, NumericOperand operand)
        {
            if (null == a) return false;
            if (operand == NumericOperand.GreaterThan)
                return a.Length > length;
            if (operand == NumericOperand.GreaterThanOrEqual)
                return a.Length >= length;
            if (operand == NumericOperand.LessThan)
                return a.Length < length;
            if (operand == NumericOperand.LessThanOrEqual)
                return a.Length <= length;
            if (operand == NumericOperand.NotEqual)
                return a.Length != length;
            if (operand == NumericOperand.ValueEquality)
                return a.Length == length;
            throw new ArgumentException("operand " + operand + " not mapped");
        }
    }
}