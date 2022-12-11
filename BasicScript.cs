using System.Text.Json.Serialization;

namespace Fyson;

public class BasicScript
{
    [JsonPropertyName("scripts")] public List<Operation> Operations { get; set; } = new();

    [JsonIgnore] public Context Context { get; set; } = new();

    public void Execute(int startIndex)
    {
        for (Context.CommandIndex = startIndex;
             Context.CommandIndex < Operations.Count;
             Context.CommandIndex++)
            Operations[Context.CommandIndex].Execute(Context);
    }
}