"use client";

import {
  Bold,
  Code,
  Heading2,
  Italic,
  Link,
  List,
  ListOrdered,
  Quote,
  Redo2,
  RemoveFormatting,
  Underline,
  Undo2,
} from "lucide-react";

import {
  useEffect,
  useRef,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

interface RichTextEditorProps {
  value?: string;

  onChange?: (
    html: string,
  ) => void;

  placeholder?: string;

  minHeight?: number;

  disabled?: boolean;

  error?: boolean;
}

export function RichTextEditor({
  value = "",
  onChange,
  placeholder =
    "Nhập nội dung...",
  minHeight = 260,
  disabled = false,
  error = false,
}: RichTextEditorProps) {
  const editorRef =
    useRef<HTMLDivElement>(
      null,
    );

  useEffect(() => {
    if (
      editorRef.current &&
      editorRef.current
        .innerHTML !== value
    ) {
      editorRef.current.innerHTML =
        value;
    }
  }, [value]);

  function execute(
    command: string,
    commandValue?: string,
  ) {
    if (disabled) {
      return;
    }

    editorRef.current?.focus();

    document.execCommand(
      command,
      false,
      commandValue,
    );

    emitChange();
  }

  function emitChange() {
    onChange?.(
      editorRef.current
        ?.innerHTML ?? "",
    );
  }

  function addLink() {
    const url =
      window.prompt(
        "Nhập đường dẫn:",
        "https://",
      );

    if (!url) {
      return;
    }

    execute(
      "createLink",
      url,
    );
  }

  return (
    <div
      className={cn(
        "overflow-hidden",
        "rounded-[9px]",
        "border",
        "bg-white",

        error
          ? "border-[#ef453f]"
          : "border-[#dedbd6]",

        disabled &&
          "opacity-60",
      )}
    >
      <div
        className="
          flex
          flex-wrap
          items-center
          gap-1
          border-b
          border-[#ebe6df]
          bg-[#faf9f7]
          p-2
        "
      >
        <ToolbarButton
          title="Hoàn tác"
          onClick={() =>
            execute(
              "undo",
            )
          }
        >
          <Undo2
            size={15}
          />
        </ToolbarButton>

        <ToolbarButton
          title="Làm lại"
          onClick={() =>
            execute(
              "redo",
            )
          }
        >
          <Redo2
            size={15}
          />
        </ToolbarButton>

        <Separator />

        <ToolbarButton
          title="Tiêu đề"
          onClick={() =>
            execute(
              "formatBlock",
              "h2",
            )
          }
        >
          <Heading2
            size={15}
          />
        </ToolbarButton>

        <ToolbarButton
          title="In đậm"
          onClick={() =>
            execute(
              "bold",
            )
          }
        >
          <Bold size={15} />
        </ToolbarButton>

        <ToolbarButton
          title="In nghiêng"
          onClick={() =>
            execute(
              "italic",
            )
          }
        >
          <Italic
            size={15}
          />
        </ToolbarButton>

        <ToolbarButton
          title="Gạch chân"
          onClick={() =>
            execute(
              "underline",
            )
          }
        >
          <Underline
            size={15}
          />
        </ToolbarButton>

        <Separator />

        <ToolbarButton
          title="Danh sách"
          onClick={() =>
            execute(
              "insertUnorderedList",
            )
          }
        >
          <List size={15} />
        </ToolbarButton>

        <ToolbarButton
          title="Danh sách đánh số"
          onClick={() =>
            execute(
              "insertOrderedList",
            )
          }
        >
          <ListOrdered
            size={15}
          />
        </ToolbarButton>

        <ToolbarButton
          title="Trích dẫn"
          onClick={() =>
            execute(
              "formatBlock",
              "blockquote",
            )
          }
        >
          <Quote
            size={15}
          />
        </ToolbarButton>

        <ToolbarButton
          title="Code"
          onClick={() =>
            execute(
              "formatBlock",
              "pre",
            )
          }
        >
          <Code size={15} />
        </ToolbarButton>

        <Separator />

        <ToolbarButton
          title="Chèn liên kết"
          onClick={
            addLink
          }
        >
          <Link size={15} />
        </ToolbarButton>

        <ToolbarButton
          title="Xóa định dạng"
          onClick={() =>
            execute(
              "removeFormat",
            )
          }
        >
          <RemoveFormatting
            size={15}
          />
        </ToolbarButton>
      </div>

      <div className="relative">
        <div
          ref={editorRef}
          contentEditable={
            !disabled
          }
          suppressContentEditableWarning
          onInput={
            emitChange
          }
          style={{
            minHeight,
          }}
          className="
            rich-text-editor
            w-full
            overflow-y-auto
            px-4
            py-3
            text-[12px]
            leading-[1.75]
            text-[#3d3d3d]
            outline-none
          "
        />

        {!value && (
          <span
            className="
              pointer-events-none
              absolute
              left-4
              top-3
              text-[12px]
              text-[#aaa]
            "
          >
            {placeholder}
          </span>
        )}
      </div>
    </div>
  );
}

function ToolbarButton({
  title,
  onClick,
  children,
}: {
  title: string;
  onClick: () => void;
  children: React.ReactNode;
}) {
  return (
    <button
      type="button"
      title={title}
      onMouseDown={(
        event,
      ) =>
        event.preventDefault()
      }
      onClick={
        onClick
      }
      className="
        flex h-8 w-8
        items-center
        justify-center
        rounded-[6px]
        text-[#666]
        transition
        hover:bg-white
        hover:text-[#ef241c]
      "
    >
      {children}
    </button>
  );
}

function Separator() {
  return (
    <span
      className="
        mx-1
        h-5
        w-px
        bg-[#ddd8d0]
      "
    />
  );
}
