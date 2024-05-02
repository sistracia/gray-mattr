module Excerpt

open GrayMattr

// excerpt as a boolean

Mattr.Parse(
    ([| "---"; "foo: bar"; "---"; "This is an excerpt."; "---"; "This is content" |]
     |> String.concat "\n"),
    Mattr.DefaultOption().SetExcerpt(true)
)
|> ignore

// excerpt as an object

type GrayExcerpt<'TData>() =
    interface IGrayExcerpt<'TData, GrayOption<'TData>> with
        member _.Excerpt(file: GrayFile<'TData>, option: GrayOption<'TData>) : GrayFile<'TData> =
            // returns the first 4 lines of the contents
            file.SetExcerpt(String.concat " " (file.Content.Split('\n') |> Array.take 4))

Mattr.Parse(
    ([| "---"
        "foo: bar"
        "---"
        "Only this"
        "will be"
        "in the"
        "excerpt"
        "but not this..." |]
     |> String.concat "\n"),
    Mattr.DefaultOption().SetExcerpt(GrayExcerpt())
)
|> ignore
