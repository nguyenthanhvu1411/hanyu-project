import {
  Eye,
  EyeOff,
  KeyRound,
  LockKeyhole,
  LogIn,
  LogOut,
  Mail,
  ShieldCheck,
  Smartphone,
  UserRound,
} from "lucide-react";

export const AuthIcons = {
  Login: LogIn,
  Logout: LogOut,
  Email: Mail,
  Password: LockKeyhole,
  Key: KeyRound,
  User: UserRound,
  Shield: ShieldCheck,
  TwoFactor: Smartphone,
  ShowPassword: Eye,
  HidePassword: EyeOff,
} as const;
