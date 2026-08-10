import { redirect } from "next/navigation";

import { ROUTES } from "@/constants/route.constants";

export default function HomePage() {
  redirect(ROUTES.DANG_NHAP);
}
