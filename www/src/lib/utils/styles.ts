import clsx from 'clsx';

type ButtonStyles = {
    variant?: 'surface' | 'base' | 'primary' | 'negative' | 'secondary';
    outlined?: boolean;
    icon?: boolean;
    size?: 'md' | 'sm';
} & EitherOr<{ filled?: boolean }, { ghost?: boolean }>;

export const button = ({
    variant = 'base',
    filled,
    ghost,
    outlined,
    icon,
    size
}: ButtonStyles = {}) => {
    return clsx(
        'c-button',
        `c-button--${variant}`,
        filled && 'c-button--filled',
        ghost && 'c-button--ghost',
        outlined && 'c-button--outlined',
        icon && 'c-button--icon',
        size && `c-button--${size}`,
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

type MenuStyles = { part: 'content'; variant?: never } | { part: 'item'; variant?: 'negative' };
export const menu = ({ part, variant }: MenuStyles) => {
    return clsx(`c-menu-${part}`, variant != null && `c-menu-item--${variant}`);
};

export const C = {
    button,
    input,
    label,
    field,
    menu,
};
