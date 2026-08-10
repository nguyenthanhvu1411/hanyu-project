import "axios";

declare module "axios" {
  export interface InternalAxiosRequestConfig {
    _retry?: boolean;
    skipAuthRefresh?: boolean;
  }

  export interface AxiosRequestConfig {
    _retry?: boolean;
    skipAuthRefresh?: boolean;
  }
}
