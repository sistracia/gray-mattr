namespace GrayMattr

open System
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions

module Grayr =
    let defaultEngines<'TData> () : Map<string, IGrayEngine<'TData, GrayOption<'TData>>> =
        Map[("json", DefaultGrayJSONEngine<'TData>())
            ("yaml", DefaultGrayYAMLEngine<'TData>())]

/// Ref: Defining static classes in F#
/// https://stackoverflow.com/q/13101995/12976234
[<AbstractClass; Sealed>]
type Mattr private () =
    static member DefaultOption() : GrayOption<IDictionary<string, obj>> =
        { Engines = Grayr.defaultEngines ()
          Delimiters = (Defaults.delimiter, Defaults.delimiter)
          Language = Defaults.language
          ExcerptSeparator = Defaults.excerptSeparator
          Excerpt = DefaultGrayExcerptFalse<IDictionary<string, obj>>() }

    static member DefaultOption<'TData>() : GrayOption<'TData> =
        let defaultOption: GrayOption<IDictionary<string, obj>> = Mattr.DefaultOption()

        { Engines = Grayr.defaultEngines ()
          Delimiters = defaultOption.Delimiters
          Language = defaultOption.Language
          ExcerptSeparator = defaultOption.ExcerptSeparator
          Excerpt = DefaultGrayExcerptFalse() }

    static member DefaultFile() : GrayFile<IDictionary<string, obj>> =
        { Data = Some (new Dictionary<string, obj>())
          Content = ""
          Excerpt = ""
          Empty = Some ""
          IsEmpty = false
          Orig = [||]
          Language = ""
          Matter = ""
          Path = None }

    static member DefaultFile<'TData>() : GrayFile<'TData> =
        let defaultFile: GrayFile<IDictionary<string, obj>> = Mattr.DefaultFile()

        { Data = None
          Content = defaultFile.Content
          Excerpt = defaultFile.Excerpt
          Empty = defaultFile.Empty
          IsEmpty = defaultFile.IsEmpty
          Orig = defaultFile.Orig
          Language = defaultFile.Language
          Matter = defaultFile.Matter
          Path = defaultFile.Path }

    static member ToFile(input: string) =
        let defaultFile: GrayFile<IDictionary<string, obj>> = Mattr.DefaultFile()

        { Data = None
          Content = Utils.ToString input
          Excerpt = defaultFile.Excerpt
          Empty = defaultFile.Empty
          IsEmpty = defaultFile.IsEmpty
          Orig = Utils.ToBuffer input
          Language = defaultFile.Language
          Matter = defaultFile.Matter
          Path = defaultFile.Path }

    static member Test(str: string) = Mattr.Test(str, Mattr.DefaultOption())

    static member Test(str: string, option: GrayOption<_>) =
        let (op: string) = fst option.Delimiters
        Utils.StartsWith str op None

    static member Language(str: string) : GrayLanguage =
        Mattr.Language(str, (Mattr.DefaultOption()))

    static member Language<'TData>(str: string, option: GrayOption<'TData>) : GrayLanguage =
        let op: string = fst option.Delimiters
        let str: string = if (Mattr.Test(str)) then str.[op.Length ..] else str

        let language: string = (str.[.. str.IndexOf(Environment.NewLine) - 1])

        { GrayLanguage.Raw = language
          GrayLanguage.Name = language.Trim() }

    static member ParseMattr<'TData>(file: GrayFile<'TData>, option: GrayOption<'TData>) : GrayFile<'TData> =
        let (op: string, close: string) = option.Delimiters
        let close: string = "\n" + close
        let str: string = file.Content

        let file: GrayFile<'TData> =
            if (option.Language <> "") then
                { file with Language = option.Language }
            else
                file

        let openLen: int = op.Length

        if (not (Utils.StartsWith str op (Some openLen))) then
            option.Excerpt.Excerpt(file, option)
        else if str.Length > openLen && str.[openLen] = op.[op.Length - 1] then
            option.Excerpt.Excerpt(file, option)
        else
            let str: string = str.[openLen..]
            let len: int = str.Length
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

            let closeIndex: int = str.IndexOf(close)
            let closeIndex: int = if (closeIndex = -1) then len else closeIndex

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
                        { file with
                            Data = Some (engines.Parse(file.Matter, option)) }

            let content: string =
                if (closeIndex = len) then
                    ""
                else
                    let newContent: string = str.[(closeIndex + close.Length) ..]

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

    static member Read<'TData>(filePath: string) : GrayFile<'TData> =
        let option: GrayOption<'TData> = Mattr.DefaultOption<'TData>()
        Mattr.Read(filePath, option)

    static member Read<'TData>(filePath: string, option: GrayOption<'TData>) : GrayFile<'TData> =
        let file: string = File.ReadAllText(filePath)

        { Mattr.Parse(file, option) with
            Path = Some filePath }
