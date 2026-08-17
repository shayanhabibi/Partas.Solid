namespace System.Net
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
namespace Partas.Solid.Web

open System
open System.Runtime.CompilerServices
open Partas.Solid
open Fable.Core

module HeadTag =
    [<StringEnum; RequireQualifiedAccess>]
    type Tag =
        | Title
        | Meta
        | Link
        | Style
        | Script
        | Base

[<JS.Pojo>]
type HeadTag(
    tag: HeadTag.Tag,
    props: obj,
    ?key: U2<string, unit -> string>
    ) =
    [<Erase>] member val tag = tag with get, set
    [<Erase>] member val props = props with get, set
    [<Erase>] member val key = key with get, set



[<AutoOpen>]
module Bindings =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    module Spec =
        [<Literal>]
        let path = "@solidjs/web"



    [<PartasImport("Dynamic", Spec.path)>]
    type Dynamic<'T>() =
        interface HtmlElement

        [<DefaultValue; Erase>]
        val mutable component': TagValue

        [<Erase>]
        member this.componentAsString
            with inline set (value: string) = this.component' <- unbox value
            and inline get () = unbox<string> this.component'

        [<Obsolete(message = "Not implemented yet", error = true)>]
        [<CustomOperation "dynamicAttr">]
        member inline _.DynamicAttrOp([<InlineIfLambda>] PARTAS_FIRST, PARTAS_DYN_MAP: 'T -> 'U, PARTAS_DYN_VAL: 'U) : HtmlContainerFun =
            ignore PARTAS_DYN_MAP
            ignore PARTAS_DYN_VAL
            PARTAS_FIRST

        [<Erase>]
        member inline _.Zero() : HtmlContainerFun = ignore

        [<Erase>]
        member inline _.Yield(PARTAS_CONT: unit) : HtmlContainerFun = ignore

        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #HtmlElement) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

        [<Erase>]
        member inline _.Yield(PARTAS_TEXT: string) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_TEXT

        [<Erase>]
        member inline _.Yield(PARTAS_VALUE: int) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_VALUE

        [<Erase>]
        member inline _.Yield(PARTAS_VALUE: float) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_VALUE

        [<Erase>]
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER

        [<Erase>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> HtmlContainerFun) =
            PARTAS_DELAY ()

        [<Erase>]
        member inline _.For
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: unit -> HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND () PARTAS_BUILDER

    [<Erase>]
    type Extensions =
        [<Extension; Erase>]
        static member Run(PARTAS_THIS: Dynamic<'T>, PARTAS_RUNEXPR: HtmlContainerFun) =
            PARTAS_RUNEXPR Unchecked.defaultof<_>
            PARTAS_THIS

    [<Import("Portal", Spec.path)>]
    [<Erase>]
    type Portal() =
        interface HtmlContainer
        [<Erase; DefaultValue>] val mutable mount: Browser.Types.Element

[<AutoOpen; Erase>]
type Bindings =
    // todo - better clientOnly bindings
    [<ImportMember(Spec.path); ParamObject(1)>]
    static member clientOnly<'T when 'T :> HtmlElement>(fn: unit -> JS.Promise<'T>, ?``lazy``: bool): unit -> 'T = jsNative
    [<ImportMember(Spec.path)>]
    static member clientOnly<'T when 'T :> HtmlElement>(fn: unit -> JS.Promise<'T>): unit -> 'T = jsNative

    [<ImportMember(Spec.path); ParamObject(2)>]
    static member httpHeader(name: string, value: string, ?append: bool): unit = jsNative
    [<ImportMember(Spec.path)>]
    static member httpStatus(code: System.Net.HttpStatusCode, ?text: string): unit = jsNative

    [<ImportMember(Spec.path); ParamObject(2)>]
    static member hydrate(code: unit -> HtmlElement, element: Browser.Types.Node, ?renderId: string, ?owner: objnull): DisposalFunc = jsNative

    [<ImportMember(Spec.path)>]
    static member isDev: bool = jsNative
    [<ImportMember(Spec.path)>]
    static member isServer: bool = jsNative

    [<ImportMember(Spec.path)>]
    static member renderToStream<'T>(fn: unit -> 'T): obj = jsNative
    [<ImportMember(Spec.path); ParamObject(1)>]
    static member renderToStream<'T>(
        fn: unit -> 'T,
        ?nonce: string,
        ?renderId: string,
        ?noScripts: bool,
        ?plugins: obj[],
        ?manifest: obj,
        ?onCompleteShell: {| write: string -> unit |} -> unit,
        ?onCompleteAll: {| write: string -> unit |} -> unit,
        ?onError: obj -> unit,
        ?onHead: string -> unit
        ): obj = jsNative

    [<ImportMember(Spec.path)>]
    static member renderToString<'T>(fn: unit -> 'T): string = jsNative
    [<ImportMember(Spec.path); ParamObject(1)>]
    static member renderToString<'T>(
        fn: unit -> 'T,
        ?nonce: string,
        ?renderId: string,
        ?noScripts: bool,
        ?plugins: obj[],
        ?manifest: obj,
        ?onError: obj -> unit,
        ?onHead: string -> unit
        ): string = jsNative

    [<ImportMember(Spec.path)>]
    static member render(fn: unit -> HtmlElement, element: Browser.Types.Node, ?init: obj, ?options: {| renderId: string option |} -> obj): DisposalFunc = jsNative

    [<ImportMember(Spec.path)>]
    static member useHead(tag: HeadTag): unit = jsNative
    [<ImportMember(Spec.path)>]
    static member useHead(tag: HeadTag[]): unit = jsNative
    [<ImportMember(Spec.path)>]
    static member useHead(tag: unit -> HeadTag): unit = jsNative
    [<ImportMember(Spec.path)>]
    static member useHead(tag: unit -> HeadTag[]): unit = jsNative
    [<ImportMember(Spec.path)>]
    static member dynamic<'T when 'T :> HtmlElement>(source: unit -> 'T): unit -> 'T = jsNative
    [<ImportMember(Spec.path)>]
    static member dynamic<'T when 'T :> HtmlElement>(source: unit -> JS.Promise<'T>): unit -> 'T = jsNative
