<script lang="ts">
	const {
		glitchColors = ['#004FB0', '#B07200', '#33261A'],
		glitchSpeed = 100,
		smooth = false,
		characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$&*()-_+=/[]{};:<>.,0123456789',
	}: {
		glitchColors?: string[];
		glitchSpeed?: number;
		smooth?: boolean;
		characters?: string;
	} = $props();
	let letters: {
		char: string;
		color: string;
		targetColor: string;
		colorProgress: number;
	}[] = [];

	let animationRef: number | null = null;
	let grid = { columns: 0, rows: 0 };
	let context: CanvasRenderingContext2D | null = null;
	let lastGlitchTime = Date.now();

	const lettersAndSymbols = Array.from(characters);

	const fontSize = 14;
	const charWidth = 10;
	const charHeight = 20;

	const getRandomChar = () => {
		return lettersAndSymbols[Math.floor(Math.random() * lettersAndSymbols.length)];
	};

	const getRandomColor = () => {
		return glitchColors[Math.floor(Math.random() * glitchColors.length)];
	};

	const hexToRgb = (hex: string) => {
		const shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
		hex = hex.replace(shorthandRegex, (_m, r, g, b) => {
			return r + r + g + g + b + b;
		});

		const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
		return result
			? {
					r: parseInt(result[1], 16),
					g: parseInt(result[2], 16),
					b: parseInt(result[3], 16),
				}
			: null;
	};

	const interpolateColor = (
		start: { r: number; g: number; b: number },
		end: { r: number; g: number; b: number },
		factor: number
	) => {
		const result = {
			r: Math.round(start.r + (end.r - start.r) * factor),
			g: Math.round(start.g + (end.g - start.g) * factor),
			b: Math.round(start.b + (end.b - start.b) * factor),
		};
		return `rgb(${result.r}, ${result.g}, ${result.b})`;
	};

	const calculateGrid = (width: number, height: number) => {
		const columns = Math.ceil(width / charWidth);
		const rows = Math.ceil(height / charHeight);
		return { columns, rows };
	};

	const initializeLetters = (columns: number, rows: number) => {
		grid = { columns, rows };
		const totalLetters = columns * rows;
		letters = Array.from({ length: totalLetters }, () => ({
			char: getRandomChar(),
			color: getRandomColor(),
			targetColor: getRandomColor(),
			colorProgress: 1,
		}));
	};

	const resizeCanvas = (canvas: HTMLCanvasElement) => {
		const parent = canvas.parentElement;
		if (!parent) return;

		const dpr = window.devicePixelRatio || 1;
		const rect = parent.getBoundingClientRect();

		canvas.width = rect.width * dpr;
		canvas.height = rect.height * dpr;

		canvas.style.width = `${rect.width}px`;
		canvas.style.height = `${rect.height}px`;

		if (context) {
			context.setTransform(dpr, 0, 0, dpr, 0, 0);
		}

		const { columns, rows } = calculateGrid(rect.width, rect.height);
		initializeLetters(columns, rows);
		drawLetters(canvas);
	};

	const drawLetters = (canvas: HTMLCanvasElement) => {
		if (!context || letters.length === 0) return;
		const ctx = context;
		const { width, height } = canvas.getBoundingClientRect();
		ctx.clearRect(0, 0, width, height);
		ctx.font = `${fontSize}px Geist Mono`;
		ctx.textBaseline = 'top';

		letters.forEach((letter, index) => {
			const x = (index % grid.columns) * charWidth;
			const y = Math.floor(index / grid.columns) * charHeight;
			ctx.fillStyle = letter.color;
			ctx.fillText(letter.char, x, y);
		});
	};

	const updateLetters = () => {
		if (!letters || letters.length === 0) return;

		const updateCount = Math.max(1, Math.floor(letters.length * 0.05));

		for (let i = 0; i < updateCount; i++) {
			const index = Math.floor(Math.random() * letters.length);
			if (!letters[index]) continue;

			letters[index].char = getRandomChar();
			letters[index].targetColor = getRandomColor();

			if (!smooth) {
				letters[index].color = letters[index].targetColor;
				letters[index].colorProgress = 1;
			} else {
				letters[index].colorProgress = 0;
			}
		}
	};

	const handleSmoothTransitions = (canvas: HTMLCanvasElement) => {
		let needsRedraw = false;
		letters.forEach((letter) => {
			if (letter.colorProgress < 1) {
				letter.colorProgress += 0.05;
				if (letter.colorProgress > 1) letter.colorProgress = 1;

				const startRgb = hexToRgb(letter.color);
				const endRgb = hexToRgb(letter.targetColor);
				if (startRgb && endRgb) {
					letter.color = interpolateColor(startRgb, endRgb, letter.colorProgress);
					needsRedraw = true;
				}
			}
		});

		if (needsRedraw) {
			drawLetters(canvas);
		}
	};

	const animate = (canvas: HTMLCanvasElement) => {
		const now = Date.now();
		if (now - lastGlitchTime >= glitchSpeed) {
			updateLetters();
			drawLetters(canvas);
			lastGlitchTime = now;
		}

		if (smooth) {
			handleSmoothTransitions(canvas);
		}

		animationRef = requestAnimationFrame(() => animate(canvas));
	};
</script>

<div class="fixed inset-0">
	<canvas
		class="block h-full w-full"
		{@attach (node) => {
			context = node.getContext('2d');
			resizeCanvas(node);
			animate(node);

			let resizeTimeout: NodeJS.Timeout;

			const handleResize = () => {
				clearTimeout(resizeTimeout);
				resizeTimeout = setTimeout(() => {
					cancelAnimationFrame(animationRef as number);
					resizeCanvas(node);
					animate(node);
				}, 100);
			};

			window.addEventListener('resize', handleResize);

			return () => {
				cancelAnimationFrame(animationRef!);
				window.removeEventListener('resize', handleResize);
			};
		}}
	>
	</canvas>
</div>
