import { run, bench } from 'mitata';
import { stringify } from 'node:querystring';

bench('build url (URL)', () => {
	let url = new URL('https://example.com');
	url = new URL('/api/a/b/c/v1', url);
	url.searchParams.set('p1', 'p1');
	url.searchParams.set('p2', 'p2');
	url.searchParams.set('p3', 'p3');
	url.searchParams.set('p4', 'p4');
	url.searchParams.set('p5', 'p5');
	url.searchParams.set('p6', 'p6');
	url.searchParams.set('p7', 'p7');
}).gc('inner');
bench('build url (string concat)', () => {
	let url = 'https://example.com';
	url += '/' + 'api' + '/' + 'a/b/c' + '/' + 'v1';
	url += stringify({
		p1: 'p1',
		p2: 'p2',
		p3: 'p3',
		p4: 'p4',
		p5: 'p5',
		p6: 'p6',
		p7: 'p7',
	});
}).gc('inner');

await run();
