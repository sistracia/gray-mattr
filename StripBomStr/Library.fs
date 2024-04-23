namespace StripBomStr

module BOM =
    let remove (str: string) =
        if str.Length > 0 && str.[0] = '\uFEFF' then
            str.[1..]
        else
            str
