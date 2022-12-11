namespace Fyson;

public class Variable
{
    public Variable(string name, object? value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public object? Value { get; set; }
}

public class Variable<T> : Variable
{
    public Variable(string name, T value) : base(name, value) => Value = value;

    public new T Value { get; set; }
}