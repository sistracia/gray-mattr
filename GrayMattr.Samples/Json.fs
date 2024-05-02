module Json

open GrayMattr

Mattr.Parse(
    [| "---json"
       "{"
       "  \"name\": \"gray-matter\""
       "}"
       "---"
       "This is content" |]
    |> String.concat "\n"
)
|> ignore
