import { defineConfig } from "@solidjs/start/config";
import devtools from "solid-devtools/vite";
import { withSolidBase } from "@kobalte/solidbase/config";

export default defineConfig(withSolidBase(
    {
        // Change to project name as required
        appRoot: "./Partas.Solid",
        routeDir: "./routes",
        // Recommended to use .fs.jsx extension so that fable clean will not
        // wipe out solid-start files
        extensions: [ 'js', 'jsx' , 'ts', 'tsx', 'fs.jsx', 'mdx', 'md' ],
        // Vite config
        vite: {
            plugins:[
                devtools({
                    autoname: true
                })
            ],
            server: {
                watch: {
                    ignore: [
                        "**/*.fs",
                        "**/*.fsx"
                    ]
                }
            }
        },
        // Vinxi/Nitro/SolidStart
        server: {
            prerender: {
                crawlLinks: true
            }
            // preset: "vercel"
        }
    },
    // solidbase config
    {
        title: "Partas.Solid",
        titleTemplate: ":title - Partas.Solid",
        description: "Fully featured, fully customisable static site generation for SolidStart",
        themeConfig: {
            sidebar: {
                "/" : {
                    items: [
                        {
                            title: "Overview",
                            collapsed: false,
                            items: [
                                {
                                    title: "Home",
                                    link: "/"
                                },
                                {
                                    title: "About",
                                    link: "/about"
                                }
                            ]
                        }
                    ]
                }
            }
        }
    }
));
