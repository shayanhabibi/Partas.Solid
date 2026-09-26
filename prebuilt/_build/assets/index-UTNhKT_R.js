import{Z as d,h as t,t as s}from"./index-CZ23QLAh.js";const i=[{title:"Introduction",href:"#introduction",children:[{title:"What this means",href:"#what-this-means",children:[]}]},{title:"Contents",href:"#contents",children:[{title:"Getting Started",href:"#getting-started",children:[]},{title:"Benefits of JSX Output",href:"#benefits-of-jsx-output",children:[]},{title:"Learn More About Us",href:"#learn-more-about-us",children:[]}]}],o={title:"Partas.Solid"};function a(n){const e={a:"a",code:"code",em:"em",h1:"h1",h2:"h2",hr:"hr",li:"li",p:"p",strong:"strong",ul:"ul",...d(),...n.components},{DirectiveContainer:r}=e;return r||h("DirectiveContainer"),[t(r,{type:"tip",get children(){return t(e.p,{get children(){return["If you're not aware of ",t(e.a,{href:"https://fable.io",children:"Fable"}),", and/or are ",t(e.a,{href:"https://fsharp.org/",children:"new to F#"}),`, then I suggest you visit their website\r
to familiarise yourself with the technology.`]}})}}),`
`,t(e.hr,{}),`
`,t(e.h1,{id:"introduction",get children(){return t(e.a,{"data-auto-heading":"",href:"#introduction",children:"Introduction"})}}),`
`,t(e.p,{get children(){return[t(e.code,{children:"Partas.Solid"})," is a plugin for the F# to JS transpiler ",t(e.code,{children:"Fable"}),"."]}}),`
`,t(e.p,{children:"Partas.Solid differs from other front end plugins in F# in its overarching goal:"}),`
`,t(r,{type:"important",title:" ",get children(){return t(e.p,{get children(){return["To define a DSL that allows building HTML/JSX trees for native, imported, ",t(e.strong,{children:"and"})," user defined components"]}})}}),`
`,t(e.h2,{id:"what-this-means",get children(){return t(e.a,{"data-auto-heading":"",href:"#what-this-means",children:"What this means"})}}),`
`,t(e.ul,{get children(){return[`
`,t(e.li,{get children(){return["You can ",t(e.strong,{get children(){return t(e.em,{children:"define"})}})," a component and use it ",t(e.strong,{get children(){return t(e.em,{children:"within the same DSL"})}})]}}),`
`]}}),`
`,t(e.p,{get children(){return["This means User defined components are ",t(e.em,{get children(){return t(e.strong,{children:"indistinguishable"})}}),` from native or library imported components in their usage.\r
Forming DOM trees is consistently uniform in syntax, making it easier to read, understand, refactor, and use.`]}}),`
`,t(e.ul,{get children(){return[`
`,t(e.li,{children:`When creating a UI element, you can inherit the ability to receive children or not, have classes, have event signatures,\r
all by interfacing a single type. No need to overload. No need to define optional signatures. Just go.`}),`
`,t(e.li,{children:"Creating components with optional properties/attributes are as intuitive as the source material."}),`
`]}}),`
`,t(e.p,{children:"These are unique goals for Partas.Solid."}),`
`,t(e.hr,{}),`
`,t(e.h1,{id:"contents",get children(){return t(e.a,{"data-auto-heading":"",href:"#contents",children:"Contents"})}}),`
`,t(e.p,{get children(){return["This documentation is organised (",t(e.em,{children:"or disorganised"}),") in the following manner:"]}}),`
`,t(e.ul,{get children(){return[`
`,t(e.li,{get children(){return[`About
`,t(e.ul,{get children(){return[`
`,t(e.li,{children:"Context/History of the plugin/fork"}),`
`,t(e.li,{children:"Advantages of JSX"}),`
`]}}),`
`]}}),`
`,t(e.li,{get children(){return[`Documentation
`,t(e.ul,{get children(){return[`
`,t(e.li,{children:"Features of the Plugin and general introduction to Solid-JS"}),`
`,t(e.li,{children:"Bindings and signatures for bindings where relevant"}),`
`]}}),`
`]}}),`
`,t(e.li,{get children(){return[`Dev
`,t(e.ul,{get children(){return[`
`,t(e.li,{children:"Minor plugin details (the source is better documented)"}),`
`]}}),`
`]}}),`
`,t(e.li,{children:"Troubleshooting"}),`
`]}}),`
`,t(e.p,{children:"Further examples and recipes are to come."}),`
`,t(e.p,{get children(){return["A separate section is available for the ",t(e.a,{href:"/ecosystem",get children(){return t(e.em,{children:"Ecosystem"})}})]}}),`
`,t(e.h2,{id:"getting-started",get children(){return t(e.a,{"data-auto-heading":"",href:"#getting-started",get children(){return t(e.a,{href:"/partas-solid/installation",children:"Getting Started"})}})}}),`
`,t(e.h2,{id:"benefits-of-jsx-output",get children(){return t(e.a,{"data-auto-heading":"",href:"#benefits-of-jsx-output",get children(){return t(e.a,{href:"/partas-solid/about/jsx-output",children:"Benefits of JSX Output"})}})}}),`
`,t(e.h2,{id:"learn-more-about-us",get children(){return t(e.a,{"data-auto-heading":"",href:"#learn-more-about-us",get children(){return t(e.a,{href:"/partas-solid/about/oxpecker-fork",children:"Learn More About Us"})}})}}),`
`,t(r,{type:"details",title:"Isn't this Oxpecker.Solid?",get children(){return[t(e.p,{get children(){return["Partas.Solid is an opinionated fork of Lanayx's ",t(e.a,{href:"https://github.com/lanayx/oxpecker",children:"Oxpecker.Solid"}),`, the plugin aggressively transforms\r
the Fable AST to produce Solid-js compatible `,t(e.strong,{children:"JSX"})," with lots of syntactic sugar sprinkled in for a smooth developer experience."]}}),t(e.p,{get children(){return[`There are fundamental differences with the goals of each plugin, which is why the fork exists. Where possible, everything is done to\r
simultaneously support and contribute to `,t(e.a,{href:"https://github.com/lanayx/oxpecker",children:"Oxpecker.Solid"}),"."]}})]}})]}function u(n={}){const{wrapper:e}={...d(),...n.components};return e?t(e,s(n,{get children(){return t(a,n)}})):a(n)}function h(n,e){throw new Error("Expected component `"+n+"` to be defined: you likely forgot to import, pass, or provide it.")}const l={frontmatter:typeof o<"u"?o??{}:{},toc:typeof i<"u"?i:void 0,editLink:"",lastUpdated:1752090867e3};typeof window<"u"&&(window.$$SolidBase_page_data??={},window.$$SolidBase_page_data["C:/pdocs/Partas.Solid/routes/partas-solid/about/index.mdx"]=l);const g=l;export{g as $$SolidBase_page_data,u as default,o as frontmatter};
