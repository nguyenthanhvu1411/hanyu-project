import { ApiError, ApiProblemDetails } from "./api-error";
import { refreshAccessToken } from "./refresh-token-manager";
import { getAuthState } from "@/stores/auth.store";

// Keep this aligned with HanYu/Properties/launchSettings.json for local development.
// Production/staging must override it with NEXT_PUBLIC_API_URL.
const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5216";

type ApiRequestOptions = Omit<RequestInit, "body"> & {
  body?: unknown;
  skipAuthRefresh?: boolean;
};

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

function buildHeaders(options: ApiRequestOptions, accessToken: string | null): Headers {
  const headers = new Headers(options.headers);

  if (!headers.has("Accept")) {
    headers.set("Accept", "application/json");
  }

  if (options.body !== undefined && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  if (accessToken && !headers.has("Authorization")) {
    headers.set("Authorization", `Bearer ${accessToken}`);
  }

  return headers;
}

async function sendRequest(
  path: string,
  options: ApiRequestOptions,
  accessToken: string | null,
): Promise<Response> {
  const { skipAuthRefresh: _skipAuthRefresh, ...requestOptions } = options;

  return fetch(`${API_URL}${path}`, {
    ...requestOptions,
    credentials: "include",
    headers: buildHeaders(options, accessToken),
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
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
