namespace GrayMattr

open System.Text.Json
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions

[<Struct>]
type GrayFile<'TData> =
    { Data: 'TData
      Content: string
      Excerpt: string
      Empty: string option
      IsEmpty: bool
      Orig: byte array
      Language: string
      Matter: string }

    member this.SetData<'TNewData>(data: 'TNewData) : GrayFile<'TNewData> =
        { Data = data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter }

    member this.SetContent(content: string) : GrayFile<'TData> =
        { Data = this.Data
          Content = content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter }

    member this.SetExcerpt(excerpt: string) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter }

    member this.SetEmpty(empty: string option) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter }

    member this.SetIsEmpty(isEmpty: bool) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = isEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = this.Matter }

    member this.SetOrig(orig: byte array) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = orig
          Language = this.Language
          Matter = this.Matter }

    member this.SetLanguage(language: string) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = language
          Matter = this.Matter }

    member this.SetMatter(matter: string) : GrayFile<'TData> =
        { Data = this.Data
          Content = this.Content
          Excerpt = this.Excerpt
          Empty = this.Empty
          IsEmpty = this.IsEmpty
          Orig = this.Orig
          Language = this.Language
          Matter = matter }

type IGrayEngine<'TData, 'TOption> =
    abstract member Parse: string -> 'TData
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

type DefaultGrayExcerptTrue<'TData>() =
    interface IGrayExcerpt<'TData, GrayOption<'TData>> with
        member _.Excerpt(file: GrayFile<'TData>, _: GrayOption<'TData>) : GrayFile<'TData> = file

type DefaultGrayExcerptFalse<'TData>() =
    interface IGrayExcerpt<'TData, GrayOption<'TData>> with
        member _.Excerpt(file: GrayFile<'TData>, option: GrayOption<'TData>) : GrayFile<'TData> =
            let delimiter =
                match option.ExcerptSeparator with
                | "" -> fst option.Delimiters
                | (sep: string) -> sep

            let idx: int = file.Content.IndexOf delimiter

            if idx <> -1 then
                { file with
                    Content = file.Content.[..idx] }
            else
                file

type DefaultGrayJSONEngine<'TData>() =
    interface IGrayEngine<'TData, GrayOption<'TData>> with
        member _.Parse(content: string) : 'TData =
            JsonSerializer.Deserialize<'TData> content

        member _.Stringify(data: 'TData, _: GrayOption<'TData>) : string = JsonSerializer.Serialize data

type DefaultGrayYAMLEngine<'TData>() =
    interface IGrayEngine<'TData, GrayOption<'TData>> with
        member _.Parse(content: string) : 'TData =
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
        { Engines = this.Engines
          Delimiters = this.Delimiters
          Language = this.Language
          ExcerptSeparator = excerptSeparator
          Excerpt = this.Excerpt }

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
    let newline (str: string) =
        if str.EndsWith("\n") then str else str + "\n"

    let parse<'TData> (file: GrayFile<'TData>) (_todo: 'TData option) (option: GrayOption<'TData>) : string =
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
            let matter: string = engine.Stringify(file.Data, option).Trim()

            let mutable buf: string =
                if (matter <> "{}") then
                    newline op + newline matter + newline close
                else
                    ""

            if (file.Excerpt <> "" && file.Content.IndexOf(file.Excerpt.Trim()) = -1) then
                buf <- buf + newline file.Excerpt + newline close

            buf + newline file.Content


type GrayFile<'TData> with

    member this.Stringify(additionalData: 'TData, option: GrayOption<'TData>) : string =
        Stringify.parse this (Some additionalData) option
