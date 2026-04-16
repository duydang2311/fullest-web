import {
    IconCheckCircle,
    IconCircleDashedOutline,
    IconCircleHalf,
    IconCircleOutline,
    IconPauseCircle,
    IconQuestion,
    IconXCircle,
} from '../components/icons';
import { StatusCategory, type Status } from '../models/status';

export function getStatusColor(status: Pick<Status, 'color' | 'category'>) {
    return status.color || getStatusCategoryColor(status.category);
}

export function getStatusCategoryColor(category: StatusCategory) {
    // for tailwind to pick up
    // --color-status-proposed
    // --color-status-ready
    // --color-status-active
    // --color-status-paused
    // --color-status-completed
    // --color-status-review
    // --color-status-canceled
    // --color-status-proposed
    return `var(--color-status-${category})`;
}

export function getStatusIcon(status?: Pick<Status, 'category'> | null) {
    if (!status) {
        return IconCircleDashedOutline;
    }
    switch (status.category) {
        case StatusCategory.Proposed:
            return IconCircleDashedOutline;
        case StatusCategory.Ready:
            return IconCircleOutline;
        case StatusCategory.Active:
            return IconCircleHalf;
        case StatusCategory.Paused:
            return IconPauseCircle;
        case StatusCategory.Review:
            return IconQuestion;
        case StatusCategory.Completed:
            return IconCheckCircle;
        case StatusCategory.Canceled:
            return IconXCircle;
        case StatusCategory.Archived:
            return IconXCircle;
    }
}
