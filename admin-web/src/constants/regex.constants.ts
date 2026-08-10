export const REGEX = {
  EMAIL:
    /^[^\s@]+@[^\s@]+\.[^\s@]+$/,

  USERNAME:
    /^[a-zA-Z0-9._-]{3,50}$/,

  SLUG:
    /^[a-z0-9]+(?:-[a-z0-9]+)*$/,

  PHONE_VN:
    /^(0|\+84)[0-9]{9,10}$/,
} as const;
