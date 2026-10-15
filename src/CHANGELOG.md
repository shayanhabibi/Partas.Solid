---
last_commit_released: 6ab02ad31a93d30428a8c4d425d0431af3037325
pre_release: rc
name: Partas.Solid
include:
  - ../Partas.Solid/
  - ../Partas.Solid.FablePlugin/
  - ../tools/Partas.Nacara.Plugins.Solid/
updaters:
  - xml:
      file: Partas.Solid/Partas.Solid.fsproj
      selector: /Project/PropertyGroup/Version
  - xml:
      file: Partas.Solid.FablePlugin/Partas.Solid.FablePlugin.fsproj
      selector: /Project/PropertyGroup/Version
  - xml:
      file: ../tools/Partas.Nacara.Plugins.Solid/Partas.Nacara.Plugins.Solid.fsproj
      selector: /Project/PropertyGroup/Version
---

# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

This changelog is generated using [EasyBuild.ShipIt](https://github.com/easybuild-org/EasyBuild.ShipIt).

⚠ Only edit the front matter metadata at the top of this file. All other changes will be overwritten when a new release is created.

## 3.0.0-rc.2 - 2026-10-15

### 🚀 Features

* Add reactive match! and preserve JSX child semantics ([6ab02ad](https://github.com/shayanhabibi/Partas.Solid/commit/6ab02ad31a93d30428a8c4d425d0431af3037325))

<strong><small>[View changes on Github](https://github.com/shayanhabibi/Partas.Solid/compare/b1ed26be939192b85abec2b23254657321e0e2ae..6ab02ad31a93d30428a8c4d425d0431af3037325)</small></strong>

## 3.0.0-rc.1 - 2026-10-13

### 🏗️ Breaking changes

* Port bindings and plugin to Solid 2.0.0-rc.9 ([cc4d08f](https://github.com/shayanhabibi/Partas.Solid/commit/cc4d08ff9982e7d9233f95f3ba4ee610cd61ca5f))
* Remove the SolidStart bindings ([5a9bcd8](https://github.com/shayanhabibi/Partas.Solid/commit/5a9bcd823d1052269cf57abaab6fae1dd723ea5b))
* Generate the HTML attributes from Solid's JSX declarations ([648ba39](https://github.com/shayanhabibi/Partas.Solid/commit/648ba3951cc02be5116406946e817c1f6ddf2b14))
* Follow @solidjs/meta 1.0; exclude the router for now ([53f1a63](https://github.com/shayanhabibi/Partas.Solid/commit/53f1a638dc428f50e1026f29aa935c2c9314e419))
* Generate ARIA, SVG and events; take DOM types from Xantham 0.2.1 ([1cf4e44](https://github.com/shayanhabibi/Partas.Solid/commit/1cf4e44e1278e78a3f8d20339bb0c35d796a27e8))

### 🚀 Features

* Generate Tags.fs, adding slot, template, selectedcontent and MathML ([4bb023b](https://github.com/shayanhabibi/Partas.Solid/commit/4bb023b7f774a95b6d2bf349ce8d7dcd0605e686))
* Bind isWrappable, isEqual, NotReadyError, isHydrating and isHydratable ([938c70c](https://github.com/shayanhabibi/Partas.Solid/commit/938c70c67b9ee128a840f683d7e738138c87104e))
* Type input min/max/step as U2<float, string> ([69fde1b](https://github.com/shayanhabibi/Partas.Solid/commit/69fde1bacb9c9475b7ef53ff9189e7a52716dd0c))
* Report components that would compile to a lowercase tag ([d21cba6](https://github.com/shayanhabibi/Partas.Solid/commit/d21cba67c71c92f7f9cf1e205d9d367c92dd4629))
* Bind @solidjs/router 2 (next.38) ([58fdf97](https://github.com/shayanhabibi/Partas.Solid/commit/58fdf97fe34fef7f75012777bebad18a89683718))
* Bind the router's liveQuery; accept async iterables in createMemo ([fd5e582](https://github.com/shayanhabibi/Partas.Solid/commit/fd5e582063314bc2eb302eaf22867b5d96f9bbcf))

### 🐞 Bug Fixes

* Replace the shadowed class' object extension with classObj ([971ee9f](https://github.com/shayanhabibi/Partas.Solid/commit/971ee9fb519b3daf6be3ea9d5c9fc9db34dd9285))
* Keep SolidComponent call arguments reactive ([d7ab136](https://github.com/shayanhabibi/Partas.Solid/commit/d7ab1368726b7c77fe644746d53debccb3cf8486))
* *(plugin)* Only unroll toArray/toList over a delayed sequence ([cd6d4e2](https://github.com/shayanhabibi/Partas.Solid/commit/cd6d4e226173a73f1b699e05f25d155998f95ab0))
* *(plugin)* Close the known runtime bugs tracked by it.fails specs ([e08ad85](https://github.com/shayanhabibi/Partas.Solid/commit/e08ad85626d6aa6f173a65b1431224a4cdc0d7af))

<strong><small>[View changes on Github](https://github.com/shayanhabibi/Partas.Solid/compare/0e25620966b4b8e2b824a3496c14a7759ac89211..b1ed26be939192b85abec2b23254657321e0e2ae)</small></strong>

## 2.1.3 - 2025-10-10

Releases up to and including 2.1.3 are recorded in [docs/RELEASE_NOTES.md](../docs/RELEASE_NOTES.md).
