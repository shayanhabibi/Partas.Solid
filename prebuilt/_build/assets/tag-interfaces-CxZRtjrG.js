import{Z as a,h as t,t as c}from"./index-CZ23QLAh.js";const r=[{title:"Tag Interfaces",href:"#tag-interfaces",children:[{title:"Foundation Interfaces",href:"#foundation-interfaces",children:[{title:"HtmlElement",href:"#htmlelement",children:[]},{title:"HtmlTag",href:"#htmltag",children:[]},{title:"HtmlContainer",href:"#htmlcontainer",children:[]}]},{title:"Derivative Interfaces",href:"#derivative-interfaces",children:[{title:"RegularNode",href:"#regularnode",children:[]},{title:"VoidNode",href:"#voidnode",children:[]},{title:"FragmentNode",href:"#fragmentnode",children:[]}]},{title:"Special Interfaces",href:"#special-interfaces",children:[{title:"ChildLambdaProvider",href:"#childlambdaprovider",children:[]},{title:"ChildLambdaProviderStrict",href:"#childlambdaproviderstrict",children:[]},{title:"Polymorph",href:"#polymorph",children:[]}]}]}],i={title:"Tag Interfaces"};function d(n){const e={a:"a",blockquote:"blockquote",code:"code",h1:"h1",h2:"h2",h3:"h3",p:"p",...a(),...n.components};return[t(e.h1,{id:"tag-interfaces",get children(){return t(e.a,{"data-auto-heading":"",href:"#tag-interfaces",children:"Tag Interfaces"})}}),`
`,t(e.p,{children:`The following interfaces are exposed by the Partas.Solid library to provide\r
inheritance for common component structures and properties.`}),`
`,t(e.p,{get children(){return["Beyond this, feel free to inherit from any of the concrete classes such as ",t(e.code,{children:"div"}),"."]}}),`
`,t(e.h2,{id:"foundation-interfaces",get children(){return t(e.a,{"data-auto-heading":"",href:"#foundation-interfaces",children:"Foundation Interfaces"})}}),`
`,t(e.h3,{id:"htmlelement",get children(){return t(e.a,{"data-auto-heading":"",href:"#htmlelement",get children(){return t(e.code,{children:"HtmlElement"})}})}}),`
`,t(e.p,{children:"Base type for all elements."}),`
`,t(e.p,{get children(){return[`To allow a class/type to be accepted as a child node in the computation expressions,\r
the type must derive from `,t(e.code,{children:"HtmlElement"}),", or one of it's subsequent derivatives."]}}),`
`,t(e.h3,{id:"htmltag",get children(){return t(e.a,{"data-auto-heading":"",href:"#htmltag",get children(){return t(e.code,{children:"HtmlTag"})}})}}),`
`,t(e.p,{get children(){return["Implementation of ",t(e.code,{children:"HtmlElement"})," with common/global solid-js attributes."]}}),`
`,t(e.p,{get children(){return["Interface provides the ",t(e.a,{href:"/partas-solid/extension-methods",children:"support for extension methods"}),"."]}}),`
`,t(e.h3,{id:"htmlcontainer",get children(){return t(e.a,{"data-auto-heading":"",href:"#htmlcontainer",get children(){return t(e.code,{children:"HtmlContainer"})}})}}),`
`,t(e.p,{children:"Interface enables the computation expression to construct child elements."}),`
`,t(e.h2,{id:"derivative-interfaces",get children(){return t(e.a,{"data-auto-heading":"",href:"#derivative-interfaces",children:"Derivative Interfaces"})}}),`
`,t(e.h3,{id:"regularnode",get children(){return t(e.a,{"data-auto-heading":"",href:"#regularnode",get children(){return t(e.code,{children:"RegularNode"})}})}}),`
`,t(e.p,{children:`An interface which provides common/global solid-js attributes, extension methods,\r
and accepts children nodes.`}),`
`,t(e.blockquote,{get children(){return[`
`,t(e.p,{children:"This is essentially just a convenient interface to both HtmlContainer and HtmlTag"}),`
`]}}),`
`,t(e.h3,{id:"voidnode",get children(){return t(e.a,{"data-auto-heading":"",href:"#voidnode",get children(){return t(e.code,{children:"VoidNode"})}})}}),`
`,t(e.p,{children:`An interface which provides common/global solid-js attributes, extension methods,\r
but does not accept any children nodes.`}),`
`,t(e.blockquote,{get children(){return[`
`,t(e.p,{children:"This is essentially just an alias to HtmlTag."}),`
`,t(e.p,{children:`Historically, the Derivative Interfaces were concrete classes. The change\r
to interfaces make this more-or-less redundant.`}),`
`]}}),`
`,t(e.h3,{id:"fragmentnode",get children(){return t(e.a,{"data-auto-heading":"",href:"#fragmentnode",get children(){return t(e.code,{children:"FragmentNode"})}})}}),`
`,t(e.p,{get children(){return["Special interface which renders ",t(e.code,{children:"<> </>"})," in the plugin."]}}),`
`,t(e.p,{get children(){return["At face value, it is just a derivative of ",t(e.code,{children:"HtmlContainer"}),`. It is implemented\r
by the concrete class `,t(e.code,{children:"Fragment()"}),"."]}}),`
`,t(e.h2,{id:"special-interfaces",get children(){return t(e.a,{"data-auto-heading":"",href:"#special-interfaces",children:"Special Interfaces"})}}),`
`,t(e.h3,{id:"childlambdaprovider",get children(){return t(e.a,{"data-auto-heading":"",href:"#childlambdaprovider",get children(){return t(e.code,{children:"ChildLambdaProvider"})}})}}),`
`,t(e.p,{get children(){return[`Provides interfaces which implement the lambda constructor functionality that is\r
already existing in components such as `,t(e.code,{children:"For"})," and ",t(e.code,{children:"Index"}),"."]}}),`
`,t(e.p,{children:`A numeric suffix to the interface name provides more explicit type parameter\r
numbers.`}),`
`,t(e.p,{children:"The plugin uncurries the lambda function that is passed as a child to the element."}),`
`,t(e.h3,{id:"childlambdaproviderstrict",get children(){return t(e.a,{"data-auto-heading":"",href:"#childlambdaproviderstrict",get children(){return t(e.code,{children:"ChildLambdaProviderStrict"})}})}}),`
`,t(e.p,{get children(){return["An implementation of the ",t(e.code,{children:"ChildLambdaProvider"}),` family of interfaces, with the\r
addendum that you can specify the exact type that the child must be.`]}}),`
`,t(e.p,{get children(){return["An example use is for elements like the solidjs/router ",t(e.code,{children:"Router"}),`, which can only\r
accept children of type `,t(e.code,{children:"Route"}),"."]}}),`
`,t(e.h3,{id:"polymorph",get children(){return t(e.a,{"data-auto-heading":"",href:"#polymorph",get children(){return t(e.code,{children:"Polymorph"})}})}}),`
`,t(e.p,{get children(){return["Library support for polymorphic elements from ",t(e.code,{children:"Kobalte"})," and ",t(e.code,{children:"ArkUI"}),"."]}}),`
`,t(e.p,{get children(){return["Usage is best described in the ",t(e.a,{href:"/ecosystem/kobalte",children:"Partas.Solid.Kobalte"}),` binding\r
library documentation.`]}})]}function h(n={}){const{wrapper:e}={...a(),...n.components};return e?t(e,c(n,{get children(){return t(d,n)}})):d(n)}const o={frontmatter:typeof i<"u"?i??{}:{},toc:typeof r<"u"?r:void 0,editLink:"",lastUpdated:1751873401e3};typeof window<"u"&&(window.$$SolidBase_page_data??={},window.$$SolidBase_page_data["C:/pdocs/Partas.Solid/routes/partas-solid/tag-interfaces.mdx"]=o);const s=o;export{s as $$SolidBase_page_data,h as default,i as frontmatter};
