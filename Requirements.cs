using System.Text.Json.Serialization;

namespace Fyson;

public class Requirements
{
    private bool _initializedLibraries;
    private bool _successfullyInitializedLibraries;

    [JsonPropertyName("vs")] public int Version { get; set; }

    [JsonPropertyName("lib")] public List<string> Libraries { get; set; } = new();

    [JsonIgnore] public bool MeetRequirements => Version <= 0 && InitializeLibraries();

    private bool InitializeLibraries()
    {
        if (_initializedLibraries)
            return _successfullyInitializedLibraries;
        _initializedLibraries = true;
        try
        {
            foreach (var library in Libraries)
                library.LoadLibrary();
        }
        catch (FileNotFoundException)
        {
            return false;
        }

        return _successfullyInitializedLibraries = true;
    }
}