module Stringify

open System.Collections.Generic
open Expecto
open GrayMattr

let stringifyTests =
    testList
        ".stringify"
        [

          test "should stringify front-matter from a file object" {
              let file: GrayFile<IDictionary<string, obj>> =
                  (Mattr.DefaultFile())
                      .SetContent("Name: {{name}}\n")
                      .SetData(Map[("name", "gray-matter")])

              let expected: string =
                  [| "---"; "name: gray-matter"; "---"; "Name: {{name}}\n" |]
                  |> String.concat "\n"

              let actual: string = Mattr.Stringify file

              Expect.equal actual expected "The strings should equal"
          }

          test "should stringify from a string" {
              let input: string = "Name: {{name}}\n"
              let expected: string = "Name: {{name}}\n"
              let actual: string = Mattr.Stringify input

              Expect.equal actual expected "The strings should equal"
          }

          test "should use custom delimiters to stringify" {
              let input: string = "Name: {{name}}"
              let data: IDictionary<string, obj> = dict [ ("name", "gray-matter") ]

              let option: GrayOption<IDictionary<string, obj>> =
                  Mattr.DefaultOption().SetDelimiters("~~~")

              let expected: string =
                  [| "~~~"; "name: gray-matter"; "~~~"; "Name: {{name}}\n" |]
                  |> String.concat "\n"

              let actual: string = Mattr.Stringify(input, data, option)

              Expect.equal actual expected "The strings should equal"
          }

          test "should stringify a file object" {
              let file: GrayFile<IDictionary<string, obj>> =
                  (Mattr.DefaultFile())
                      .SetContent("Name: {{name}}")
                      .SetData(Map[("name", "gray-matter")])

              let expected: string =
                  [| "---"; "name: gray-matter"; "---"; "Name: {{name}}\n" |]
                  |> String.concat "\n"

              let actual: string = Mattr.Stringify file

              Expect.equal actual expected "The strings should equal"
          }

          test "should stringify an excerpt" {
              let file: GrayFile<IDictionary<string, obj>> =
                  (Mattr.DefaultFile())
                      .SetContent("Name: {{name}}")
                      .SetData<IDictionary<string, obj>>(Map[("name", "gray-matter")])
                      .SetExcerpt("This is an excerpt.")

              let expected: string =
                  [| "---"
                     "name: gray-matter"
                     "---"
                     "This is an excerpt."
                     "---"
                     "Name: {{name}}\n" |]
                  |> String.concat "\n"

              let actual: string = Mattr.Stringify file
              Expect.equal actual expected "The strings should equal"
          }

          test "should not add an excerpt if it already exists" {
              let file: GrayFile<IDictionary<string, obj>> =
                  (Mattr.DefaultFile())
                      .SetContent("Name: {{name}}\n\nThis is an excerpt.")
                      .SetData<IDictionary<string, obj>>(Map[("name", "gray-matter")])
                      .SetExcerpt("This is an excerpt.")

              let expected: string =
                  [| "---"
                     "name: gray-matter"
                     "---"
                     "Name: {{name}}\n\nThis is an excerpt.\n" |]
                  |> String.concat "\n"

              let actual: string = Mattr.Stringify file

              Expect.equal actual expected "The strings should equal"
          }

          ]
