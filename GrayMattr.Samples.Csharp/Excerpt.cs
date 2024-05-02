namespace GrayMattr.Samples.Csharp.Excerpt;

class GrayExcerpt<TData> : IGrayExcerpt<TData, GrayOption<TData>>
{
    public GrayFile<TData> Excerpt(GrayFile<TData> file, GrayOption<TData> option)
    {
        return file.SetExcerpt(string.Concat(file.Content.Split("\n").Take(4), " "));
    }
}

public class Excerpt
{
    static void Main()
    {
        // excerpt as a boolean

        GrayFile<IDictionary<string, string>> file1 = Mattr.Parse<IDictionary<string, string>>(
            string.Join("\n", ["---", "foo: bar", "---", "This is an excerpt.", "---", "This is content"])
        );

        // excerpt as an object

        GrayFile<IDictionary<string, string>> file2 = Mattr.Parse(
            string.Join("\n", ["---", "foo: bar", "---", "Only this", "will be", "in the", "excerpt", "but not this..."]),
            Mattr.DefaultOption<IDictionary<string, string>>().SetExcerpt(new GrayExcerpt<IDictionary<string, string>>())
        );
    }
}

