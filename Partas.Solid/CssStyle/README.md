# Css Styles

There are several ways you can manage css styling, with or without type hints etc.

The most intuitive (at least on jetbrains Rider) is to just use the constructor property `style` and provide a string with an object.

```fsharp
div(style = "{ align-content: baseline }")
```

This is because the author of Oxpecker.Solid utilised the capacity to inject CSS language into strings. So you should get the usual inline completions you would with css.

Alternatively, you can use any object or expression that will compile into a POJO using the `.style'` tag extension.

There is an overload for this extension which automatically takes a list of string object tuples and compiles them using `createObj`. You can utilise the predefined helpers from this folder to get autocompletions and typing to some degree.