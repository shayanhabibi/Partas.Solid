// Semantic oracle for compiler output. JSX is lowered to inert trees, independently
// of either Solid version; this checks values, child retention and side effects.
import fs from 'node:fs';
import path from 'node:path';
import vm from 'node:vm';
import {createRequire} from 'node:module';
import {pathToFileURL} from 'node:url';
const root = path.resolve(import.meta.dirname, '../..');
const require = createRequire(path.join(root, 'tests/Partas.Solid.Tests.Runtime/package.json'));
const {transformSync, types: t} = require('@babel/core');
const h = (tag, content = '', attrs = '') => `<${tag}${attrs}>${content}</${tag}>`;
const span = s => h('span', s), b = s => h('b', s), i = s => h('i', s), div = s => h('div', s);
const scenarios = [];
const add = (name, args, expected, logs = [], options = {}) => scenarios.push({name, args, expected, logs, ...options});
add('Baseline', [], div(span('one') + 'two'));
for (const flag of [true,false]) {
  add('IfOnly', [flag], div(flag ? span('yes') : ''));
  add('IfElse', [flag], div(flag ? span('yes') : b('no')));
  add('IfText', [flag], div(flag ? 'yes' : 'no'));
  add('IfMixed', [flag], div(flag ? span('yes') : 'no'));
  add('IfBeforeSibling', [flag], div((flag ? span('yes') : '') + b('tail')));
  add('IfAfterSibling', [flag], div(b('head') + (flag ? span('yes') : '')));
  add('IfBetweenSiblings', [flag], div(b('head') + (flag ? span('yes') : i('no')) + b('tail')));
  add('LetInBranch', [flag,'hello'], div(flag ? span('HELLO') : 'no'));
  add('StatementInBranch', [flag,'$log'], div(flag ? span('yes') : 'no'), flag ? ['effect'] : []);
  add('ExplicitYield', [flag], div('head' + (flag ? span('yes') : '') + 'tail'));
  add('AttributeBranch', [flag], h('div','body', ` title=${JSON.stringify(flag ? 'yes' : 'no')}`));
  add('OutsideConditional', [flag], div(flag ? 'yes' : 'no'));
  add('TryExpression', [flag], div(flag ? 'caught' : 'ok'));
  add('NestedArrayBranch', [flag], flag ? span('yes') : b('no'));
}
for (const a of [true,false]) for (const second of [true,false]) {
  add('NestedIf', [a,second], div(a ? (second ? span('ab') : i('a')) : 'none'));
  add('MatchTuple', [a,second], div(a && second ? span('both') : a || second ? b('one') : i('none')));
}
for (const [kind, expected] of [['$Alpha',span('a')],['$Beta:hello',b('hello')],['$Other',i('o')]]) add('MatchOnly',[kind],div(expected));
for (const [kind, expected] of [['$Alpha',b('other')],['$Other',b('other')],['$Beta:hi',i('hi')],['$Beta:hello',span('hello')]]) add('MatchGuard',[kind],div(expected));
for (const kind of ['$Alpha','$Other','$Beta:hi']) {
  add('StatementInMatch',[kind,'$log'],div(kind === '$Alpha' ? span('yes') : 'no'),kind === '$Alpha' ? ['effect'] : []);
  add('OutsideMatch',[kind],div(kind === '$Alpha' ? 'yes' : 'no'));
}
add('LetText',['hello'],div(span('HELLO')));
add('LetSibling',['hello'],div('head' + span('HELLO') + 'tail'));
add('StatementBefore',['$log'],div(span('yes')),['before']);
add('StatementBetween',['$log'],div(span('head') + b('tail')),['middle']);
add('StatementAfter',['$log'],div(span('yes')),['after']);
for (const name of ['ArrayMap','ListMap','SeqExpression']) add(name,[['one','two']],span('one') + span('two'));
for (const name of ['ArrayLiteral','ListLiteral']) add(name,[],span('one') + b('two'));
add('ArrayComprehension',[['one','','two']],span('one') + span('two'));
// Fable flattens successive function parameters into a single JS call.
add('LambdaReturn',[],span('lazy'));
add('LetElement',[],div(span('bound')));
add('LetElementInCe',[],div(span('bound')));
add('FunctionLocal',['hello'],div(span('hello')));
add('AttributeQuotes',[],h('div','body',` title=${JSON.stringify('a"b\n<&')}`));
add('AttributeLambda',['$log'],h('button','button'),['click'],{click:true});
add('TryFinally',['$log'],div('ok'),['finally']);
for(const kind of ['$Alpha','$Other','$Beta:xy']) {
  const beta = kind.startsWith('$Beta:');
  add('UnionPayloadNested',[kind],div(beta ? span(b('xy')) : 'empty'));
  add('UnionPayloadLength',[kind],div(beta ? span('2') : 'empty'));
  add('UnionPayloadAttribute',[kind],div(beta ? h('span','xy',' title="xy"') : 'empty'));
  add('UnionPayloadSiblings',[kind],div(beta ? span('xy') + b('tail') : 'empty'));
  add('UnionPayloadLocal',[kind],div(beta ? span('XY') + b('xy') : 'empty'));
  add('UnionPayloadGuarded',[kind],div(beta ? i('xy') : 'empty'));
  add('MatchTwoElements',[kind],div(kind === '$Alpha' ? span('one') + b('two') : i('three') + h('strong','four')));
  add('AttributeUnionMatch',[kind],h('div','body',` title=${JSON.stringify(beta ? 'xy' : kind === '$Alpha' ? 'alpha' : 'other')}`));
  add('AttributeArrayUnion',[kind],h('div','body',` title=${JSON.stringify(beta ? 'head:xy:tail' : 'head:empty:tail')}`));
}
add('UnionPayloadGuarded',['$Beta:abcd'],div(span('abcd') + b('long')));
for(const flag of [true,false]) {
  add('IfTwoElements',[flag],div(flag ? span('one') + b('two') : i('three') + h('strong','four')));
  add('IfElementText',[flag],div(flag ? span('one') + 'two' : 'empty'));
  add('IfTextElement',[flag],div(flag ? 'one' + span('two') : 'empty'));
  add('IfLocalTwoElements',[flag,'xy'],div(flag ? span('XY') + b('xy') : 'empty'));
  add('AttributeArrayBranch',[flag],h('div','body',` title=${JSON.stringify(flag ? 'head:yes:tail' : 'head:no:tail')}`));
  add('AttributeArrayComprehension',[flag],h('div','body',` title=${JSON.stringify(flag ? 'head:yes:tail' : 'head:tail')}`));
  add('AttributeUserThunk',[flag],h('div','body',` title=${JSON.stringify(flag ? 'yes' : 'no')}`));
}
add('LetThreeChildren',['xy'],div(span('XY') + 'middle' + b('xy')));
add('NestedLocalTwoChildren',['xy'],div(span(b('XY') + i('xy')) + 'tail'));

