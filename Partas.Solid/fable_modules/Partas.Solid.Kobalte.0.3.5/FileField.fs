namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core
open Browser.Types

[<AllowNullLiteral>]
[<Interface>]
type FileRejection =
    abstract member file: File with get
    abstract member errors: FileError[] with get
[<AllowNullLiteral>]
[<Interface>]
type Details =
    abstract member acceptedFiles: File array with get
    abstract member rejectedFiles: FileRejection array with get

[<Erase; Import("Root", Spec.fileField)>]
type FileField() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable multiple : bool 
    [<DefaultValue>] val mutable maxFiles : int 
    [<DefaultValue>] val mutable accept : U2<string, string[]> 
    [<DefaultValue>] val mutable allowDragAndDrop : bool 
    [<DefaultValue>] val mutable maxFileSize : int 
    [<DefaultValue>] val mutable minFileSize : int 
    [<DefaultValue>] val mutable onFileAccept : File[] -> unit 
    [<DefaultValue>] val mutable onFileReject : FileRejection[] -> unit 
    [<DefaultValue>] val mutable onFileChange : Details -> unit 
    [<DefaultValue>] val mutable validateFile : File -> FileError[] option 
    /// <summary>
    /// The name of the select.
    /// Submitted with its owning form as part of a name/value pair.
    /// </summary>
    [<DefaultValue>]
    val mutable name: string
    /// <summary>
    /// Whether the select should display its "valid" or "invalid" visual styling.
    /// </summary>
    [<DefaultValue>]
    val mutable validationState: ValidationState
    /// <summary>
    /// Whether the user must select an item before the owning form can be submitted.
    /// </summary>
    [<DefaultValue>]
    val mutable required: bool
    /// <summary>
    /// Whether the select is disabled.
    /// </summary>
    [<DefaultValue>]
    val mutable disabled: bool
    /// <summary>
    /// Whether the select is read only.
    /// </summary>
    [<DefaultValue>]
    val mutable readOnly: bool

[<Erase; RequireQualifiedAccess>]
module FileField =
    [<Erase; Import("Item", Spec.fileField)>]
    type Item() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable file : File 
    [<Erase; Import("ItemSize", Spec.fileField)>]
    type ItemSize() =
        interface VoidNode
        interface Polymorph
        [<DefaultValue>] val mutable precision : int 
    [<Erase; Import("ItemPreview", Spec.fileField)>]
    type ItemPreview() =
        interface VoidNode
        interface Polymorph
        [<DefaultValue>] val mutable type' : string 
    [<Erase; Import("Dropzone", Spec.fileField)>]
    type Dropzone() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Trigger", Spec.fileField)>]
    type Trigger() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Label", Spec.fileField)>]
    type Label() =
        inherit label()
        interface Polymorph
    [<Erase; Import("HiddenInput", Spec.fileField)>]
    type HiddenInput() =
        inherit input()
        interface Polymorph
    [<Erase; Import("ItemList", Spec.fileField)>]
    type ItemList() =
        interface HtmlTag
        interface Polymorph
        interface ChildLambdaProvider<File>
    [<Erase; Import("ItemPreviewImage", Spec.fileField)>]
    type ItemPreviewImage() =
        inherit img()
        interface Polymorph
    [<Erase; Import("ItemName", Spec.fileField)>]
    type ItemName() =
        inherit span()
        interface Polymorph
    [<Erase; Import("ItemDeleteTrigger", Spec.fileField)>]
    type ItemDeleteTrigger() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Description", Spec.fileField)>]
    type Description() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ErrorMessage", Spec.fileField)>]
    type ErrorMessage() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool 

[<Erase; AutoOpen>]
module FileFieldContext =
    [<AllowNullLiteral>]
    [<Interface>]
    type FileFieldContext =
        abstract member inputId: Accessor<string> with get
        abstract member fileInputRef: Accessor<HTMLInputElement option> with get
        abstract member setFileInputRef: Setter<HTMLInputElement option> with get
        abstract member dropzoneRef: Accessor<HTMLElement option> with get
        abstract member setDropzoneRef: Setter<HTMLElement option> with get
        abstract member disabled: Accessor<bool option> with get
        abstract member multiple: Accessor<bool option> with get
        abstract member accept: Accessor<string option> with get
        abstract member allowDragAndDrop: Accessor<bool option> with get
        abstract member processFiles: (ResizeArray<File> -> unit) with get
        abstract member acceptedFiles: ResizeArray<File> with get
        abstract member rejectedFiles: ResizeArray<FileRejection> with get
        abstract member removeFile: (File -> unit) with get
    
    [<ImportMember(Spec.fileField)>]
    let useFileFieldContext(): FileFieldContext = jsNative
