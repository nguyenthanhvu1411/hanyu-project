import {
  Breadcrumb,
} from "./breadcrumb";

import {
  PageTitle,
} from "./page-title";

interface PageHeaderProps {
  title: string;
  description?: string;
  actions?: React.ReactNode;
}

export function PageHeader({
  title,
  description,
  actions,
}: PageHeaderProps) {
  return (
    <div
      className="
        mb-5
        flex
        flex-col
        gap-4
        sm:flex-row
        sm:items-end
        sm:justify-between
      "
    >
      <div className="min-w-0">
        <div className="mb-2">
          <Breadcrumb />
        </div>

        <PageTitle
          title={
            title
          }
          description={
            description
          }
        />
      </div>

      {actions && (
        <div
          className="
            flex
            shrink-0
            flex-wrap
            items-center
            gap-2
          "
        >
          {actions}
        </div>
      )}
    </div>
  );
}
