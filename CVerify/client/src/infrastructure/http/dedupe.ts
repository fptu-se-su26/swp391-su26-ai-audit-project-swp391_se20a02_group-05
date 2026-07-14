import type { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';

type GetFn = AxiosInstance['get'];

const buildKey = (url: string, config?: AxiosRequestConfig): string =>
  `${url}|${config?.params ? JSON.stringify(config.params) : ''}`;

/**
 * Collapses identical GET requests that are in flight at the same time into a
 * single network call. Several components mount together and each fetch the
 * same endpoint (the settings page alone issued /auth/providers three times),
 * which pushed a single page load past nginx's request burst limit and made it
 * answer some requests with 503.
 *
 * Only concurrent requests share a promise — nothing is cached once a request
 * settles, so callers still see fresh data on every new render pass.
 *
 * Browser-only: on the server a shared map would let concurrent SSR renders for
 * different users receive each other's responses.
 */
export function installGetDeduplication(client: AxiosInstance): void {
  if (typeof window === 'undefined') return;

  const inFlight = new Map<string, Promise<AxiosResponse>>();
  const originalGet = client.get.bind(client) as GetFn;

  const dedupedGet = (url: string, config?: AxiosRequestConfig) => {
    // A shared promise cannot honour per-caller cancellation: aborting one
    // caller would reject every other caller waiting on the same request.
    if (config?.signal) {
      return originalGet(url, config);
    }

    const key = buildKey(url, config);
    const pending = inFlight.get(key);
    if (pending) return pending;

    const request = originalGet(url, config).finally(() => {
      inFlight.delete(key);
    });

    inFlight.set(key, request);
    return request;
  };

  client.get = dedupedGet as GetFn;
}
