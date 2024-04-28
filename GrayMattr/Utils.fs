namespace GrayMattr

open System.Text
open StripBomStr

[<AbstractClass; Sealed>]
type Utils private () =

    /// Cast `input` to a buffer
    static member ToBuffer(input: string) : byte array =
        System.Text.Encoding.UTF8.GetBytes(input)

    /// Cast `input` to a buffer
    static member ToBuffer(input: byte array) : byte array = input

    /// Cast `val` to a string.
    static member ToString(input: string) = BOM.remove input

    /// Cast `val` to a string.
    static member ToString(input: byte array) =
        BOM.remove (Encoding.Default.GetString input)

    /// Returns true if `str` starts with `substr`.
    static member StartsWith (str: string) (substr: string) (len: int option) =
        let effectiveLen: int =
            match len with
            | Some(l: int) -> l
            | None -> substr.Length

        if (effectiveLen > str.Length) then
            false
        else
            str.[.. effectiveLen - 1] = substr
