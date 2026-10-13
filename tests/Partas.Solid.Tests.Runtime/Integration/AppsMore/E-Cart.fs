module Partas.Solid.Tests.Runtime.Integration.AppsMore.Cart

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

type CatalogItem =
    {| sku: string
       name: string
       /// Price in cents.
       price: int |}

type CartLine =
    { sku: string
      name: string
      price: int
      mutable qty: int }

/// Cents -> "$12.30".
let money (cents: int) =
    let c = string (abs cents % 100)
    let sign = if cents < 0 then "-" else ""
    sign + "$" + string (abs cents / 100) + "." + (if c.Length = 1 then "0" + c else c)

/// One catalog entry with an "Add" button.
[<Erase>]
type CatalogRow() =
    inherit li()

    [<Erase>]
    member val item: CatalogItem = unbox null with get, set

    [<Erase>]
    member val onAdd: CatalogItem -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        li(class' = "catalog-item").data ("sku", props.item.sku) {
            span (class' = "name") { props.item.name }
            span (class' = "price") { money props.item.price }
            button (class' = "add", onClick = fun _ -> props.onAdd props.item) { "Add" }
        }

/// One cart line: -/+ buttons, a numeric quantity input, remove, and a line total.
[<Erase>]
type CartRow() =
    inherit li()

    [<Erase>]
    member val line: CartLine = unbox null with get, set

    [<Erase>]
    member val onQty: int -> unit = unbox null with get, set

    [<Erase>]
    member val onRemove: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        li(class' = "line").data ("sku", props.line.sku) {
            span (class' = "line-name") { props.line.name }
            button (class' = "dec", onClick = fun _ -> props.onQty (props.line.qty - 1)) { "-" }

            input (
                class' = "qty",
                type' = "number",
                value = string props.line.qty,
                onChange =
                    fun e ->
                        let raw: string = !!e.currentTarget?value
                        let parsed = JS.parseInt raw 10
                        props.onQty (if JS.isNaN parsed then 0 else int parsed)
            )

            button (class' = "inc", onClick = fun _ -> props.onQty (props.line.qty + 1)) { "+" }
            span (class' = "line-total") { money (props.line.price * props.line.qty) }
            button (class' = "remove", onClick = fun _ -> props.onRemove props.line.sku) { "Remove" }
        }

/// Shopping cart: store of lines, memo-derived totals, discount codes and free-shipping threshold.
[<Erase>]
type ShopApp() =
    inherit div()

    [<Erase>]
    member val catalog: CatalogItem array = unbox null with get, set

    /// Called every time the total memo recomputes (to observe memo granularity).
    [<Erase>]
    member val onTotal: int -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let cart, setCart = createStore<CartLine array> [||]
        let code, setCode = createSignal ""
        let appliedCode, setAppliedCode = createSignal ""

        let itemCount =
            createMemo (fun (_: int option) -> cart.Value |> Array.sumBy (fun l -> l.qty))

        let subtotal =
            createMemo (fun (_: int option) -> cart.Value |> Array.sumBy (fun l -> l.price * l.qty))

        let discount =
            createMemo (fun (_: int option) ->
                match appliedCode () with
                | "SAVE10" -> subtotal () / 10
                | "FIVEOFF" -> min (subtotal ()) 500
                | _ -> 0)

        let shipping =
            createMemo (fun (_: int option) ->
                if subtotal () = 0 || subtotal () - discount () >= 5000 then 0 else 499)

        let total =
            createMemo (fun (_: int option) ->
                let t = subtotal () - discount () + shipping ()

                if not (isNull (box props.onTotal)) then
                    props.onTotal t

                t)

        let add (item: CatalogItem) =
            if cart.Value |> Array.exists (fun l -> l.sku = item.sku) then
                setCart (fun lines ->
                    for l in lines do
                        if l.sku = item.sku then
                            l.qty <- l.qty + 1

                    lines)
            else
                setCart (fun lines ->
                    Array.append
                        lines
                        [| { sku = item.sku
                             name = item.name
                             price = item.price
                             qty = 1 } |])

        let remove (sku: string) =
            setCart (fun lines -> lines |> Array.filter (fun l -> l.sku <> sku))

        let setQty (sku: string) (qty: int) =
            if qty <= 0 then
                remove sku
            else
                setCart (fun lines ->
                    for l in lines do
                        if l.sku = sku then
                            l.qty <- qty

                    lines)

        div (class' = "shop") {
            ul (class' = "catalog") {
                For.Keyed (each = props.catalog) { yield fun item _ -> CatalogRow(item = item, onAdd = add) }
            }

            section (class' = "cart") {
                h2 () { $"Cart ({itemCount ()})" }

                ul (class' = "lines") {
                    For.Keyed (each = cart.Value, fallback = li (class' = "empty") { "Your cart is empty" }) {
                        yield
                            fun line _ ->
                                CartRow(line = line, onQty = setQty line.sku, onRemove = remove)
                    }
                }

                div (class' = "promo") {
                    input (class' = "code", value = code (), onInput = fun e -> setCode (!!e.currentTarget?value))

                    button (class' = "apply", onClick = fun _ -> setAppliedCode (code().Trim().ToUpper ())) {
                        "Apply"
                    }

                    Show (when' = (appliedCode () <> "" && discount () = 0 && subtotal () > 0)) {
                        span (class' = "code-error") { "Code not valid" }
                    }
                }

                dl (class' = "summary") {
                    dt () { "Subtotal" }
                    dd (class' = "subtotal") { money (subtotal ()) }

                    Show (when' = (discount () > 0)) {
                        dt () { "Discount" }
                        dd (class' = "discount") { money (-discount ()) }
                    }

                    dt () { "Shipping" }
                    dd (class' = "shipping") { if shipping () = 0 then "Free" else money (shipping ()) }
                    dt () { "Total" }
                    dd (class' = "total") { money (total ()) }
                }

                button (class' = "checkout", disabled = (itemCount () = 0)) { "Checkout" }
            }
        }
