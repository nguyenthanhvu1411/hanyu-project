interface PageTitleProps {
  title: string;
  description?: string;
}

export function PageTitle({
  title,
  description,
}: PageTitleProps) {
  return (
    <div className="min-w-0">
      <h1
        className="
          truncate
          text-[20px]
          font-semibold
          tracking-[-0.2px]
          text-[#242424]
        "
      >
        {title}
      </h1>

      {description && (
        <p
          className="
            mt-1
            text-[12px]
            leading-[18px]
            text-[#818181]
          "
        >
          {description}
        </p>
      )}
    </div>
  );
}
