import {
  AlertCircle,
} from "lucide-react";

interface FormFieldErrorProps {
  message?: string;
}

export function FormFieldError({
  message,
}: FormFieldErrorProps) {
  if (!message) {
    return null;
  }

  return (
    <div
      className="
        mt-[6px]
        flex
        items-start
        gap-1
        text-[11px]
        leading-[16px]
        text-[#e23a32]
      "
    >
      <AlertCircle
        size={13}
        className="
          mt-[1px]
          shrink-0
        "
      />

      <span>
        {message}
      </span>
    </div>
  );
}
