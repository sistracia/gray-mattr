module MattrRead

open System.Collections.Generic
open Expecto
open GrayMattr
open Utils

let mattrReadTests =
    testList
        ".read"
        [

          test "should extract YAML front matter from files with content." {
              let actual: GrayFile<IDictionary<string, obj>> = Mattr.Read(getFixture "basic.txt")

              Expect.equal (Option.isSome actual.Path) true "Should be have some path"
              Expect.equal (actual.Data.Value.ContainsKey "title") true "Data should contains `title` key"
              Expect.equal actual.Data.Value["title"] "Basic" "Data title should be equal"
              Expect.equal actual.Content "this is content." "Content should be equal"
          }

          test "should parse complex YAML front matter." {
              let actual: GrayFile<IDictionary<string, obj>> = Mattr.Read(getFixture "complex.md")

              Expect.equal (printType actual.Data.Value) "Dictionary`2" "Data key should be exist"
              Expect.equal actual.Data.Value["root"] "_gh_pages" "Data root should be equal"
              Expect.equal (Option.isSome actual.Path) true "Should be have some path"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"
              Expect.equal (actual.Data.Value.ContainsKey "root") true "Data should contains `root` key"
          }

          test "should return an object when a file is empty." {
              let actual: GrayFile<IDictionary<string, obj>> = Mattr.Read(getFixture "empty.md")

              Expect.equal (Option.isSome actual.Path) true "Should be have some path"
              Expect.equal (Option.isNone actual.Data) true "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"
          }

          test "should return an object when no front matter exists." {
              let actual: GrayFile<IDictionary<string, obj>> = Mattr.Read(getFixture "empty.md")

              Expect.equal (Option.isSome actual.Path) true "Should be have some path"
              Expect.equal (Option.isNone actual.Data) true "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"
          }

          test "should parse YAML files directly" {
              let actual: GrayFile<IDictionary<string, obj>> = Mattr.Read(getFixture "all.yaml")

              Expect.equal (Option.isSome actual.Path) true "Should be have some path"
              Expect.equal (printType actual.Data.Value) "Dictionary`2" "Data key should be exist"
              Expect.equal (printType actual.Content) "String" "Content key should be exist"
              Expect.equal (printType actual.Orig) "Byte[]" "Orig key should be exist"

              Expect.equal (actual.Data.Value.ContainsKey "one") true "Data should contains `one` key"
              Expect.equal (actual.Data.Value.ContainsKey "two") true "Data should contains `two` key"
              Expect.equal (actual.Data.Value.ContainsKey "three") true "Data should contains `three` key"

              Expect.equal actual.Data.Value.["one"] "foo" "One key should be equal"
              Expect.equal actual.Data.Value.["two"] "bar" "Two key should be equal"
              Expect.equal actual.Data.Value.["three"] "baz" "Three key should be equal"
          }

          ]
