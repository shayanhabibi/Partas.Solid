﻿namespace Partas.Solid.Start

open Fable.Core
open Partas.Solid

/// <summary>
/// Copied from <c>System.Net</c>
/// </summary>
type HttpStatusCode =
    // Informational 1xx
    | Continue = 100
    | SwitchingProtocols = 101
    | Processing = 102
    | EarlyHints = 103

    // Successful 2xx
    | OK = 200
    | Created = 201
    | Accepted = 202
    | NonAuthoritativeInformation = 203
    | NoContent = 204
    | ResetContent = 205
    | PartialContent = 206
    | MultiStatus = 207
    | AlreadyReported = 208

    | IMUsed = 226

    // Redirection 3xx
    | MultipleChoices = 300
    | Ambiguous = 300
    | MovedPermanently = 301
    | Moved = 301
    | Found = 302
    | Redirect = 302
    | SeeOther = 303
    | RedirectMethod = 303
    | NotModified = 304
    | UseProxy = 305
    | Unused = 306
    | TemporaryRedirect = 307
    | RedirectKeepVerb = 307
    | PermanentRedirect = 308

    // Client Error 4xx
    | BadRequest = 400
    | Unauthorized = 401
    | PaymentRequired = 402
    | Forbidden = 403
    | NotFound = 404
    | MethodNotAllowed = 405
    | NotAcceptable = 406
    | ProxyAuthenticationRequired = 407
    | RequestTimeout = 408
    | Conflict = 409
    | Gone = 410
    | LengthRequired = 411
    | PreconditionFailed = 412
    | RequestEntityTooLarge = 413
    | RequestUriTooLong = 414
    | UnsupportedMediaType = 415
    | RequestedRangeNotSatisfiable = 416
    | ExpectationFailed = 417
    // From https://github.com/dotnet/runtime/issues/15650:
    // "It would be a mistake to add it to .NET now. See golang/go#21326,
    // nodejs/node#14644, requests/requests#4238 and aspnet/HttpAbstractions#915".
    // ImATeapot = 41

    | MisdirectedRequest = 421
    | UnprocessableEntity = 422
    | UnprocessableContent = 422
    | Locked = 423
    | FailedDependency = 424

    | UpgradeRequired = 426

    | PreconditionRequired = 428
    | TooManyRequests = 429

    | RequestHeaderFieldsTooLarge = 431

    | UnavailableForLegalReasons = 451

    // Server Error 5xx
    | InternalServerError = 500
    | NotImplemented = 501
    | BadGateway = 502
    | ServiceUnavailable = 503
    | GatewayTimeout = 504
    | HttpVersionNotSupported = 505
    | VariantAlsoNegotiates = 506
    | InsufficientStorage = 507
    | LoopDetected = 508

    | NotExtended = 510
    | NetworkAuthenticationRequired = 511

[<AutoOpen>]
module Bindings =
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    module Spec =
        [<Literal>]
        let path = "@solidjs/start"

    [<Erase; AutoOpen>]
    type Exports =
        [<Emit("\"use server\"", isStatement = true)>]
        static member useServer: unit = jsNative

        [<Mangle>]
        static member inline getServerFunctionMeta =
            let inline fn () : unit -> {| id: string |} =
                (JsInterop.import "getServerFunctionMeta" Spec.path)

            (fn () ()).id

        [<ImportMember(Spec.path
                       + "/config")>]
        static member defineConfig(config: obj) : obj = jsNative

        static member inline defineConfig(objList: (string * obj) list) =
            defineConfig (JsInterop.createObj objList)

    [<ImportMember(Spec.path
                   + "/server")>]
    type StartServer() =
        interface FragmentNode

        [<DefaultValue>]
        val mutable document:
            ({| children: HtmlElement
                assets: link
                scripts: script |}
                -> html)

    [<Erase>]
    module Client =
        [<ImportMember(Spec.path
                       + "/client")>]
        let mount (fn: unit -> HtmlElement, el: obj) = jsNative

    [<ImportMember(Spec.path
                   + "/client")>]
    type StartClient() =
        interface HtmlElement

    type private T = HttpStatusCode

    [<ImportMember(Spec.path)>]
    type HttpStatusCode() =
        interface VoidNode

        [<DefaultValue>]
        val mutable code: T

    [<ImportMember(Spec.path)>]
    type HttpHeader() =
        interface VoidNode

        [<DefaultValue>]
        val mutable name: string

        [<DefaultValue>]
        val mutable value: string

    [<ImportMember(Spec.path
                   + "/router")>]
    type FileRoutes() =
        interface HtmlElement

        [<Emit("$0")>]
        member this.ToRoute() =
            unbox<Router.Bindings.Route> this

    type Exports with
        [<ImportMember(Spec.path)>]
        static member clientOnly(importFunc: unit -> JS.Promise<HtmlElement>) : TagValue = jsNative
