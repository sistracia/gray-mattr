module MattrWindow

open System.Text
open System.Collections.Generic
open Expecto
open GrayMattr
open Utils

[<CLIMutable>]
type YamlData = { abc: string; version: int }

let mattrWindowTests =
    testList
        "gray-matter (windows carriage returns)"
        [

          test "should extract YAML front matter" {
              let actual: GrayFile<IDictionary<string, obj>> =
                  Mattr.Parse("---\r\nabc: xyz\r\n---")

              Expect.equal (printType actual.Data.Value) "Dictionary`2" "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"
              Expect.equal actual.Data.Value["abc"] "xyz" "Data abc should be equal"
          }

          test "should cache orig string as a buffer on the `orig property`" {
              let fixture: string = "---\r\nabc: xyz\r\n---"
              let actual: GrayFile<IDictionary<string, obj>> = Mattr.Parse(fixture)

              Expect.equal (isBuffer actual.Orig) true "Orig should be buffer"
              Expect.equal (Encoding.Default.GetString actual.Orig) fixture "Orig string should be equal fixture"
          }

          test "should return an object when the string is 0 length:" {
              let actual: GrayFile<IDictionary<string, obj>> = Mattr.Parse("")
              Expect.equal (printType actual) "GrayFile`1" "Should be GrayFile type"
          }

          test "should extract YAML front matter and content" {
              let fixture: string =
                  "---\r\nabc: xyz\r\nversion: 2\r\n---\r\n\r\n<span class=\"alert alert-info\">This is an alert</span>\r\n"

              let actual: GrayFile<YamlData> = Mattr.Parse(fixture)

              Expect.equal actual.Data.Value.abc "xyz" "Data abc should be equal"
              Expect.equal actual.Data.Value.version 2 "Data version should be equal"

              Expect.equal
                  actual.Content
                  "\r\n<span class=\"alert alert-info\">This is an alert</span>\r\n"
                  "Content should be equal"

              Expect.equal (Encoding.Default.GetString actual.Orig) fixture "Orig string should be equal fixture"

          }

          test "should use a custom delimiter as a string." {
              let fixture: string =
                  "~~~\r\nabc: xyz\r\nversion: 2\r\n~~~\r\n\r\n<span class=\"alert alert-info\">This is an alert</span>\r\n"

              let actual: GrayFile<YamlData> =
                  Mattr.Parse(fixture, Mattr.DefaultOption<YamlData>().SetDelimiters("~~~"))

              Expect.equal actual.Data.Value.abc "xyz" "Data abc should be equal"
              Expect.equal actual.Data.Value.version 2 "Data version should be equal"

              Expect.equal
                  actual.Content
                  "\r\n<span class=\"alert alert-info\">This is an alert</span>\r\n"
                  "Content should be equal"

              Expect.equal (Encoding.Default.GetString actual.Orig) fixture "Orig string should be equal fixture"

          }

          test "should use custom delimiters as a tuple." {
              let fixture: string =
                  "~~~\r\nabc: xyz\r\nversion: 2\r\n~~~\r\n\r\n<span class=\"alert alert-info\">This is an alert</span>\r\n"

              let actual: GrayFile<YamlData> =
                  Mattr.Parse(fixture, Mattr.DefaultOption<YamlData>().SetDelimiters(("~~~", "~~~")))

              Expect.equal actual.Data.Value.abc "xyz" "Data abc should be equal"
              Expect.equal actual.Data.Value.version 2 "Data version should be equal"

              Expect.equal
                  actual.Content
                  "\r\n<span class=\"alert alert-info\">This is an alert</span>\r\n"
                  "Content should be equal"

              Expect.equal (Encoding.Default.GetString actual.Orig) fixture "Orig string should be equal fixture"

          }

          test "should correctly identify delimiters and ignore strings that look like delimiters." {
              let fixture: string =
                  "---\r\nname: \"troublesome --- value\"\r\n---\r\nhere is some content\r\n"

              let actual: GrayFile<IDictionary<string, string>> = Mattr.Parse(fixture)

              Expect.equal actual.Data.Value["name"] "troublesome --- value" "Data name should be equal"
              Expect.equal actual.Content "here is some content\r\n" "Content should be equal"
              Expect.equal (Encoding.Default.GetString actual.Orig) fixture "Orig string should be equal fixture"

          }

          test "should correctly parse a string that only has an opening delimiter" {
              let fixture: string = "---\r\nname: \"troublesome --- value\"\r\n"

              let actual: GrayFile<IDictionary<string, string>> = Mattr.Parse(fixture)

              Expect.equal actual.Data.Value["name"] "troublesome --- value" "Data name should be equal"
              Expect.equal actual.Content "" "Content should be equal"
              Expect.equal (Encoding.Default.GetString actual.Orig) fixture "Orig string should be equal fixture"

          }

          test "should not try to parse a string has content that looks like front-matter." {
              let fixture: string = "-----------name--------------value\r\nfoo"
              let actual: GrayFile<IDictionary<string, string>> = Mattr.Parse(fixture)

              Expect.equal (Option.isNone actual.Data) true "Should be have none data"
              Expect.equal actual.Content fixture "Content should be equal"
              Expect.equal (Encoding.Default.GetString actual.Orig) fixture "Orig string should be equal fixture"

          }

          ]
