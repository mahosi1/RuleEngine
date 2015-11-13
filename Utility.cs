using System.Workflow.Activities.Rules;

namespace RuleEngine
{
    public static class Utility
    {
        public static void Apply(this IRuleProcess target, IBuildRuleSet ruleSetBuilder)
        {
            var execution = new RuleExecution(new RuleValidation(target.GetType(), null), target);
            ruleSetBuilder.BuildRuleSet().Execute(execution);
            target.Halted = execution.Halted;
        }
    }
}