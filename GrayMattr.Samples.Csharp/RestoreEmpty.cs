namespace GrayMattr.Samples.Csharp.RestoreEmpty;

public class RestoreEmpty
{
    static void Main()
    {
        // Parse YAML front-matter

        string str = @"---
---
This is content";

        GrayFile<IDictionary<string, string>> file = Mattr.Parse<IDictionary<string, string>>(str);

        if (file.IsEmpty)
        {
            file = file.SetContent(str);
        }
    }
}

