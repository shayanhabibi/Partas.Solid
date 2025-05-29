namespace Partas.Solid

open System
open Fable
open Fable.AST
open Fable.AST.Fable
/// Identifies what type of member the transformation sequence originated from
/// Currently ?unutilised
[<RequireQualifiedAccess>]
type internal TransformationKind =
    | MemberDecl
    | MemberCall
    | TypeMemberDecl
/// Used in Active Pattern Matches to identify if a MemberRef is one of these
[<RequireQualifiedAccess>]
type internal MemberRefType =
    | Setter
    | Getter
    | Constructor
    | Generated
    | None

/// Idents we are interested in that impact the flow of transformation.
[<RequireQualifiedAccess>]
type internal IdentType =
    // Compiler generated
    | ReturnVal
    // Partas type with _$ctor
    | Constructor
    // identifies props identifier
    | Props
    // identifies ContextProviders
    | ContextProvider 
    // Identifies for loops
    // | Enumerator of EnumType
    
    // Builders - PARTAS_{...}
    | Yield
    | First
    | Second
    | Element
    | Builder
    | Delay
    | Cont
    | Value
    | Text
    
    // Special cases

    // Unknown
    | Other

module Attributes =
    [<Literal>]
    let pojo = "Fable.Core.JS.PojoAttribute"

type ComponentFlag =
    /// Default transformations.
    | Default = 0b0000
    /// Prints the AST tree before transformation. Please supply a minimal
    /// example with this flag set when submitting issues.
    | DebugMode = 0b0001
    /// SolidComponents optimise Pojo constructors so that
    /// setter members which are not in the primary constructor that
    /// are used when building the pojo, are compiled into the object
    /// initialiser in JS instead of being deferred
    | SkipPojoOptimisation = 0b0010
    /// Noisy. Prints any expressions that are disposed of in transformation.
    | PrintDisposals = 0b0100
    /// SolidComponents optimise out computation expressions, particularly of
    /// lists etc. In Fable 5, this should be handled by the compiler.
    | SkipCEOptimisation = 0b1000

[<RequireQualifiedAccess>]
module ComponentFlag =
    /// Minimal transformations. Any optimisations that are skippable are skipped.
    [<Literal>]
    let None =
        ComponentFlag.SkipPojoOptimisation ||| ComponentFlag.SkipCEOptimisation

    [<Literal>]
    let VerboseDebugMode = ComponentFlag.DebugMode ||| ComponentFlag.PrintDisposals
/// A record which is passed through almost all transformation patterns and methods.
/// It can therefor be used to pass contextual information, or access the plugin helper
/// from within the transformation tree (to shoot out a warning or something)
type internal PluginContext =
    {
        Helper: PluginHelper
        Kind: TransformationKind
        SetterArray: ResizeArray<string * Expr>
        GetterArray: ResizeArray<string>
        SetterCollector: (string * Expr) -> unit
        GetterCollector: string -> unit
        Flags: ComponentFlag
    }
    member this.HasFlag flag = this.Flags.HasFlag flag

