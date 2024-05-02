namespace GrayMattr.Samples.Csharp.YamlStringify;

public class YamlStringify
{
    static void Main()
    {
        // Stringify back to YAML

        GrayFile<IDictionary<string, string>> file1 = Mattr.Parse<IDictionary<string, string>>(
            string.Join("\n", ["---", "foo: bar", "---", "This is content"])
        );

        file1.Stringify();

        // custom data

        GrayFile<IDictionary<string, string[]>> file2 = Mattr.Parse<IDictionary<string, string[]>>(
            string.Join("\n", ["This is content"])
        );

        file2.Stringify(new Dictionary<string, string[]>() { { "baz", ["one", "two", "three"] } });
    }
}

