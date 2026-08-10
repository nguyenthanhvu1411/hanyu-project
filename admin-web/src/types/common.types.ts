export type Nullable<T> =
  T | null;

export type Optional<T> =
  T | undefined;

export interface IdName {
  id:
    | string
    | number;

  name: string;
}

export interface CreatedInfo {
  createdAt?: string;

  createdBy?: string;
}

export interface UpdatedInfo {
  updatedAt?: string;

  updatedBy?: string;
}
