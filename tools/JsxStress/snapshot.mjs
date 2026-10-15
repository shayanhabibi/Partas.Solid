import fs from 'node:fs';
import path from 'node:path';
const root = path.resolve(import.meta.dirname,'../..');
const work = path.join(root,'.research/jsx-stress');
const results = JSON.parse(fs.readFileSync(path.join(work,'results.json'),'utf8'));
const cases = [...new Set(results.map(r=>r.case))].sort();
const passes = (lib,name) => results.filter(r=>r.library===lib && r.case===name).every(r=>r.pass);
const groups = {both:[],partasOnly:[],oxpeckerOnly:[],neither:[]};
for(const name of cases) {
  const p = passes('Partas',name), o = passes('Oxpecker',name);
  groups[p ? (o ? 'both' : 'partasOnly') : (o ? 'oxpeckerOnly' : 'neither')].push(name);
}
const emitted = {};
for(const lib of ['Partas','Oxpecker']) {
  emitted[lib] = {};
  for(const name of cases) {
    const file = path.join(work,lib,name+'.fs.jsx');
    emitted[lib][name] = fs.existsSync(file) ? fs.readFileSync(file,'utf8') : null;
  }
}
const evidence = {
  metadata:JSON.parse(fs.readFileSync(path.join(work,'metadata.json'),'utf8')),
  interpretation:'Targeted corpus; pass requires correct neutral tree/log semantics and no detected CE leakage. This is a survey, not a representative compatibility rate.',
  groups, results, emitted,
  compileLogs:Object.fromEntries(['Partas','Oxpecker'].map(lib=>[lib,fs.readFileSync(path.join(work,lib+'.compile.log'),'utf8')]))
};
let serialized = JSON.stringify(evidence,null,2)+'\n';
for(const spelling of [root,root.replaceAll('\\','/')]) serialized = serialized.replaceAll(JSON.stringify(spelling).slice(1,-1),'<repo>');
fs.writeFileSync(path.join(root,'docs/research/solid-jsx-stress-evidence.json'),serialized);
console.log(Object.fromEntries(Object.entries(groups).map(([key,value])=>[key,value.length])));
