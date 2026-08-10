import {
  FormFieldError,
} from "./form-field-error";

import {
  FormRequiredMark,
} from "./form-required-mark";

import {
  cn,
} from "@/lib/utils/cn";

interface FormFieldProps {
  label: string;

  htmlFor?: string;

  required?: boolean;

  description?: string;

  error?: string;

  children: React.ReactNode;

  className?: string;
}

export function FormField({
  label,
  htmlFor,
  required = false,
  description,
  error,
  children,
  className,
}: FormFieldProps) {
  return (
    <div
      className={cn(
        "min-w-0",
        className,
      )}
    >
      <label
        htmlFor={htmlFor}
        className="
          mb-[7px]
          block
          text-[12px]
          font-medium
          text-[#3b3b3b]
        "
      >
        {label}

        {required && (
          <FormRequiredMark />
        )}
      </label>

      {children}

      {error ? (
        <FormFieldError
          message={error}
        />
      ) : (
        description && (
          <div
            className="
              mt-[6px]
              text-[10px]
              leading-[16px]
              text-[#909090]
            "
          >
            {description}
          </div>
        )
      )}
    </div>
  );
}
