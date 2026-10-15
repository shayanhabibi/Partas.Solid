import fs from 'node:fs';
import path from 'node:path';
import {spawnSync} from 'node:child_process';
import './prepare.mjs';
const root = path.resolve(import.meta.dirname,'../..');
const work = path.join(root,'.research/jsx-stress');
const metadata = {date:new Date().toISOString(),node:process.version,compiles:{}};
function run(args) {
  return spawnSync('rtk',['proxy',...args],{cwd:root,encoding:'utf8',maxBuffer:20*1024*1024});
}
metadata.fable = run(['.research/tools/fable','--version']).stdout.trim();
metadata.partas = run(['git','rev-parse','HEAD']).stdout.trim();
metadata.oxpecker = run(['git','-C','.research/Oxpecker','rev-parse','HEAD']).stdout.trim();
for(const lib of ['Partas','Oxpecker']) {
  const dir = path.join(work,lib);
  // Remove only emitted case files, so failed cases cannot reuse stale output.
  for(const file of fs.readdirSync(dir).filter(f=>f.endsWith('.fs.jsx'))) fs.unlinkSync(path.join(dir,file));
  const result = run(['.research/tools/fable',`.research/jsx-stress/${lib}/Stress.fsproj`,'--exclude',`${lib}.Solid.FablePlugin`,'--noCache','-e','.fs.jsx','--optimize','--typedArrays','false']);
  fs.writeFileSync(path.join(work,`${lib}.compile.log`),result.stdout + result.stderr);
  metadata.compiles[lib] = {exitCode:result.status, error:result.error?.message};
  console.log(`${lib} Fable exit: ${result.status}; log: .research/jsx-stress/${lib}.compile.log`);
}
fs.writeFileSync(path.join(work,'metadata.json'),JSON.stringify(metadata,null,2)+'\n');
await import('./check.mjs');
