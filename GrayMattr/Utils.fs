namespace GrayMattr

open System.Text
open StripBomStr

[<AbstractClass; Sealed>]
type Utils private () =
    /// Returns true if `val` is a buffer
    static member isBuffer(input: obj) : bool =
        match input with
        | :? (byte array) as _ -> true
        | _ -> false

    /// Cast `input` to a buffer
    static member toBuffer(input: string) : byte array =
        System.Text.Encoding.UTF8.GetBytes(input)

    /// Cast `input` to a buffer
    static member toBuffer(input: byte array) : byte array = input

    /// Cast `val` to a string.
    static member toString(input: string) = BOM.remove input

    /// Cast `val` to a string.
    static member toString(input: byte array) =
        BOM.remove (Encoding.Default.GetString input)

    /// Returns true if `str` starts with `substr`.
    static member startsWith (str: string) (substr: string) (len: int option) =
        let effectiveLen: int =
            match len with
            | Some(l: int) -> l
            | None -> substr.Length

        str.Substring(0, effectiveLen) = substr
