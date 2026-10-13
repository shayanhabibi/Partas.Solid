// Partas.Nacara.Plugins.Solid: mounts each example into the placeholder the build left for it.
//
// A page's examples are one bundle beside this file, named by the key on the placeholder, and
// export mount(cell, element). A classic deferred script, so document.currentScript still says
// where it was served from, and the placeholders are all parsed by the time it runs.
(() => {
    const base = document.currentScript ? document.currentScript.src : location.href;
    const placeholders = document.querySelectorAll("[data-partas-cell]");
    if (placeholders.length === 0) return;

    const style = document.createElement("style");
    style.textContent = `
.partas-solid { margin-block: 1rem; padding: 1rem; border: 1px solid color-mix(in oklab, currentColor 20%, transparent); border-radius: .5rem; }
.partas-solid:empty::before { content: "Loading example..."; opacity: .6; }
.partas-solid.partas-solid--inline { display: contents; margin: 0; padding: 0; border: 0; }
.partas-solid.partas-solid--inline:empty::before { content: none; }
.partas-solid__error { margin: 0; white-space: pre-wrap; color: #dc2626; font-size: .875em; }
`;
    document.head.append(style);

    const fail = (element, error) => {
        console.error(error);
        // Inline, the page reads on around it, so a line of code says enough; the console has the rest.
        if (element.classList.contains("partas-solid--inline")) {
            const note = document.createElement("code");
            note.className = "partas-solid__error";
            note.textContent = `${error && error.message ? error.message : error}`;
            element.replaceChildren(note);
            return;
        }
        const box = document.createElement("pre");
        box.className = "partas-solid__error";
        box.textContent = `This example did not run.\n${error && error.stack ? error.stack : error}`;
        element.replaceChildren(box);
    };

    const pages = new Map();
    for (const element of placeholders) {
        const page = element.dataset.partasPage;
        if (!pages.has(page)) pages.set(page, []);
        pages.get(page).push(element);
    }

    for (const [page, elements] of pages) {
        import(new URL(`${page}.js`, base).href).then(
            module => {
                for (const element of elements) {
                    try {
                        element.replaceChildren();
                        module.mount(element.dataset.partasCell, element);
                    } catch (error) {
                        fail(element, error);
                    }
                }
            },
            error => elements.forEach(element => fail(element, error)));
    }
})();
