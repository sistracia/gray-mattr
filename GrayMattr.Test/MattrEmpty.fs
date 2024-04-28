module MattrEmpty

open Expecto
open GrayMattr

let mattrEmptyTests =
    testList
        "gray-matter"
        [

          test "should work with empty front-matter" {
              let actual: GrayFile<obj> = Mattr.Parse("---\n---\nThis is content")
              Expect.equal actual.Content "This is content" "Content should be equal"
              Expect.equal (Option.isNone actual.Data) true "Should be have none data"

              let actual: GrayFile<obj> = Mattr.Parse("---\n\n---\nThis is content")
              Expect.equal actual.Content "This is content" "Content should be equal"
              Expect.equal (Option.isNone actual.Data) true "Should be have none data"

              let actual: GrayFile<obj> = Mattr.Parse("---\n\n\n\n\n\n---\nThis is content")
              Expect.equal actual.Content "This is content" "Content should be equal"
              Expect.equal (Option.isNone actual.Data) true "Should be have none data"
          }

          test "should add content with empty front matter to file.empty" {
              let fixture: string = "---\n---"
              let actual: GrayFile<obj> = Mattr.Parse(fixture)
              Expect.equal actual.Empty.Value fixture "Empty should be equal"
          }

          test "should update file.isEmpty to true" {
              let actual: GrayFile<obj> = Mattr.Parse("---\n---")
              Expect.equal actual.IsEmpty true "IsEmpty should be equal"
          }

          test "should work when front-matter has comments" {
              let fixture: string = "---\n # this is a comment\n# another one\n---"
              let actual: GrayFile<obj> = Mattr.Parse(fixture)
              Expect.equal actual.Empty.Value fixture "Empty should be equal"
          }

          ]
