namespace RuleEngine
{
    /// <summary>
    /// interface that all objects must implement to be applied to
    /// the <see cref="RuleApplicant"/> RuleApplicant
    /// </summary>
    public interface IRuleProcess
    {
        // result from running rule successfully
        object Result { get; set; }
        // did we halt
        bool Halted { get; set; }
    }
}