export const trimStart = (str: string, charCode: number) => {
	let start = 0;

	if (str.charCodeAt(start) !== charCode) {
		return str;
	}

	while (str.charCodeAt(start) === charCode) {
		++start;
	}
	return str.substring(start);
};

export const trimEnd = (str: string, charCode: number) => {
	let end = str.length - 1;

	if (str.charCodeAt(end) !== charCode) {
		return str;
	}

	while (str.charCodeAt(end) === charCode) {
		--end;
	}
	return str.substring(0, end + 1);
};

export const trim = (str: string, charCode: number) => {
	return trimEnd(trimStart(str, charCode), charCode);
};

export const indexOf = (str: string, charCode: number, skip = 0) => {
	for (let i = 0, size = str.length; i < size; i++) {
		if (str.charCodeAt(i) === charCode) {
			if (skip === 0) {
				return i;
			}
			--skip;
		}
	}
	return -1;
};
