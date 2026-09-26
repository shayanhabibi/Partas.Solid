---
title: Rootless
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/rootless`.

## RootPoolFactory

Type: `delegate`

```fsharp
type RootPoolFactory<'Arg, 'Result> =
    delegate of arg: Accessor<'Arg> * active: Accessor<bool> * dispose: DisposeCallback -> 'Result
```

The callback for `createRootPool`. It is called when a new root is created.

| Param | Desc |
| --- | --- |
| `arg` | An accessor of the argument passed to the pool function. |
| `active` | An accessor of whether the root is in use. When `false`, the root is waiting in the pool to be reused. |
| `dispose` | Disposes the root, so it is not reused. |

## Bindings

| Member | Desc |
| --- | --- |
| `createSubRoot` | Creates a reactive sub-root that is disposed automatically with its owner. Cleaning up any of the `owners` disposes it. `owners` defaults to `getOwner()`. |
| `createCallback` | Wraps a callback so it runs under the given owner. |
| `createDisposable` | Like `createSubRoot`, but returns the dispose function. |
| `createSingletonRoot` | Creates one shared root, set up on first use and shared by every caller. |
| `createHydratableSingletonRoot` | `createSingletonRoot` that is safe to hydrate. |
| `createRootPool` | Creates a pool of roots to reuse. Useful for components that mount and unmount often. |

`createRootPool` calls `factory` whenever it creates a new root. You create roots by calling the returned function.
When a root is cleaned up it is not disposed, but put back in the pool. The next call reuses it and updates it with
the new `arg`. `limit` is the size of the pool, and defaults to `100`.

:::note
Core Partas.Solid 3.0 still binds `createRoot`, `getOwner` and `runWithOwner`. `getOwner` returns
`Owner option`, and `runWithOwner` accepts either an `Owner` or an `Owner option`.
:::

:::details Bindings
```fsharp
[<AutoOpen; Erase>]
type Rootless =
    [<ImportMember(Spec.path)>]
    static member createSubRoot<'T>(fn: DisposeCallback -> 'T, [<ParamArray>] owners: obj): 'T = jsNative
    [<ImportMember(Spec.path)>]
    static member createCallback<'T>(callback: 'T, ?owner: obj): 'T = jsNative
    [<ImportMember(Spec.path)>]
    static member createDisposable(fn: DisposeCallback -> unit, [<ParamArray>] owners: obj): DisposeCallback = jsNative
    [<ImportMember(Spec.path)>]
    static member createSingletonRoot<'T>(
        factory: DisposeCallback -> 'T,
        ?detachedOwner: obj
        ): Accessor<'T> = jsNative
    [<ImportMember(Spec.path)>]
    static member createHydratableSingletonRoot<'T>(factory: DisposeCallback -> 'T): Accessor<'T> = jsNative
    [<ImportMember(Spec.path); ParamObject(1)>]
    static member createRootPool<'Arg, 'Result>(
            factory: Rootless.RootPoolFactory<'Arg, 'Result>,
            limit: int
        ): 'Arg -> 'Result = jsNative
    [<ImportMember(Spec.path)>]
    static member createRootPool<'Arg, 'Result>(
            factory: Rootless.RootPoolFactory<'Arg, 'Result>
        ): 'Arg -> 'Result = jsNative
```
:::
