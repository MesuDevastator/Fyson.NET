using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Fyson;

public static partial class ExpressionExtension
{
    [GeneratedRegex(@"[A-Za-z_]")]
    private static partial Regex VariableRegex();

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"[~+\-*/%!^&\|=]")]
    private static partial Regex OperatorRegex();

    public enum LabelType
    {
        Variable,
        Number,
        Operator,
        Unset
    }

    public static (List<string>, List<LabelType>) LexExpression(this string expression)
    {
        List<string> strings = new();
        List<LabelType> types = new();
        var lastType = LabelType.Unset;
        var lastBuilder = new StringBuilder();
        for (var index = 0; index < expression.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(expression[index].ToString()))
                continue;
            switch (lastType)
            {
                case LabelType.Unset when expression[index].IsVariable():
                    lastBuilder.Append(expression[index]);
                    lastType = LabelType.Variable;
                    break;
                case LabelType.Unset when expression[index].IsNumber():
                    lastBuilder.Append(expression[index]);
                    lastType = LabelType.Number;
                    break;
                case LabelType.Unset when expression[index].IsOperator():
                    lastBuilder.Append(expression[index]);
                    lastType = LabelType.Operator;
                    break;
                case LabelType.Variable when expression[index].IsVariable():
                    lastBuilder.Append(expression[index]);
                    break;
                case LabelType.Variable when expression[index].IsNumber():
                    lastBuilder.Append(expression[index]);
                    break;
                case LabelType.Variable when expression[index].IsOperator():
                    strings.Add(lastBuilder.ToString());
                    types.Add(lastType);
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Operator;
                    break;
                case LabelType.Number when expression[index].IsVariable():
                    strings.Add(lastBuilder.ToString());
                    types.Add(lastType);
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Variable;
                    break;
                case LabelType.Number when expression[index].IsNumber():
                    lastBuilder.Append(expression[index]);
                    break;
                case LabelType.Number when expression[index].IsOperator():
                    strings.Add(lastBuilder.ToString());
                    types.Add(lastType);
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Operator;
                    break;
                case LabelType.Operator when expression[index].IsVariable():
                    strings.Add(lastBuilder.ToString());
                    types.Add(lastType);
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Variable;
                    break;
                case LabelType.Operator when expression[index].IsNumber():
                    strings.Add(lastBuilder.ToString());
                    types.Add(lastType);
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Number;
                    break;
                case LabelType.Operator when expression[index].IsOperator():
                    lastBuilder.Append(expression[index]);
                    break;
                default:
                    lastBuilder.Append(expression[index]);
                    lastType = LabelType.Unset;
                    break;
            }
        }

        strings.Add(lastBuilder.ToString());
        types.Add(lastType);
        return (strings, types);
    }

    private static bool IsVariable(this char character) => VariableRegex().IsMatch(character.ToString());

    private static bool IsNumber(this char character) => NumberRegex().IsMatch(character.ToString());

    private static bool IsOperator(this char character) => OperatorRegex().IsMatch(character.ToString());
}

public class Expression
{
    public object ParseValue(string expression, Context context)
    {
        var lex = expression.LexExpression();
        for (var index = 0; index < Math.Min(lex.Item1.Count, lex.Item2.Count); index++)
        {
            if (lex.Item2[index] is not ExpressionExtension.LabelType.Variable) continue;
            lex.Item1[index] = context.GetVariable(lex.Item1[index])?.Value?.ToString() ?? string.Empty;
            lex.Item2[index] = ExpressionExtension.LabelType.Number;
        }

        var builder = new StringBuilder();
        foreach (var character in lex.Item1)
            builder.Append(character);
        return new DataTable().Compute(builder.ToString(), "false");
    }

    private string ParseExpression(string expression, Context context)
    {
        if (!expression.StartsWith('§'))
            return expression;
        for (var index = 0; index < expression.Length - 2; index++)
        {
            if (expression.Substring(index, 2) != "#{")
                continue;
            var fullExpression = expression[(index + 2)..expression.IndexOf('}', index)];
            expression = expression.Replace($"#{{{fullExpression}}}", ParseValue(fullExpression, context).ToString());
        }

        return expression.Substring(1, expression.Length - 2);
    }

    public Expression(string expression, Context context)
    {
        RawExpression = expression;
        ExpressionResult = ParseExpression(expression, context);
    }

    public string RawExpression { get; }

    public string ExpressionResult { get; }
}