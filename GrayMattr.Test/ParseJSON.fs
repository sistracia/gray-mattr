module ParseJSON

open System.Collections.Generic
open Expecto
open GrayMattr
open Utils

type JsonData = { title: string; description: string }

let parseJSONTests =
    testList
        "parse json"
        [

          test "should stringify front-matter from a file object" {
              let actual: GrayFile<IDictionary<string, string>> =
                  Mattr.Read(
                      getFixture "lang-json.md",
                      Mattr.DefaultOption<IDictionary<string, string>>().SetLanguage("json")
                  )

              Expect.equal actual.Data.Value["title"] "JSON" "Data title should be equal"
              Expect.equal (printType actual.Data.Value) "Dictionary`2" "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"
          }

          test "should auto-detect JSON as the language" {
              let actual: GrayFile<JsonData> =
                  Mattr.Read(getFixture "autodetect-json.md", Mattr.DefaultOption<JsonData>())

              Expect.equal actual.Data.Value.title "autodetect-JSON" "Data title should be equal"
              Expect.equal (printType actual.Data.Value) ((typeof<JsonData>).Name) "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"
          }

          ]
