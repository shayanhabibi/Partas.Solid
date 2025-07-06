namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid


[<Erase; Import("Root", Spec.separator)>]
type Separator() =
    inherit hr()
    interface Polymorph
    [<DefaultValue>] val mutable orientation : Orientation 

[<Erase; Import("Root", Spec.skeleton)>]
type Skeleton() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable visible : bool 
    [<DefaultValue>] val mutable animate : bool 
    [<DefaultValue>] val mutable width : int 
    [<DefaultValue>] val mutable height : int 
    [<DefaultValue>] val mutable radius : int 
    [<DefaultValue>] val mutable circle : bool 
    [<DefaultValue>] val mutable children : Fragment 
