using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Workflow.Activities.Rules;

namespace RuleEngine
{
    /// <summary>
    /// strongly typed RuleSet wrapper for numeric and string expressions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RuleHelper<T> : IBuildRuleSet where T : IRuleProcess 
    {
        const string ResultProperty = "Result";
        const string Halt = "Halt";
        readonly List<BaseExpression> _expressions = new List<BaseExpression>();
        readonly string[] _props;
        object _elseValue;
        object _thenValue;


        public RuleHelper(string ruleName) : this()
        {
            this.RuleName = ruleName;
        }

        public RuleHelper()
        {
            this._props = typeof (T).GetProperties().Select(x => x.Name).ToArray();
        }

        public static RuleHelper<T> Deserialize(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = 0;
                return (RuleHelper<T>) new BinaryFormatter().Deserialize(stream);
            }
        }

        public string RuleName { get; set; }

        public int RemoveCondition(Guid conditionId)
        {
            BaseExpression item = this._expressions.Single(x => x.Guid == conditionId);
            int index = this._expressions.IndexOf(item);
            this._expressions.Remove(item);
            return index;
        }

        public bool RemoveCondition(int index)
        {
            if (index <= this._expressions.Count - 1)
            {
                this._expressions.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void Then(object value)
        {
            this.VerifyProperty(ResultProperty);
            this._thenValue = value;
        }

        public void Then(bool halt)
        {
            this.Then(halt ? Halt : null);
        }

        public void Else(object value)
        {
            if (null == this._thenValue)
                throw new InvalidOperationException("must first set Then");
            this._elseValue = value;
        }

        public void Else(bool halt)
        {
            this.Else(halt ? Halt : null);
        }

        #region Strings

        public Guid AddCondition(
            Expression<Func<T, string>> stringProperty,
            StringOperand operand,
            string value,
            Conditional conditional)
        {
            return AddCondition(
                GetPropertyNameString(stringProperty),
                operand,
                value,
                conditional);
        }

        public Guid AddCondition(
            string stringProperty,
            StringOperand operand,
            string value,
            Conditional conditional)
        {
            this.VerifyProperty(stringProperty);
            var expression = new StringExpression(stringProperty, operand, value, conditional);
            this._expressions.Add(expression);
            return expression.Guid;
        }

        public Guid AddCondition(
            Expression<Func<T, string>> stringProperty,
            NumericOperand operand,
            int value,
            Conditional conditional)
        {
            return AddCondition(
                GetPropertyNameString(stringProperty),
                operand,
                value,
                conditional);
        }

        public Guid AddCondition(
            string stringProperty,
            NumericOperand operand,
            int value,
            Conditional conditional)
        {
            this.VerifyProperty(stringProperty);
            var expression = new StringExpression(stringProperty, operand, value, conditional);
            this._expressions.Add(expression);
            return expression.Guid;
        }


        public Guid ReplaceCondition(
            Guid guid,
            Expression<Func<T, string>> property,
            StringOperand operand,
            string value,
            Conditional conditional)
        {
            return ReplaceCondition(
                guid,
                GetPropertyNameString(property),
                operand,
                value,
                conditional);
        }


        public Guid ReplaceCondition(Guid guid, string property, StringOperand operand, string value,
                                     Conditional conditional)
        {
            this.VerifyProperty(property);
            int extant = this.RemoveCondition(guid);
            var expression = new StringExpression(property, operand, value, conditional);
            this._expressions.Insert(extant, expression);
            return expression.Guid;
        }

        public Guid ReplaceCondition(Guid guid, string property, NumericOperand operand, int value,
                                     Conditional conditional)
        {
            this.VerifyProperty(property);
            int extant = this.RemoveCondition(guid);
            var expression = new StringExpression(property, operand, value, conditional);
            this._expressions.Insert(extant, expression);
            return expression.Guid;
        }

        public Guid ReplaceCondition(Guid guid, Expression<Func<T, string>> property, NumericOperand operand, int value,
                                     Conditional conditional)
        {
            return ReplaceCondition(guid, GetPropertyNameString(property), operand, value, conditional);
        }

        #endregion

        #region Numerics

        public Guid AddCondition(Expression<Func<T, double>> numericProperty, NumericOperand operand, double value,
                                 Conditional conditional)
        {
            return AddCondition(GetPropertyNameString(numericProperty), operand, value, conditional);
        }

        public Guid AddCondition(string numericProperty, NumericOperand operand, double value, Conditional conditional)
        {
            this.VerifyProperty(numericProperty);
            var expression = new NumericExpression(numericProperty, operand, value, conditional);
            this._expressions.Add(expression);
            return expression.Guid;
        }


        public Guid ReplaceCondition(Expression<Func<T, int>> numericProperty, NumericOperand numericOperand,
                                     double value, Guid guid,
                                     Conditional conditional)
        {
            return this.ReplaceCondition(GetPropertyNameString(numericProperty), numericOperand, value, guid,
                                         conditional);
        }

        public Guid ReplaceCondition(string numericProperty, NumericOperand numericOperand, double value, Guid guid,
                                     Conditional conditional)
        {
            this.VerifyProperty(numericProperty);
            int extant = this.RemoveCondition(guid);
            var expression = new NumericExpression(numericProperty, numericOperand, value, conditional);
            this._expressions.Insert(extant, expression);
            return expression.Guid;
        }

        #endregion

        public RuleSet BuildRuleSet()
        {
            var ruleSet = new RuleSet(this.RuleName);
            var rule = new Rule(this.RuleName);
            ruleSet.Rules.Add(rule);
            CodeBinaryOperatorExpression current = null;
            var conditional = CodeBinaryOperatorType.Divide;

            foreach (BaseExpression expression in this._expressions)
            {
                if (null == current)
                {
                    current = expression.CodeBinaryOperatorExpression;
                    conditional = expression.ConditionalType;
                }
                else
                {
                    current = new CodeBinaryOperatorExpression
                        {
                            Left = current,
                            Operator = conditional,
                            Right = expression.CodeBinaryOperatorExpression
                        };
                    conditional = expression.ConditionalType;
                }
            }

            rule.Condition = new RuleExpressionCondition(current);

            if (this._thenValue as string == Halt)
                rule.ThenActions.Add(new RuleHaltAction());
            else
                rule.ThenActions.Add(Make(this._thenValue));

            if (null != this._elseValue)
            {
                if (this._elseValue as string == Halt)
                    rule.ElseActions.Add(new RuleHaltAction());
                else
                    rule.ElseActions.Add(Make(this._elseValue));
            }
            return ruleSet;
        }

        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, this);
                return stream.ToArray();
            }
        }

        void VerifyProperty(string propertyName)
        {
            if (this._props.All(x => x != propertyName))
                throw new ArgumentException("can't find property " + propertyName);
        }

        static RuleStatementAction Make(object value)
        {
            return new RuleStatementAction(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        new CodeThisReferenceExpression(), ResultProperty),
                    new CodePrimitiveExpression(value)));
        }

        static string GetPropertyNameString<TOwner, TProperty>(
            Expression<Func<TOwner, TProperty>> propertyGetter)
        {
            MemberExpression expression =
                propertyGetter.Body as MemberExpression
                ?? (MemberExpression) ((UnaryExpression) propertyGetter.Body).Operand;
            return expression.Member.Name;
        }
    }
}
