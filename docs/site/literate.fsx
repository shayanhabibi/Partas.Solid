// What the literate pages (content/**/*.fsx) type-check against: this repository's Partas.Solid, built in
// Release by docs.fsproj. A page loads it in a hidden block and opens what the live examples' prelude opens:
//
//     (*** hide ***)
//     #load "../literate.fsx"
//     open Partas.Solid
//     open Fable.Core
//     open Fable.Core.JsInterop
#i "nuget: https://nuget.cloudsmith.io/shayanhabibi/xantham/v3/index.json"
#i "nuget: https://nuget.cloudsmith.io/shayanhabibi/partas-animejs/v3/index.json"
#r "nuget: Fable.Core, 5.3.0"
#r "nuget: Fable.AST, 5.0.0"
#r "nuget: Xantham.Fable.Core.TS, 0.2.1"
#r "nuget: Partas.AnimeJs, 0.1.1"
#r "../../src/Partas.Solid/bin/Release/net8.0/Partas.Solid.FablePlugin.dll"
#r "../../src/Partas.Solid/bin/Release/net8.0/Partas.Solid.dll"
