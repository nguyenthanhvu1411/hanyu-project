"use client";

import {
  Languages,
  Volume2,
} from "lucide-react";

import {
  useState,
} from "react";

import {
  AudioUpload,
} from "@/components/common/audio-upload";

import {
  MultiSelect,
} from "@/components/common/multi-select";

import {
  RichTextEditor,
} from "@/components/common/rich-text-editor";

import {
  TagInput,
} from "@/components/common/tag-input";

import {
  FormActions,
} from "@/components/forms/form-actions";

import {
  FormField,
} from "@/components/forms/form-field";

import {
  FormRow,
} from "@/components/forms/form-row";

import {
  FormSection,
} from "@/components/forms/form-section";

import {
  Combobox,
} from "@/components/ui/combobox";

import {
  Input,
} from "@/components/ui/input";

import {
  Select,
} from "@/components/ui/select";

import {
  Switch,
} from "@/components/ui/switch";

export function VocabularyForm() {
  const [
    hskLevel,
    setHskLevel,
  ] = useState("");

  const [
    partOfSpeech,
    setPartOfSpeech,
  ] = useState("");

  const [
    topics,
    setTopics,
  ] = useState<
    string[]
  >([]);

  const [
    tags,
    setTags,
  ] = useState<
    string[]
  >([]);

  const [
    content,
    setContent,
  ] = useState("");

  const [
    published,
    setPublished,
  ] = useState(true);

  const [
    audio,
    setAudio,
  ] =
    useState<File | null>(
      null,
    );

  return (
    <form className="space-y-5">
      <FormSection
        title="Thông tin từ vựng"
        description="Thông tin chữ Hán, Pinyin và phân loại."
        icon={
          <Languages
            size={18}
          />
        }
      >
        <FormRow columns={2}>
          <FormField
            label="Chữ giản thể"
            required
          >
            <Input
              className="h-[42px]"
              placeholder="学习"
            />
          </FormField>

          <FormField
            label="Chữ phồn thể"
          >
            <Input
              className="h-[42px]"
              placeholder="學習"
            />
          </FormField>
        </FormRow>

        <FormRow columns={3}>
          <FormField
            label="Pinyin"
            required
          >
            <Input
              className="h-[42px]"
              placeholder="xuéxí"
            />
          </FormField>

          <FormField
            label="Cấp độ HSK"
          >
            <Select
              value={
                hskLevel
              }
              onValueChange={
                setHskLevel
              }
              options={[
                {
                  label:
                    "HSK 1",
                  value:
                    "1",
                },
                {
                  label:
                    "HSK 2",
                  value:
                    "2",
                },
              ]}
            />
          </FormField>

          <FormField
            label="Loại từ"
          >
            <Combobox
              value={
                partOfSpeech
              }
              onValueChange={
                setPartOfSpeech
              }
              options={[
                {
                  label:
                    "Động từ",
                  value:
                    "verb",
                },
                {
                  label:
                    "Danh từ",
                  value:
                    "noun",
                },
              ]}
            />
          </FormField>
        </FormRow>

        <FormField
          label="Chủ đề"
        >
          <MultiSelect
            value={
              topics
            }
            onValueChange={
              setTopics
            }
            options={[
              {
                label:
                  "Giáo dục",
                value:
                  "education",
              },
              {
                label:
                  "Giao tiếp",
                value:
                  "communication",
              },
              {
                label:
                  "Công việc",
                value:
                  "work",
              },
            ]}
          />
        </FormField>

        <FormField
          label="Thẻ"
          description="Nhấn Enter hoặc dấu phẩy để thêm thẻ."
        >
          <TagInput
            value={tags}
            onChange={
              setTags
            }
          />
        </FormField>
      </FormSection>

      <FormSection
        title="Nội dung giải thích"
      >
        <RichTextEditor
          value={
            content
          }
          onChange={
            setContent
          }
        />
      </FormSection>

      <FormSection
        title="Phát âm"
        icon={
          <Volume2
            size={18}
          />
        }
      >
        <AudioUpload
          value={audio}
          onChange={
            setAudio
          }
        />
      </FormSection>

      <FormSection
        title="Xuất bản"
      >
        <Switch
          checked={
            published
          }
          onCheckedChange={
            setPublished
          }
          label="Cho phép hiển thị"
          description="Từ vựng sẽ hiển thị trên hệ thống Public."
        />
      </FormSection>

      <FormActions
        submitText="Lưu từ vựng"
        onCancel={() => {
          history.back();
        }}
      />
    </form>
  );
}
