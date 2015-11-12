using System.Workflow.Activities.Rules;

namespace RuleEngine
{
    /// <summary>
    /// Rule Engine
    /// </summary>
    public static class RuleApplicant
    {
        public static void Apply(IRuleProcess target, RuleSet ruleset) 
        {
            var execution = new RuleExecution(new RuleValidation(target.GetType(), null), target);
            ruleset.Execute(execution);
            target.Halted = execution.Halted;
        }
    }
}