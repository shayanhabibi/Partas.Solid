import fs from 'node:fs';
import path from 'node:path';
import {spawnSync} from 'node:child_process';
const root = path.resolve(import.meta.dirname,'../..');
const dir = path.join(root,'.research/ast-triage');
fs.mkdirSync(dir,{recursive:true});
fs.writeFileSync(path.join(dir,'Probes.fs'),fs.readFileSync(path.join(import.meta.dirname,'triage.fs.template')));
fs.writeFileSync(path.join(dir,'Helpers.fs'),`module AstTriageHelpers
open Partas.Solid
let OpaqueHelper () = span() { "opaque" }
[<SolidComponent>]
let AnnotatedHelper () = span() { "annotated" }
`);
fs.writeFileSync(path.join(dir,'Probes.fsproj'),`<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
<ItemGroup><Compile Include="Helpers.fs" /><Compile Include="Probes.fs" /><ProjectReference Include="../../src/Partas.Solid/Partas.Solid.fsproj" /></ItemGroup>
</Project>`);
const args = ['proxy','.research/tools/fable','.research/ast-triage/Probes.fsproj','--exclude','Partas.Solid.FablePlugin','--noCache','-e','.fs.jsx','--optimize','--typedArrays','false'];
const result = spawnSync('rtk',args,{cwd:root,encoding:'utf8',maxBuffer:30*1024*1024});
fs.writeFileSync(path.join(dir,'compile.log'),result.stdout + result.stderr);
const dumps = [...result.stdout.matchAll(/START MEMBER DECL!!!\s*([\s\S]*?)END MEMBER DECL!!!/g)].map(m=>m[1]);
const names = [...fs.readFileSync(path.join(dir,'Probes.fs'),'utf8').matchAll(/\[<SolidComponent\(ComponentFlag.DebugMode[^\n]*\nlet (\w+)/g)].map(m=>m[1]);
if(dumps.length !== names.length) throw new Error('Debug dump count does not match probe count');
for(let index=0;index<dumps.length;index++) fs.writeFileSync(path.join(dir,names[index]+'.ast.txt'),dumps[index]);
const excerpts = Object.fromEntries(names.map((name,index)=>{
  const lines = dumps[index].split(/\r?\n/);
  const interesting = lines.flatMap((line,i)=>/UnionField|FieldIndex|Name = "PARTAS_(?:FIRST|SECOND|ELEMENT|BUILDER|YIELD)|Selector = "(?:empty|singleton)"|Name = "produce"/.test(line) ? [i] : []);
  return [name,interesting.slice(0,24).map(i=>({line:i+1,text:lines.slice(Math.max(0,i-2),i+12).join('\n')}))];
}));
const evidence = {renderingContract:'Partas retains F#-discarded expressions in JSX children. DiscardedCall and YieldedCall emitting the same child expression is expected, not a bug.',fable:'5.21.0',exitCode:result.status,inputs:fs.readFileSync(path.join(dir,'Probes.fs'),'utf8'),excerpts,
  output:fs.readFileSync(path.join(dir,'Probes.fs.jsx'),'utf8'),helpers:fs.readFileSync(path.join(dir,'Helpers.fs.jsx'),'utf8'),
  note:'Pre-transform DebugMode excerpts, including nearby metadata; full AST dumps are regenerated under .research/ast-triage. Excerpts are not a complete-AST equivalence test.'};
fs.writeFileSync(path.join(dir,'evidence.json'),JSON.stringify(evidence,null,2).replaceAll(root.replaceAll('\\','/'),'<repo>')+'\n');
console.log({exitCode:result.status,dumps:dumps.length,names,log:'.research/ast-triage/compile.log'});
process.exitCode = result.status ?? 1;
