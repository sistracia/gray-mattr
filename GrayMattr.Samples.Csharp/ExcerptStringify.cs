namespace GrayMattr.Samples.Csharp.ExcerptStringify;

public class ExcerptStringify
{
    static void Main()
    {
        GrayFile<IDictionary<string, string>> file = Mattr.Parse(
            string.Join("\n", ["---", "foo: bar", "---", "This is an excerpt.", "<!-- sep -->", "This is content"]),
            Mattr.DefaultOption<IDictionary<string, string>>().SetExcerptSeparator("<!-- sep -->")
        );

        file.Stringify();
    }
}

