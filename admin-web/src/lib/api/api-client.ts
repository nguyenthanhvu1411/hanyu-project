import { ApiError, ApiProblemDetails } from "./api-error";
import { refreshAccessToken } from "./refresh-token-manager";
import { getAuthState } from "@/stores/auth.store";

// Canonical contract:
// - NEXT_PUBLIC_API_BASE_URL includes the API version prefix, e.g. http://localhost:5216/api/v1
// - feature endpoints are relative to that prefix, e.g. /admin/hsk-levels
// Legacy feature paths that still start with /api/v1 are normalized here to prevent
// accidental URLs such as /api/v1/api/v1/admin/...
const API_URL = (
  process.env.NEXT_PUBLIC_API_BASE_URL ??
  process.env.NEXT_PUBLIC_API_URL ??
  "http://localhost:5216/api/v1"
).replace(/\/+$/, "");

const API_VERSION_PREFIX = "/api/v1";

type ApiRequestOptions = Omit<RequestInit, "body"> & {
  body?: unknown;
  skipAuthRefresh?: boolean;
};

function buildApiUrl(path: string): string {
  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  let normalizedPath = path.startsWith("/") ? path : `/${path}`;

  if (
    API_URL.endsWith(API_VERSION_PREFIX) &&
    (normalizedPath === API_VERSION_PREFIX || normalizedPath.startsWith(`${API_VERSION_PREFIX}/`))
  ) {
    normalizedPath = normalizedPath.slice(API_VERSION_PREFIX.length) || "/";
  }

  return `${API_URL}${normalizedPath}`;
}

async function parseError(response: Response): Promise<ApiError> {
  let problem: ApiProblemDetails | null = null;

  try {
    problem = (await response.json()) as ApiProblemDetails;
  } catch {
    // Response may not contain a JSON ProblemDetails body.
  }

  return new ApiError(
    problem?.detail ?? problem?.title ?? `Yêu cầu thất bại (${response.status})`,
    response.status,
    problem?.code,
    problem?.errors,
  );
}

function isFormData(value: unknown): value is FormData {
  return typeof FormData !== "undefined" && value instanceof FormData;
}

function buildHeaders(options: ApiRequestOptions, accessToken: string | null): Headers {
  const headers = new Headers(options.headers);

  if (!headers.has("Accept")) {
    headers.set("Accept", "application/json");
  }

  // Browser must generate the multipart boundary. Never set Content-Type manually for FormData.
  if (
    options.body !== undefined &&
    !isFormData(options.body) &&
    !headers.has("Content-Type")
  ) {
    headers.set("Content-Type", "application/json");
  }

  if (accessToken && !headers.has("Authorization")) {
    headers.set("Authorization", `Bearer ${accessToken}`);
  }

  return headers;
}

function buildRequestBody(body: unknown): BodyInit | undefined {
  if (body === undefined) return undefined;
  if (isFormData(body)) return body;
  return JSON.stringify(body);
}

async function sendRequest(
  path: string,
  options: ApiRequestOptions,
  accessToken: string | null,
): Promise<Response> {
  const { skipAuthRefresh: _skipAuthRefresh, ...requestOptions } = options;

  return fetch(buildApiUrl(path), {
    ...requestOptions,
    credentials: "include",
    headers: buildHeaders(options, accessToken),
    body: buildRequestBody(options.body),
    cache: "no-store",
  });
}

async function readSuccess<T>(response: Response): Promise<T> {
  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export async function apiClient<T>(
  path: string,
  options: ApiRequestOptions = {},
): Promise<T> {
  const initialAccessToken = getAuthState().accessToken;

  let response = await sendRequest(path, options, initialAccessToken);

  if (
    response.status === 401 &&
    !options.skipAuthRefresh &&
    getAuthState().refreshToken
  ) {
    try {
      const refreshedAccessToken = await refreshAccessToken();
      response = await sendRequest(path, options, refreshedAccessToken);
    } catch {
      getAuthState().clearAuth();
      throw await parseError(response);
    }
  }

  if (!response.ok) {
    if (response.status === 401) {
      getAuthState().clearAuth();
    }

    throw await parseError(response);
  }

  return readSuccess<T>(response);
}

apiClient.get = <T>(
  path: string,
  options?: Omit<ApiRequestOptions, "method">,
) => apiClient<T>(path, { ...options, method: "GET" });

apiClient.post = <T>(
  path: string,
  body?: unknown,
  options?: Omit<ApiRequestOptions, "method" | "body">,
) => apiClient<T>(path, { ...options, body, method: "POST" });

apiClient.put = <T>(
  path: string,
  body?: unknown,
  options?: Omit<ApiRequestOptions, "method" | "body">,
) => apiClient<T>(path, { ...options, body, method: "PUT" });

apiClient.patch = <T>(
  path: string,
  body?: unknown,
  options?: Omit<ApiRequestOptions, "method" | "body">,
) => apiClient<T>(path, { ...options, body, method: "PATCH" });

apiClient.delete = <T>(
  path: string,
  options?: Omit<ApiRequestOptions, "method">,
) => apiClient<T>(path, { ...options, method: "DELETE" });
