import {
  Activity,
  Bell,
  BookOpen,
  BookText,
  Brain,
  ChartNoAxesCombined,
  CircleUserRound,
  ClipboardCheck,
  FileQuestion,
  FolderKanban,
  Gauge,
  GraduationCap,
  Languages,
  LibraryBig,
  ListChecks,
  Settings,
  ShieldCheck,
  Tags,
  Target,
  UsersRound,
} from "lucide-react";

export const NavigationIcons = {
  Dashboard:
    Gauge,

  Users:
    UsersRound,

  Roles:
    ShieldCheck,

  Permissions:
    ListChecks,

  Hsk:
    GraduationCap,

  Courses:
    BookOpen,

  Chapters:
    LibraryBig,

  Lessons:
    BookText,

  Vocabulary:
    Languages,

  Topics:
    FolderKanban,

  PartOfSpeech:
    Tags,

  LearningGoal:
    Target,

  LearningActivity:
    Activity,

  LearningSummary:
    ChartNoAxesCombined,

  QuestionBank:
    Brain,

  Question:
    FileQuestion,

  Quiz:
    ClipboardCheck,

  Notification:
    Bell,

  Settings,

  Profile:
    CircleUserRound,
} as const;
