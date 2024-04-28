module MattrExcerpt

open System.Collections.Generic
open Expecto
open GrayMattr

type CustomExcerptor<'TData>() =
    interface IGrayExcerpt<'TData, GrayOption<'TData>> with
        member _.Excerpt(file: GrayFile<'TData>, _: GrayOption<'TData>) : GrayFile<'TData> =
            { file with Excerpt = "custom" }

let mattrExcerptTests =
    testList
        ".language"
        [

          test "should get an excerpt after front matter" {
              let actual: GrayFile<IDictionary<string, obj>> =
                  Mattr.Parse("---\nabc: xyz\n---\nfoo\nbar\nbaz\n---\ncontent", Mattr.DefaultOption().SetExcerpt(true))

              Expect.equal actual.Matter "\nabc: xyz" "Matter should be equal"
              Expect.equal actual.Content "foo\nbar\nbaz\n---\ncontent" "Content should be equal"
              Expect.equal actual.Excerpt "foo\nbar\nbaz\n" "Excerpt should be equal"
              Expect.equal actual.Data.Value["abc"] "xyz" "Data abc should be equal"
          }

          test "should not get excerpt when disabled" {
              let actual: GrayFile<IDictionary<string, obj>> =
                  Mattr.Parse("---\nabc: xyz\n---\nfoo\nbar\nbaz\n---\ncontent")

              Expect.equal actual.Matter "\nabc: xyz" "Matter should be equal"
              Expect.equal actual.Content "foo\nbar\nbaz\n---\ncontent" "Content should be equal"
              Expect.equal actual.Excerpt "" "Excerpt should be equal"
              Expect.equal actual.Data.Value["abc"] "xyz" "Data abc should be equal"
          }

          test "should use a custom separator" {
              let actual: GrayFile<IDictionary<string, obj>> =
                  Mattr.Parse(
                      "---\nabc: xyz\n---\nfoo\nbar\nbaz\n<!-- endexcerpt -->\ncontent",
                      Mattr.DefaultOption().SetExcerptSeparator("<!-- endexcerpt -->")
                  )

              Expect.equal actual.Matter "\nabc: xyz" "Matter should be equal"
              Expect.equal actual.Content "foo\nbar\nbaz\n<!-- endexcerpt -->\ncontent" "Content should be equal"
              Expect.equal actual.Excerpt "foo\nbar\nbaz\n" "Excerpt should be equal"
              Expect.equal actual.Data.Value["abc"] "xyz" "Data abc should be equal"
          }

          test "should use a custom separator when no front-matter exists" {
              let actual: GrayFile<IDictionary<string, obj>> =
                  Mattr.Parse(
                      "foo\nbar\nbaz\n<!-- endexcerpt -->\ncontent",
                      Mattr.DefaultOption().SetExcerptSeparator("<!-- endexcerpt -->")
                  )

              Expect.equal actual.Matter "" "Matter should be equal"
              Expect.equal actual.Content "foo\nbar\nbaz\n<!-- endexcerpt -->\ncontent" "Content should be equal"
              Expect.equal actual.Excerpt "foo\nbar\nbaz\n" "Excerpt should be equal"
              Expect.equal (Option.isNone actual.Data) true "Data key should be exist"
          }

          test "should use a custom function to get excerpt" {
              let actual: GrayFile<IDictionary<string, obj>> =
                  Mattr.Parse(
                      "---\nabc: xyz\n---\nfoo\nbar\nbaz\n---\ncontent",
                      Mattr.DefaultOption().SetExcerpt(CustomExcerptor())
                  )

              Expect.equal actual.Matter "\nabc: xyz" "Matter should be equal"
              Expect.equal actual.Content "foo\nbar\nbaz\n---\ncontent" "Content should be equal"
              Expect.equal actual.Excerpt "custom" "Excerpt should be equal"
              Expect.equal actual.Data.Value["abc"] "xyz" "Data abc should be equal"
          }

          ]
