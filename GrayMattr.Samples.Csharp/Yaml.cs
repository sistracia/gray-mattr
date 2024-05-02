namespace GrayMattr.Samples.Csharp.Yaml;

public class Yaml
{
    static void Main()
    {
        // Parse YAML front-matter

        GrayFile<IDictionary<string, string>> file = Mattr.Parse<IDictionary<string, string>>(
            string.Join("\n", ["---", "foo: bar", "---", "This is content"])
        );
    }
}

