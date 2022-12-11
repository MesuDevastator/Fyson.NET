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
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (lastType)
            {
                case LabelType.Unset when expression[index].IsVariable():   // -> expression[0] (var)
                    lastType = LabelType.Variable;
                    lastBuilder.Append(expression[index]);                  // APPEND ONLY
                    break;
                case LabelType.Number when expression[index].IsVariable():  // expression[n] (num) -> expression[n+1] (var)
                    lastType = LabelType.Unset;
                    lastBuilder.Append(expression[index]);
                    strings.Add(LabelType.Number, lastBuilder.ToString());
                    lastBuilder.Clear();                                    // APPEND & FLUSH
                    break;
                case LabelType.Unset when expression[index].IsNumber():     // -> expression[0] (num)
                    lastType = LabelType.Number;
                    lastBuilder.Append(expression[index]);                  // APPEND ONLY
                    break;
                case LabelType.Unset when expression[index].IsOperator():   // -> expression[0] (op)
                case LabelType.Variable when expression[index].IsOperator():// expression[n] (var) -> expression[n+1] (op)
                case LabelType.Number when expression[index].IsOperator():  // expression[n] (num) -> expression[n+1] (op)
                    lastType = LabelType.Unset;
                    strings.Add(LabelType.Operator, expression[index].ToString());
                    lastBuilder.Clear();                                    // FLUSH ONLY (Append is not necessary, one operator is defaulted to one character)
                    break;
                case LabelType.Variable when expression[index].IsVariable():// expression[n] (var) -> expression[n+1] (var)
                case LabelType.Variable when expression[index].IsNumber():  // expression[n] (var) -> expression[n+1] (num)
                case LabelType.Number when expression[index].IsNumber():    // expression[n] (num) -> expression[n+1] (num)
                default:
                    break;                                                  // UNCHANGED
            }
        }

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
        for (var index = 0; index < expression.Length; index++)
        {
            string fullExpression;
            if ((fullExpression = expression.Substring(index, expression.IndexOf('}', index) - 1)).StartsWith("#{"))
                expression = expression.Replace(fullExpression,
                    ParseValue(fullExpression[2..], context).ToString(CultureInfo.CurrentCulture));
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