namespace GrayMattr.Samples.Csharp.ExcerptSeparator;

public class ExcerptSeparator
{
    static void Main()
    {
        GrayFile<IDictionary<string, string>> file = Mattr.Parse(
            string.Join("\n", ["---", "foo: bar", "---", "This is an excerpt.", "<!-- sep -->", "This is content"]),
            Mattr.DefaultOption<IDictionary<string, string>>().SetExcerptSeparator("<!-- sep -->")
        );
    }
}

