import { createContext } from 'svelte';
import type { HttpClient } from '../services/http_client';

export interface Runtime {
    http: HttpClient;
}

const [useRuntime, setRuntime] = createContext<Runtime>();

export { useRuntime, setRuntime };
