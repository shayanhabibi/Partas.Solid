# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- ADD: RELEASE_NOTES.md
- Nicer CI output
- Add tool restore to CI
- Bash on unix
- Do not compile from Compiled folder
- Ignore scratchtests
- New Expecto test structure
- Old test structure
- ADD: build script
- Merge branch 'master' into develop

### Removed
- Remove scratchtest

## [1.1.5] - 2025-07-14

### Changed
- Wild card for matches in ExperimentalBuilders test by @shayanhabibi in [#34](https://github.com/shayanhabibi/Partas.Solid/pull/34)
- Update README.md by @shayanhabibi
- Update README.md by @shayanhabibi
- Update issue templates by @shayanhabibi

## [1.1.5] - 2025-07-10

### Changed
- Fix #29: TypeCastDrill on Set expressions for Attributes. Fix FileRoutes to use emit macro to prevent fail on runtime. by @shayanhabibi in [#30](https://github.com/shayanhabibi/Partas.Solid/pull/30)
- ==1.1.5==
- SolidStart FileRoutes binding to use emit macro to prevent compiling an invalid function call on runtime
- ADD: Start Bindings api expanded by @shayanhabibi
- ADD: Meta namespace with bindings to Solid-Meta by @shayanhabibi
- Spread method extension handles field getters by @shayanhabibi
- Nuget collision with historical local version 1.0.1 by @shayanhabibi
- Style property no longer routes through JSX.jsx by @shayanhabibi
- Fix icon link by @shayanhabibi

## [1.0.0] - 2025-06-05

### Added
- Add support for `use:` directives by @shayanhabibi

### Changed
- 1.0.0 Release by @shayanhabibi
- LICENSE.txt by @shayanhabibi
- Make SVGs native tags by @shayanhabibi
- Make SVGs native tags by @shayanhabibi
- Bump alpha and cleanup svg viewbox and aspectratio by @shayanhabibi
- Refactor SVG attributes and tags to again mimic source material, this time with automated elevation of colliding attributes into individual interfaces by @shayanhabibi
- Reorder, Refactor, Take changes from Oxpecker.Solid by Vladimir Schur with edits by @shayanhabibi
- Reorder and refactor by @shayanhabibi
- Reorder files for net6 compatibility by @shayanhabibi
- Reorder files for net6 compatibility by @shayanhabibi
- Update and upgrade Annotations attribute to match jsx plugin. by @shayanhabibi
- Create FUNDING.yml by @shayanhabibi
- Refactor and deprecations by @shayanhabibi
- Refactor implementations of elements to be based on solid-js `jsx.d.ts`. Inject HTML for attributes by default. by @shayanhabibi
- Deprecate PartasProxyImportAttribute (because I never actually thought ImportAttribute was as featured as it was) by @shayanhabibi
- Bump version by @shayanhabibi
- Introduce special handling for Pojo attribute by @shayanhabibi

### Removed
- Remove solidbindings event handlers for now by @shayanhabibi

## [1.0.0-alpha4] - 2025-05-24

### Added
- Add tests for PartasProxyImport attribute by @shayanhabibi
- Add missing borderWidth shorthand style by @shayanhabibi
- Add a new attribute which mimics Import, except provides an additional parameter which acts as the key for a proxy in the case of importing from a library that provides composed components via a proxy like Motion-one by @shayanhabibi
- Add some better overloads for some of the styles by @shayanhabibi

### Changed
- Bump alpha by @shayanhabibi
- Replace the overloaded Invocation for Navigator with InvokeOptions to prevent collision when providing one parameter. by @shayanhabibi
- Provide getters for the event handlers so they can be conditionally composed in components by @shayanhabibi
- Provide support for PartasProxyImport by @shayanhabibi
- Trying to jump around dotnet 6 compatibility by @shayanhabibi
- Fix namespace/module issues with Fable. by @shayanhabibi

## [1.0.0-alpha1] - 2025-05-22

### Changed
- 1.0.0 alpha by @shayanhabibi
- Merge pull request #25 from shayanhabibi/develop by @shayanhabibi in [#25](https://github.com/shayanhabibi/Partas.Solid/pull/25)
- Fix ChildLambdaProvider interfaces; provide support in the plugin for up to 4 curried arguments for the interfaces; provide a test by @shayanhabibi
- Better docs for children CE by @shayanhabibi
- Provide typed cssStyles. Update experimental union implicit converters to be inlined by @shayanhabibi
- AutoOpen Polymorphism namespace to ease use of Kobalte by @shayanhabibi
- Transform 'ThisArg' in call expressions. fixes #28 by @shayanhabibi
- Include `className` alias similar to `class'` for easier React/feliz adoption by @shayanhabibi
- Overload for Navigator which exposes the options as optional parameters using ParamObject by @shayanhabibi
- Experimental module includes submodule `U` which reimplements fable erased unions but with implicit erasedcasting by @shayanhabibi
- Fix #27 by @shayanhabibi
- Stop being lazy and re-include analyser on spec test by @shayanhabibi
- Fix spec test namespaces by @shayanhabibi
- Fix spec test imports by @shayanhabibi
- Fix spec test namespace by @shayanhabibi
- More spec test by @shayanhabibi
- Make the prop setter more specific to prevent lifting setters #24 by @shayanhabibi
- Test spec start by @shayanhabibi
- Prebuilt generic builders up to four parameters by @shayanhabibi
- Fix #27 by @shayanhabibi

## [0.2.35] - 2025-05-17

### Changed
- 0.2.35 by @shayanhabibi
- Transform 'ThisArg' in call expressions. fixes #28 by @shayanhabibi
- .gitignore by @shayanhabibi
- CreatResource overloads to match solid-js spec. Cannot be tupled by @shayanhabibi
- CreatResource overloads to match solid-js spec by @shayanhabibi

## [0.2.32] - 2025-04-22

### Changed
- Fix #18 by @shayanhabibi
- Hotfix for trimming by @shayanhabibi
- 0.2.30 by @shayanhabibi

## [0.2.30] - 2025-04-21

### Changed
- Fix regression by @shayanhabibi
- Fix for #16, mutable val pattern for properties can be overloaded by @shayanhabibi
- Experimental syntax helpers as builders available in Partas.Solid.Experimental. by @shayanhabibi

## [0.2.28] - 2025-04-16

### Added
- Add overload to createResource that accepts Async by @shayanhabibi
- Add Secondary Primitives by @shayanhabibi
- Add documentation to op_bangAt
- Add some basic doc for `Context<'T>'` type
- Add unix/macosx test suite Fli compatibility

### Changed
- Update readme by @shayanhabibi
- Fix build error for test by @shayanhabibi
- - replace option types with optional args and ParamObject attribute by @shayanhabibi
- Complete createSignal binding by @shayanhabibi
- Cleanup first pass transformation by @shayanhabibi
- Refactor TagInfo out of transformation. Replace with initialising ElementBuilder
- Documentation for Polymorph interface
- Bring Aria attributes to parity by adding accessors
- - PascalCase Intent enum
- Erase Op_bangAt from transpilation
- Vestigial type
- Fix #15 by @shayanhabibi
- Update readme by @shayanhabibi

### Removed
- Remove stale todos

## New Contributors
* @ made their first contribution
## [0.2.26] - 2025-03-22

### Added
- Add tool manifest with fable version by @shayanhabibi
- Add recipe for adding builders to components by @shayanhabibi
- Add an attribute which replaces the hard coded import injection for solid-js imports by @shayanhabibi
- Add extensible polymorphic attribute definition by @shayanhabibi

### Changed
- Fixes #14 by @shayanhabibi
- Invoke method extension for Signal Setters by @shayanhabibi
- SVG elements have extensions for ad-hoc attribute settings etc by @shayanhabibi
- Alpha implementation of SVG elements by @shayanhabibi
- Update README.md by @shayanhabibi
- Bugfix: #14 by @shayanhabibi
- Feature: by @shayanhabibi
- Feature: by @shayanhabibi
- Feature: by @shayanhabibi
- Bump vers by @shayanhabibi
- Fix - Portal imports from /web by @shayanhabibi
- Feature: `[<PartasImport({selector},{path})>]` replaces `[<Import({selector},{path}>]` for types with builder computations by @shayanhabibi
- Feature: `[<PartasImport({selector},{path})>]` replaces `[<Import({selector},{path}>]` for types with builder computations by @shayanhabibi
- Test bug: Spurious failure on parallel runs of tests which is unrelated to transformation. by @shayanhabibi
- Transform inside string interpolation by @shayanhabibi

## [0.2.15] - 2025-03-14

### Added
- Support for polymorphism by @shayanhabibi

### Changed
- Render index accesses within builders especially by @shayanhabibi
- Update readme with warning about release mode vs debug mode by @shayanhabibi
- Update README.md by @shayanhabibi
- Update readme by @shayanhabibi
- 0.2.11 by @shayanhabibi

### Removed
- Remove TagValue type argument by @shayanhabibi

## [0.2.11] - 2025-03-11

### Added
- Add support for Polymorphism in Kobalte by @shayanhabibi

### Changed
- Implement TagsAsValues using `!@` to reference a tag as a value (function) in jsx, and then use infix `%` to construct tag in jsx using ident by @shayanhabibi
- Foundation for TagValue test by @shayanhabibi
- Fix for #5 by @shayanhabibi
- Merge remote-tracking branch 'origin/master' by @shayanhabibi
- Update Partas.Solid.sln by @shayanhabibi
- Update dotnet.yml by @shayanhabibi
- Update dotnet.yml by @shayanhabibi
- CI by @shayanhabibi
- Fixes #2 by @shayanhabibi

## [0.2.9] - 2025-03-09

### Changed
- Prevent FieldGetters in computations being unwrapped by @shayanhabibi

## [0.2.8] - 2025-03-09

### Changed
- Prevent tupled accessors in builder being unwrapped by @shayanhabibi
- Hotfix 0.2.7 by @shayanhabibi
- Update README.md by @shayanhabibi

## [0.2.6] - 2025-03-08

### Changed
- Aggressively unroll attribute values. by @shayanhabibi
- Documentation by @shayanhabibi

## [0.2.5] - 2025-03-07

### Changed
- Implement context providers by @shayanhabibi
- Render `Fragment` correctly by @shayanhabibi

## [0.2.4] - 2025-03-07

### Changed
- Emits have their parameters transformed by @shayanhabibi
- Observe member val getters and setters the same as method calls by @shayanhabibi

## [0.2.2] - 2025-03-07

### Changed
- 0.2.2 by @shayanhabibi
- Fix for `member val` sets being dropped in prop collection by @shayanhabibi

## [0.2.1] - 2025-03-07

### Added
- Add tests by @shayanhabibi

### Changed
- Fix expression transformations by @shayanhabibi
- Prevent prop getters in builders being transformed out by @shayanhabibi
- Transform inside arrays by @shayanhabibi
- Transform inside anon records by @shayanhabibi
- Log warning if setting multiple of the same property by @shayanhabibi
- Prevent a builder from containing only a setter to create open tags instead of a self closing tag by @shayanhabibi
- Prevent setters at the head of a builder not being transformed. by @shayanhabibi
- Compartmentalise code so actual transformation logic is clear by @shayanhabibi
- Update readme by @shayanhabibi
- Clean up the documentation by @shayanhabibi
- Clean up the documentation by @shayanhabibi
- Implementation of patterns to green informal tests of SolidBindings from Oxpecker.Solid.Tests by @shayanhabibi
- Workable iteration of new plugin transpiler. by @shayanhabibi
- More forky and less derivative by @shayanhabibi
- More forky and less derivative by @shayanhabibi
- Update README.md by @shayanhabibi
- Init 0.1.0 by @shayanhabibi

### Removed
- Remove artifacts from setting defaults for properties within builders by @shayanhabibi

## New Contributors
* @shayanhabibi made their first contribution
[unreleased]: https://github.com/shayanhabibi/Partas.Solid/compare/v1.1.5..HEAD
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