/// Container for Funcs of the PluginContext type
[<RequireQualifiedAccess>]
module internal PluginContext =
    /// <summary>
    /// Instantiates a PluginContext record which is threaded through
    /// the transformation sequences
    /// </summary>
    /// <param name="helper">PluginHelper provided in the <c>Transform</c> and <c>TransformCall</c>
    /// parameters</param>
    /// <param name="kind">Unutilised. Pass a <c>TransformationKind</c> DU</param>
    /// <param name="flags">Flags to modify transformations</param>
    /// <example><code>
    /// type SolidComponentAttribute() =
    ///     inherit MemberDeclarationPluginAttribute()
    ///     override this.Transform(pluginHelper, file, memberDecl) =
    ///     let ctx = PluginContext.create pluginHelper TransformationKind.MemberDecl
    /// </code></example>
    let create helper kind flags =
        let ctx = {
            Helper = helper
            Kind = kind
            SetterArray = new ResizeArray<string * Expr> [||]
            GetterArray = new ResizeArray<string> [||]
            SetterCollector = fun _  -> ()
            GetterCollector = fun _ -> ()
            Flags = flags
        }
        { ctx with SetterCollector = ctx.SetterArray.Add; GetterCollector = ctx.GetterArray.Add }
    
    /// Condition. Check if flags contains enum
    let hasFlag flag (ctx: PluginContext)= ctx.HasFlag flag
    
    /// Get the PluginHelper from the context    
    let helper = _.Helper
    
    /// Get the Entity from an EntityRef
    let getEntity = _.Helper.GetEntity
    
    /// Get the MemberFunctionOrValue from a MemberRef
    let getMember = _.Helper.GetMember
    
    /// Emit a msg on Fable compilation as a warning
    let logWarning = _.Helper.LogWarning
    
    /// Emit a msg on Fable compilation as an error.
    /// This will complete the transformation before emitting all collected errors.
    let logError = _.Helper.LogError
    
    /// Access the setter array without clearing it
    let peekSetters = _.SetterArray.ToArray() >> Array.toList
    
    /// Access the getter array without clearing it
    let peekGetters = _.GetterArray.ToArray() >> Array.toList
    
    /// Logs a warning if a setter key has already been provided a value.
    let checkDuplicateSetter (ctx: PluginContext) (setter: string * Expr) =
        ctx
        |> _.SetterArray
        |> _.Exists(fst >> (=) (fst setter))
        |> function
            | true -> $"Multiple defaults for the same property in a `SolidTypeComponent` are not allowed: '{fst setter}' was set more than once" |> logError ctx
            | false -> ()
            
    /// <summary>
    /// Adds an attribute (or more precise to say the element <c>props</c>) property set name and the value
    /// it is being set to. This is lifted at the end of the transformations to produce a <c>solid-js</c> mergeProps.
    /// </summary>
    let addSetter ctx setter=
        checkDuplicateSetter ctx setter
        ctx.SetterCollector setter
    
    /// <summary>
    /// Adds an attribute (or more precise to say the element <c>props</c>) property access selector to the context.
    /// This is lifted at the end of the transformations to produce a <c>solid-js</c> splitProps.
    /// </summary>
    let addGetter = _.GetterCollector
    
    
    /// <summary>
    /// Lift the getter array values. This clears the context values for the array. Use <c>peekGetters</c> to observe
    /// the elements without clearing them.
    /// </summary>
    let getGetters (ctx: PluginContext) =
        let getters = peekGetters ctx
        ctx.GetterArray.Clear()
        getters
    
    /// <summary>
    /// Lift the setter array values. This clears the context values for the array. Use <c>peekSetters</c> to observe
    /// the elements without clearing them.
    /// </summary>
    let getSetters (ctx: PluginContext) =
        let setters = peekSetters ctx
        ctx.SetterArray.Clear()
        setters
    
    /// <summary>
    /// When disposing of an expression (ie not including it in the final transformation tree), you can apply this
    /// func with a <c>PluginContext</c> and a context string (such as in what process this is occuring, eg: 'property
    /// collection') to log a warning of the disposed expression when the fable cli is passed the <c>--verbose</c> flag. <br/>
    /// This is useful for trying to debug why an expression might not be rendering on transpilation.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="disposalContext"></param>
    /// <param name="expr"></param>
    let debugDisposal (ctx: PluginContext) (disposalContext: string) (expr: Expr)=
        if ctx|> hasFlag ComponentFlag.PrintDisposals then
            match expr with
            | Value(UnitConstant, _)
            | IdentExpr({ Name = "returnVal" }) -> ()
            | expr ->
                let helper = ctx |> helper
                let msg = $"An expression was disposed of during {disposalContext}:\n{expr}"
                match expr.Range with
                | Some range ->
                    helper.LogWarning(msg, range)
                | _ -> helper.LogWarning(msg)

/// DU which holds a string of the name for the Tag, or an expression containing the name of the tag, and where it
/// should be imported from on compilation.
[<RequireQualifiedAccess>]
type internal TagSource =
    | AutoImport of tagName: string
    | LibraryImport of importExpr: Expr

/// <summary>
/// Alias for a <c>string * Expr</c> tuple which are compiled into an attribute key,value pair.
/// </summary>
type internal PropInfo = string * Expr

/// Alias for a list of PropInfos (which in turn is an alias for a string Expr tuple)
type internal PropList = PropInfo list

/// The intermediary object that clearly categorizes expressions for rendering, irregardless of whether the
/// types like TagInfo are refactored
type internal ElementBuilder =
    {
        TagSource: TagSource
        Properties: PropList
        Children: Expr list
        Range: SourceLocation option
    }
    static member create tagSource properties range = { TagSource = tagSource; Properties = properties; Children = []; Range = range }
