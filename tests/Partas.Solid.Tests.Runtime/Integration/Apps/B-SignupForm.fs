module Partas.Solid.Tests.Runtime.Integration.Apps.SignupForm

open Partas.Solid
open Partas.Solid.Aria
open Fable.Core
open Fable.Core.JsInterop

type SignupValues = { name: string; email: string; code: string }

/// Controlled form: every input is driven by a signal, validation messages are memos,
/// errors appear after blur ("touched") or after a submit attempt.
[<Erase>]
type SignupForm() =
    inherit div()

    [<Erase>]
    member val onSave: SignupValues -> unit = unbox null with get, set

    [<Erase>]
    member val minNameLength: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.minNameLength <- 2

        let name, setName = createSignal ""
        let email, setEmail = createSignal ""
        let code, setCode = createSignal ""
        let touchedName, setTouchedName = createSignal false
        let touchedEmail, setTouchedEmail = createSignal false
        let attempted, setAttempted = createSignal false
        let welcome, setWelcome = createSignal ""

        let nameError =
            createMemo (fun (_: string option) ->
                let n = name().Trim ()

                if n = "" then
                    "Name is required"
                elif n.Length < props.minNameLength then
                    $"Name must be at least {props.minNameLength} characters"
                else
                    "")

        let emailError =
            createMemo (fun (_: string option) ->
                let e = email().Trim ()

                if e = "" then "Email is required"
                elif not (e.Contains "@") then "Email is invalid"
                else "")

        let errorCount =
            createMemo (fun (_: int option) ->
                (if nameError () <> "" then 1 else 0)
                + (if emailError () <> "" then 1 else 0))

        let showNameError () =
            (touchedName () || attempted ()) && nameError () <> ""

        let showEmailError () =
            (touchedEmail () || attempted ()) && emailError () <> ""

        form (
            class' = "signup",
            noValidate = true,
            onSubmit =
                fun e ->
                    e.preventDefault ()
                    setAttempted true

                    if errorCount () = 0 then
                        props.onSave
                            { name = name().Trim ()
                              email = email().Trim ()
                              code = code () }

                        setWelcome (name().Trim ())
                        setName ""
                        setEmail ""
                        setCode ""
                        setTouchedName false
                        setTouchedEmail false
                        setAttempted false
        ) {
            label (for' = "su-name") { "Name" }

            input (
                id = "su-name",
                class' = "name",
                name = "name",
                value = name (),
                ariaInvalid = (if showNameError () then "true" else "false"),
                onInput = (fun e -> setName (!!e.currentTarget?value)),
                onBlur = fun _ -> setTouchedName true
            )

            span (class' = "counter") { $"{name().Length} chars" }

            Show (when' = showNameError ()) { p (class' = "error name-error", role = "alert") { nameError () } }

            label (for' = "su-email") { "Email" }

            input (
                id = "su-email",
                class' = "email",
                type' = "email",
                value = email (),
                ariaInvalid = (if showEmailError () then "true" else "false"),
                onInput = (fun e -> setEmail (!!e.currentTarget?value)),
                onBlur = fun _ -> setTouchedEmail true
            )

            Show (when' = showEmailError ()) { p (class' = "error email-error", role = "alert") { emailError () } }

            // Transforming controlled input: the signal stores an upper-cased value.
            input (
                class' = "code",
                value = code (),
                onInput = fun e -> setCode ((!!e.currentTarget?value: string).ToUpper ())
            )

            Show (when' = (attempted () && errorCount () > 0)) {
                p (class' = "summary") {
                    $"""Please fix {errorCount ()} {if errorCount () = 1 then "error" else "errors"}"""
                }
            }

            button (type' = "submit", class' = "submit") { "Sign up" }

            Show (when' = (welcome () <> "")) { p (class' = "success") { $"Welcome, {welcome ()}!" } }
        }
