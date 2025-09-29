import clsx from 'clsx';

type ButtonStyles = {
	variant?: 'base' | 'primary';
	outlined?: boolean;
	icon?: boolean;
} & EitherOr<{ filled?: boolean }, { ghost?: boolean }>;

export const button = ({ variant = 'base', filled, ghost, outlined, icon }: ButtonStyles = {}) => {
	return clsx(
		'c-button',
		`c-button--${variant}`,
		filled && 'c-button--filled',
		ghost && 'c-button--ghost',
		outlined && 'c-button--outlined',
		icon && 'c-button--icon'
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

export const menu = ({ part }: { part: 'content' | 'item' }) => {
	return `c-menu--${part}`;
};
