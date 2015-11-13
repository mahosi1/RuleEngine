# RuleEngine
## C# Rule Engine

### 4 steps

1. Auto-Implement IRuleProcess on your target object 
```
    public class TestObject : IRuleProcess 
    { 
        ...
        public object Result { get; set; }
        public bool Halted { get; set; }
        ...
    }
    ...  
    var obj = new TestObject { AccountId = 10, StringField = "some string" };
```

2. Build a strongly-typed RuleHelper of your target type
```
    var helper = new RuleHelper<TestObject>();
```  

3. Apply conditions and rules
```
    helper.AddCondition(x => x.StringField, StringOperand.DoesNotEqual, null, Conditional.And);
    helper.AddCondition(x => x.StringField, StringOperand.DoesNotContain, "str", Conditional.Or);
    helper.Then("then case");
    helper.Else("else case");
```

4. Dispatch Rule Engine (extension method on IRuleProcess)
```
    obj.Apply(helper);
    Console.Out.WriteLine(obj.Result); // == "then case"
```
