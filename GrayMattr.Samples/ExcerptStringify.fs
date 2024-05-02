module ExcerptStringify

open GrayMattr

let file: GrayFile<obj> =
    Mattr.Parse(
        ([| "---"
            "foo: bar"
            "---"
            "This is an excerpt."
            "<!-- sep -->"
            "This is content" |]
         |> String.concat "\n"),
        Mattr.DefaultOption().SetExcerptSeparator("<!-- sep -->")
    )

file.Stringify() |> ignore
