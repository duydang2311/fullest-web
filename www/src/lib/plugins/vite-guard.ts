import MagicString from 'magic-string';
import { parse } from 'oxc-parser';
import { Visitor, type Plugin } from 'vite';

export function guardPlugin() {
    let prod = false;
    return {
        name: 'vite-guard',
        configResolved: (config) => {
            prod = config.mode === 'production';
        },
        async transform(code: string, id: string) {
            if (!id.endsWith('.ts') && !id.endsWith('.js') && !id.endsWith('.svelte')) return;
            const result = await parse(id, code);
            const s = new MagicString(code);
            const visitor = new Visitor({
                CallExpression(decl) {
                    if (decl.callee.type === 'Identifier' && decl.callee.name === 'guardNull') {
                        if (prod) {
                            s.remove(decl.start, decl.end);
                            return;
                        }
                        const arg = decl.arguments[0];
                        s.appendLeft(decl.end - 1, `, "${code.slice(arg.start, arg.end)}"`);
                    }
                },
            });
            visitor.visit(result.program);
            return {
                code: s.toString(),
                map: s.generateMap({ hires: true }),
            };
        },
    } satisfies Plugin;
}
