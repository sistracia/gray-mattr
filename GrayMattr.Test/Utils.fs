module Utils

open System.IO

let getFixture (fileName: string) =
    Path.Combine("../../..", "fixtures", fileName)

let printType (x: obj) = x.GetType().Name

let isBuffer(input: obj) : bool =
        match input with
        | :? (byte array) as _ -> true
        | _ -> false