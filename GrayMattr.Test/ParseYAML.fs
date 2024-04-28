module ParseYAML

open System.Collections.Generic
open Expecto
open GrayMattr
open Utils

[<CLIMutable>]
type YamlData = { title: string; user: string }

let parseYAMLTests =
    testList
        "parse YAML"
        [

          test "should parse YAML" {
              let actual: IDictionary<string, string> =
                  (Mattr.Read(getFixture "all.yaml")).Data.Value

              Expect.equal (actual.ContainsKey "one") true "Data should contains `one` key"
              Expect.equal (actual.ContainsKey "two") true "Data should contains `two` key"
              Expect.equal (actual.ContainsKey "three") true "Data should contains `three` key"

              Expect.equal actual["one"] "foo" "One key should be equal"
              Expect.equal actual["two"] "bar" "Two key should be equal"
              Expect.equal actual["three"] "baz" "Three key should be equal"

          }

          test "should parse YAML with closing ..." {
              let actual: IDictionary<string, string> =
                  (Mattr.Read(getFixture "all-dots.yaml")).Data.Value

              Expect.equal (actual.ContainsKey "one") true "Data should contains `one` key"
              Expect.equal (actual.ContainsKey "two") true "Data should contains `two` key"
              Expect.equal (actual.ContainsKey "three") true "Data should contains `three` key"

              Expect.equal actual["one"] "foo" "One key should be equal"
              Expect.equal actual["two"] "bar" "Two key should be equal"
              Expect.equal actual["three"] "baz" "Three key should be equal"

          }

          test "should parse YAML front matter" {
              let actual: GrayFile<IDictionary<string, string>> =
                  (Mattr.Read(getFixture "lang-yaml.md"))

              Expect.equal actual.Data.Value["title"] "YAML" "Data title should be equal"
              Expect.equal (printType actual.Data.Value) "Dictionary`2" "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"

          }

          test "should detect YAML as the language with no language defined after the first fence" {
              let actual: GrayFile<YamlData> = (Mattr.Read(getFixture "autodetect-no-lang.md"))

              Expect.equal actual.Data.Value.title "autodetect-no-lang" "Title should be equal"
              Expect.equal (printType actual.Data.Value) ((typeof<YamlData>).Name) "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"

          }

          test "should detect YAML as the language" {
              let actual: GrayFile<YamlData> = (Mattr.Read(getFixture "autodetect-yaml.md"))

              Expect.equal actual.Data.Value.title "autodetect-yaml" "Title should be equal"
              Expect.equal (printType actual.Data.Value) ((typeof<YamlData>).Name) "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"

          }

          ]
