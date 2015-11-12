namespace RuleEngine
{
    public class TestObject : IRuleProcess
    {
        public string StringField { get; set; }
        public int IntField { get; set; }

        public object Result { get; set; }
        public int? RuleId { get; set; }
        public int AccountId { get; set; }
        public bool Halted { get; set; }
    }
}