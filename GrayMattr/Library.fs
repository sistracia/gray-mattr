namespace GrayMattr

module Grayr =
    let defaultEngines<'TData> () : Map<string, IGrayEngine<'TData, GrayOption<'TData>>> =
        Map[("json", DefaultGrayJSONEngine<'TData>())
            ("yaml", DefaultGrayYAMLEngine<'TData>())]

    let parseMatter<'TData> (file: GrayFile<'TData>) (option: GrayOption<'TData>) : GrayFile<'TData> = file

/// Ref: Defining static classes in F#
/// https://stackoverflow.com/q/13101995/12976234
[<AbstractClass; Sealed>]
type Mattr private () =
    static member DefaultOption() : GrayOption<Map<string, obj>> =
        { Engines = Grayr.defaultEngines ()
          Delimiters = ("---", "---")
          Language = "yaml"
          ExcerptSeparator = ""
          Excerpt = DefaultGrayExcerptFalse<Map<string, obj>>() }

    static member DefaultOption<'TData>() : GrayOption<'TData> =
        let defaultOption: GrayOption<Map<string, obj>> = Mattr.DefaultOption()

        { Engines = Grayr.defaultEngines ()
          Delimiters = defaultOption.Delimiters
          Language = defaultOption.Language
          ExcerptSeparator = defaultOption.ExcerptSeparator
          Excerpt = DefaultGrayExcerptFalse() }

    static member DefaultFile() : GrayFile<Map<string, obj>> =
        { Data = Map []
          Content = ""
          Excerpt = ""
          Empty = Some ""
          IsEmpty = false
          Orig = [||]
          Language = ""
          Matter = "" }

    static member DefaultFile<'TData>() : GrayFile<'TData> =
        let defaultFile: GrayFile<Map<string, obj>> = Mattr.DefaultFile()

        { Data = Unchecked.defaultof<'TData>
          Content = defaultFile.Content
          Excerpt = defaultFile.Excerpt
          Empty = defaultFile.Empty
          IsEmpty = defaultFile.IsEmpty
          Orig = defaultFile.Orig
          Language = defaultFile.Language
          Matter = defaultFile.Matter }

    static member ToFile(input: string) =
        let defaultFile: GrayFile<Map<string, obj>> = Mattr.DefaultFile()

        { Data = Unchecked.defaultof<'TData>
          Content = Utils.toString input
          Excerpt = defaultFile.Excerpt
          Empty = defaultFile.Empty
          IsEmpty = defaultFile.IsEmpty
          Orig = Utils.toBuffer input
          Language = defaultFile.Language
          Matter = defaultFile.Matter }

    static member Parse<'TData>(input: string) : GrayFile<'TData> =
        let file: GrayFile<'TData> =
            if (input = "") then
                (Mattr.DefaultFile<'TData>())
            else
                Mattr.ToFile input

        let option: GrayOption<'TData> = Mattr.DefaultOption<'TData>()

        Grayr.parseMatter file option

    static member Parse<'TData>(input: string, option: GrayOption<'TData>) : GrayFile<'TData> =
        let file: GrayFile<'TData> =
            if (input = "") then
                (Mattr.DefaultFile<'TData>())
            else
                Mattr.ToFile input

        Grayr.parseMatter file option

    static member Parse<'TData>(file: GrayFile<'TData>) : GrayFile<'TData> =
        let option: GrayOption<'TData> = Mattr.DefaultOption<'TData>()
        Grayr.parseMatter file option

    static member Parse<'TData>(file: GrayFile<'TData>, option: GrayOption<'TData>) : GrayFile<'TData> =
        Grayr.parseMatter file option

    static member Stringify(file: GrayFile<Map<string, obj>>) : string =
        Stringify.parse file None (Mattr.DefaultOption())

    static member Stringify(input: string) : string = input

    static member Stringify<'TData>(input: string, data: 'TData, option: GrayOption<'TData>) : string = input
