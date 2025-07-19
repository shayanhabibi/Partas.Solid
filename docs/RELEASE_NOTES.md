# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.6] - 2025-07-19

<h3><!-- 0 -->Features</h3>
- CI and Expecto Tests by @shayanhabibi in [#35](https://github.com/shayanhabibi/Partas.Solid/pull/35)

<h3><!-- 5 -->Fixed</h3>
- Build.fsx packaging declares source explicitly by @shayanhabibi


## [1.1.5] - 2025-07-14

<h3><!-- 3 -->Changed</h3>
- Update README.md by @shayanhabibi
- Update README.md by @shayanhabibi
- Update issue templates by @shayanhabibi

<h3><!-- 5 -->Fixed</h3>
- Wild card for matches in ExperimentalBuilders test by @shayanhabibi in [#34](https://github.com/shayanhabibi/Partas.Solid/pull/34)


## [1.1.5] - 2025-07-10

<h3><!-- 1 -->Added</h3>
- ADD: Start Bindings api expanded by @shayanhabibi
- ADD: Meta namespace with bindings to Solid-Meta by @shayanhabibi

<h3><!-- 5 -->Fixed</h3>
- Fix #29: TypeCastDrill on Set expressions for Attributes. Fix FileRoutes to use emit macro to prevent fail on runtime. by @shayanhabibi in [#30](https://github.com/shayanhabibi/Partas.Solid/pull/30)
- Fix icon link by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Spread method extension handles field getters by @shayanhabibi
- Nuget collision with historical local version 1.0.1 by @shayanhabibi
- Style property no longer routes through JSX.jsx by @shayanhabibi


## [1.0.0] - 2025-06-05

<h3><!-- 1 -->Added</h3>
- Add support for `use:` directives by @shayanhabibi

<h3><!-- 2 -->Removed</h3>
- Remove solidbindings event handlers for now by @shayanhabibi

<h3><!-- 3 -->Changed</h3>
- Refactor SVG attributes and tags to again mimic source material, this time with automated elevation of colliding attributes into individual interfaces by @shayanhabibi
- Update and upgrade Annotations attribute to match jsx plugin. by @shayanhabibi
- Create FUNDING.yml by @shayanhabibi
- Refactor and deprecations by @shayanhabibi
- Refactor implementations of elements to be based on solid-js `jsx.d.ts`. Inject HTML for attributes by default. by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- 1.0.0 Release by @shayanhabibi
- LICENSE.txt by @shayanhabibi
- Make SVGs native tags by @shayanhabibi
- Make SVGs native tags by @shayanhabibi
- Bump alpha and cleanup svg viewbox and aspectratio by @shayanhabibi
- Reorder, Refactor, Take changes from Oxpecker.Solid by Vladimir Schur with edits by @shayanhabibi
- Reorder and refactor by @shayanhabibi
- Reorder files for net6 compatibility by @shayanhabibi
- Reorder files for net6 compatibility by @shayanhabibi
- Deprecate PartasProxyImportAttribute (because I never actually thought ImportAttribute was as featured as it was) by @shayanhabibi
- Bump version by @shayanhabibi
- Introduce special handling for Pojo attribute by @shayanhabibi


## [1.0.0-alpha4] - 2025-05-24

<h3><!-- 1 -->Added</h3>
- Add tests for PartasProxyImport attribute by @shayanhabibi
- Add missing borderWidth shorthand style by @shayanhabibi
- Add a new attribute which mimics Import, except provides an additional parameter which acts as the key for a proxy in the case of importing from a library that provides composed components via a proxy like Motion-one by @shayanhabibi
- Add some better overloads for some of the styles by @shayanhabibi

<h3><!-- 5 -->Fixed</h3>
- Fix namespace/module issues with Fable. by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Bump alpha by @shayanhabibi
- Replace the overloaded Invocation for Navigator with InvokeOptions to prevent collision when providing one parameter. by @shayanhabibi
- Provide getters for the event handlers so they can be conditionally composed in components by @shayanhabibi
- Provide support for PartasProxyImport by @shayanhabibi
- Trying to jump around dotnet 6 compatibility by @shayanhabibi


## [1.0.0-alpha1] - 2025-05-22

<h3><!-- 5 -->Fixed</h3>
- Fix ChildLambdaProvider interfaces; provide support in the plugin for up to 4 curried arguments for the interfaces; provide a test by @shayanhabibi
- Fix #27 by @shayanhabibi
- Fix spec test namespaces by @shayanhabibi
- Fix spec test imports by @shayanhabibi
- Fix spec test namespace by @shayanhabibi
- Test spec start by @shayanhabibi
- Fix #27 by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- 1.0.0 alpha by @shayanhabibi
- Merge pull request #25 from shayanhabibi/develop by @shayanhabibi in [#25](https://github.com/shayanhabibi/Partas.Solid/pull/25)
- Better docs for children CE by @shayanhabibi
- Provide typed cssStyles. Update experimental union implicit converters to be inlined by @shayanhabibi
- AutoOpen Polymorphism namespace to ease use of Kobalte by @shayanhabibi
- Transform 'ThisArg' in call expressions. fixes #28 by @shayanhabibi
- Include `className` alias similar to `class'` for easier React/feliz adoption by @shayanhabibi
- Overload for Navigator which exposes the options as optional parameters using ParamObject by @shayanhabibi
- Experimental module includes submodule `U` which reimplements fable erased unions but with implicit erasedcasting by @shayanhabibi
- Stop being lazy and re-include analyser on spec test by @shayanhabibi
- More spec test by @shayanhabibi
- Make the prop setter more specific to prevent lifting setters #24 by @shayanhabibi
- Prebuilt generic builders up to four parameters by @shayanhabibi


## [0.2.35] - 2025-05-17

<h3><!-- 3 -->Changed</h3>
- CreatResource overloads to match solid-js spec. Cannot be tupled by @shayanhabibi
- CreatResource overloads to match solid-js spec by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- 0.2.35 by @shayanhabibi
- Transform 'ThisArg' in call expressions. fixes #28 by @shayanhabibi
- .gitignore by @shayanhabibi


## [0.2.32] - 2025-04-22

<h3><!-- 5 -->Fixed</h3>
- Fix #18 by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Hotfix for trimming by @shayanhabibi
- 0.2.30 by @shayanhabibi


## [0.2.30] - 2025-04-21

<h3><!-- 5 -->Fixed</h3>
- Fix regression by @shayanhabibi
- Fix for #16, mutable val pattern for properties can be overloaded by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Experimental syntax helpers as builders available in Partas.Solid.Experimental. by @shayanhabibi


## [0.2.28] - 2025-04-16

<h3><!-- 1 -->Added</h3>
- Add overload to createResource that accepts Async by @shayanhabibi
- Add Secondary Primitives by @shayanhabibi
- Add documentation to op_bangAt
- Add some basic doc for `Context<'T>'` type
- Add unix/macosx test suite Fli compatibility

<h3><!-- 2 -->Removed</h3>
- Remove stale todos

<h3><!-- 3 -->Changed</h3>
- Update readme by @shayanhabibi
- Complete createSignal binding by @shayanhabibi
- Cleanup first pass transformation by @shayanhabibi
- Refactor TagInfo out of transformation. Replace with initialising ElementBuilder
- Update readme by @shayanhabibi

<h3><!-- 5 -->Fixed</h3>
- Fix build error for test by @shayanhabibi
- Fix #15 by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- - replace option types with optional args and ParamObject attribute by @shayanhabibi
- Documentation for Polymorph interface
- Bring Aria attributes to parity by adding accessors
- - PascalCase Intent enum
- Erase Op_bangAt from transpilation
- Vestigial type

<h2>New Contributors</h2>
* @ made their first contribution

## [0.2.26] - 2025-03-22

<h3><!-- 1 -->Added</h3>
- Add tool manifest with fable version by @shayanhabibi
- Add recipe for adding builders to components by @shayanhabibi
- Add an attribute which replaces the hard coded import injection for solid-js imports by @shayanhabibi
- Add extensible polymorphic attribute definition by @shayanhabibi

<h3><!-- 3 -->Changed</h3>
- Update README.md by @shayanhabibi

<h3><!-- 5 -->Fixed</h3>
- Fixes #14 by @shayanhabibi
- Fix - Portal imports from /web by @shayanhabibi
- Test bug: Spurious failure on parallel runs of tests which is unrelated to transformation. by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Invoke method extension for Signal Setters by @shayanhabibi
- SVG elements have extensions for ad-hoc attribute settings etc by @shayanhabibi
- Alpha implementation of SVG elements by @shayanhabibi
- Bugfix: #14 by @shayanhabibi
- Feature: by @shayanhabibi
- Feature: by @shayanhabibi
- Feature: by @shayanhabibi
- Bump vers by @shayanhabibi
- Feature: `[<PartasImport({selector},{path})>]` replaces `[<Import({selector},{path}>]` for types with builder computations by @shayanhabibi
- Feature: `[<PartasImport({selector},{path})>]` replaces `[<Import({selector},{path}>]` for types with builder computations by @shayanhabibi
- Transform inside string interpolation by @shayanhabibi


## [0.2.15] - 2025-03-14

<h3><!-- 1 -->Added</h3>
- Support for polymorphism by @shayanhabibi

<h3><!-- 2 -->Removed</h3>
- Remove TagValue type argument by @shayanhabibi

<h3><!-- 3 -->Changed</h3>
- Update readme with warning about release mode vs debug mode by @shayanhabibi
- Update README.md by @shayanhabibi
- Update readme by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Render index accesses within builders especially by @shayanhabibi
- 0.2.11 by @shayanhabibi


## [0.2.11] - 2025-03-11

<h3><!-- 1 -->Added</h3>
- Add support for Polymorphism in Kobalte by @shayanhabibi

<h3><!-- 3 -->Changed</h3>
- Update Partas.Solid.sln by @shayanhabibi
- Update dotnet.yml by @shayanhabibi
- Update dotnet.yml by @shayanhabibi
- CI by @shayanhabibi

<h3><!-- 5 -->Fixed</h3>
- Fix for #5 by @shayanhabibi
- Fixes #2 by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Implement TagsAsValues using `!@` to reference a tag as a value (function) in jsx, and then use infix `%` to construct tag in jsx using ident by @shayanhabibi
- Foundation for TagValue test by @shayanhabibi
- Merge remote-tracking branch 'origin/master' by @shayanhabibi


## [0.2.9] - 2025-03-09

<h3><!-- 9 -->Other</h3>
- Prevent FieldGetters in computations being unwrapped by @shayanhabibi


## [0.2.8] - 2025-03-09

<h3><!-- 3 -->Changed</h3>
- Update README.md by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Prevent tupled accessors in builder being unwrapped by @shayanhabibi
- Hotfix 0.2.7 by @shayanhabibi


## [0.2.6] - 2025-03-08

<h3><!-- 9 -->Other</h3>
- Aggressively unroll attribute values. by @shayanhabibi
- Documentation by @shayanhabibi


## [0.2.5] - 2025-03-07

<h3><!-- 9 -->Other</h3>
- Implement context providers by @shayanhabibi
- Render `Fragment` correctly by @shayanhabibi


## [0.2.4] - 2025-03-07

<h3><!-- 9 -->Other</h3>
- Emits have their parameters transformed by @shayanhabibi
- Observe member val getters and setters the same as method calls by @shayanhabibi


## [0.2.2] - 2025-03-07

<h3><!-- 5 -->Fixed</h3>
- Fix for `member val` sets being dropped in prop collection by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- 0.2.2 by @shayanhabibi


## [0.2.1] - 2025-03-07

<h3><!-- 1 -->Added</h3>
- Add tests by @shayanhabibi

<h3><!-- 2 -->Removed</h3>
- Remove artifacts from setting defaults for properties within builders by @shayanhabibi

<h3><!-- 3 -->Changed</h3>
- Compartmentalise code so actual transformation logic is clear by @shayanhabibi
- Update readme by @shayanhabibi
- Clean up the documentation by @shayanhabibi
- Clean up the documentation by @shayanhabibi
- Update README.md by @shayanhabibi

<h3><!-- 5 -->Fixed</h3>
- Fix expression transformations by @shayanhabibi

<h3><!-- 9 -->Other</h3>
- Prevent prop getters in builders being transformed out by @shayanhabibi
- Transform inside arrays by @shayanhabibi
- Transform inside anon records by @shayanhabibi
- Log warning if setting multiple of the same property by @shayanhabibi
- Prevent a builder from containing only a setter to create open tags instead of a self closing tag by @shayanhabibi
- Prevent setters at the head of a builder not being transformed. by @shayanhabibi
- Implementation of patterns to green informal tests of SolidBindings from Oxpecker.Solid.Tests by @shayanhabibi
- Workable iteration of new plugin transpiler. by @shayanhabibi
- More forky and less derivative by @shayanhabibi
- More forky and less derivative by @shayanhabibi
- Init 0.1.0 by @shayanhabibi

<h2>New Contributors</h2>
* @shayanhabibi made their first contribution

[1.1.6]: https://github.com/shayanhabibi/Partas.Solid/compare/v1.1.5..1.1.6
[1.1.5]: https://github.com/shayanhabibi/Partas.Solid/compare/1.1.5..v1.1.5
[1.1.5]: https://github.com/shayanhabibi/Partas.Solid/compare/1.0.0..1.1.5
[1.0.0]: https://github.com/shayanhabibi/Partas.Solid/compare/1.0.0-alpha4..1.0.0
[1.0.0-alpha4]: https://github.com/shayanhabibi/Partas.Solid/compare/1.0.0-alpha1..1.0.0-alpha4
[1.0.0-alpha1]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.35..1.0.0-alpha1
[0.2.35]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.32..0.2.35
[0.2.32]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.30..0.2.32
[0.2.30]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.28..0.2.30
[0.2.28]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.26..0.2.28
[0.2.26]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.15..0.2.26
[0.2.15]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.11..0.2.15
[0.2.11]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.9..0.2.11
[0.2.9]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.8..0.2.9
[0.2.8]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.6..0.2.8
[0.2.6]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.5..0.2.6
[0.2.5]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.4..0.2.5
[0.2.4]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.2..0.2.4
[0.2.2]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.1..0.2.2
[0.2.1]: https://github.com/shayanhabibi/Partas.Solid/compare/0.2.0..0.2.1

<!-- generated by git-cliff -->
