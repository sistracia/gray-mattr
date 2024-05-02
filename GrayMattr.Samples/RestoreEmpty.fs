module RestoreEmpty

open GrayMattr

// Parse YAML front-matter

let str =
    @"---
---
This is content"

let file: GrayFile<obj> = Mattr.Parse str

let newFile: GrayFile<obj> =
    if file.IsEmpty then file.SetContent(str) else file

newFile |> ignore
