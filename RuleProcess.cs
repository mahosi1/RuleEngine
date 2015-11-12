namespace MortgageReturns.RecordManagement.RuleEngineV2
{
    public class RuleProcess : IRuleProcess
    {
        public object Result { get; set; }
        public int? RuleId { get; set; }
        public int AccountId { get; set; }
        public bool Halted { get; set; }
    }
}