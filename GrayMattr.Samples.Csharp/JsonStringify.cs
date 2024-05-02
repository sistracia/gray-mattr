namespace GrayMattr.Samples.Csharp.JsonStringify;

public class JsonStringify
{
    static void Main()
    {
        GrayFile<IDictionary<string, string>> file1 = Mattr.Parse<IDictionary<string, string>>(
            string.Join("\n", ["---json", "{", "  \"name\": \"gray-matter\"", "}", "---", "This is content"])
        );

        file1.Stringify(
            new Dictionary<string, string>(),
            Mattr.DefaultOption<IDictionary<string, string>>().SetLanguage("yaml")
        );

        GrayFile<IDictionary<string, string>> file2 = Mattr.Parse<IDictionary<string, string>>(
            string.Join("\n", ["---", "title: Home", "---", "This is content"])
        );

        file2.Stringify(
            new Dictionary<string, string>(),
            Mattr.DefaultOption<IDictionary<string, string>>().SetLanguage("json")
        );
    }
}

