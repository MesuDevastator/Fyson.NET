namespace Fyson;

public class Context
{
    private readonly List<Variable> _variables = new();

    public void SetVariable(string name, object? value)
    {
        lock (_variables)
        {
            Variable? variable;
            if ((variable = GetVariable(name)) is null)
                _variables.Add(new Variable(name, value));
            else
                variable.Value = value;
        }
    }

    public void SetVariable<T>(string name, T value)
    {
        lock (_variables)
        {
            Variable<T>? variable;
            if ((variable = GetVariable<T>(name)) is null)
                _variables.Add(new Variable(name, value));
            else
                variable.Value = value;
        }
    }

    public Variable? GetVariable(string name)
    {
        lock (_variables)
            return (from var in _variables where var.Name == name select var).FirstOrDefault();
    }

    public Variable<T>? GetVariable<T>(string name)
    {
        lock (_variables)
            return (Variable<T>?)(from var in _variables where var is Variable<T> where var.Name == name select var)
                .FirstOrDefault();
    }
}