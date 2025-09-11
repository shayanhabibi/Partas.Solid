<div id="top"></div>

<br />


<div align="center">
  <a href="https://github.com/shayanhabibi/Partas.Solid" target="_blank">
  <img src="https://github.com/shayanhabibi/Partas.Solid/blob/master/Public/Partas_d00b_00a%20icon.png" height="42px"/>
  </a>
<h3 align="center">Partas.Solid</h3>
  <p align="center">
    <img src="https://www.solidjs.com/img/logo/without-wordmark/logo.svg" height="24px" style="border-radius:8px;" />
    <kbd>Solid-JS wrapper in Oxpecker style.</kbd>
    <img src="https://fsharp.org/img/logo/fsharp256.png" height="24px" />
  </p>
</div>

<div align="center">

![NuGet Version](https://img.shields.io/nuget/v/Partas.Solid?style=flat)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/shayanhabibi/Partas.Solid/dotnet.yml)

</div>
<div align="center">

[![Scc Count Badge](https://sloc.xyz/github/shayanhabibi/Partas.Solid/?category=code&badge-bg-color=9100FF)](https://github.com/shayanhabibi/Partas.Solid/)
[![Scc Count Badge](https://sloc.xyz/github/shayanhabibi/Partas.Solid/?category=comments&badge-bg-color=5E00B5)](https://github.com/shayanhabibi/Partas.Solid/)
[![Scc Count Badge](https://sloc.xyz/github/shayanhabibi/Partas.Solid/?category=cocomo&badge-bg-color=3B0086)](https://github.com/shayanhabibi/Partas.Solid/)


</div>

---

## Getting Started

[See the docs to get started!](https://partas-solid.vercel.app/partas-solid)

[//]: # (~~[Legacy Docs]&#40;http://shayanhabibi.github.io/partas-solid-docs/&#41;~~)

## v2.1.0 - Storybook Support

Storybook is implicitly supported by the plugin. [You can see how its used here](https://partas-solid.vercel.app/partas-solid/storybook).


# Dev

To develop the plugin, ensure you exclude the plugin on compilation:

`fable --exclude Partas.Solid.FablePlugin --noCache -o output -e .fs.jsx --run dotnet restore`

## FAKE Scripts

There are a suite of tests to run to help inform if any changes have broken something else.

You can run them from your IDE or using the FAKE script:

`dotnet fsi build.fsx`

Format with fantomas:

`dotnet fsi build.fsx target format`

> [!NOTE]
> The FablePlugin `Plugin.fs` is excluded from formatting as the heavily nested
> active patterns do not *jive* well with it.

I've done my best to heavily document the plugin and the method of transformations.
