using System.Workflow.Activities.Rules;

namespace RuleEngine
{
    public interface IBuildRuleSet
    {
        RuleSet BuildRuleSet();
    }
}