open Expecto
open StripBomStr

[<Tests>]
let tests =
    testList
        "strip-bom-str"
        [

          test "should strip bom:" {
              let actual = BOM.remove "\ufefffoo"
              Expect.equal actual "foo" "The strings should equal"
          }

          test "should return same as input" {
              let actual: string = BOM.remove "foo"
              Expect.equal actual actual "The strings should equal"
          }

          ]


[<EntryPoint>]
let main args = runTestsWithCLIArgs [] args tests
