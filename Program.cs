using System;

namespace RuleEngine
{
    class Program
    {
        public static void Main()
        {
            var obj = new TestObject { AccountId = 10, StringField = "cde" };
            var helper1 = new RuleHelper<TestObject>("rule1");

            helper1.AddCondition(x => x.StringField, StringOperand.DoesNotEqual, null, Conditional.And);
            helper1.AddCondition(x => x.StringField, StringOperand.DoesNotContain, "de", Conditional.Or);
            helper1.Then("then");
            helper1.Else("else");
            obj.Apply(helper1);
            Console.Out.WriteLine(obj.Result);
            //helper1 = rules.First().RuleHelper;
            //helper1.Then("a");
            //helper1.Else("b");
            //provider.Update(rules);
            //helper1.RemoveCondition(0);


            //helper1.AddCondition(x => x.StringField, StringOperand.DoesNotEqual, null, Conditional.And);
            //helper1.AddCondition(x => x.IntField, NumericOperand.GreaterThanOrEqual, 8, Conditional.Or);
            //helper1.AddCondition(x => x.StringField, StringOperand.Contains, "def", Conditional.Or);

            //var bindingRule = helper1.GenerateBindingRule();

            //helper1.AddCondition()
            //rules[0].Order = 9;
            //helper1.RemoveCondition(1);
            //provider.Update(rules);

            //provider.Update(rules);









            //var helper = new RuleHelper<TestObject>("whatever");
            //helper.AddCondition(x => x.StringField, StringOperand.Equals, null, Conditional.And);
            //helper.AddCondition(x => x.IntField, NumericOperand.GreaterThan, 1, Conditional.And);
            //helper.Then("isnull");
            //helper.Else("isnotnull");





            //provider.Add(new RuleMapping<TestObject>(1, helper, obj.AccountId));


            //RuleApplicant.Apply(obj, provider);
            //provider.Dispose();



        }
    }
}