function jsxPlugin() {
  const name = n => t.stringLiteral(n.name);
  const text = value => value.split(/\r?\n/).map((line, index, all) => {
    let s = line.replace(/\t/g,' ');
    if(index) s = s.trimStart();
    if(index !== all.length - 1) s = s.trimEnd();
    return s;
  }).filter(Boolean).join(' ');
  const childrenOf = el => el.children.flatMap(c => c.type === 'JSXText' ? (text(c.value) ? [t.stringLiteral(text(c.value))] : []) : c.type === 'JSXExpressionContainer' ? (c.expression.type === 'JSXEmptyExpression' ? [] : [c.expression]) : [c]);
  return {visitor: {
    ExportNamedDeclaration(p) { p.replaceWith(p.node.declaration); },
    JSXElement: {exit(p) {
      const el = p.node;
      const props = el.openingElement.attributes.map(a => t.objectProperty(t.stringLiteral(a.name.name), a.value === null ? t.booleanLiteral(true) : a.value.type === 'JSXExpressionContainer' ? a.value.expression : a.value));
      const children = childrenOf(el);
      p.replaceWith(t.callExpression(t.identifier('__jsx'), [name(el.openingElement.name),t.objectExpression(props),t.arrayExpression(children)]));
    }},
    JSXFragment: {exit(p) {
      p.replaceWith(t.arrayExpression(childrenOf(p.node)));
    }}
  }};
}
function render(value) {
  if (value == null || typeof value === 'boolean') return '';
  if (typeof value === 'string' || typeof value === 'number') return String(value);
  // A CE continuation is a defect even if a Solid runtime happens to hide it.
  if (typeof value === 'function') throw new Error('CE/function leaked into child value');
  if (value.__node) {
    const attrs = Object.entries(value.props).filter(([key]) => !key.startsWith('on')).map(([key,val]) => ` ${key}=${JSON.stringify(val)}`).join('');
    return h(value.tag, value.children.map(render).join(''), attrs);
  }
  if (typeof value[Symbol.iterator] === 'function') return [...value].map(render).join('');
  throw new Error('Non-JSX object used as child: ' + String(value));
}
const results = [];
for (const lib of ['Partas','Oxpecker']) {
  const dir = path.join(root,'.research/jsx-stress',lib);
  const cache = new Map();
  for (const scenario of scenarios) {
    const {name} = scenario;
    const record = {library:lib,case:name,args:scenario.args,expected:scenario.expected,expectedLogs:scenario.logs};
    try {
      if (!cache.has(name)) {
        const file = path.join(dir, name + '.fs.jsx');
        if (!fs.existsSync(file)) throw new Error('No emitted file (compiler rejection; see compile log)');
        const source = fs.readFileSync(file,'utf8');
        const compiled = transformSync(source, {configFile:false,babelrc:false,parserOpts:{plugins:['jsx']},plugins:[jsxPlugin()]} );
        // Resolve actual Fable helpers, but flag erased DSL constructors/Run as leakage.
        const imports = {};
        const parsed = require('@babel/parser').parse(source,{sourceType:'module',plugins:['jsx']});
        for (const decl of parsed.program.body.filter(n => n.type === 'ImportDeclaration')) {
          const target = decl.source.value;
          if (target.endsWith('.fs.jsx')) {
            for (const s of decl.specifiers) imports[s.local.name] = () => {throw new Error('Untransformed DSL import: ' + s.local.name);};
          } else {
            const module = await import(pathToFileURL(path.resolve(dir,target)).href);
            for (const s of decl.specifiers) imports[s.local.name] = module[s.imported?.name ?? 'default'];
          }
        }
        const code = compiled.code.replace(/^import .*?;\s*$/gm,'');
        const context = vm.createContext({...imports,__jsx:(tag,props,children)=>({__node:true,tag,props,children})});
        vm.runInContext(code + `\nglobalThis.__entry = ${name}; globalThis.__kind = Kind;`,context);
        cache.set(name,{context,source});
      }
      const {context,source} = cache.get(name);
      const logs = [];
      const listModule = await import(pathToFileURL(path.join(dir,'fable_modules/fable-library-js.5.21.0/List.js')).href);
      const args = scenario.args.map(a => {
        if(a === '$log') return s => {logs.push(s);};
        if(a === '$Alpha') return context.__kind.Alpha;
        if(a === '$Other') return context.__kind.Other;
        if(typeof a === 'string' && a.startsWith('$Beta:')) return new context.__kind(1,[a.slice(6)]);
        if(name === 'ListMap' && Array.isArray(a)) return listModule.ofArray(a);
        return a;
      });
      let value = context.__entry(...args);
      if(scenario.invokeResult) value = value();
      if(scenario.click) value.props.onClick({});
      record.actual = render(value);
      record.logs = logs;
      record.ceMarkers = /HtmlContainerExtensions_Run|\b(?:cont|builder|PARTAS_BUILDER|PARTAS_YIELD)\b/.test(source);
      record.pass = record.actual === scenario.expected && JSON.stringify(logs) === JSON.stringify(scenario.logs) && !record.ceMarkers;
    } catch(e) {record.pass = false;record.error = e.message;}
    results.push(record);
  }
}
fs.writeFileSync(path.join(root,'.research/jsx-stress/results.json'),JSON.stringify(results,null,2)+'\n');
for(const lib of ['Partas','Oxpecker']) {
  const all = results.filter(r=>r.library === lib);
  const names = [...new Set(all.map(r=>r.case))];
  const failed = names.filter(n => all.some(r=>r.case === n && !r.pass));
  console.log(`${lib}: ${all.filter(r=>r.pass).length}/${all.length} scenarios pass; ${names.length-failed.length}/${names.length} cases pass`);
  for(const name of failed) console.log(`  ${name}: ${all.filter(r=>r.case === name && !r.pass).map(r=>r.error ?? `${r.actual}; logs=${JSON.stringify(r.logs)}`).join(' | ')}`);
}
