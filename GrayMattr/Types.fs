namespace GrayMattr

open System.Text.Json
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions
open System

[<Struct>]
type GrayLanguage = { Raw: string; Name: string }

[<Struct>]
type GrayFile<'TData> =
    { Data: 'TData option
      Content: string
      Excerpt: string
      Empty: string option
      IsEmpty: bool
      Orig: byte array
      Language: string
      Matter: string
      Path: string option }

    member this.SetData<'TNewData>(data: 'TNewData) : GrayFile<'TNewData> =
        { Data = Some data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter
          Path = this.Path }

    member this.SetContent(content: string) : GrayFile<'TData> =
        { Data = this.Data
          Content = content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter
          Path = this.Path }

    member this.SetExcerpt(excerpt: string) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter
          Path = this.Path }

    member this.SetEmpty(empty: string option) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter
          Path = this.Path }

    member this.SetIsEmpty(isEmpty: bool) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = isEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter
          Path = this.Path }

    member this.SetOrig(orig: byte array) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = orig
          Language = this.Language
          Matter = this.Matter
          Path = this.Path }

    member this.SetLanguage(language: string) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = language
          Matter = this.Matter
          Path = this.Path }

    member this.SetMatter(matter: string) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = matter
          Path = this.Path }

    member this.SetPath(path: string option) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter
          Path = path }

type IGrayEngine<'TData, 'TOption> =
    abstract member Parse: string * 'TOption -> 'TData
    abstract member Stringify: 'TData * 'TOption -> string

type IGrayExcerpt<'TData, 'TOption> =
    abstract member Excerpt: GrayFile<'TData> * 'TOption -> GrayFile<'TData>

[<Struct>]
type GrayOption<'TData> =
    { Engines: Map<string, IGrayEngine<'TData, GrayOption<'TData>>>
      Delimiters: string * string
      Language: string
      ExcerptSeparator: string
      Excerpt: IGrayExcerpt<'TData, GrayOption<'TData>> }

type DefaultGrayExcerptFalse<'TData>() =
    interface IGrayExcerpt<'TData, GrayOption<'TData>> with
        member _.Excerpt(file: GrayFile<'TData>, _: GrayOption<'TData>) : GrayFile<'TData> = file

type DefaultGrayExcerptTrue<'TData>() =
    interface IGrayExcerpt<'TData, GrayOption<'TData>> with
        member _.Excerpt(file: GrayFile<'TData>, option: GrayOption<'TData>) : GrayFile<'TData> =
            let delimiter: string =
                match option.ExcerptSeparator with
                | "" -> fst option.Delimiters
                | (sep: string) -> sep

            let idx: int = file.Content.IndexOf delimiter

            if idx <> -1 then
                { file with
                    Excerpt = file.Content.[.. idx - 1] }
            else
                file

