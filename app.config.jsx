import { defineConfig } from "@solidjs/start/config";
import devtools from "solid-devtools/vite";
import { withSolidBase } from "@kobalte/solidbase/config";
import tailwindcss from "@tailwindcss/vite";

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
                }),
                // tailwindcss()
            ],
            server: {
                watch: {
                    ignore: [
                        "**/*.fs",
                        "**/*.fsx"
                    ]
                }
            },
        },
        // Vinxi/Nitro/SolidStart
        server: {
            ssr: true,
            prerender: {
                crawlLinks: true
            },
            preset: "vercel",
        }
    },
    // solidbase config
    {
        title: "Partas",
        titleTemplate: ":title - Partas.Solid",
        description: "Built using the Partas.SolidStart SolidBase template",
        lang: "en",
        themeConfig: {
            socialLinks: {
                github: "https://github.com/shayanhabibi/Partas.Solid",
                discord: "https://discord.gg/YemTHH6VM6"
            },
            nav: [
                {
                    text: "Partas.Solid",
                    link: "/partas-solid"
                },
                {
                    text: "Ecosystem",
                    link: "/ecosystem"
                }
            ],
            sidebar: {
                "/": [],
                "/partas-solid" : {
                    items: [
                        {
                            title: "About",
                            collapsed: false,
                            items: [
                                {
                                    title: "Home",
                                    link: "/"
                                },
                                {
                                    title: "About",
                                    link: "/about"
                                },
                                {
                                    title: "Oxpecker Fork",
                                    link: "/about/oxpecker-fork"
                                },
                                {
                                    title: "JSX Output",
                                    link: "/about/jsx-output"
                                }
                            ]
                        },
                        {
                            title: "Overview",
                            collapsed: false,
                            items: [
                                {
                                    title: "Getting Started",
                                    link: "/installation"
                                },
                                {
                                    title: "Usage Overview",
                                    link: "/overview"
                                }
                            ]
                        },
                        {
                            title: "Documentation",
                            collapsed: true,
                            items: [
                                {
                                    title: "Oxpecker DSL",
                                    link: "/oxpecker-dsl",
                                },
                                {
                                    title: "SolidComponent",
                                    link: "/solid-component-attribute"
                                },
                                {
                                    title: "SolidTypeComponent",
                                    link: "/solid-type-attribute"
                                },
                                {
                                    title: "Building the DOM",
                                    link: "/building-the-dom",
                                },
                                {
                                    title: "Tag Interfaces",
                                    link: "/tag-interfaces",
                                },
                                {
                                    title: "Extension Methods",
                                    link: "/extension-methods"
                                },
                                {
                                    title: "Solid-JS",
                                    link: "/solid-js",
                                },
                                {
                                    title: "Aria Attributes",
                                    link: "/aria-attributes"
                                },
                                {
                                    title: "Svg Elements",
                                    link: "/svg-elements"
                                },
                                {
                                    title: "Solid/Router",
                                    link: "/solid-router"
                                },
                                {
                                    title: "Solid/Meta",
                                    link: "/solid-meta"
                                },
                                {
                                    title: "Solid/Start",
                                    link: "/solid-start"
                                },
                                {
                                    title: "Tag Values",
                                    link: "/tag-values"
                                },
                                {
                                    title: "API Differences",
                                    link: "/api-differences"
                                },
                                {
                                    title: "Experimental",
                                    link: "/experimental"
                                }
                            ]
                        },
                        {
                            title: "Examples",
                            collapsed: true,
                            items: [
                                {
                                    title: "Data Tables",
                                    items: [
                                        {
                                            title: "Getting Started",
                                            link: "/examples/data-table"
                                        },
                                        {
                                            title: "Utilities",
                                            link: "/examples/data-table/utils"
                                        },
                                        {
                                            title: "Table Component",
                                            link: "/examples/data-table/table-component"
                                        },
                                        {
                                            title: "DataTable Component",
                                            link: "/examples/data-table/datatable-component"
                                        },
                                        {
                                            title: "Column Definitions",
                                            link: "/examples/data-table/column-defs"
                                        },
                                        {
                                            title: "Render the Table",
                                            link: "/examples/data-table/table-render"
                                        },
                                        {
                                            title: "Extending - Selectable Rows",
                                            link: "/examples/data-table/selectable-rows"
                                        },
                                    ]
                                }
                            ]
                        },
                        {
                            title: "Troubleshooting",
                            collapsed: true,
                            items: [
                                {
                                    title: "Attribute Flags",
                                    link: "/attribute-flags"
                                },
                                {
                                    title: "Common Issues",
                                    link: "/common-issues"
                                },
                                {
                                    title: "Submit Issues",
                                    link: "/submit-issues"
                                }
                            ]
                        },
                        {
                            title: "Dev",
                            collapsed: true,
                            items: [
                                {
                                    title: "File Structure",
                                    link: "/dev/file-structure"
                                },
                                {
                                    title: "Types",
                                    link: "/dev/types",
                                },
                                {
                                    title: "Utilities",
                                    link: "/dev/utilities",
                                    
                                },
                                {
                                    title: "Spec",
                                    link: "/dev/spec",
                                },
                                {
                                    title: "Patterns",
                                    link: "/dev/patterns",
                                    status: "next"
                                },
                                {
                                    title: "Transformation",
                                    link: "/dev/transformation",
                                    status: "next"
                                }
                            ]
                        },
                        {
                            title: "Contributing",
                            link: "/contributing"
                        },
                    ],
                },
                "/ecosystem" : {
                        items: [
                            {
                                title: "Templates",
                                link: "/../partas-solid/installation#partassolidstart",
                                status: "new"
                            },
                            {
                                title: "Polymorphism",
                                link: "/polymorphism"
                            },
                            {
                                title: "Making Bindings",
                                link: "/bindings"
                            },
                            {
                                title: "Component Libraries",
                                collapsed: true,
                                items: [
                                    {
                                        title: "Partas.Solid.UI",
                                        link: "/partas-solid-ui",
                                        status: "next"
                                    }
                                ]
                            },
                            {
                                title: "Primitives",
                                collapsed: true,
                                items: [
                                    {
                                        title: "Solid Primitives",
                                        link: "/primitives/"
                                    },
                                    {
                                        title: "Mouse",
                                        link: "/primitives/mouse"
                                    },
                                    {
                                        title: "ActiveElement",
                                        link: "/primitives/active-element"
                                    },
                                    {
                                        title: "AutoFocus",
                                        link: "/primitives/autofocus"
                                    },
                                    {
                                        title: "Bounds",
                                        link: "/primitives/bounds"
                                    },
                                    {
                                        title: "BroadcastChannel",
                                        link: "/primitives/broadcast-channel"
                                    },
                                    {
                                        title: "Clipboard",
                                        link: "/primitives/clipboard",
                                        status: "updated"
                                    },
                                    {
                                        title: "Devices",
                                        link: "/primitives/devices",
                                        status: "new"
                                    },
                                    {
                                        title: "Event Listener",
                                        link: "/primitives/event-listener",
                                        status: "new"
                                    },
                                    {
                                        title: "EventBus",
                                        link: "/primitives/event-bus"
                                    },
                                    {
                                        title: "Idle",
                                        link: "/primitives/idle"
                                    },
                                    {
                                        title: "Keyboard",
                                        link: "/primitives/keyboard"
                                    },
                                    {
                                        title: "Media",
                                        link: "/primitives/media"
                                    },
                                    {
                                        title: "RAF",
                                        link: "/primitives/raf"
                                    },
                                    {
                                        title: "Scheduled",
                                        link: "/primitives/scheduled"
                                    },
                                    {
                                        title: "Scroll",
                                        link: "/primitives/scroll"
                                    },
                                    {
                                        title: "Spring",
                                        link: "/primitives/spring"
                                    },
                                    {
                                        title: "Timer",
                                        link: "/primitives/timer"
                                    },
                                    {
                                        title: "Trigger",
                                        link: "/primitives/trigger"
                                    },
                                    {
                                        title: "Tween",
                                        link: "/primitives/tween"
                                    },
                                    // {
                                    //     title: "WebSocket",
                                    //     link: "/primitives/websocket"
                                    // }
                                ]
                            },
                            {
                                title: "Bindings",
                                collapsed: true,
                                items: [
                                    {
                                        title: "Kobalte",
                                        link: "/kobalte",
                                        status: {
                                            text: "0.3.0",
                                            color: "purple"
                                        }
                                    },
                                    {
                                        title: "Motion",
                                        link: "/motion",
                                        status: {
                                            text: "0.2.1",
                                            color: "purple"
                                        }
                                    },
                                    {
                                        title: "ApexCharts",
                                        link: "/apexcharts",
                                        status: {
                                            text: "0.2.0",
                                            color: "purple"
                                        }
                                    },
                                    {
                                        title: "Cmdk",
                                        link: "/cmdk",
                                        status: {
                                            text: "0.2.0",
                                            color: "purple"
                                        }
                                    },
                                    {
                                        title: "Lucide",
                                        link: "/lucide",
                                        status: {
                                            text: "0.514.0",
                                            color: "purple"
                                        }
                                    },
                                    {
                                        title: "ModularForms",
                                        link: "/modular-forms",
                                        status: {
                                            text: "0.2.0",
                                            color: "purple"
                                        }
                                    },
                                    {
                                        title: "NeoDrag",
                                        link: "/neodrag",
                                        status: {
                                            text: "0.2.0",
                                            color: "purple"
                                        }
                                    },
                                    {
                                        title: "TanStack.Table",
                                        link: "/tan-stack-table",
                                        status: {
                                            text: "0.2.0",
                                            color: "purple"
                                        }
                                    },
                                    {
                                        title: "Storybook",
                                        link: "/storybook",
                                        status: {
                                            text: "0.1.2",
                                            color: "purple"
                                        }
                                    }
                                ]
                            }
                        ]
                }
            }
        }
    }
));
