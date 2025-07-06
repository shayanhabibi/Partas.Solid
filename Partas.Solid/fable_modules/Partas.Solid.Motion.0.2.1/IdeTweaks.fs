namespace JetBrains.Annotations

open System
open Fable.Core

[<Erase>]
type internal InjectedLanguage =
    | CSS = 0
    | HTML = 1
    | JAVASCRIPT = 2
    | JSON = 3
    | XML = 4

[<AttributeUsage(AttributeTargets.Parameter
                 ||| AttributeTargets.Field
                 ||| AttributeTargets.Property)>]
[<Erase>]
type internal LanguageInjectionAttribute private (?injectedLanguage: InjectedLanguage, ?injectedLanguageName: string) =
    inherit Attribute()
    [<Erase>]
    member x.InjectedLanguage = injectedLanguage
    [<Erase>]
    member x.InjectedLanguageName = injectedLanguageName
    [<Erase>]
    member val Prefix = "" with get, set
    [<Erase>]
    member val Suffix = "" with get, set
    
    new(injectedLanguage: InjectedLanguage) = LanguageInjectionAttribute(injectedLanguage)
    new(injectedLanguageName: string) = LanguageInjectionAttribute(injectedLanguageName = injectedLanguageName)
