"use client";

import { EvidenceItem } from "@/types/project";
import { Card, Button, TextField, Input, TextArea, Select, ListBox, Label } from "@heroui/react";
import { Plus, Trash2, ExternalLink, Image, FileText, Link2, FlaskConical, FolderOpen, StickyNote } from "lucide-react";
import { useState, useCallback, useRef } from "react";

const EVIDENCE_TYPES = [
  { id: "commit" as const, label: "Commit/PR Link", icon: Link2, placeholder: "https://github.com/..." },
  { id: "screenshot" as const, label: "Screenshot", icon: Image, placeholder: "Paste or describe screenshot" },
  { id: "demo" as const, label: "Demo Link", icon: ExternalLink, placeholder: "https://..." },
  { id: "test" as const, label: "Test Result", icon: FlaskConical, placeholder: "Describe test results..." },
  { id: "file" as const, label: "Related Files", icon: FolderOpen, placeholder: "src/components/..." },
  { id: "note" as const, label: "Additional Notes", icon: StickyNote, placeholder: "Additional proof notes..." },
];

function getTypeConfig(type: EvidenceItem["type"]) {
  return EVIDENCE_TYPES.find((t) => t.id === type) || EVIDENCE_TYPES[5];
}

/** Compress an image blob to a small base64 thumbnail */
async function compressImageToThumbnail(blob: Blob, maxWidth = 320, quality = 0.6): Promise<string> {
  return new Promise((resolve, reject) => {
    const img = document.createElement("img");
    const url = URL.createObjectURL(blob);
    img.onload = () => {
      const canvas = document.createElement("canvas");
      const scale = Math.min(1, maxWidth / img.width);
      canvas.width = img.width * scale;
      canvas.height = img.height * scale;
      const ctx = canvas.getContext("2d");
      if (!ctx) { reject(new Error("Canvas not supported")); return; }
      ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
      const dataUrl = canvas.toDataURL("image/webp", quality);
      URL.revokeObjectURL(url);
      resolve(dataUrl);
    };
    img.onerror = () => { URL.revokeObjectURL(url); reject(new Error("Failed to load image")); };
    img.src = url;
  });
}

interface EvidenceSectionProps {
  evidence: EvidenceItem[];
  onChange: (items: EvidenceItem[]) => void;
}

