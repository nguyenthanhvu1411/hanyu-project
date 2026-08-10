import {
  AlertCircle,
  Ban,
  CheckCircle2,
  Circle,
  Clock3,
  Info,
  Lock,
  PauseCircle,
  XCircle,
} from "lucide-react";

export const StatusIcons = {
  Active:
    CheckCircle2,

  Inactive:
    PauseCircle,

  Locked:
    Lock,

  Pending:
    Clock3,

  Error:
    XCircle,

  Warning:
    AlertCircle,

  Info,

  Disabled:
    Ban,

  Neutral:
    Circle,
} as const;
