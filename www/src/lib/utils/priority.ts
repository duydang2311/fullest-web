import {
    IconPriorityHigh,
    IconPriorityLow,
    IconPriorityMedium,
    IconPriorityNone,
    IconPriorityUrgent,
} from '../components/icons';
import { PriorityCategory, type Priority } from '../models/priority';

export function getPriorityIcon(priority?: Pick<Priority, 'category'> | null) {
    if (!priority) {
        return IconPriorityNone;
    }
    switch (priority.category) {
        case PriorityCategory.Low:
            return IconPriorityLow;
        case PriorityCategory.Medium:
            return IconPriorityMedium;
        case PriorityCategory.High:
            return IconPriorityHigh;
        case PriorityCategory.Urgent:
            return IconPriorityUrgent;
    }
}
