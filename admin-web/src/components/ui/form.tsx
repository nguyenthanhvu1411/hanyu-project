import {
  cn,
} from "@/lib/utils/cn";

export function Form({
  className,
  ...props
}: React.FormHTMLAttributes<HTMLFormElement>) {
  return (
    <form
      className={cn(
        "space-y-5",
        className,
      )}
      {...props}
    />
  );
}

export function FormGroup({
  className,
  ...props
}: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      className={cn(
        "space-y-4",
        className,
      )}
      {...props}
    />
  );
}
