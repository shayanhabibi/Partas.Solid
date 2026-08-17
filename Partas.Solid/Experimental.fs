/// <summary>
/// <b>WARNING</b><br/>
/// This module contains syntax sugars that are not yet battle tested.<br/><br/>
/// Contains the following builders:<br/>effect<br/>memo<br/>mount<br/>cleanup<br/>batch<br/>lazyload<br/>selector<br/>children
/// </summary>
namespace Partas.Solid.Experimental

open Partas.Solid
open Fable.Core

#nowarn 64 49

/// <summary>
/// Lambdas that are of the signature <c>unit -> unit</c> are common enough that we can
/// build this as a base for things like <c>createEffect</c> to inherit
/// </summary>
[<EditorBrowsable(EditorBrowsableState.Never)>]
[<Erase>]
type NullLambdaBuilder() =
    member inline _.Return(x) =
        fun () -> x

    member inline _.Bind(m, f) =
        fun () -> f (m ()) ()

    member inline _.Zero() = ignore
    member inline _.Delay(f) = f

    member inline _.Combine([<InlineIfLambda>] PARTAS_FIRST: 'T -> unit, [<InlineIfLambda>] PARTAS_SECOND) =
        fun () ->
            PARTAS_FIRST ()
            PARTAS_SECOND ()

/// <summary>
/// Lambdas that take null parameters and return a type are common enough to build
/// a sugar wrapper that things like <c>createMemo</c> can inherit
/// </summary>
[<EditorBrowsable(EditorBrowsableState.Never)>]
[<Erase>]
type BaseLambdaBuilder() =
    member inline _.Return(x) =
        fun () -> x

    member inline _.Bind(m, f) =
        fun () -> f (m ()) ()

    member inline _.Delay(f) = f

    member inline _.Combine([<InlineIfLambda>] PARTAS_FIRST: 'T -> unit, [<InlineIfLambda>] PARTAS_SECOND) =
        ignore PARTAS_FIRST
        PARTAS_SECOND ()

    member inline _.Yield(PARTAS_VALUE) =
        PARTAS_VALUE

[<EditorBrowsable(EditorBrowsableState.Never)>]
[<Erase>]
type LambdaBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Zero() = ignore

    member inline _.Run(code: unit -> 'T) : unit -> 'T = fun () -> code ()
