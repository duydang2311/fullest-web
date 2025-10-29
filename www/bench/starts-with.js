import { bench, run } from 'mitata';

const PROXY_PREFIX = '/_/api';
const PROXY_PREFIX_LENGTH = PROXY_PREFIX.length;

const CODE_SLASH = '/'.charCodeAt(0);
const CODE_UNDERSCORE = '_'.charCodeAt(0);
const CODE_A = 'a'.charCodeAt(0);
const CODE_P = 'p'.charCodeAt(0);
const CODE_I = 'i'.charCodeAt(0);

const samples = [
	'/_/api',
	'/_/api/projects',
	'/_/apix',
	'/api/upload',
	'/dashboard',
	'/api/bff/upload/sign',
];

// --- Benchmark functions ---

function charCodeCheck(pathname) {
	return (
		pathname.length >= PROXY_PREFIX_LENGTH &&
		pathname.charCodeAt(0) === CODE_SLASH &&
		pathname.charCodeAt(1) === CODE_UNDERSCORE &&
		pathname.charCodeAt(2) === CODE_SLASH &&
		pathname.charCodeAt(3) === CODE_A &&
		pathname.charCodeAt(4) === CODE_P &&
		pathname.charCodeAt(5) === CODE_I
	);
}

function startsWithCheck(pathname) {
	return pathname.startsWith(PROXY_PREFIX);
}

// --- Mitata Benchmarks ---

for (const sample of samples) {
	bench(`charCodeCheck("${sample}")`, () => {
		charCodeCheck(sample);
	});
	bench(`startsWithCheck("${sample}")`, () => {
		startsWithCheck(sample);
	});
}

await run();
