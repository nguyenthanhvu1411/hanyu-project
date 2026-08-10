"use client";

import {
  ChevronLeft,
  ChevronRight,
} from "lucide-react";

interface CalendarProps {
  month: Date;

  onMonthChange: (
    date: Date,
  ) => void;

  value?: Date;

  onSelect?: (
    date: Date,
  ) => void;
}

export function Calendar({
  month,
  onMonthChange,
  value,
  onSelect,
}: CalendarProps) {
  const year =
    month.getFullYear();

  const monthIndex =
    month.getMonth();

  const firstDay =
    new Date(
      year,
      monthIndex,
      1,
    );

  const totalDays =
    new Date(
      year,
      monthIndex + 1,
      0,
    ).getDate();

  const start =
    firstDay.getDay();

  const cells =
    Array.from({
      length:
        Math.ceil(
          (start +
            totalDays) /
            7,
        ) * 7,
    });

  function previous() {
    onMonthChange(
      new Date(
        year,
        monthIndex - 1,
        1,
      ),
    );
  }

  function next() {
    onMonthChange(
      new Date(
        year,
        monthIndex + 1,
        1,
      ),
    );
  }

  return (
    <div
      className="
        w-[280px]
        rounded-[10px]
        border
        border-[#e6e1da]
        bg-white
        p-3
      "
    >
      <div
        className="
          flex
          items-center
          justify-between
        "
      >
        <button
          type="button"
          onClick={
            previous
          }
          className="
            flex h-8 w-8
            items-center
            justify-center
            rounded
            hover:bg-[#f5f5f5]
          "
        >
          <ChevronLeft
            size={15}
          />
        </button>

        <div
          className="
            text-[12px]
            font-semibold
          "
        >
          Tháng{" "}
          {monthIndex +
            1}
          /{year}
        </div>

        <button
          type="button"
          onClick={next}
          className="
            flex h-8 w-8
            items-center
            justify-center
            rounded
            hover:bg-[#f5f5f5]
          "
        >
          <ChevronRight
            size={15}
          />
        </button>
      </div>

      <div
        className="
          mt-3
          grid
          grid-cols-7
          text-center
          text-[9px]
          text-[#999]
        "
      >
        {[
          "CN",
          "T2",
          "T3",
          "T4",
          "T5",
          "T6",
          "T7",
        ].map(
          (day) => (
            <div
              key={day}
              className="py-1"
            >
              {day}
            </div>
          ),
        )}
      </div>

      <div
        className="
          grid
          grid-cols-7
          gap-[2px]
        "
      >
        {cells.map(
          (
            _,
            index,
          ) => {
            const day =
              index -
              start +
              1;

            if (
              day < 1 ||
              day >
                totalDays
            ) {
              return (
                <div
                  key={
                    index
                  }
                  className="h-8"
                />
              );
            }

            const date =
              new Date(
                year,
                monthIndex,
                day,
              );

            const selected =
              value &&
              sameDate(
                date,
                value,
              );

            return (
              <button
                key={
                  day
                }
                type="button"
                onClick={() =>
                  onSelect?.(
                    date,
                  )
                }
                className={`
                  flex h-8
                  items-center
                  justify-center
                  rounded-[6px]
                  text-[10px]
                  ${
                    selected
                      ? "bg-[#ef241c] text-white"
                      : "text-[#555] hover:bg-[#fff0ee]"
                  }
                `}
              >
                {day}
              </button>
            );
          },
        )}
      </div>
    </div>
  );
}

function sameDate(
  a: Date,
  b: Date,
) {
  return (
    a.getFullYear() ===
      b.getFullYear() &&
    a.getMonth() ===
      b.getMonth() &&
    a.getDate() ===
      b.getDate()
  );
}