export default function EvidenceSection({ evidence, onChange }: EvidenceSectionProps) {
  const [addingType, setAddingType] = useState<EvidenceItem["type"] | null>(null);
  const [newLabel, setNewLabel] = useState("");
  const [newContent, setNewContent] = useState("");
  const [newDescription, setNewDescription] = useState("");
  const dropRef = useRef<HTMLDivElement>(null);
  const [isDragging, setIsDragging] = useState(false);

  const addEvidence = useCallback((item: Omit<EvidenceItem, "id" | "timestamp">) => {
    const newItem: EvidenceItem = {
      ...item,
      id: Math.random().toString(36).substr(2, 9),
      timestamp: new Date().toISOString(),
    };
    onChange([...evidence, newItem]);
  }, [evidence, onChange]);

  const removeEvidence = useCallback((id: string) => {
    onChange(evidence.filter((e) => e.id !== id));
  }, [evidence, onChange]);

  // Handle clipboard paste for screenshots
  const handlePaste = useCallback(async (e: React.ClipboardEvent) => {
    const items = e.clipboardData.items;
    for (const item of items) {
      if (item.type.startsWith("image/")) {
        e.preventDefault();
        const blob = item.getAsFile();
        if (!blob) continue;
        try {
          const thumbnail = await compressImageToThumbnail(blob);
          addEvidence({
            type: "screenshot",
            label: `Screenshot ${new Date().toLocaleTimeString()}`,
            content: "",
            thumbnail,
            fileName: blob.name || "pasted-image",
          });
        } catch (err) {
          console.error("Failed to process pasted image:", err);
        }
        return;
      }
    }
  }, [addEvidence]);

  // Handle drag & drop
  const handleDrop = useCallback(async (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
    const files = e.dataTransfer.files;
    for (const file of files) {
      if (file.type.startsWith("image/")) {
        try {
          const thumbnail = await compressImageToThumbnail(file);
          addEvidence({
            type: "screenshot",
            label: file.name || `Screenshot ${new Date().toLocaleTimeString()}`,
            content: "",
            thumbnail,
            fileName: file.name,
          });
        } catch (err) {
          console.error("Failed to process dropped image:", err);
        }
      }
    }
  }, [addEvidence]);

  const handleAddManual = () => {
    if (!addingType || !newLabel.trim()) return;
    addEvidence({
      type: addingType,
      label: newLabel.trim(),
      content: newContent.trim(),
      description: newDescription.trim() || undefined,
    });
    setAddingType(null);
    setNewLabel("");
    setNewContent("");
    setNewDescription("");
  };

  const isUrl = (type: EvidenceItem["type"]) => type === "commit" || type === "demo";
  const isTextBlock = (type: EvidenceItem["type"]) => type === "test" || type === "note";

  return (
    <div
      className="flex flex-col gap-3"
      onPaste={handlePaste}
    >
      <div className="flex justify-between items-center">
        <h4 className="text-sm font-semibold">Evidence / Proof (Minh chứng)</h4>
        <div className="flex gap-2">
          <Select
            selectedKey={addingType}
            onSelectionChange={(key) => setAddingType(key as EvidenceItem["type"])}
            className="w-40"
            aria-label="Evidence type"
          >
            <Select.Trigger>
              <Select.Value>{(value) => value?.defaultChildren || <span className="text-default-400">Add evidence...</span>}</Select.Value>
              <Select.Indicator />
            </Select.Trigger>
            <Select.Popover>
              <ListBox>
                {EVIDENCE_TYPES.map((t) => (
                  <ListBox.Item key={t.id} id={t.id} textValue={t.label}>
                    {t.label}
                    <ListBox.ItemIndicator />
                  </ListBox.Item>
                ))}
              </ListBox>
            </Select.Popover>
          </Select>
        </div>
      </div>

      {/* Drop zone for screenshots */}
      <div
        ref={dropRef}
        onDrop={handleDrop}
        onDragOver={(e) => { e.preventDefault(); setIsDragging(true); }}
        onDragLeave={() => setIsDragging(false)}
        className={`border-2 border-dashed rounded-lg p-3 text-center text-xs transition-colors ${isDragging
            ? "border-primary bg-primary/5 text-primary"
            : "border-border text-default-400"
          }`}
      >
        <Image className="w-4 h-4 mx-auto mb-1 opacity-50" />
        <span>Paste screenshot (Ctrl+V) or drag & drop images here</span>
      </div>

      {/* Add manual evidence form */}
      {addingType && (
        <Card className="bg-surface-secondary/30 border border-border">
          <div className="p-4 flex flex-col gap-3">
            <div className="flex items-center gap-2 text-sm font-medium">
              {(() => { const T = getTypeConfig(addingType); return <T.icon className="w-4 h-4" />; })()}
              {getTypeConfig(addingType).label}
            </div>
            <TextField>
              <Label>Label</Label>
              <Input
                value={newLabel}
                onChange={(e) => setNewLabel(e.target.value)}
                placeholder="Brief title for this evidence"
              />
            </TextField>
            {isTextBlock(addingType) ? (
              <TextField>
                <Label>Content</Label>
                <TextArea
                  value={newContent}
                  onChange={(e) => setNewContent(e.target.value)}
                  placeholder={getTypeConfig(addingType).placeholder}
                />
              </TextField>
            ) : (
              <TextField>
                <Label>{isUrl(addingType) ? "URL" : "Reference"}</Label>
                <Input
                  value={newContent}
                  onChange={(e) => setNewContent(e.target.value)}
                  placeholder={getTypeConfig(addingType).placeholder}
                />
              </TextField>
            )}
            <div className="flex justify-end gap-2">
              <Button size="sm" variant="ghost" onPress={() => { setAddingType(null); setNewLabel(""); setNewContent(""); setNewDescription(""); }}>
                Cancel
              </Button>
              <Button size="sm" variant="secondary" onPress={handleAddManual} isDisabled={!newLabel.trim()}>
                <Plus className="w-3 h-3 mr-1 inline" />
                Add
              </Button>
            </div>
          </div>
        </Card>
      )}

      {/* Evidence cards grid */}
      {evidence.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
          {evidence.map((item) => {
            const config = getTypeConfig(item.type);
            const Icon = config.icon;
            return (
              <Card key={item.id} className="bg-surface border border-border">
                <div className="p-3 flex flex-col gap-2">
                  <div className="flex items-start justify-between gap-2">
                    <div className="flex items-center gap-2 min-w-0">
                      <div className="shrink-0 p-1 rounded bg-surface-secondary">
                        <Icon className="w-3.5 h-3.5 text-default-500" />
                      </div>
                      <div className="min-w-0">
                        <p className="text-xs font-medium truncate">{item.label}</p>
                        <span className="text-[10px] text-default-400 uppercase tracking-wider">{config.label}</span>
                      </div>
                    </div>
                    <Button
                      isIconOnly
                      size="sm"
                      variant="ghost"
                      className="text-danger shrink-0 w-6 h-6"
                      onPress={() => removeEvidence(item.id)}
                    >
                      <Trash2 className="w-3 h-3" />
                    </Button>
                  </div>

                  {/* Thumbnail preview */}
                  {item.thumbnail && (
                    <div className="rounded border border-border overflow-hidden bg-surface-secondary">
                      {/* eslint-disable-next-line @next/next/no-img-element */}
                      <img src={item.thumbnail} alt={item.label} className="w-full h-24 object-cover" />
                    </div>
                  )}

                  {/* Content / Link */}
                  {item.content && (
                    <div className="text-xs text-default-500 truncate">
                      {isUrl(item.type) ? (
                        <a href={item.content} target="_blank" rel="noopener noreferrer" className="text-primary hover:underline flex items-center gap-1">
                          <ExternalLink className="w-3 h-3 shrink-0 inline" />
                          <span className="truncate">{item.content}</span>
                        </a>
                      ) : (
                        <p className="line-clamp-2">{item.content}</p>
                      )}
                    </div>
                  )}

                  {item.description && (
                    <p className="text-[11px] text-default-400 line-clamp-2">{item.description}</p>
                  )}
                </div>
              </Card>
            );
          })}
        </div>
      )}

      {evidence.length === 0 && !addingType && (
        <p className="text-xs text-default-400 italic">No evidence attached yet.</p>
      )}
    </div>
  );
}