type DefaultGrayJSONEngine<'TData>() =
    interface IGrayEngine<'TData, GrayOption<'TData>> with
        member _.Parse(content: string, _: GrayOption<'TData>) : 'TData =
            JsonSerializer.Deserialize<'TData> content

        member _.Stringify(data: 'TData, _: GrayOption<'TData>) : string = JsonSerializer.Serialize data

type DefaultGrayYAMLEngine<'TData>() =
    interface IGrayEngine<'TData, GrayOption<'TData>> with
        member _.Parse(content: string, _: GrayOption<'TData>) : 'TData =
            ((DeserializerBuilder())
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build())
                .Deserialize<'TData>
                content

        member _.Stringify(data: 'TData, _: GrayOption<'TData>) : string =
            ((SerializerBuilder())
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build())
                .Serialize
                data

type GrayOption<'TData> with

    member this.SetEngines(enginees: Map<string, IGrayEngine<'TData, GrayOption<'TData>>>) : GrayOption<'TData> =
        { Engines = enginees
          Delimiters = this.Delimiters
          Language = this.Language
          ExcerptSeparator = this.ExcerptSeparator
          Excerpt = this.Excerpt }

    member this.AddEngine<'TData>(key: string, engine: IGrayEngine<'TData, GrayOption<'TData>>) : GrayOption<'TData> =
        { Engines = this.Engines.Add(key, engine)
          Delimiters = this.Delimiters
          Language = this.Language
          ExcerptSeparator = this.ExcerptSeparator
          Excerpt = this.Excerpt }

    member this.SetExcerpt(excerpt: IGrayExcerpt<'TData, GrayOption<'TData>>) : GrayOption<'TData> =
        { Engines = this.Engines
          Delimiters = this.Delimiters
          Language = this.Language
          ExcerptSeparator = this.ExcerptSeparator
          Excerpt = excerpt }

    member this.SetExcerpt(excerpt: bool) : GrayOption<'TData> =
        let excerpt: IGrayExcerpt<'TData, GrayOption<'TData>> =
            match excerpt with
            | true -> DefaultGrayExcerptTrue<'TData>()
            | false -> DefaultGrayExcerptFalse<'TData>()

        { Engines = this.Engines
          Delimiters = this.Delimiters
          Language = this.Language
          ExcerptSeparator = this.ExcerptSeparator
          Excerpt = excerpt }

    member this.SetExcerptSeparator(excerptSeparator: string) : GrayOption<'TData> =
        let excerpt: IGrayExcerpt<'TData, GrayOption<'TData>> =
            match excerptSeparator with
            | "" -> this.Excerpt
            | _ -> DefaultGrayExcerptTrue<'TData>()

        { Engines = this.Engines
          Delimiters = this.Delimiters
          Language = this.Language
          ExcerptSeparator = excerptSeparator
          Excerpt = excerpt }

    member this.SetLanguage(language: string) : GrayOption<'TData> =
        { Engines = this.Engines
          Delimiters = this.Delimiters
          Language = language
          ExcerptSeparator = this.ExcerptSeparator
          Excerpt = this.Excerpt }

    member this.SetDelimiters(delimiter: string) : GrayOption<'TData> =
        { Engines = this.Engines
          Delimiters = (delimiter, delimiter)
          Language = this.Language
          ExcerptSeparator = this.ExcerptSeparator
          Excerpt = this.Excerpt }

    member this.SetDelimiters(delimiters: string * string) : GrayOption<'TData> =
        { Engines = this.Engines
          Delimiters = delimiters
          Language = this.Language
          ExcerptSeparator = this.ExcerptSeparator
          Excerpt = this.Excerpt }

module Stringify =
    /// Ref:
    /// https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/values/null-values
    /// https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions#avoid-the-use-of-the-uncheckeddefaultof_-attribute
    let nullValues<'TData> () : 'TData = Unchecked.defaultof<'TData>

    let newline (str: string) =
        if str.EndsWith("\n") then str else str + "\n"

    let parse<'TData> (file: GrayFile<'TData>) (data: 'TData option) (option: GrayOption<'TData>) : string =
        let language: string =
            match file.Language with
            | "" -> option.Language
            | (fileLanguage: string) -> fileLanguage

        let engine: IGrayEngine<'TData, GrayOption<'TData>> option =
            option.Engines |> Map.tryFind language

        match engine with
        | None -> ""
        | Some(engine: IGrayEngine<'TData, GrayOption<'TData>>) ->
            let (op: string, close: string) = option.Delimiters

            let (data: 'TData) =
                match data with
                | Some(data: 'TData) -> data
                | None ->
                    match file.Data with
                    | None -> nullValues<'TData> ()
                    | Some(data: 'TData) -> data

            let matter: string = engine.Stringify(data, option).Trim()

            let mutable buf: string =
                if (matter <> Defaults.delimiter) then
                    newline op + newline matter + newline close
                else
                    ""

            if (file.Excerpt <> "" && file.Content.IndexOf(file.Excerpt.Trim()) = -1) then
                buf <- buf + newline file.Excerpt + newline close

            buf + newline file.Content


type GrayFile<'TData> with

    member this.Stringify(additionalData: 'TData, option: GrayOption<'TData>) : string =
        Stringify.parse this (Some additionalData) option
