---
title: Modular Forms
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.ModularForms` binds [Modular Forms](https://modularforms.dev/solid/api/createForm). See its docs for the API
and components.

Apart from a couple of caveats with Fable.Remoting, and some typing helpers, the library is used as documented.

## Differences

You can map a field to the form with `name`, as the docs do. The binding adds a `map` extension method for this. It takes
a lambda that selects the field from the form, and turns it into a string path with
`Fable.Core.Experimental.namesofLambda`.

Take this form type:

```fsharp
type FormData = {
    Item: string
    Id: int
    Value: float
}
```

You connect a field to the form with a `Field` component. Instead of `Field(name = "Value")`, use `map`. It also passes
the type parameters on to the field props:

```fsharp
let form = createFormStore<FormData,obj>()
let valueField = Field(of' = form).map(_.Value) {
        yield fun field (* FieldStore<FormData, float> *) fieldProps (* FieldElementProps *) ->
            // ...
    }
```

## Fable.Remoting

The library works well with Fable.Remoting, with a couple of caveats.

### Invalid submissions

If your form data is a record, initialise every field of the record by passing an `initialValue`.

Modular Forms does not send null data, to save on transmission size. Fable.Remoting will not match an object to a record
unless every field is present with the right type.

This also means every field of the record needs a `Field` component, so that each submission has a value for it. The
fields do not have to be visible or active.

## Reset or prefill a form

To reset the form or set it to a given state, use `reset(form, initialValue = (* ... *))`. Even when resetting to an empty
form, pass an empty or default value.
