module MattrTest

open Expecto
open GrayMattr

let mattrTestTests =
    testList
        ".test"
        [

          test "should return `true` if the string has front-matter:" {
              Expect.equal (Mattr.Test("---\nabc: xyz\n---")) true "First expect should be true"

              Expect.equal
                  (Mattr.Test("---\nabc: xyz\n---", Mattr.DefaultOption().SetDelimiters("~~~")))
                  false
                  "Second expect should be false"

              Expect.equal
                  (Mattr.Test("~~~\nabc: xyz\n~~~", Mattr.DefaultOption().SetDelimiters("~~~")))
                  true
                  "Third expect should be true"

              Expect.equal (Mattr.Test("\nabc: xyz\n---")) false "Forth expect should be false"

          }

          ]
