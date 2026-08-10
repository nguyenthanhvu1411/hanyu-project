import { AuthHero } from "./auth-hero";

interface AuthShellProps {
  children: React.ReactNode;
}

export function AuthShell({
  children,
}: AuthShellProps) {
  return (
    <main
      className="
        flex
        min-h-screen
        items-center
        justify-center
        bg-[#fffaf0]
        p-[28px]
        lg:p-[30px]
      "
    >
      <div
        className="
          mx-auto
          grid
          w-full
          min-h-[600px]
          h-[calc(100vh-60px)]
          max-w-[1360px]
          overflow-hidden
          rounded-[26px]
          border
          border-[#ede5d8]
          bg-white
          shadow-[0_8px_35px_rgba(60,45,25,0.06)]
          xl:grid-cols-[1.05fr_1fr]
        "
      >
        <AuthHero />

        <section
          className="
            relative
            flex
            items-center
            justify-center
            bg-[#fff]
            px-5
            py-12
            sm:px-10
            lg:px-[55px]
          "
        >
          {children}
        </section>
      </div>
    </main>
  );
}
