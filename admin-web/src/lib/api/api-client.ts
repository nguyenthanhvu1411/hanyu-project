import { ApiError, ApiProblemDetails } from "./api-error";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

type ApiRequestOptions = Omit<RequestInit, "body"> & {
  body?: unknown;
  skipAuthRefresh?: boolean;
};

async function parseError(response: Response): Promise<ApiError> {
  let problem: ApiProblemDetails | null = null;

  try {
    problem = (await response.json()) as ApiProblemDetails;
  } catch {
    // Ignore JSON parse error if response is not JSON
  }

  return new ApiError(
    problem?.detail ?? problem?.title ?? `Yêu cầu thất bại (${response.status})`,
    response.status,
    problem?.code,
    problem?.errors
  );
}

export async function apiClient<T>(
  path: string,
  options: ApiRequestOptions = {}
): Promise<T> {
  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    credentials: "include",
    headers: {
      Accept: "application/json",
      ...(options.body !== undefined
        ? {
            "Content-Type": "application/json",
          }
        : {}),
      ...options.headers,
    },
    body:
      options.body !== undefined
        ? JSON.stringify(options.body)
        : undefined,
    cache: "no-store",
  });

  if (!response.ok) {
    throw await parseError(response);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

apiClient.get = <T>(path: string, options?: Omit<ApiRequestOptions, "method">) =>
  apiClient<T>(path, { ...options, method: "GET" });

apiClient.post = <T>(path: string, body?: unknown, options?: Omit<ApiRequestOptions, "method" | "body">) =>
  apiClient<T>(path, { ...options, body, method: "POST" });

apiClient.put = <T>(path: string, body?: unknown, options?: Omit<ApiRequestOptions, "method" | "body">) =>
  apiClient<T>(path, { ...options, body, method: "PUT" });

apiClient.patch = <T>(path: string, body?: unknown, options?: Omit<ApiRequestOptions, "method" | "body">) =>
  apiClient<T>(path, { ...options, body, method: "PATCH" });

apiClient.delete = <T>(path: string, options?: Omit<ApiRequestOptions, "method">) =>
  apiClient<T>(path, { ...options, method: "DELETE" });

