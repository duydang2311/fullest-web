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
            if (
                (!id.endsWith('.js') && !id.endsWith('.ts') && !id.endsWith('.svelte')) ||
                !code.includes('guard')
            ) {
                return null;
            }

            let s: MagicString | null = null;
            const result = await parse(id, code);
            const visitor = new Visitor({
                CallExpression(decl) {
                    if (decl.callee.type === 'Identifier') {
                        if (decl.callee.name === 'guardNull') {
                            s ??= new MagicString(code);
                            if (prod) {
                                s.remove(decl.start, decl.end);
                                return;
                            }
                            const arg = decl.arguments[0];
                            s.appendLeft(decl.end - 1, `, "${code.slice(arg.start, arg.end)}"`);
                        } else if (decl.callee.name === 'guard' && prod) {
                            s ??= new MagicString(code);
                            s.remove(decl.start, decl.end);
                            return;
                        }
                    }
                },
            });
            visitor.visit(result.program);
            return s
                ? {
                      code: (s as MagicString).toString(),
                      map: (s as MagicString).generateMap(),
                  }
                : null;
        },
    } satisfies Plugin;
}
