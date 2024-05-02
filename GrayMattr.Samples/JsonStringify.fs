module JsonStringify

open GrayMattr

let file1: GrayFile<obj> =
    Mattr.Parse(
        [| "---json"
           "{"
           "  \"name\": \"gray-matter\""
           "}"
           "---"
           "This is content" |]
        |> String.concat "\n"
    )

file1.Stringify(dict [], Mattr.DefaultOption().SetLanguage("yaml")) |> ignore

let file2: GrayFile<obj> =
    Mattr.Parse([| "---"; "title: Home"; "---"; "This is content" |] |> String.concat "\n")

file2.Stringify(dict [], Mattr.DefaultOption().SetLanguage("json")) |> ignore
