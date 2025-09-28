import clsx from 'clsx';

type ButtonStyles = {
	variant?: 'base' | 'primary';
	outlined?: boolean;
} & EitherOr<{ filled?: boolean }, { ghost?: boolean }>;

export const button = ({ variant = 'base', filled, ghost, outlined }: ButtonStyles = {}) => {
	return clsx(
		'c-button',
		`c-button--${variant}`,
		filled && 'c-button--filled',
		ghost && 'c-button--ghost',
		outlined && 'c-button--outlined'
	);
};

export const input = () => {
	return 'c-input';
};

export const label = () => {
	return 'c-label';
};

export const field = () => {
	return 'c-field';
};
