module YamlStringify

open GrayMattr

// Stringify back to YAML

let file1: GrayFile<obj> =
    Mattr.Parse([| "---"; "foo: bar"; "---"; "This is content" |] |> String.concat "\n")

file1.Stringify() |> ignore

// custom data

let file2: GrayFile<obj> =
    Mattr.Parse([| "This is content" |] |> String.concat "\n")

(file2.Stringify(dict [ "baz", [| "one"; "two"; "three" |] ])) |> ignore
