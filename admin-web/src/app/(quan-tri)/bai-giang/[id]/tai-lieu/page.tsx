import { redirect } from "next/navigation";

export default async function LessonAssetsLegacyPage(props: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await props.params;
  redirect(`/bai-giang/${id}/noi-dung`);
}
