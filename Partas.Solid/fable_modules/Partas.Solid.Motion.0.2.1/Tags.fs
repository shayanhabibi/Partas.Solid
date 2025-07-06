[<AutoOpen>]
module Partas.Solid.Motion.Tags

open Partas.Solid
open Fable.Core

[<Import("Motion", "solid-motionone")>]
type Motion() =
    interface RegularNode
    interface OptionKeys
    interface AttrKey
    interface MotionEventKeys
    // Enable polymorphism with the tag attribute name
    interface Polymorph
    [<Erase>] member val ``__PARTAS_POLYMORPHIC__tag``: string = unbox null with get,set
    [<Erase>]
    member this.tag
        with inline set(value: string) = this.``__PARTAS_POLYMORPHIC__tag`` <- value
        and inline get(): string = this.``__PARTAS_POLYMORPHIC__tag``

[<RequireQualifiedAccess; Erase>]
module Motion =
    [<Erase; Interface>]
    type MotionTagAttributes =
        inherit OptionKeys
        inherit MotionEventKeys
        
    [<Import("Motion.a", "solid-motionone")>]
    type a() =
        inherit Tags.a()
        interface MotionTagAttributes        
    [<Import("Motion.abbr", "solid-motionone")>]
    type abbr() =
        inherit Tags.abbr()
        interface MotionTagAttributes        
    [<Import("Motion.address", "solid-motionone")>]
    type address() =
        inherit Tags.address()
        interface MotionTagAttributes        
    [<Import("Motion.area", "solid-motionone")>]
    type area() =
        inherit Tags.area()
        interface MotionTagAttributes        
    [<Import("Motion.article", "solid-motionone")>]
    type article() =
        inherit Tags.article()
        interface MotionTagAttributes        
    [<Import("Motion.aside", "solid-motionone")>]
    type aside() =
        inherit Tags.aside()
        interface MotionTagAttributes        
    [<Import("Motion.audio", "solid-motionone")>]
    type audio() =
        inherit Tags.audio()
        interface MotionTagAttributes        
    [<Import("Motion.b", "solid-motionone")>]
    type b() =
        inherit Tags.b()
        interface MotionTagAttributes        
    [<Import("Motion.base", "solid-motionone")>]
    type base'() =
        inherit Tags.base'()
        interface MotionTagAttributes        
    [<Import("Motion.bdi", "solid-motionone")>]
    type bdi() =
        inherit Tags.bdi()
        interface MotionTagAttributes        
    [<Import("Motion.bdo", "solid-motionone")>]
    type bdo() =
        inherit Tags.bdo()
        interface MotionTagAttributes        
    [<Import("Motion.blockquote", "solid-motionone")>]
    type blockquote() =
        inherit Tags.blockquote()
        interface MotionTagAttributes        
    [<Import("Motion.body", "solid-motionone")>]
    type body() =
        inherit Tags.body()
        interface MotionTagAttributes        
    [<Import("Motion.br", "solid-motionone")>]
    type br() =
        inherit Tags.br()
        interface MotionTagAttributes        
    [<Import("Motion.button", "solid-motionone")>]
    type button() =
        inherit Tags.button()
        interface MotionTagAttributes        
    [<Import("Motion.canvas", "solid-motionone")>]
    type canvas() =
        inherit Tags.canvas()
        interface MotionTagAttributes        
    [<Import("Motion.caption", "solid-motionone")>]
    type caption() =
        inherit Tags.caption()
        interface MotionTagAttributes        
    [<Import("Motion.cite", "solid-motionone")>]
    type cite() =
        inherit Tags.cite()
        interface MotionTagAttributes        
    [<Import("Motion.code", "solid-motionone")>]
    type code() =
        inherit Tags.code()
        interface MotionTagAttributes        
    [<Import("Motion.col", "solid-motionone")>]
    type col() =
        inherit Tags.col()
        interface MotionTagAttributes        
    [<Import("Motion.colgroup", "solid-motionone")>]
    type colgroup() =
        inherit Tags.colgroup()
        interface MotionTagAttributes        
    [<Import("Motion.data", "solid-motionone")>]
    type data() =
        inherit Tags.data()
        interface MotionTagAttributes        
    [<Import("Motion.datalist", "solid-motionone")>]
    type datalist() =
        inherit Tags.datalist()
        interface MotionTagAttributes        
    [<Import("Motion.dd", "solid-motionone")>]
    type dd() =
        inherit Tags.dd()
        interface MotionTagAttributes        
    [<Import("Motion.del", "solid-motionone")>]
    type del() =
        inherit Tags.del()
        interface MotionTagAttributes        
    [<Import("Motion.details", "solid-motionone")>]
    type details() =
        inherit Tags.details()
        interface MotionTagAttributes        
    [<Import("Motion.dfn", "solid-motionone")>]
    type dfn() =
        inherit Tags.dfn()
        interface MotionTagAttributes        
    [<Import("Motion.dialog", "solid-motionone")>]
    type dialog() =
        inherit Tags.dialog()
        interface MotionTagAttributes        
    [<Import("Motion.div", "solid-motionone")>]
    type div() =
        inherit Tags.div()
        interface MotionTagAttributes        
    [<Import("Motion.dl", "solid-motionone")>]
    type dl() =
        inherit Tags.dl()
        interface MotionTagAttributes        
    [<Import("Motion.dt", "solid-motionone")>]
    type dt() =
        inherit Tags.dt()
        interface MotionTagAttributes        
    [<Import("Motion.em", "solid-motionone")>]
    type em() =
        inherit Tags.em()
        interface MotionTagAttributes        
    [<Import("Motion.embed", "solid-motionone")>]
    type embed() =
        inherit Tags.embed()
        interface MotionTagAttributes        
    [<Import("Motion.fieldset", "solid-motionone")>]
    type fieldset() =
        inherit Tags.fieldset()
        interface MotionTagAttributes        
    [<Import("Motion.figcaption", "solid-motionone")>]
    type figcaption() =
        inherit Tags.figcaption()
        interface MotionTagAttributes        
    [<Import("Motion.figure", "solid-motionone")>]
    type figure() =
        inherit Tags.figure()
        interface MotionTagAttributes        
    [<Import("Motion.footer", "solid-motionone")>]
    type footer() =
        inherit Tags.footer()
        interface MotionTagAttributes        
    [<Import("Motion.form", "solid-motionone")>]
    type form() =
        inherit Tags.form()
        interface MotionTagAttributes        
    [<Import("Motion.h1", "solid-motionone")>]
    type h1() =
        inherit Tags.h1()
        interface MotionTagAttributes        
    [<Import("Motion.h2", "solid-motionone")>]
    type h2() =
        inherit Tags.h2()
        interface MotionTagAttributes        
    [<Import("Motion.h3", "solid-motionone")>]
    type h3() =
        inherit Tags.h3()
        interface MotionTagAttributes        
    [<Import("Motion.h4", "solid-motionone")>]
    type h4() =
        inherit Tags.h4()
        interface MotionTagAttributes        
    [<Import("Motion.h5", "solid-motionone")>]
    type h5() =
        inherit Tags.h5()
        interface MotionTagAttributes        
    [<Import("Motion.h6", "solid-motionone")>]
    type h6() =
        inherit Tags.h6()
        interface MotionTagAttributes        
    [<Import("Motion.head", "solid-motionone")>]
    type head() =
        inherit Tags.head()
        interface MotionTagAttributes        
    [<Import("Motion.header", "solid-motionone")>]
    type header() =
        inherit Tags.header()
        interface MotionTagAttributes        
    // [<Import("Motion.hgroup", "solid-motionone")>]
    // type hgroup() =
    //     inherit Tags.hgroup()
    //     interface MotionTagAttributes    
    [<Import("Motion.hr", "solid-motionone")>]
    type hr() =
        inherit Tags.hr()
        interface MotionTagAttributes        
    [<Import("Motion.html", "solid-motionone")>]
    type html() =
        inherit Tags.html()
        interface MotionTagAttributes        
    [<Import("Motion.i", "solid-motionone")>]
    type i() =
        inherit Tags.i()
        interface MotionTagAttributes        
    [<Import("Motion.iframe", "solid-motionone")>]
    type iframe() =
        inherit Tags.iframe()
        interface MotionTagAttributes        
    [<Import("Motion.img", "solid-motionone")>]
    type img() =
        inherit Tags.img()
        interface MotionTagAttributes        
    [<Import("Motion.input", "solid-motionone")>]
    type input() =
        inherit Tags.input()
        interface MotionTagAttributes        
    [<Import("Motion.ins", "solid-motionone")>]
    type ins() =
        inherit Tags.ins()
        interface MotionTagAttributes        
    [<Import("Motion.kbd", "solid-motionone")>]
    type kbd() =
        inherit Tags.kbd()
        interface MotionTagAttributes        
    [<Import("Motion.label", "solid-motionone")>]
    type label() =
        inherit Tags.label()
        interface MotionTagAttributes        
    [<Import("Motion.legend", "solid-motionone")>]
    type legend() =
        inherit Tags.legend()
        interface MotionTagAttributes        
    [<Import("Motion.li", "solid-motionone")>]
    type li() =
        inherit Tags.li()
        interface MotionTagAttributes        
    [<Import("Motion.link", "solid-motionone")>]
    type link() =
        inherit Tags.link()
        interface MotionTagAttributes        
    [<Import("Motion.main", "solid-motionone")>]
    type main() =
        inherit Tags.main()
        interface MotionTagAttributes        
    [<Import("Motion.map", "solid-motionone")>]
    type map() =
        inherit Tags.map()
        interface MotionTagAttributes        
    [<Import("Motion.mark", "solid-motionone")>]
    type mark() =
        inherit Tags.mark()
        interface MotionTagAttributes        
    [<Import("Motion.menu", "solid-motionone")>]
    type menu() =
        inherit Tags.menu()
        interface MotionTagAttributes        
    [<Import("Motion.meta", "solid-motionone")>]
    type meta() =
        inherit Tags.meta()
        interface MotionTagAttributes        
    [<Import("Motion.meter", "solid-motionone")>]
    type meter() =
        inherit Tags.meter()
        interface MotionTagAttributes        
    [<Import("Motion.nav", "solid-motionone")>]
    type nav() =
        inherit Tags.nav()
        interface MotionTagAttributes        
    [<Import("Motion.noscript", "solid-motionone")>]
    type noscript() =
        inherit Tags.noscript()
        interface MotionTagAttributes        
    [<Import("Motion.object", "solid-motionone")>]
    type object'() =
        inherit Tags.object'()
        interface MotionTagAttributes        
    [<Import("Motion.ol", "solid-motionone")>]
    type ol() =
        inherit Tags.ol()
        interface MotionTagAttributes        
    [<Import("Motion.optgroup", "solid-motionone")>]
    type optgroup() =
        inherit Tags.optgroup()
        interface MotionTagAttributes        
    [<Import("Motion.option", "solid-motionone")>]
    type option() =
        inherit Tags.option'()
        interface MotionTagAttributes        
    [<Import("Motion.output", "solid-motionone")>]
    type output() =
        inherit Tags.output()
        interface MotionTagAttributes        
    [<Import("Motion.p", "solid-motionone")>]
    type p() =
        inherit Tags.p()
        interface MotionTagAttributes        
    [<Import("Motion.picture", "solid-motionone")>]
    type picture() =
        inherit Tags.picture()
        interface MotionTagAttributes        
    [<Import("Motion.pre", "solid-motionone")>]
    type pre() =
        inherit Tags.pre()
        interface MotionTagAttributes        
    [<Import("Motion.progress", "solid-motionone")>]
    type progress() =
        inherit Tags.progress()
        interface MotionTagAttributes        
    [<Import("Motion.q", "solid-motionone")>]
    type q() =
        inherit Tags.q()
        interface MotionTagAttributes        
    [<Import("Motion.s", "solid-motionone")>]
    type s() =
        inherit Tags.s()
        interface MotionTagAttributes        
    [<Import("Motion.samp", "solid-motionone")>]
    type samp() =
        inherit Tags.samp()
        interface MotionTagAttributes        
    [<Import("Motion.script", "solid-motionone")>]
    type script() =
        inherit Tags.script()
        interface MotionTagAttributes        
    [<Import("Motion.search", "solid-motionone")>]
    type search() =
        inherit Tags.search()
        interface MotionTagAttributes        
    [<Import("Motion.section", "solid-motionone")>]
    type section() =
        inherit Tags.section()
        interface MotionTagAttributes        
    [<Import("Motion.select", "solid-motionone")>]
    type select() =
        inherit Tags.select()
        interface MotionTagAttributes        
    // [<Import("Motion.slot", "solid-motionone")>]
    // type slot() =
    //     inherit Tags.slot()
    //     interface MotionTagAttributes    
    [<Import("Motion.small", "solid-motionone")>]
    type small() =
        inherit Tags.small()
        interface MotionTagAttributes        
    [<Import("Motion.source", "solid-motionone")>]
    type source() =
        inherit Tags.source()
        interface MotionTagAttributes        
    [<Import("Motion.span", "solid-motionone")>]
    type span() =
        inherit Tags.span()
        interface MotionTagAttributes        
    [<Import("Motion.strong", "solid-motionone")>]
    type strong() =
        inherit Tags.strong()
        interface MotionTagAttributes        
    [<Import("Motion.style", "solid-motionone")>]
    type style() =
        inherit Tags.style()
        interface MotionTagAttributes        
    [<Import("Motion.sub", "solid-motionone")>]
    type sub() =
        inherit Tags.sub()
        interface MotionTagAttributes        
    [<Import("Motion.summary", "solid-motionone")>]
    type summary() =
        inherit Tags.summary()
        interface MotionTagAttributes        
    [<Import("Motion.sup", "solid-motionone")>]
    type sup() =
        inherit Tags.sup()
        interface MotionTagAttributes        
    [<Import("Motion.table", "solid-motionone")>]
    type table() =
        inherit Tags.table()
        interface MotionTagAttributes        
    [<Import("Motion.tbody", "solid-motionone")>]
    type tbody() =
        inherit Tags.tbody()
        interface MotionTagAttributes        
    [<Import("Motion.td", "solid-motionone")>]
    type td() =
        inherit Tags.td()
        interface MotionTagAttributes        
    [<Import("Motion.template", "solid-motionone")>]
    type template() =
        interface RegularNode
        interface MotionTagAttributes        
    [<Import("Motion.textarea", "solid-motionone")>]
    type textarea() =
        inherit Tags.textarea()
        interface MotionTagAttributes        
    [<Import("Motion.tfoot", "solid-motionone")>]
    type tfoot() =
        inherit Tags.tfoot()
        interface MotionTagAttributes        
    [<Import("Motion.th", "solid-motionone")>]
    type th() =
        inherit Tags.th()
        interface MotionTagAttributes        
    [<Import("Motion.thead", "solid-motionone")>]
    type thead() =
        inherit Tags.thead()
        interface MotionTagAttributes        
    [<Import("Motion.time", "solid-motionone")>]
    type time() =
        inherit Tags.time()
        interface MotionTagAttributes        
    [<Import("Motion.title", "solid-motionone")>]
    type title() =
        inherit Tags.title()
        interface MotionTagAttributes        
    [<Import("Motion.tr", "solid-motionone")>]
    type tr() =
        inherit Tags.tr()
        interface MotionTagAttributes        
    // [<Import("Motion.track", "solid-motionone")>]
    // type track() =
    //     inherit Tags.track()
    //     interface MotionTagAttributes    
    [<Import("Motion.u", "solid-motionone")>]
    type u() =
        inherit Tags.u()
        interface MotionTagAttributes        
    [<Import("Motion.ul", "solid-motionone")>]
    type ul() =
        inherit Tags.ul()
        interface MotionTagAttributes        
    [<Import("Motion.var", "solid-motionone")>]
    type var() =
        inherit Tags.var()
        interface MotionTagAttributes        
    [<Import("Motion.video", "solid-motionone")>]
    type video() =
        inherit Tags.video()
        interface MotionTagAttributes        
    [<Import("Motion.wbr", "solid-motionone")>]
    type wbr() =
        inherit Tags.wbr()
        interface MotionTagAttributes           
    [<Import("Motion.animate", "solid-motionone")>]
    type animate() =
        inherit Svg.animate()
        interface MotionTagAttributes    
    [<Import("Motion.animateMotion", "solid-motionone")>]
    type animateMotion() =
        inherit Svg.animateMotion()
        interface MotionTagAttributes    
    [<Import("Motion.animateTransform", "solid-motionone")>]
    type animateTransform() =
        inherit Svg.animateTransform()
        interface MotionTagAttributes    
    [<Import("Motion.circle", "solid-motionone")>]
    type circle() =
        inherit Svg.circle()
        interface MotionTagAttributes    
    [<Import("Motion.clipPath", "solid-motionone")>]
    type clipPath() =
        inherit Svg.clipPath()
        interface MotionTagAttributes    
    [<Import("Motion.defs", "solid-motionone")>]
    type defs() =
        inherit Svg.defs()
        interface MotionTagAttributes    
    [<Import("Motion.desc", "solid-motionone")>]
    type desc() =
        inherit Svg.desc()
        interface MotionTagAttributes    
    [<Import("Motion.ellipse", "solid-motionone")>]
    type ellipse() =
        inherit Svg.ellipse()
        interface MotionTagAttributes    
    [<Import("Motion.feBlend", "solid-motionone")>]
    type feBlend() =
        inherit Svg.feBlend()
        interface MotionTagAttributes    
    [<Import("Motion.feColorMatrix", "solid-motionone")>]
    type feColorMatrix() =
        inherit Svg.feColorMatrix()
        interface MotionTagAttributes    
    [<Import("Motion.feComponentTransfer", "solid-motionone")>]
    type feComponentTransfer() =
        inherit Svg.feComponentTransfer()
        interface MotionTagAttributes    
    [<Import("Motion.feComposite", "solid-motionone")>]
    type feComposite() =
        inherit Svg.feComposite()
        interface MotionTagAttributes    
    [<Import("Motion.feConvolveMatrix", "solid-motionone")>]
    type feConvolveMatrix() =
        inherit Svg.feConvolveMatrix()
        interface MotionTagAttributes    
    [<Import("Motion.feDiffuseLighting", "solid-motionone")>]
    type feDiffuseLighting() =
        inherit Svg.feDiffuseLighting()
        interface MotionTagAttributes    
    [<Import("Motion.feDisplacementMap", "solid-motionone")>]
    type feDisplacementMap() =
        inherit Svg.feDisplacementMap()
        interface MotionTagAttributes    
    [<Import("Motion.feDistantLight", "solid-motionone")>]
    type feDistantLight() =
        inherit Svg.feDistantLight()
        interface MotionTagAttributes    
    [<Import("Motion.feDropShadow", "solid-motionone")>]
    type feDropShadow() =
        inherit Svg.feDropShadow()
        interface MotionTagAttributes    
    [<Import("Motion.feFlood", "solid-motionone")>]
    type feFlood() =
        inherit Svg.feFlood()
        interface MotionTagAttributes    
    [<Import("Motion.feFuncA", "solid-motionone")>]
    type feFuncA() =
        inherit Svg.feFuncA()
        interface MotionTagAttributes    
    [<Import("Motion.feFuncB", "solid-motionone")>]
    type feFuncB() =
        inherit Svg.feFuncB()
        interface MotionTagAttributes    
    [<Import("Motion.feFuncG", "solid-motionone")>]
    type feFuncG() =
        inherit Svg.feFuncG()
        interface MotionTagAttributes    
    [<Import("Motion.feFuncR", "solid-motionone")>]
    type feFuncR() =
        inherit Svg.feFuncR()
        interface MotionTagAttributes    
    [<Import("Motion.feGaussianBlur", "solid-motionone")>]
    type feGaussianBlur() =
        inherit Svg.feGaussianBlur()
        interface MotionTagAttributes    
    [<Import("Motion.feImage", "solid-motionone")>]
    type feImage() =
        inherit Svg.feImage()
        interface MotionTagAttributes    
    [<Import("Motion.feMerge", "solid-motionone")>]
    type feMerge() =
        inherit Svg.feMerge()
        interface MotionTagAttributes    
    [<Import("Motion.feMergeNode", "solid-motionone")>]
    type feMergeNode() =
        inherit Svg.feMergeNode()
        interface MotionTagAttributes    
    [<Import("Motion.feMorphology", "solid-motionone")>]
    type feMorphology() =
        inherit Svg.feMorphology()
        interface MotionTagAttributes    
    [<Import("Motion.feOffset", "solid-motionone")>]
    type feOffset() =
        inherit Svg.feOffset()
        interface MotionTagAttributes    
    [<Import("Motion.fePointLight", "solid-motionone")>]
    type fePointLight() =
        inherit Svg.fePointLight()
        interface MotionTagAttributes    
    [<Import("Motion.feSpecularLighting", "solid-motionone")>]
    type feSpecularLighting() =
        inherit Svg.feSpecularLighting()
        interface MotionTagAttributes    
    [<Import("Motion.feSpotLight", "solid-motionone")>]
    type feSpotLight() =
        inherit Svg.feSpotLight()
        interface MotionTagAttributes    
    [<Import("Motion.feTile", "solid-motionone")>]
    type feTile() =
        inherit Svg.feTile()
        interface MotionTagAttributes    
    [<Import("Motion.feTurbulence", "solid-motionone")>]
    type feTurbulence() =
        inherit Svg.feTurbulence()
        interface MotionTagAttributes    
    [<Import("Motion.filter", "solid-motionone")>]
    type filter() =
        inherit Svg.filter()
        interface MotionTagAttributes    
    [<Import("Motion.foreignObject", "solid-motionone")>]
    type foreignObject() =
        inherit Svg.foreignObject()
        interface MotionTagAttributes    
    [<Import("Motion.g", "solid-motionone")>]
    type g() =
        inherit Svg.g()
        interface MotionTagAttributes    
    [<Import("Motion.image", "solid-motionone")>]
    type image() =
        inherit Svg.image()
        interface MotionTagAttributes    
    [<Import("Motion.line", "solid-motionone")>]
    type line() =
        inherit Svg.line()
        interface MotionTagAttributes    
    [<Import("Motion.linearGradient", "solid-motionone")>]
    type linearGradient() =
        inherit Svg.linearGradient()
        interface MotionTagAttributes    
    [<Import("Motion.marker", "solid-motionone")>]
    type marker() =
        inherit Svg.marker()
        interface MotionTagAttributes    
    [<Import("Motion.mask", "solid-motionone")>]
    type mask() =
        inherit Svg.mask()
        interface MotionTagAttributes    
    [<Import("Motion.metadata", "solid-motionone")>]
    type metadata() =
        inherit Svg.metadata()
        interface MotionTagAttributes    
    [<Import("Motion.mpath", "solid-motionone")>]
    type mpath() =
        inherit Svg.mpath()
        interface MotionTagAttributes    
    [<Import("Motion.path", "solid-motionone")>]
    type path() =
        inherit Svg.path()
        interface MotionTagAttributes    
    [<Import("Motion.pattern", "solid-motionone")>]
    type pattern() =
        inherit Svg.pattern()
        interface MotionTagAttributes    
    [<Import("Motion.polygon", "solid-motionone")>]
    type polygon() =
        inherit Svg.polygon()
        interface MotionTagAttributes    
    [<Import("Motion.polyline", "solid-motionone")>]
    type polyline() =
        inherit Svg.polyline()
        interface MotionTagAttributes    
    [<Import("Motion.radialGradient", "solid-motionone")>]
    type radialGradient() =
        inherit Svg.radialGradient()
        interface MotionTagAttributes    
    [<Import("Motion.rect", "solid-motionone")>]
    type rect() =
        inherit Svg.rect()
        interface MotionTagAttributes      
    [<Import("Motion.set", "solid-motionone")>]
    type set() =
        inherit Svg.set()
        interface MotionTagAttributes    
    [<Import("Motion.stop", "solid-motionone")>]
    type stop() =
        inherit Svg.stop()
        interface MotionTagAttributes    
    [<Import("Motion.svg", "solid-motionone")>]
    type svg() =
        inherit Svg.svg()
        interface MotionTagAttributes    
    [<Import("Motion.switch", "solid-motionone")>]
    type switch() =
        inherit Svg.switch()
        interface MotionTagAttributes    
    [<Import("Motion.symbol", "solid-motionone")>]
    type symbol() =
        inherit Svg.symbol()
        interface MotionTagAttributes    
    [<Import("Motion.text", "solid-motionone")>]
    type text() =
        inherit Svg.text()
        interface MotionTagAttributes    
    [<Import("Motion.textPath", "solid-motionone")>]
    type textPath() =
        inherit Svg.textPath()
        interface MotionTagAttributes    
    [<Import("Motion.tspan", "solid-motionone")>]
    type tspan() =
        inherit Svg.tspan()
        interface MotionTagAttributes    
    [<Import("Motion.use", "solid-motionone")>]
    type use'() =
        inherit Svg.use'()
        interface MotionTagAttributes    
    [<Import("Motion.view", "solid-motionone")>]
    type view() =
        inherit Svg.view()
        interface MotionTagAttributes    
