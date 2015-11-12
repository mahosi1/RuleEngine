using System;
using System.CodeDom;

namespace RuleEngine
{
    /// <summary>
    /// Represents some numeric boolean expression
    /// </summary>
    /// <remarks>can be double or anything less than a double</remarks>
    [Serializable]
    class NumericExpression : BaseExpression
    {
        readonly CodeBinaryOperatorExpression _binaryExpression;

        public NumericExpression(
            string numericProperty,
            NumericOperand numericOperand,
            double value,
            Conditional conditional)
            : base(numericProperty, value, numericOperand, conditional)
        {
            this._binaryExpression =
                new CodeBinaryOperatorExpression(
                    new CodeCastExpression(
                        "System.Double",
                        new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(),
                            numericProperty)),
                    (CodeBinaryOperatorType)
                    Enum.Parse(typeof(CodeBinaryOperatorType), numericOperand.ToString(), true),
                    new CodePrimitiveExpression(value));
        }

        public override CodeBinaryOperatorExpression CodeBinaryOperatorExpression
        {
            get { return this._binaryExpression; }
        }

    }
}