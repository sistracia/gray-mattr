module ExcerptSeparator

open GrayMattr

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
|> ignore
