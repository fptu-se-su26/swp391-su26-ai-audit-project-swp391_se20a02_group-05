import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Input, Button, TextArea, Spinner, Card } from "@heroui/react";
import { PlusCircle, Trash2, Edit2, X } from "lucide-react";
import { type AchievementsDraftItem } from "./types";

interface AchievementsFormProps {
  draft: AchievementsDraftItem[];
  onChange: (updated: AchievementsDraftItem[]) => void;
  onSave: () => Promise<void>;
  onReset: () => void;
  isSaving: boolean;
  isDirty: boolean;
}

export const AchievementsForm: React.FC<AchievementsFormProps> = ({
  draft,
  onChange,
  onSave,
  onReset,
  isSaving,
  isDirty,
}) => {
  const { t } = useTranslation(["common"]);
  const [editingItem, setEditingItem] = useState<AchievementsDraftItem | null>(null);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleEdit = (item: AchievementsDraftItem) => {
    setEditingItem({ ...item });
    setErrors({});
  };

  const handleAddNew = () => {
    const newItem: AchievementsDraftItem = {
      id: `temp-${Date.now()}`,
      title: "",
      issuer: "",
      issueDate: "",
      description: "",
      credentialUrl: "",
      attachmentId: null,
    };
    setEditingItem(newItem);
    setErrors({});
  };

  const handleRemove = (id: string) => {
    const filtered = draft.filter((item) => item.id !== id);
    onChange(filtered);
    if (editingItem?.id === id) {
      setEditingItem(null);
    }
  };

  const validateItem = (item: AchievementsDraftItem): boolean => {
    const newErrors: Record<string, string> = {};
    if (!item.title.trim()) newErrors.title = t("common:cvManagement.validation.required");
    if (!item.issuer.trim()) newErrors.issuer = t("common:cvManagement.validation.required");
    if (!item.issueDate) newErrors.issueDate = t("common:cvManagement.validation.required");

    if (item.credentialUrl && item.credentialUrl.trim()) {
      try {
        if (!item.credentialUrl.startsWith("http://") && !item.credentialUrl.startsWith("https://")) {
          new URL("https://" + item.credentialUrl);
        } else {
          new URL(item.credentialUrl);
        }
      } catch (e) {
        newErrors.credentialUrl = t("common:cvManagement.validation.invalidUrl");
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSaveItem = () => {
    if (!editingItem) return;
    if (!validateItem(editingItem)) return;

    const exists = draft.some((item) => item.id === editingItem.id);
    let updatedList;
    if (exists) {
      updatedList = draft.map((item) => (item.id === editingItem.id ? editingItem : item));
    } else {
      updatedList = [...draft, editingItem];
    }
    onChange(updatedList);
    setEditingItem(null);
  };

  return (
    <div className="flex flex-col h-full overflow-hidden relative text-left">
      <div className="flex-1 overflow-y-auto pr-1 flex flex-col gap-4 pb-20">
        {editingItem ? (
        // Inline Edit Mode
        <div className="flex flex-col gap-5 border border-border/40 p-5 rounded-2xl bg-surface-secondary/5">
          <div className="flex justify-between items-center border-b border-border/20 pb-3 select-none">
            <span className="font-bold text-xs text-foreground">
              {editingItem.id.startsWith("temp-") ? t("common:cvManagement.labels.addAchievement") : t("common:cvManagement.labels.addAchievement") || "Edit Achievement"}
            </span>
            <Button
              isIconOnly
              size="sm"
              variant="secondary"
              className="rounded-xl border border-border/30 h-8 w-8"
              onPress={() => setEditingItem(null)}
            >
              <X className="size-4" />
            </Button>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-xs">
            <div className="flex flex-col gap-1.5 md:col-span-2">
              <label className="font-bold text-foreground">{t("common:cvManagement.labels.fullName") || "Certificate / Achievement Name"} *</label>
              <Input
                value={editingItem.title}
                onChange={(e) => setEditingItem({ ...editingItem, title: e.target.value })}
                placeholder="AWS Certified Solutions Architect"
              />
              {errors.title && <span className="text-[10px] text-danger">{errors.title}</span>}
            </div>

            <div className="flex flex-col gap-1.5">
              <label className="font-bold text-foreground">{t("common:cvManagement.labels.issuer")} *</label>
              <Input
                value={editingItem.issuer}
                onChange={(e) => setEditingItem({ ...editingItem, issuer: e.target.value })}
                placeholder="Amazon Web Services (AWS)"
              />
              {errors.issuer && <span className="text-[10px] text-danger">{errors.issuer}</span>}
            </div>

            <div className="flex flex-col gap-1.5">
              <label className="font-bold text-foreground">{t("common:cvManagement.labels.issueDate")} *</label>
              <input
                type="date"
                className="flex h-10 w-full rounded-xl border border-border bg-surface px-3 py-2 text-xs outline-none focus:border-accent"
                value={editingItem.issueDate ? editingItem.issueDate.split("T")[0] : ""}
                onChange={(e) => setEditingItem({ ...editingItem, issueDate: e.target.value })}
              />
            </div>

            <div className="flex flex-col gap-1.5 md:col-span-2">
              <label className="font-bold text-foreground">{t("common:cvManagement.labels.credentialUrl")}</label>
              <Input
                value={editingItem.credentialUrl}
                onChange={(e) => setEditingItem({ ...editingItem, credentialUrl: e.target.value })}
                placeholder="https://aws.amazon.com/verify/..."
              />
              {errors.credentialUrl && <span className="text-[10px] text-danger">{errors.credentialUrl}</span>}
            </div>
          </div>

          <div className="flex flex-col gap-1.5 text-xs">
            <label className="font-bold text-foreground">{t("common:cvManagement.labels.description")}</label>
            <TextArea
              value={editingItem.description}
              onChange={(e) => setEditingItem({ ...editingItem, description: e.target.value })}
              placeholder="Provide a brief description of the achievement..."
              rows={3}
            />
          </div>

          <Button size="sm" className="bg-accent text-accent-foreground font-bold rounded-xl border-none mt-2 h-9" onPress={handleSaveItem}>
            {t("common:buttons.confirm")}
          </Button>
        </div>
      ) : (
        // List Mode
        <div className="flex flex-col gap-4">
          <div className="flex justify-between items-center select-none">
            <span className="text-xs font-bold text-foreground">{t("common:cvManagement.labels.addAchievement")}</span>
            <Button
              size="sm"
              variant="secondary"
              className="rounded-xl text-[10px] font-bold flex items-center gap-1 border border-border/30 h-8"
              onPress={handleAddNew}
            >
              <PlusCircle className="size-3.5" />
              {t("common:cvManagement.labels.addAchievement")}
            </Button>
          </div>

          <div className="flex flex-col gap-3">
            {draft.length === 0 ? (
              <div className="py-10 text-center border-2 border-dashed border-border/40 rounded-2xl select-none">
                <span className="text-muted-foreground text-xs">{t("common:cvManagement.labels.noAchievements")}</span>
              </div>
            ) : (
              draft.map((item) => (
                <Card key={item.id} className="p-4 border border-border/40 bg-surface flex flex-row justify-between items-center gap-4">
                  <div className="flex flex-col gap-1 min-w-0">
                    <span className="font-bold text-foreground text-xs truncate">
                      {item.title}
                    </span>
                    <span className="text-[10px] text-muted-foreground font-medium">
                      {item.issuer} ({item.issueDate})
                    </span>
                  </div>
                  <div className="flex gap-2">
                    <Button
                      isIconOnly
                      size="sm"
                      variant="secondary"
                      className="rounded-xl border border-border/30 h-8 w-8"
                      onPress={() => handleEdit(item)}
                    >
                      <Edit2 className="size-3.5" />
                    </Button>
                    <Button
                      isIconOnly
                      size="sm"
                      variant="secondary"
                      className="rounded-xl border border-border/30 h-8 w-8 text-danger"
                      onPress={() => handleRemove(item.id)}
                    >
                      <Trash2 className="size-3.5" />
                    </Button>
                  </div>
                </Card>
              ))
            )}
          </div>

        </div>
      )}
      </div>

      {!editingItem && (
        <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-border/20 bg-background/95 backdrop-blur-sm flex justify-end gap-3 shrink-0 rounded-b-xl z-20">
          <Button
            size="sm"
            variant="secondary"
            className="rounded-xl font-bold select-none border border-border/30 h-9"
            isDisabled={!isDirty || isSaving}
            onPress={onReset}
          >
            {t("common:cvWorkspace.resetChanges")}
          </Button>
          <Button
            size="sm"
            onPress={onSave}
            className={`rounded-xl font-bold select-none border-none h-9 ${
              isDirty ? "bg-accent text-accent-foreground" : "bg-neutral-300 text-neutral-500 cursor-not-allowed"
            }`}
            isDisabled={!isDirty || isSaving}
          >
            {isSaving ? <Spinner size="sm" color="current" /> : t("common:cvWorkspace.saveChanges")}
          </Button>
        </div>
      )}
    </div>
  );
};
