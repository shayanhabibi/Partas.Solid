---
# Partas.Solid and Partas.Solid.FablePlugin release together: this one changelog versions both.
name: Partas.Solid
# 3.0 targets Solid 2.0 release candidates. Remove this line to release stable versions.
pre_release: rc
# The 2.1.3 release. Earlier history lives in docs/RELEASE_NOTES.md.
last_commit_released: 0e25620966b4b8e2b824a3496c14a7759ac89211
# The packages lived at the repository root before they moved into src/.
include:
  - ../Partas.Solid/
  - ../Partas.Solid.FablePlugin/
updaters:
  - xml:
      file: 'Partas.Solid/Partas.Solid.fsproj'
      selector: /Project/PropertyGroup/Version
  - xml:
      file: 'Partas.Solid.FablePlugin/Partas.Solid.FablePlugin.fsproj'
      selector: /Project/PropertyGroup/Version
---

# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

This changelog is generated using [EasyBuild.ShipIt](https://github.com/easybuild-org/EasyBuild.ShipIt).

⚠ Only edit the front matter metadata at the top of this file. All other changes will be overwritten when a new release is created.

## 2.1.3 - 2025-10-10

Releases up to and including 2.1.3 are recorded in [docs/RELEASE_NOTES.md](../docs/RELEASE_NOTES.md).
