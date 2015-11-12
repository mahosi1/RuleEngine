using System;
using System.CodeDom;

namespace RuleEngine
{
    /// <summary>
    /// represents common fields for an expression
    /// </summary>
    [Serializable]
    abstract class BaseExpression
    {
        readonly string _propertyName;
        readonly object _value;
        readonly object _operand;
        readonly Conditional _conditional;
        readonly Guid _guid;

        protected BaseExpression(string propertyName, object value, object operand, Conditional conditional)
        {
            this._guid = Guid.NewGuid();
            this._propertyName = propertyName;
            this._value = value;
            this._operand = operand;
            this._conditional = conditional;
        }

        /// <summary>
        /// unique id for expression lookups
        /// </summary>
        public Guid Guid
        {
            get { return this._guid; }
        }

        public Conditional Conditional
        {
            get { return this._conditional; }
        }

        public object Value
        {
            get { return this._value; }
        }

        /// <summary>
        /// this will be some enumeration you need to cast back to (either NumericOperand or StringOperand)
        /// </summary>
        public object Operand
        {
            get { return this._operand; }
        }

        public string PropertyName
        {
            get { return this._propertyName; }
        }

        public CodeBinaryOperatorType ConditionalType
        {
            get
            {
                return this.Conditional == Conditional.And
                           ? CodeBinaryOperatorType.BooleanAnd
                           : CodeBinaryOperatorType.BooleanOr;
            }
        }

        public abstract CodeBinaryOperatorExpression CodeBinaryOperatorExpression { get; }

    }
}