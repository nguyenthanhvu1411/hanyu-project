import { Plus } from "lucide-react";

import { Alert } from "@/components/ui/alert";
import { Card, CardContent } from "@/components/ui/card";

export function VocabularyEditorSection({
  title,
  description,
  icon,
  error,
  children,
}: {
  title: string;
  description: string;
  icon: React.ReactNode;
  error?: string | null;
  children: React.ReactNode;
}) {
  return (
    <Card>
      <CardContent className="p-4">
        <div className="mb-4 flex items-start gap-3">
          <span className="mt-0.5 text-[#ef241c]">{icon}</span>
          <div>
            <h2 className="text-[16px] font-semibold text-[#333]">{title}</h2>
            <p className="mt-1 text-[13px] leading-5 text-[#777]">{description}</p>
          </div>
        </div>
        {error && <Alert variant="danger" className="mb-4">{error}</Alert>}
        {children}
      </CardContent>
    </Card>
  );
}

export function VocabularyEditorRow({
  title,
  subtitle,
  children,
}: {
  title: string;
  subtitle: string;
  children: React.ReactNode;
}) {
  return (
    <div className="flex flex-col gap-3 rounded-[9px] border border-[#e8e3dc] px-4 py-3 sm:flex-row sm:items-center sm:justify-between">
      <div className="min-w-0">
        <div className="text-[14px] font-semibold text-[#3f3f3f]">{title}</div>
        <div className="mt-1 text-[13px] leading-5 text-[#777]">{subtitle}</div>
      </div>
      <div className="flex shrink-0 flex-wrap gap-2">{children}</div>
    </div>
  );
}

export function VocabularyEditorEmpty({ text }: { text: string }) {
  return (
    <div className="rounded-[9px] border border-dashed border-[#ddd8d1] px-4 py-8 text-center text-[13px] text-[#999]">
      <Plus size={17} className="mx-auto mb-2" />
      {text}
    </div>
  );
}
