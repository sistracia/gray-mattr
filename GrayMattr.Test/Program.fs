open Expecto
open Stringify
open ParseYAML
open ParseJSON
open ParseCustom
open MattrTest
open MattrRead
open MattrLanguage
open Mattr
open MattrExcerpt
open MattrWindow
open MattrEmpty

[<Tests>]
let tests =
    testList
        "gray-mattr"
        [ stringifyTests
          parseYAMLTests
          parseJSONTests
          parseCustomTests
          mattrTestTests
          mattrReadTests
          mattrLanguageTests
          mattrTests
          mattrExcerptTests
          mattrWindowTests
          mattrEmptyTests ]


[<EntryPoint>]
let main args = runTestsWithCLIArgs [] args tests
