module ParseCustom

open System.Collections.Generic
open Expecto
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions
open GrayMattr
open Utils

type CustomYAMLEngine<'TData>() =
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

let parseCustomTests =
    testList
        "custom parser:"
        [

          test "should allow a custom parser to be registered:" {
              let actual: GrayFile<IDictionary<string, string>> =
                  (Mattr.Read(
                      (getFixture "lang-yaml.md"),
                      Mattr
                          .DefaultOption<IDictionary<string, string>>()
                          .AddEngine("yaml", CustomYAMLEngine())
                  ))

              Expect.equal actual.Data.Value["title"] "YAML" "Data title should be equal"
              Expect.equal (printType actual.Data.Value) "Dictionary`2" "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"

          }

          ]
