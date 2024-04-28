module MattrLanguage

open Expecto
open GrayMattr

let mattrLanguageTests =
    testList
        ".language"
        [

          test "should detect the name of the language to parse" {
              let actual: GrayLanguage = Mattr.Language("---\nfoo: bar\n---")
              Expect.equal actual.Raw "" "Raw should be equal"
              Expect.equal actual.Name "" "Name should be equal"

              let actual: GrayLanguage = Mattr.Language("---js\nfoo: bar\n---")
              Expect.equal actual.Raw "js" "Raw should be equal"
              Expect.equal actual.Name "js" "Name should be equal"

              let actual: GrayLanguage = Mattr.Language("---coffee\nfoo: bar\n---")
              Expect.equal actual.Raw "coffee" "Raw should be equal"
              Expect.equal actual.Name "coffee" "Name should be equal"
          }

          test "should work around whitespace" {
              let actual: GrayLanguage = Mattr.Language("--- \nfoo: bar\n---")
              Expect.equal actual.Raw " " "Raw should be equal"
              Expect.equal actual.Name "" "Name should be equal"

              let actual: GrayLanguage = Mattr.Language("--- js \nfoo: bar\n---")
              Expect.equal actual.Raw " js " "Raw should be equal"
              Expect.equal actual.Name "js" "Name should be equal"

              let actual: GrayLanguage = Mattr.Language("---  coffee \nfoo: bar\n---")
              Expect.equal actual.Raw "  coffee " "Raw should be equal"
              Expect.equal actual.Name "coffee" "Name should be equal"
          }

          ]
