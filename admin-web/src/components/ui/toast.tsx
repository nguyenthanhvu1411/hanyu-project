"use client";

import {
  toast,
} from "sonner";

export const appToast = {
  success(
    message: string,
    description?: string,
  ) {
    toast.success(
      message,
      {
        description,
      },
    );
  },

  error(
    message: string,
    description?: string,
  ) {
    toast.error(
      message,
      {
        description,
      },
    );
  },

  warning(
    message: string,
    description?: string,
  ) {
    toast.warning(
      message,
      {
        description,
      },
    );
  },

  info(
    message: string,
    description?: string,
  ) {
    toast.info(
      message,
      {
        description,
      },
    );
  },

  loading(
    message: string,
  ) {
    return toast.loading(
      message,
    );
  },

  dismiss(
    id?: string | number,
  ) {
    toast.dismiss(
      id,
    );
  },

  promise<T>(
    promise:
      | Promise<T>
      | (() => Promise<T>),

    messages: {
      loading: string;
      success: string;
      error: string;
    },
  ) {
    return toast.promise(
      promise,
      messages,
    );
  },
};
