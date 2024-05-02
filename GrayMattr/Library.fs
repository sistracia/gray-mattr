namespace GrayMattr

open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions

/// Ref: Defining static classes in F#
/// https://stackoverflow.com/q/13101995/12976234
[<AbstractClass; Sealed>]
type Mattr private () =
    static member DefaultOption<'TData>() : GrayOption<'TData> = Stringify.defaultOption<'TData> ()

    static member DefaultFile<'TData>() : GrayFile<'TData> =
        { Data = None
          Content = ""
          Excerpt = ""
          Empty = Some ""
          IsEmpty = false
          Orig = [||]
          Language = ""
          Matter = ""
          Path = None }

    /// Normalize the given value to ensure an object is returned
    /// with the expected properties.
    static member ToFile<'TData>(input: string) : GrayFile<'TData> =
        let defaultFile: GrayFile<'TData> = Mattr.DefaultFile<'TData>()

        { Data = defaultFile.Data
          Content = Utils.ToString input // strip BOM and ensure that "file.content" is a string
          Excerpt = defaultFile.Excerpt
          Empty = defaultFile.Empty
          IsEmpty = defaultFile.IsEmpty
          Orig = Utils.ToBuffer input // set non-enumerable properties on the file object
          Language = defaultFile.Language
          Matter = defaultFile.Matter
          Path = defaultFile.Path }

    /// Returns true if the given `string` has front matter.
    static member Test(str: string) = Mattr.Test(str, Mattr.DefaultOption())

    static member Test(str: string, option: GrayOption<_>) =
        let (op: string) = fst option.Delimiters
        Utils.StartsWith str op None

    /// Detect the language to use, if one is defined after the first front-matter delimiter.
    static member Language(str: string) : GrayLanguage =
        Mattr.Language(str, (Mattr.DefaultOption()))

    /// Detect the language to use, if one is defined after the first front-matter delimiter.
    static member Language<'TData>(str: string, option: GrayOption<'TData>) : GrayLanguage =
        let op: string = fst option.Delimiters
        let str: string = if (Mattr.Test(str)) then str.[op.Length ..] else str
        let language: string = str.[.. Regex.Match(str, @"\r?\n").Index - 1]

        { GrayLanguage.Raw = language
          GrayLanguage.Name = language.Trim() }

    /// Parse front matter
    static member ParseMattr<'TData>(file: GrayFile<'TData>, option: GrayOption<'TData>) : GrayFile<'TData> =
        let (op: string, close: string) = option.Delimiters
        let close: string = "\n" + close
        let str: string = file.Content

        let file: GrayFile<'TData> =
            if (option.Language <> "") then
                { file with Language = option.Language }
            else
                file

        // get the length of the opening delimiter
        let openLen: int = op.Length

        if (not (Utils.StartsWith str op (Some openLen))) then
            option.Excerpt.Excerpt(file, option)

        // if the next character after the opening delimiter is
        // a character from the delimiter, then it's not a front-matter delimiter
        else if str.Length > openLen && str.[openLen] = op.[op.Length - 1] then
            option.Excerpt.Excerpt(file, option)
        else

            // strip the opening delimiter
            let str: string = str.[openLen..]
            let len: int = str.Length

            // use the language defined after first delimiter, if it exists
            let language: GrayLanguage = Mattr.Language(str, option)

            let file: GrayFile<'TData> =
                if (language.Name <> "") then
                    { file with Language = language.Name }
                else
                    file

            let str: string =
                if (language.Name <> "") then
                    str.[language.Raw.Length ..]
                else
                    str

            // get the index of the closing delimiter
            let closeIndex: int = str.IndexOf close
            let closeIndex: int = if (closeIndex = -1) then len else closeIndex

            // get the raw front-matter block
            let file: GrayFile<'TData> =
                { file with
                    Matter = str.[.. closeIndex - 1] }

            let block: string =
                Regex.Replace(file.Matter, @"^\s*#[^\n]+", "", RegexOptions.Multiline).Trim()

            let file: GrayFile<'TData> =
                if (block = "") then
                    { file with
                        IsEmpty = true
                        Empty = Some file.Content
                        Data = None }
                else
                    match option.Engines |> Map.tryFind file.Language with
                    | None -> file
                    | Some(engines: IGrayEngine<'TData, GrayOption<'TData>>) ->
                        // create file.Data by parsing the raw file.Matter block
                        { file with
                            Data = Some(engines.Parse(file.Matter, option)) }

            let content: string =
                if (closeIndex = len) then
                    ""
                else
                    let newContent: string = str.[closeIndex + close.Length ..]

                    let newContent: string =
                        if (newContent.Length > 0 && newContent.[0] = '\r') then
                            newContent.[1..]
                        else
                            newContent

                    let newContent: string =
                        if (newContent.Length > 0 && newContent.[0] = '\n') then
                            newContent.[1..]
                        else
                            newContent

                    newContent

            // update file.Content
            let file: GrayFile<'TData> = { file with Content = content }
            option.Excerpt.Excerpt(file, option)

    static member Parse<'TData>(input: string) : GrayFile<'TData> =
        let option: GrayOption<'TData> = Mattr.DefaultOption<'TData>()
        Mattr.Parse(input, option)

    static member Parse<'TData>(input: string, option: GrayOption<'TData>) : GrayFile<'TData> =
        let file: GrayFile<'TData> =
            if (input = "") then
                (Mattr.DefaultFile<'TData>())
            else
                Mattr.ToFile input

        Mattr.Parse(file, option)

    static member Parse<'TData>(file: GrayFile<'TData>) : GrayFile<'TData> =
        let option: GrayOption<'TData> = Mattr.DefaultOption<'TData>()
        Mattr.Parse(file, option)

    static member Parse<'TData>(file: GrayFile<'TData>, option: GrayOption<'TData>) : GrayFile<'TData> =
        Mattr.ParseMattr(file, option)

    static member Stringify<'TData>(input: string) : string =
        let option: GrayOption<'TData> = Mattr.DefaultOption<'TData>()
        let file: GrayFile<'TData> = Mattr.Parse(input, option)
        Stringify.parse file None option

    static member Stringify<'TData>(file: GrayFile<'TData>) : string =
        Stringify.parse file None (Mattr.DefaultOption<'TData>())

    static member Stringify<'TData>(input: string, data: 'TData, option: GrayOption<'TData>) : string =
        let file: GrayFile<'TData> = Mattr.Parse(input, option)
        Stringify.parse file (Some data) option

    /// Synchronously read a file from the file system and parse
    /// front matter. Returns the same object as the `Parse` method.
    static member Read<'TData>(filePath: string) : GrayFile<'TData> =
        let option: GrayOption<'TData> = Mattr.DefaultOption<'TData>()
        Mattr.Read(filePath, option)

    /// Synchronously read a file from the file system and parse
    /// front matter. Returns the same object as the `Parse` method.
    static member Read<'TData>(filePath: string, option: GrayOption<'TData>) : GrayFile<'TData> =
        let file: string = File.ReadAllText(filePath)

        { Mattr.Parse(file, option) with
            Path = Some filePath }
