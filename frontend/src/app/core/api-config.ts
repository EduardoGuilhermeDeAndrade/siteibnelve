import { environment } from '../../environments/environment';

/** URL base da API — vem de `environments/environment.ts`, trocado por build (`fileReplacements`) na build de produção. */
export const API_BASE_URL = environment.apiBaseUrl;
