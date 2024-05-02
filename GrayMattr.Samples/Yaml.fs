module Yaml

open GrayMattr

// Parse YAML front-matter

Mattr.Parse([| "---"; "foo: bar"; "---"; "This is content" |] |> String.concat "\n")
|> ignore
