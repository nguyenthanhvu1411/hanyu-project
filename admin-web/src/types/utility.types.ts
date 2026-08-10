export type ValueOf<T> =
  T[keyof T];

export type DeepPartial<T> = {
  [P in keyof T]?:
    T[P] extends object
      ? DeepPartial<
          T[P]
        >
      : T[P];
};

export type MaybePromise<T> =
  T | Promise<T>;
