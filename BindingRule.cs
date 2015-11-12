using System;
using System.Collections.Generic;

namespace RuleEngine
{
    public class BindingRule
    {
        readonly IEnumerable<BindingCondition> _conditions;
        readonly object _then;
        readonly object _else;

        public BindingRule(IEnumerable<BindingCondition> conditions, object then, object @else)
        {
            this._conditions = conditions;
            this._then = then;
            this._else = @else;
        }

        public IEnumerable<BindingCondition> Conditions
        {
            get { return this._conditions; }
        }

        public object Then
        {
            get { return this._then; }
        }

        public object Else
        {
            get { return this._else; }
        }

        public bool IsHalt(object field)
        {
            return ((string)field) == "Halt";
        }

        public class BindingCondition
        {
            readonly Guid _id;
            readonly string _propertyName;
            readonly object _operand;
            readonly object _value;
            readonly Conditional _conditional;

            public BindingCondition(Guid id, string propertyName, object operand, object value, Conditional conditional)
            {
                this._id = id;
                this._propertyName = propertyName;
                this._operand = operand;
                this._value = value;
                this._conditional = conditional;
            }

            public Guid Id
            {
                get { return this._id; }
            }

            public string PropertyName
            {
                get { return this._propertyName; }
            }

            public object Operand
            {
                get { return this._operand; }
            }

            public object Value
            {
                get { return this._value; }
            }

            public Conditional Conditional
            {
                get { return this._conditional; }
            }

        }

    }
}