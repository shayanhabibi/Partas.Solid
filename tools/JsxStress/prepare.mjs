import fs from 'node:fs';
import path from 'node:path';

const root = path.resolve(import.meta.dirname, '../..');
const base = fs.readFileSync(path.join(import.meta.dirname, 'cases.fs.template'), 'utf8');
const extraCases = fs.readFileSync(path.join(import.meta.dirname, 'extra.fs.template'), 'utf8').split('[<SolidComponent>]').slice(1);
const template = base + '\n[<SolidComponent>]' + extraCases.join('[<SolidComponent>]');
for (const lib of ['Partas', 'Oxpecker']) {
  const dir = path.join(root, '.research', 'jsx-stress', lib);
  fs.mkdirSync(dir, {recursive: true});
  const library = lib === 'Partas'
    ? path.join(root, 'src/Partas.Solid/Partas.Solid.fsproj')
    : path.join(root, '.research/Oxpecker/src/Oxpecker.Solid/Oxpecker.Solid.fsproj');
  const parts = template.replaceAll('__LIB__', lib).split('[<SolidComponent>]');
  const header = parts.shift();
  const files = parts.map(part => {
    const name = /let (\w+)/.exec(part)[1];
    fs.writeFileSync(path.join(dir, `${name}.fs`), header.replace('module Stress', `module Stress.${name}`) + '[<SolidComponent>]' + part);
    return name + '.fs';
  });
  fs.writeFileSync(path.join(dir, 'Stress.fsproj'), `<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
  <ItemGroup>${files.map(f => `<Compile Include="${f}" />`).join('')}</ItemGroup>
  <ItemGroup><ProjectReference Include="${library.replaceAll('\\', '/')}" /></ItemGroup>
</Project>\n`);
}
