module Partas.Solid.Tests.Runtime.Integration.AppsMore.Form

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

type AccountValues =
    { mutable username: string
      mutable email: string
      mutable password: string
      mutable confirm: string
      mutable agree: bool }

type Touched =
    { mutable username: bool
      mutable email: bool
      mutable password: bool
      mutable confirm: bool
      mutable agree: bool }

type AccountErrors =
    { username: string
      email: string
      password: string
      confirm: string
      agree: string }

let emptyValues () : AccountValues =
    { username = ""
      email = ""
      password = ""
      confirm = ""
      agree = false }

let untouched () : Touched =
    { username = false
      email = false
      password = false
      confirm = false
      agree = false }

let validate (v: AccountValues) : AccountErrors =
    { username =
        if v.username.Trim() = "" then "Username is required"
        elif v.username.Contains " " then "Username cannot contain spaces"
        else ""
      email =
        if v.email = "" then "Email is required"
        elif not (v.email.Contains "@") || not (v.email.Contains ".") then "Enter a valid email"
        else ""
      password =
        if v.password.Length < 8 then "Password must be at least 8 characters"
        else ""
      confirm = if v.confirm <> v.password then "Passwords do not match" else ""
      agree = if v.agree then "" else "You must accept the terms" }

/// Account form backed by stores. Values and touched flags are stores; the error record is a
/// memo over the values store. Submitting calls the async `onSubmit`, which resolves with a
/// server-side error ("" means success).
[<Erase>]
type AccountForm() =
    inherit div()

    [<Erase>]
    member val onSubmit: AccountValues -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let values, setValues = createStore (emptyValues ())
        let touched, setTouched = createStore (untouched ())
        let submitted, setSubmitted = createSignal false
        let saving, setSaving = createSignal false
        let serverError, setServerError = createSignal ""
        let savedAs, setSavedAs = createSignal ""

        let errors = createMemo (fun (_: AccountErrors option) -> validate values.Value)

        let errorCount =
            createMemo (fun (_: int option) ->
                let e = errors ()

                [ e.username; e.email; e.password; e.confirm; e.agree ]
                |> List.filter (fun s -> s <> "")
                |> List.length)

        let show (isTouched: bool) (msg: string) = (isTouched || submitted ()) && msg <> ""

        let reset () =
            setValues (fun _ -> emptyValues ())
            setTouched (fun _ -> untouched ())
            setSubmitted false
            setServerError ""

        let submit () =
            setSubmitted true
            setServerError ""

            if errorCount () = 0 then
                setSaving true
                // Hand the handler a plain copy, not the live store proxy.
                let copy: AccountValues =
                    { username = values.Value.username
                      email = values.Value.email
                      password = values.Value.password
                      confirm = values.Value.confirm
                      agree = values.Value.agree }

                props
                    .onSubmit(copy)
                    .``then``(fun (err: string) ->
                        setSaving false

                        if err = "" then
                            setSavedAs copy.username
                            reset ()
                        else
                            setServerError err)
                |> ignore

        let field (name: string) (inputType: string) (value: string) (err: string) (isTouched: bool) (onValue: string -> unit) (onBlur: unit -> unit) =
            div (class' = (if show isTouched err then $"field field-{name} invalid" else $"field field-{name}")) {
                input (
                    name = name,
                    type' = inputType,
                    value = value,
                    onInput = (fun e -> onValue (!!e.currentTarget?value)),
                    onBlur = fun _ -> onBlur ()
                )

                Show (when' = show isTouched err) { span (class' = "error") { err } }
            }

        form (
            class' = "account",
            onSubmit =
                fun e ->
                    e.preventDefault ()
                    submit ()
        ) {
            field "username" "text" values.Value.username (errors().username) touched.Value.username
                (fun v -> setValues (fun s -> s.username <- v; s))
                (fun () -> setTouched (fun t -> t.username <- true; t))

            field "email" "email" values.Value.email (errors().email) touched.Value.email
                (fun v -> setValues (fun s -> s.email <- v; s))
                (fun () -> setTouched (fun t -> t.email <- true; t))

            field "password" "password" values.Value.password (errors().password) touched.Value.password
                (fun v -> setValues (fun s -> s.password <- v; s))
                (fun () -> setTouched (fun t -> t.password <- true; t))

            field "confirm" "password" values.Value.confirm (errors().confirm) touched.Value.confirm
                (fun v -> setValues (fun s -> s.confirm <- v; s))
                (fun () -> setTouched (fun t -> t.confirm <- true; t))

            div (class' = "field field-agree") {
                input (
                    name = "agree",
                    type' = "checkbox",
                    checked' = values.Value.agree,
                    onChange =
                        fun e ->
                            let c: bool = !!e.currentTarget?``checked``
                            setValues (fun s -> s.agree <- c; s)
                )

                Show (when' = show touched.Value.agree (errors().agree)) {
                    span (class' = "error") { errors().agree }
                }
            }

            Show (when' = (submitted () && errorCount () > 0)) {
                p (class' = "summary") { $"Please fix {errorCount ()} field(s)" }
            }

            Show (when' = (serverError () <> "")) { p (class' = "server-error") { serverError () } }

            Show (when' = (savedAs () <> "")) { p (class' = "saved") { $"Account {savedAs ()} created" } }

            button (type' = "submit", class' = "submit", disabled = saving ()) {
                if saving () then "Saving..." else "Create account"
            }

            button (type' = "button", class' = "reset", onClick = fun _ -> reset ()) { "Reset" }
        }
