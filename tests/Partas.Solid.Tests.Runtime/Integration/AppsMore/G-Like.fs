module Partas.Solid.Tests.Runtime.Integration.AppsMore.Like

open Partas.Solid
open Partas.Solid.Aria
open Fable.Core
open Fable.Core.JsInterop

/// F# has no generator syntax, so a two-step generator (`yield first; ... return`) for `action`
/// is written by hand: `next(v)` runs the next step with the value sent back in.
let private twoStep (first: unit -> obj) (second: obj -> unit) : obj =
    let mutable i = 0

    createObj
        [ "next"
          ==> fun (v: obj) ->
              i <- i + 1

              if i = 1 then
                  createObj [ "done" ==> false; "value" ==> first () ]
              else
                  second v
                  createObj [ "done" ==> true; "value" ==> null ]
          "throw" ==> fun (e: obj) -> raise (unbox<exn> e) ]

/// A like button with optimistic UI.
/// Clicking flips the heart and the count immediately (createOptimistic), then the action awaits
/// the server; on success the authoritative signals are written, on failure the optimistic values
/// revert and an error is shown.
[<Erase>]
type LikeButton() =
    inherit div()

    [<Erase>]
    member val initialLikes: int = unbox null with get, set

    [<Erase>]
    member val initiallyLiked: bool = unbox null with get, set

    /// Persists the new liked state; resolves with the server's like count.
    [<Erase>]
    member val save: bool -> JS.Promise<int> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let likes, setLikes = createSignal props.initialLikes
        let liked, setLiked = createSignal props.initiallyLiked
        let error, setError = createSignal ""

        let optLikes, setOptLikes = createOptimistic<int> (fun () -> likes ())
        let optLiked, setOptLiked = createOptimistic<bool> (fun () -> liked ())

        let toggleLike: bool -> JS.Promise<unit> =
            action (fun (target: bool) ->
                twoStep
                    (fun () ->
                        setOptLiked target
                        setOptLikes (optLikes () + (if target then 1 else -1))
                        box (props.save target))
                    (fun confirmed ->
                        setLikes (unbox<int> confirmed)
                        setLiked target))

        div (class' = "like-widget") {
            button (
                class' = (if optLiked () then "like liked" else "like"),
                onClick =
                    fun _ ->
                        setError ""

                        toggleLike(not (optLiked ()))
                            .catch(fun (e: obj) -> setError ("Could not save: " + (string e?message)))
                        |> ignore
            ) {
                if optLiked () then "♥" else "♡"
            }

            span (class' = "count") { $"{optLikes ()} likes" }
            span (class' = "confirmed") { $"{likes ()}" }

            Show (when' = (error () <> "")) { p (class' = "like-error", role = "alert") { error () } }
        }
