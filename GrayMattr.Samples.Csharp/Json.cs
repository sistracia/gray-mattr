namespace GrayMattr.Samples.Csharp.Json;

public class Json
{
    static void Main()
    {
        GrayFile<IDictionary<string, string>> file = Mattr.Parse<IDictionary<string, string>>(
            string.Join("\n", ["---json", "{", "  \"name\": \"gray-matter\"", "}", "---", "This is content"])
        );
    }
}

