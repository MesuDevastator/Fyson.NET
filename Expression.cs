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

    [GeneratedRegex(@"[+\-*/%]")]
    private static partial Regex OperatorRegex();

    public enum LabelType
    {
        Variable,
        Number,
        Operator,
        Unset
    }

    public static Dictionary<LabelType, string> LexExpression(this string expression)
    {
        Dictionary<LabelType, string> strings = new();
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
                    strings.Add(lastType, lastBuilder.ToString());
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Operator;
                    break;
                case LabelType.Number when expression[index].IsVariable():
                    strings.Add(lastType, lastBuilder.ToString());
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Variable;
                    break;
                case LabelType.Number when expression[index].IsNumber():
                    lastBuilder.Append(expression[index]);
                    break;
                case LabelType.Number when expression[index].IsOperator():
                    strings.Add(lastType, lastBuilder.ToString());
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Operator;
                    break;
                case LabelType.Operator when expression[index].IsVariable():
                    strings.Add(lastType, lastBuilder.ToString());
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Variable;
                    break;
                case LabelType.Operator when expression[index].IsNumber():
                    strings.Add(lastType, lastBuilder.ToString());
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Number;
                    break;
                case LabelType.Operator when expression[index].IsOperator():
                    strings.Add(lastType, lastBuilder.ToString());
                    lastBuilder.Clear().Append(expression[index]);
                    lastType = LabelType.Operator;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lastType), lastType,
                        $"{nameof(lastType)} out of range");
            }
        }

        strings.Add(lastType, lastBuilder.ToString());
        return strings;
    }

    private static bool IsVariable(this char character) => VariableRegex().IsMatch(character.ToString());

    private static bool IsNumber(this char character) => NumberRegex().IsMatch(character.ToString());

    private static bool IsOperator(this char character) => OperatorRegex().IsMatch(character.ToString());
}

public class Expression
{
    private double ParseValue(string expression, Context context)
    {
        var strings = expression.LexExpression();
        return 0;
    }

    private string ParseExpression(string expression, Context context)
    {
        if (!expression.StartsWith('§'))
            return expression;
        // TODO: Use StringBuilder
        for (var index = 0; index < expression.Length - 2; index++)
        {
            if (expression.Substring(index, 2) != "#{")
                continue;
            var fullExpression = expression.Substring(index + 2, expression.IndexOf('}', index) - index - 2);
            expression = expression.Replace(fullExpression,
                ParseValue(fullExpression, context).ToString(CultureInfo.CurrentCulture));
        }

        return expression;
    }

    public Expression(string expression, Context context)
    {
        RawExpression = expression;
        ExpressionResult = ParseExpression(expression, context);
    }

    public string RawExpression { get; }

    public string ExpressionResult { get; }
}