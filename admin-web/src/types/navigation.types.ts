import type { LucideIcon } from "lucide-react";

export interface NavigationItem {
  title: string;
  href?: string;
  icon?: LucideIcon;

  badge?: string | number;

  permission?: string;

  children?: NavigationItem[];
}

export interface NavigationGroup {
  title?: string;
  items: NavigationItem[];
}
