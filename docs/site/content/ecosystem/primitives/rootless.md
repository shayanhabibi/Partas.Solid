---
title: Rootless
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/rootless`.

```fsharp
let createSubRoot<'T>(fn: DisposeCallback -> 'T, [<ParamArray>] owners: obj): 'T
let createCallback<'T>(callback: 'T, ?owner: obj): 'T
let createDisposable(fn: DisposeCallback -> unit, [<ParamArray>] owners: obj): DisposeCallback
let createSingletonRoot<'T>(factory: DisposeCallback -> 'T, ?detachedOwner: obj): Accessor<'T>
let createHydratableSingletonRoot<'T>(factory: DisposeCallback -> 'T): Accessor<'T>
let createRootPool<'Arg, 'Result>(factory: RootPoolFactory<'Arg, 'Result>, ?limit: int): 'Arg -> 'Result
```

| Member | Desc |
| --- | --- |
| `createSubRoot` | A reactive sub-root, disposed when any of `owners` is cleaned up. `owners` defaults to `getOwner()`. |
| `createCallback` | Wraps a callback so it runs under the given owner. |
| `createDisposable` | Like `createSubRoot`, but returns the dispose function. |
| `createSingletonRoot` | One shared root, set up on first use and shared by every caller. |
| `createHydratableSingletonRoot` | `createSingletonRoot` that is safe to hydrate. |
| `createRootPool` | A pool of roots to reuse, for components that mount and unmount often. |

## createRootPool

Call the function `createRootPool` returns to get a root. A root that is cleaned up goes back to the pool instead of
being disposed, and the next call reuses it with the new argument. `limit` is the pool size, and defaults to `100`.

```fsharp
type RootPoolFactory<'Arg, 'Result> =
    delegate of arg: Accessor<'Arg> * active: Accessor<bool> * dispose: DisposeCallback -> 'Result
```

The pool calls the factory whenever it creates a new root. `arg` reads the argument of the current use, `active` is
`false` while the root waits in the pool, and `dispose` removes the root from the pool for good.

:::note
Core Partas.Solid 3.0 binds `createRoot`, `getOwner` and `runWithOwner`. `getOwner` returns `Owner option`, and
`runWithOwner` accepts either an `Owner` or an `Owner option`.
:::
