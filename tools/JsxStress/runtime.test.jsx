import {describe,it,expect} from 'vitest';
import {render} from '@solidjs/web';
import {IfTwoElements} from '../../.research/jsx-stress/Partas/IfTwoElements.fs.jsx';
import {LetSibling} from '../../.research/jsx-stress/Partas/LetSibling.fs.jsx';
import {AttributeArrayComprehension as PartasArray} from '../../.research/jsx-stress/Partas/AttributeArrayComprehension.fs.jsx';
import {AttributeArrayComprehension as OxpeckerArray} from '../../.research/jsx-stress/Oxpecker/AttributeArrayComprehension.fs.jsx';
import {MatchOnly as PartasMatch,Kind as PartasKind} from '../../.research/jsx-stress/Partas/MatchOnly.fs.jsx';
import {MatchOnly as OxpeckerMatch,Kind as OxpeckerKind} from '../../.research/jsx-stress/Oxpecker/MatchOnly.fs.jsx';
import {StatementBefore as PartasStatement} from '../../.research/jsx-stress/Partas/StatementBefore.fs.jsx';
import {StatementBefore as OxpeckerStatement} from '../../.research/jsx-stress/Oxpecker/StatementBefore.fs.jsx';
import {IfOnly} from '../../.research/jsx-stress/Oxpecker/IfOnly.fs.jsx';

function mount(fn) {
  const host = document.createElement('div');
  document.body.append(host);
  let dispose;
  try { dispose = render(fn,host); return host.innerHTML; }
  finally { dispose?.(); host.remove(); }
}
describe('differential output under the Solid 2 DOM compiler',()=>{
  it('hand-written JSX preserves both branch children',()=>{
    expect(mount(()=><div><span>one</span><b>two</b></div>)).toBe('<div><span>one</span><b>two</b></div>');
  });
  // BUG: Partas lowers multiple branch children to a comma expression.
  it.fails('Partas preserves both conditional branch children',()=>{
    expect(mount(()=>IfTwoElements(true))).toBe('<div><span>one</span><b>two</b></div>');
  });
  // BUG: Partas drops the bound child before a following sibling.
  it.fails('Partas retains a bound child and following sibling',()=>{
    expect(mount(()=>LetSibling('hello'))).toBe('<div>head<span>HELLO</span>tail</div>');
  });
  // BUG: Partas inserts an empty array entry in place of omission.
  it.fails('Partas omits a false array-comprehension branch',()=>{
    expect(mount(()=>PartasArray(false))).toBe('<div title="head:tail">body</div>');
  });
  it('Oxpecker omits a false array-comprehension branch',()=>{
    expect(mount(()=>OxpeckerArray(false))).toBe('<div title="head:tail">body</div>');
  });
  it('Oxpecker preserves a matched union payload',()=>{
    expect(mount(()=>OxpeckerMatch(new OxpeckerKind(1,['hello'])))).toBe('<div><b>hello</b></div>');
  });
  // BUG: Partas substitutes the union object for its string field.
  it.fails('Partas preserves a matched union payload',()=>{
    expect(mount(()=>PartasMatch(new PartasKind(1,['hello'])))).toBe('<div><b>hello</b></div>');
  });
  it('Partas preserves a statement and following child',()=>{
    const p=[];
    expect(mount(()=>PartasStatement(x=>{p.push(x);}))).toBe('<div><span>yes</span></div>');
    expect(p).toEqual(['before']);
  });
  // BUG: Oxpecker discards the effect and child after it.
  it.fails('Oxpecker preserves a statement and following child',()=>{
    const o=[];
    const html = mount(()=>OxpeckerStatement(x=>{o.push(x);}));
    expect({html,logs:o}).toEqual({html:'<div><span>yes</span></div>',logs:['before']});
  });
  it('Oxpecker false-branch CE husk currently renders empty',()=>{
    expect(mount(()=>IfOnly(false))).toBe('<div></div>');
  });
});
