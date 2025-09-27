import { run, bench } from 'mitata';

const str = '/(auth-optional)/a/b/c/d/e/f/g/h/i/j/k/l/m/n/o/p/q/r/s/t/u/v/w/x/y/z';
const length = '/(auth-optional)'.length;

bench('string.includes', () => str.includes('(auth-optional)')).gc();
bench('string.includes substring', () => str.substring(0, length).includes('(auth-optional)')).gc();

await run();
