import {createRequire} from 'node:module';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
const runtime = path.resolve(import.meta.dirname,'../../tests/Partas.Solid.Tests.Runtime');
const require = createRequire(path.join(runtime,'package.json'));
const pluginModule = await import(pathToFileURL(require.resolve('@solidjs/vite-plugin')).href);
const solid = pluginModule.default?.default ?? pluginModule.default;
const root = path.resolve(import.meta.dirname,'../..');
export default {
  root,
  cacheDir:path.join(root,'.research/jsx-stress/vite-cache'),
  plugins:[solid({include:['**/*.jsx'],exclude:['**/fable_modules/**'],dev:true})],
  resolve:{
    conditions:['browser','development'],
    alias:[
      {find:'solid-js/internal',replacement:path.join(runtime,'node_modules/solid-js/dist/internal.js')},
      {find:'solid-js/refresh',replacement:path.join(runtime,'node_modules/solid-js/dist/refresh.dev.js')},
      {find:/^solid-js$/,replacement:path.join(runtime,'node_modules/solid-js/dist/solid.dev.js')},
      {find:/^@solidjs\/web$/,replacement:path.join(runtime,'node_modules/@solidjs/web/dist/web.dev.js')}
    ]
  },
  test:{environment:'jsdom',include:['tools/JsxStress/runtime.test.jsx'],server:{deps:{inline:[/solid-js/,/@solidjs\//]}}}
};
