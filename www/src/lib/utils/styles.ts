import clsx from 'clsx';

export const button = ({
	variant = 'base',
	filled,
	outlined,
}: {
	variant: 'base' | 'primary';
	filled?: boolean;
	outlined?: boolean;
}) => {
	return clsx(
		'c-button',
		`c-button--${variant}`,
		filled && 'c-button--filled',
		outlined && 'c-button--outlined'
	);
};
