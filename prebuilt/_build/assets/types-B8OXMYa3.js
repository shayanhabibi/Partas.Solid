import{Z as d,h as t,t as l}from"./index-CZ23QLAh.js";const i=[{title:"Types",href:"#types",children:[{title:"ElementBuilder",href:"#elementbuilder",children:[]},{title:"PluginContext",href:"#plugincontext",children:[]}]}],o={title:"Plugin Types"};function a(n){const e={a:"a",code:"code",h1:"h1",h2:"h2",p:"p",...d(),...n.components},{DirectiveContainer:r}=e;return r||c("DirectiveContainer"),[t(e.h1,{id:"types",get children(){return t(e.a,{"data-auto-heading":"",href:"#types",children:"Types"})}}),`
`,t(e.p,{children:"Contains DUs, types, and functions relating to those types."}),`
`,t(r,{type:"note",get children(){return[t(e.p,{children:"The plugin is extensively documented in the source code to explain usage."}),t(e.p,{children:"For this reason, only sparse notes are included here."})]}}),`
`,t(e.p,{children:`Many patterns relate to abstracting away recursive drilling of a node\r
which may be a wrapper such as a typecast, to elucidate a type or identity\r
within.`}),`
`,t(e.p,{children:`Unions are used as the result of these patterns, such that the consumption\r
for higher order patterns is simplified.`}),`
`,t(e.p,{get children(){return["Examples of these are the union types ",t(e.code,{children:"MemberRefType"})," and ",t(e.code,{children:"IdentType"}),`. The\r
primary purpose of the latter, is to remove the purposely derranged identifiers\r
that we match against from consuming patterns, such as `,t(e.code,{children:"PARTAS_YIELD"}),"."]}}),`
`,t(e.h2,{id:"elementbuilder",get children(){return t(e.a,{"data-auto-heading":"",href:"#elementbuilder",get children(){return t(e.code,{children:"ElementBuilder"})}})}}),`
`,t(e.p,{children:"This record is the end objective for tag transformations."}),`
`,t(e.p,{get children(){return[`Collecting and constructing this record, can then be processed uniformly into\r
a `,t(e.code,{children:"JSX.create"})," call."]}}),`
`,t(e.h2,{id:"plugincontext",get children(){return t(e.a,{"data-auto-heading":"",href:"#plugincontext",get children(){return t(e.code,{children:"PluginContext"})}})}}),`
`,t(e.p,{children:"This record is threaded all transformations."}),`
`,t(e.p,{children:`It provides the cache arrays for collecting getters and setters, and exposes\r
the component flags to transformations to conditionally alter behaviour.`}),`
`,t(e.p,{children:`It also exposes the plugin helper to all transformations, allowing warnings and\r
errors to be passed to the Fable compiler.`}),`
`,t(e.p,{children:"It will be commonly seen as the first argument to many patterns and functions."})]}function p(n={}){const{wrapper:e}={...d(),...n.components};return e?t(e,l(n,{get children(){return t(a,n)}})):a(n)}function c(n,e){throw new Error("Expected component `"+n+"` to be defined: you likely forgot to import, pass, or provide it.")}const s={frontmatter:typeof o<"u"?o??{}:{},toc:typeof i<"u"?i:void 0,editLink:"",lastUpdated:1751873401e3};typeof window<"u"&&(window.$$SolidBase_page_data??={},window.$$SolidBase_page_data["C:/pdocs/Partas.Solid/routes/partas-solid/dev/types.mdx"]=s);const u=s;export{u as $$SolidBase_page_data,p as default,o as frontmatter};
