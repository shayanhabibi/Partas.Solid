/// <summary>
/// Reimplements the Erased Unions with implicit conversion from types into the erasure.
/// This simplifies bindings and consuming bindings as type signatures more closely resemble
/// the target.<br/>
/// Experimenting to see whether this improves or detracts from the developer experience.
/// </summary>
module Partas.Solid.Experimental.U

open Fable.Core

/// Erased union type to represent one of two possible values.
/// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
[<Erase>]
type U2<'a, 'b> =
    | Case1 of 'a
    | Case2 of 'b

    static member op_ErasedCast(x: 'a) = Case1 x
    static member op_ErasedCast(x: 'b) = Case2 x
    static member inline op_Implicit(x: 'a): U2<'a, 'b> = Case1 x
    static member inline op_Implicit(x: 'b): U2<'a, 'b> = Case2 x

/// Erased union type to represent one of three possible values.
/// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
[<Erase>]
type U3<'a, 'b, 'c> =
    | Case1 of 'a
    | Case2 of 'b
    | Case3 of 'c

    static member op_ErasedCast(x: 'a) = Case1 x
    static member op_ErasedCast(x: 'b) = Case2 x
    static member op_ErasedCast(x: 'c) = Case3 x
    static member inline op_Implicit(x: 'a): U3<'a, 'b, 'c> = Case1 x
    static member inline op_Implicit(x: 'b): U3<'a, 'b, 'c> = Case2 x
    static member inline op_Implicit(x: 'c): U3<'a, 'b, 'c> = Case3 x

/// Erased union type to represent one of four possible values.
/// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
[<Erase>]
type U4<'a, 'b, 'c, 'd> =
    | Case1 of 'a
    | Case2 of 'b
    | Case3 of 'c
    | Case4 of 'd

    static member op_ErasedCast(x: 'a) = Case1 x
    static member op_ErasedCast(x: 'b) = Case2 x
    static member op_ErasedCast(x: 'c) = Case3 x
    static member op_ErasedCast(x: 'd) = Case4 x
    static member inline op_Implicit(x: 'a): U4<'a, 'b, 'c, 'd> = Case1 x
    static member inline op_Implicit(x: 'b): U4<'a, 'b, 'c, 'd> = Case2 x
    static member inline op_Implicit(x: 'c): U4<'a, 'b, 'c, 'd> = Case3 x
    static member inline op_Implicit(x: 'd): U4<'a, 'b, 'c, 'd> = Case4 x

/// Erased union type to represent one of five possible values.
/// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
[<Erase>]
type U5<'a, 'b, 'c, 'd, 'e> =
    | Case1 of 'a
    | Case2 of 'b
    | Case3 of 'c
    | Case4 of 'd
    | Case5 of 'e

    static member op_ErasedCast(x: 'a) = Case1 x
    static member op_ErasedCast(x: 'b) = Case2 x
    static member op_ErasedCast(x: 'c) = Case3 x
    static member op_ErasedCast(x: 'd) = Case4 x
    static member op_ErasedCast(x: 'e) = Case5 x
    static member inline op_Implicit(x: 'a): U5<'a, 'b, 'c, 'd, 'e> = Case1 x
    static member inline op_Implicit(x: 'b): U5<'a, 'b, 'c, 'd, 'e> = Case2 x
    static member inline op_Implicit(x: 'c): U5<'a, 'b, 'c, 'd, 'e> = Case3 x
    static member inline op_Implicit(x: 'd): U5<'a, 'b, 'c, 'd, 'e> = Case4 x
    static member inline op_Implicit(x: 'e): U5<'a, 'b, 'c, 'd, 'e> = Case5 x

/// Erased union type to represent one of six possible values.
/// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
[<Erase>]
type U6<'a, 'b, 'c, 'd, 'e, 'f> =
    | Case1 of 'a
    | Case2 of 'b
    | Case3 of 'c
    | Case4 of 'd
    | Case5 of 'e
    | Case6 of 'f

    static member op_ErasedCast(x: 'a) = Case1 x
    static member op_ErasedCast(x: 'b) = Case2 x
    static member op_ErasedCast(x: 'c) = Case3 x
    static member op_ErasedCast(x: 'd) = Case4 x
    static member op_ErasedCast(x: 'e) = Case5 x
    static member op_ErasedCast(x: 'f) = Case6 x
    static member inline op_Implicit(x: 'a): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case1 x
    static member inline op_Implicit(x: 'b): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case2 x
    static member inline op_Implicit(x: 'c): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case3 x
    static member inline op_Implicit(x: 'd): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case4 x
    static member inline op_Implicit(x: 'e): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case5 x
    static member inline op_Implicit(x: 'f): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case6 x

/// Erased union type to represent one of seven possible values.
/// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
[<Erase>]
type U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> =
    | Case1 of 'a
    | Case2 of 'b
    | Case3 of 'c
    | Case4 of 'd
    | Case5 of 'e
    | Case6 of 'f
    | Case7 of 'g

    static member op_ErasedCast(x: 'a) = Case1 x
    static member op_ErasedCast(x: 'b) = Case2 x
    static member op_ErasedCast(x: 'c) = Case3 x
    static member op_ErasedCast(x: 'd) = Case4 x
    static member op_ErasedCast(x: 'e) = Case5 x
    static member op_ErasedCast(x: 'f) = Case6 x
    static member op_ErasedCast(x: 'g) = Case7 x
    static member inline op_Implicit(x: 'a): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case1 x
    static member inline op_Implicit(x: 'b): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case2 x
    static member inline op_Implicit(x: 'c): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case3 x
    static member inline op_Implicit(x: 'd): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case4 x
    static member inline op_Implicit(x: 'e): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case5 x
    static member inline op_Implicit(x: 'f): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case6 x
    static member inline op_Implicit(x: 'g): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case7 x

/// Erased union type to represent one of eight possible values.
/// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
[<Erase>]
type U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> =
    | Case1 of 'a
    | Case2 of 'b
    | Case3 of 'c
    | Case4 of 'd
    | Case5 of 'e
    | Case6 of 'f
    | Case7 of 'g
    | Case8 of 'h

    static member op_ErasedCast(x: 'a) = Case1 x
    static member op_ErasedCast(x: 'b) = Case2 x
    static member op_ErasedCast(x: 'c) = Case3 x
    static member op_ErasedCast(x: 'd) = Case4 x
    static member op_ErasedCast(x: 'e) = Case5 x
    static member op_ErasedCast(x: 'f) = Case6 x
    static member op_ErasedCast(x: 'g) = Case7 x
    static member op_ErasedCast(x: 'h) = Case8 x
    static member inline op_Implicit(x: 'a): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case1 x
    static member inline op_Implicit(x: 'b): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case2 x
    static member inline op_Implicit(x: 'c): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case3 x
    static member inline op_Implicit(x: 'd): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case4 x
    static member inline op_Implicit(x: 'e): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case5 x
    static member inline op_Implicit(x: 'f): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case6 x
    static member inline op_Implicit(x: 'g): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case7 x
    static member inline op_Implicit(x: 'h): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case8 x

/// Erased union type to represent one of nine or more possible values.
/// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
[<Erase>]
type U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> =
    | Case1 of 'a
    | Case2 of 'b
    | Case3 of 'c
    | Case4 of 'd
    | Case5 of 'e
    | Case6 of 'f
    | Case7 of 'g
    | Case8 of 'h
    | Case9 of 'i

    static member op_ErasedCast(x: 'a) = Case1 x
    static member op_ErasedCast(x: 'b) = Case2 x
    static member op_ErasedCast(x: 'c) = Case3 x
    static member op_ErasedCast(x: 'd) = Case4 x
    static member op_ErasedCast(x: 'e) = Case5 x
    static member op_ErasedCast(x: 'f) = Case6 x
    static member op_ErasedCast(x: 'g) = Case7 x
    static member op_ErasedCast(x: 'h) = Case8 x
    static member op_ErasedCast(x: 'i) = Case9 x
    static member inline op_Implicit(x: 'a): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case1 x
    static member inline op_Implicit(x: 'b): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case2 x
    static member inline op_Implicit(x: 'c): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case3 x
    static member inline op_Implicit(x: 'd): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case4 x
    static member inline op_Implicit(x: 'e): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case5 x
    static member inline op_Implicit(x: 'f): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case6 x
    static member inline op_Implicit(x: 'g): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case7 x
    static member inline op_Implicit(x: 'h): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case8 x
    static member inline op_Implicit(x: 'i): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case9 x

    static member inline op_ErasedCast(x: 't) : U9<_, _, _, _, _, _, _, _, ^U> =
        Case9(^U: (static member op_ErasedCast: 't -> ^U) x)
