"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { Accordion, TextField, Input, Button, TextArea, Select, ListBox, CheckboxGroup, Checkbox, Card, Separator, Label } from "@heroui/react";
import { Plus, Trash2, Save, ArrowRight, ArrowLeft, ChevronDown, Link2, Unlink } from "lucide-react";
import { useEffect, useCallback, useState, useMemo, useRef } from "react";
import { AiAuditData, PromptLogEntry, EvidenceItem } from "@/types/project";
import EvidenceSection from "./EvidenceSection";
import { useUnsavedChanges } from "@/lib/useUnsavedChanges";
import ConfirmDeleteModal from "./ConfirmDeleteModal";
import MemberSelect from "./MemberSelect";
import { useFormDraft } from "@/hooks/useFormDraft";

export default function Step4Form({ projectId }: { projectId: string }) {
  const { projects, updateAiAudit } = useProjectStore();
  const project = projects[projectId];
  const [deleteTarget, setDeleteTarget] = useState<{ index: number; name: string; type: string } | null>(null);

  const { control, handleSubmit, formState: { isDirty }, reset, watch, setValue } = useForm<AiAuditData>({
    defaultValues: {
      toolsUsed: [],
      usageTargetsText: "",
      auditEntries: [],
      usageMatrix: [],
      issues: [],
      verificationMethodsText: "",
      personalContributionText: "",
      groupContributions: []
    }
  });

  const auditEntries = watch("auditEntries") || [];

  const { fields: auditFields, append: appendAudit, remove: removeAudit } = useFieldArray({
    control,
    name: "auditEntries"
  });

  const { fields: issueFields, append: appendIssue, remove: removeIssue } = useFieldArray({
    control,
    name: "issues"
  });

  const { fields: matrixFields, append: appendMatrix, remove: removeMatrix } = useFieldArray({
    control,
    name: "usageMatrix"
  });

  const { fields: groupFields, append: appendGroup, remove: removeGroup } = useFieldArray({
    control,
    name: "groupContributions"
  });

  const originalData = {
    toolsUsed: project?.aiAudit?.toolsUsed || [],
    usageTargetsText: project?.aiAudit?.usageTargetsText || "",
    auditEntries: project?.aiAudit?.auditEntries || [],
    usageMatrix: project?.aiAudit?.usageMatrix || [],
    issues: project?.aiAudit?.issues || [],
    verificationMethodsText: project?.aiAudit?.verificationMethodsText || "",
    personalContributionText: project?.aiAudit?.personalContributionText || "",
    groupContributions: project?.aiAudit?.groupContributions || []
  };

  const { DraftStatusIndicator, isActuallyDirty } = useFormDraft({
    projectId,
    stepKey: "step4",
    watch,
    reset,
    originalData
  });

  const onSubmit = (data: AiAuditData) => {
    updateAiAudit(data);
    reset(data);
  };

  const handleSaveForm = useCallback(async () => {
    let success = false;
    await new Promise<void>((resolve) => {
      handleSubmit(
        (data) => {
          onSubmit(data);
          success = true;
          resolve();
        },
        () => {
          success = false;
          resolve();
        }
      )();
    });
    return success;
  }, [handleSubmit, onSubmit]);

  const saveHandlerRef = useRef(handleSaveForm);
  useEffect(() => {
    saveHandlerRef.current = handleSaveForm;
  }, [handleSaveForm]);

  useEffect(() => {
    const { registerSaveHandler } = useProjectStore.getState();
    registerSaveHandler(async () => saveHandlerRef.current());
    return () => registerSaveHandler(null);
  }, []);

  const { UnsavedModal, guardNavigation } = useUnsavedChanges({
    isDirty: isActuallyDirty,
    onSave: handleSubmit(onSubmit),
  });

  const aiToolOptions = ["ChatGPT", "Gemini", "Claude", "GitHub Copilot", "Cursor", "Antigravity", "Microsoft Copilot", "Perplexity"];
  const usageLevelOptions = ["Không dùng AI", "AI hỗ trợ ít", "AI hỗ trợ nhiều", "AI sinh chính"];

  // Get prompt log entries for linking
  const projectPrompts = project?.prompts;
  const promptEntries: PromptLogEntry[] = useMemo(() => projectPrompts || [], [projectPrompts]);

  const handleLinkPrompts = useCallback((index: number, selectedIds: string[]) => {
    setValue(`auditEntries.${index}.linkedPromptIds`, selectedIds, { shouldDirty: true });

    if (selectedIds.length === 0) return;

    // Auto-populate from linked prompts
    const linkedPrompts = promptEntries.filter(p => selectedIds.includes(p.id));
    if (linkedPrompts.length === 0) return;

    const first = linkedPrompts[0];
    setValue(`auditEntries.${index}.aiTool`, first.aiTool, { shouldDirty: true });

    if (linkedPrompts.length === 1) {
      setValue(`auditEntries.${index}.prompt`, first.promptText, { shouldDirty: true });
      setValue(`auditEntries.${index}.aiResponseSummary`, first.aiResponse, { shouldDirty: true });
      setValue(`auditEntries.${index}.usedContent`, first.appliedResult, { shouldDirty: true });
      setValue(`auditEntries.${index}.modifications`, first.improvements, { shouldDirty: true });
      setValue(`auditEntries.${index}.purpose`, first.purpose, { shouldDirty: true });
    } else {
      // Concatenate multiple prompts
      const separator = "\n\n---\n\n";
      setValue(`auditEntries.${index}.prompt`,
        linkedPrompts.map((p, i) => `[Prompt ${i + 1}: ${p.purpose}]\n${p.promptText}`).join(separator),
        { shouldDirty: true });
      setValue(`auditEntries.${index}.aiResponseSummary`,
        linkedPrompts.map((p, i) => `[Prompt ${i + 1}] ${p.aiResponse}`).join(separator),
        { shouldDirty: true });
      setValue(`auditEntries.${index}.usedContent`,
        linkedPrompts.map((p, i) => `[Prompt ${i + 1}] ${p.appliedResult}`).join(separator),
        { shouldDirty: true });
      setValue(`auditEntries.${index}.modifications`,
        linkedPrompts.map((p, i) => `[Prompt ${i + 1}] ${p.improvements}`).join(separator),
        { shouldDirty: true });
      setValue(`auditEntries.${index}.purpose`,
        linkedPrompts.map(p => p.purpose).join("; "),
        { shouldDirty: true });
    }
  }, [promptEntries, setValue]);

  const handleClearLinks = useCallback((index: number) => {
    setValue(`auditEntries.${index}.linkedPromptIds`, [], { shouldDirty: true });
  }, [setValue]);

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
      <div className="flex justify-between items-center bg-surface-secondary/20 p-4 rounded-xl border border-border/80">
        <h3 className="text-lg font-bold">AI Audit & Contributions</h3>
        <DraftStatusIndicator />
      </div>
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">General AI Usage</h3>
          <Controller name="toolsUsed" control={control} render={({ field: { value, onChange } }) => (
            <CheckboxGroup value={value} onChange={onChange}>
              <Label>Select AI Tools Used</Label>
              <div className="flex flex-wrap gap-4 mt-2">
                {aiToolOptions.map(tool => (
                  <Checkbox key={tool} value={tool}>
                    <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                    <Checkbox.Content>{tool}</Checkbox.Content>
                  </Checkbox>
                ))}
                <Checkbox value="Other">
                  <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                  <Checkbox.Content>Other</Checkbox.Content>
                </Checkbox>
              </div>
            </CheckboxGroup>
          )} />

          <Controller name="usageTargetsText" control={control} render={({ field }) => (
            <TextField>
              <Label>Usage Targets (What did you use AI for?)</Label>
              <TextArea {...field} placeholder="e.g. Analysis, Design, Code Generation..." />
            </TextField>
          )} />
        </div>
      </Card>

      <Card>
        <div className="flex flex-col gap-6 p-6">
          <div className="flex justify-between items-center">
            <h3 className="text-lg font-semibold">Detailed Audit Entries</h3>
            <Button size="sm" variant="secondary" onPress={() => appendAudit({
              id: Math.random().toString(36).substr(2, 9),
              date: new Date().toISOString().split('T')[0],
              aiTool: "ChatGPT",
              purpose: "",
              category: "Coding",
              usageLevel: "Sinh chính nội dung",
              prompt: "",
              aiResponseSummary: "",
              usedContent: "",
              modifications: "",
              evidence: [],
              lessonsLearned: "",
              linkedPromptIds: []
            })}>
              <Plus className="w-4 h-4 mr-2 inline" />
              Add Audit Entry
            </Button>
          </div>
          <Separator />

          {auditFields.length === 0 ? (
            <p className="text-sm text-default-400 italic">No detailed audit logs yet.</p>
          ) : (
            <Accordion className="w-full">
              {auditFields.map((field, index) => (
                <Accordion.Item key={field.id}>
                  <Accordion.Heading>
                    <Accordion.Trigger className="bg-surface-secondary px-4 py-3 rounded-lg mb-2">
                      <div className="flex flex-col text-left">
                        <span className="text-sm font-medium">{auditEntries[index]?.purpose || `Entry ${index + 1}`}</span>
                        <div className="flex items-center gap-2">
                          <span className="text-xs text-default-500">{auditEntries[index]?.date}</span>
                          {auditEntries[index]?.linkedPromptIds?.length > 0 && (
                            <span className="text-[10px] text-primary bg-primary/10 px-1.5 py-0.5 rounded-full flex items-center gap-1">
                              <Link2 className="w-2.5 h-2.5" />
                              Linked
                            </span>
                          )}
                        </div>
                      </div>
                      <Accordion.Indicator>
                        <ChevronDown className="w-4 h-4" />
                      </Accordion.Indicator>
                    </Accordion.Trigger>
                  </Accordion.Heading>
                  <Accordion.Panel>
                    <Accordion.Body className="bg-surface border border-border p-4 rounded-lg mb-4">
                      <div className="flex flex-col gap-4 pb-4">

                        {/* ─── Prompt Log Linking ─── */}
                        {promptEntries.length > 0 && (
                          <div className="border border-primary/20 bg-primary/5 rounded-lg p-4">
                            <div className="flex justify-between items-center mb-3">
                              <div className="flex items-center gap-2">
                                <Link2 className="w-4 h-4 text-primary" />
                                <h4 className="text-sm font-semibold text-primary">Link from Prompt Log</h4>
                              </div>
                              {watch(`auditEntries.${index}.linkedPromptIds`)?.length > 0 && (
                                <Button size="sm" variant="ghost" className="text-default-500" onPress={() => handleClearLinks(index)}>
                                  <Unlink className="w-3 h-3 mr-1 inline" />
                                  Clear Links
                                </Button>
                              )}
                            </div>
                            <p className="text-xs text-default-400 mb-3">Select prompt entries to auto-fill shared fields. You can still edit them after linking.</p>
                            <Controller name={`auditEntries.${index}.linkedPromptIds`} control={control} render={({ field: { value } }) => (
                              <CheckboxGroup value={value || []} onChange={(ids) => handleLinkPrompts(index, ids)}>
                                <Label>Select your interests</Label>
                                <div className="flex flex-col gap-2 max-h-40 overflow-y-auto">
                                  {promptEntries.map(p => (
                                    <Checkbox key={p.id} value={p.id}>
                                      <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                                      <Checkbox.Content>
                                        <span className="text-xs">
                                          <strong>{p.purpose || "Untitled"}</strong>
                                          <span className="text-default-400 ml-2">{p.aiTool} · {p.date}</span>
                                        </span>
                                      </Checkbox.Content>
                                    </Checkbox>
                                  ))}
                                </div>
                              </CheckboxGroup>
                            )} />
                          </div>
                        )}

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <Controller name={`auditEntries.${index}.date`} control={control} render={({ field }) => (
                            <TextField>
                              <Label>Date</Label>
                              <Input {...field} type="date" />
                            </TextField>
                          )} />
                          <Controller name={`auditEntries.${index}.aiTool`} control={control} render={({ field: { value, onChange } }) => (
                            <Select selectedKey={value} onSelectionChange={onChange}>
                              <Label>AI Tool</Label>
                              <Select.Trigger>
                                <Select.Value />
                                <Select.Indicator />
                              </Select.Trigger>
                              <Select.Popover>
                                <ListBox>
                                  {aiToolOptions.map(t => <ListBox.Item key={t} id={t} textValue={t}>{t}<ListBox.ItemIndicator /></ListBox.Item>)}
                                </ListBox>
                              </Select.Popover>
                            </Select>
                          )} />
                          <Controller name={`auditEntries.${index}.category`} control={control} render={({ field: { value, onChange } }) => (
                            <Select selectedKey={value} onSelectionChange={onChange}>
                              <Label>Task Area</Label>
                              <Select.Trigger>
                                <Select.Value />
                                <Select.Indicator />
                              </Select.Trigger>
                              <Select.Popover>
                                <ListBox>
                                  {["Requirement", "Design", "Database", "Frontend", "Backend", "Testing", "Debug", "Report"].map(c => <ListBox.Item key={c} id={c} textValue={c}>{c}<ListBox.ItemIndicator /></ListBox.Item>)}
                                </ListBox>
                              </Select.Popover>
                            </Select>
                          )} />
                          <Controller name={`auditEntries.${index}.usageLevel`} control={control} render={({ field: { value, onChange } }) => (
                            <Select selectedKey={value} onSelectionChange={onChange}>
                              <Label>Usage Level</Label>
                              <Select.Trigger>
                                <Select.Value />
                                <Select.Indicator />
                              </Select.Trigger>
                              <Select.Popover>
                                <ListBox>
                                  {["Hỗ trợ ý tưởng", "Hỗ trợ một phần", "Hỗ trợ nhiều", "Sinh chính nội dung"].map(l => <ListBox.Item key={l} id={l} textValue={l}>{l}<ListBox.ItemIndicator /></ListBox.Item>)}
                                </ListBox>
                              </Select.Popover>
                            </Select>
                          )} />
                        </div>
                        <Controller name={`auditEntries.${index}.purpose`} control={control} render={({ field }) => (
                          <TextField>
                            <Label>Purpose</Label>
                            <Input {...field} />
                          </TextField>
                        )} />
                        <Controller name={`auditEntries.${index}.prompt`} control={control} render={({ field }) => (
                          <TextField className="font-mono text-sm">
                            <Label>Prompt Used</Label>
                            <TextArea {...field} />
                          </TextField>
                        )} />
                        <Controller name={`auditEntries.${index}.aiResponseSummary`} control={control} render={({ field }) => (
                          <TextField>
                            <Label>AI Response Summary</Label>
                            <TextArea {...field} />
                          </TextField>
                        )} />
                        <Controller name={`auditEntries.${index}.usedContent`} control={control} render={({ field }) => (
                          <TextField>
                            <Label>What part was used?</Label>
                            <TextArea {...field} />
                          </TextField>
                        )} />
                        <Controller name={`auditEntries.${index}.modifications`} control={control} render={({ field }) => (
                          <TextField>
                            <Label>Your modifications / improvements</Label>
                            <TextArea {...field} />
                          </TextField>
                        )} />

                        {/* ─── Evidence Section ─── */}
                        <Separator />
                        <Controller
                          name={`auditEntries.${index}.evidence`}
                          control={control}
                          render={({ field: { value, onChange } }) => (
                            <EvidenceSection
                              evidence={value || []}
                              onChange={(items: EvidenceItem[]) => onChange(items)}
                            />
                          )}
                        />

                        <Separator />
                        <Controller name={`auditEntries.${index}.lessonsLearned`} control={control} render={({ field }) => (
                          <TextField>
                            <Label>Personal / Group reflections for this entry</Label>
                            <TextArea {...field} />
                          </TextField>
                        )} />
                        <div className="flex justify-end">
                          <Button size="sm" variant="ghost" className="text-danger" onPress={() => removeAudit(index)}>
                            <Trash2 className="w-4 h-4 mr-2 inline" />
                            Delete Entry
                          </Button>
                        </div>
                      </div>
                    </Accordion.Body>
                  </Accordion.Panel>
                </Accordion.Item>
              ))}
            </Accordion>
          )}
        </div>
      </Card>

      <Card>
        <div className="flex flex-col gap-6 p-6">
          <div className="flex justify-between items-center">
            <h3 className="text-lg font-semibold">Usage Matrix</h3>
            <Button size="sm" variant="secondary" onPress={() => appendMatrix({ id: Math.random().toString(36).substr(2, 9), category: "", usageLevel: "Không dùng AI", notes: "" })}>
              <Plus className="w-4 h-4 mr-2 inline" />
              Add Row
            </Button>
          </div>
          <div className="flex flex-col gap-3">
            {matrixFields.map((field, index) => (
              <div key={field.id} className="flex gap-2 items-start">
                <Controller name={`usageMatrix.${index}.category`} control={control} render={({ field }) => (
                  <TextField className="w-1/3">
                    <Input {...field} placeholder="Category (e.g. Code Frontend)" />
                  </TextField>
                )} />
                <Controller name={`usageMatrix.${index}.usageLevel`} control={control} render={({ field: { value, onChange } }) => (
                  <Select selectedKey={value} onSelectionChange={onChange} className="w-1/4">
                    <Select.Trigger>
                      <Select.Value />
                      <Select.Indicator />
                    </Select.Trigger>
                    <Select.Popover>
                      <ListBox>
                        {usageLevelOptions.map(l => <ListBox.Item key={l} id={l} textValue={l}>{l}<ListBox.ItemIndicator /></ListBox.Item>)}
                      </ListBox>
                    </Select.Popover>
                  </Select>
                )} />
                <Controller name={`usageMatrix.${index}.notes`} control={control} render={({ field }) => (
                  <TextField className="flex-1">
                    <Input {...field} placeholder="Notes" />
                  </TextField>
                )} />
                <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeMatrix(index)} aria-label="Remove row">
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            ))}
          </div>
        </div>
      </Card>

      <Card>
        <div className="flex flex-col gap-6 p-6">
          <div className="flex justify-between items-center">
            <h3 className="text-lg font-semibold">AI Issues / Limitations</h3>
            <Button size="sm" variant="secondary" onPress={() => appendIssue({ id: Math.random().toString(36).substr(2, 9), description: "", detectionMethod: "", resolution: "" })}>
              <Plus className="w-4 h-4 mr-2 inline" />
              Add Issue
            </Button>
          </div>
          <div className="flex flex-col gap-3">
            {issueFields.map((field, index) => (
              <div key={field.id} className="flex gap-2 items-start">
                <Controller name={`issues.${index}.description`} control={control} render={({ field }) => (
                  <TextField className="flex-1">
                    <Input {...field} placeholder="Issue description" />
                  </TextField>
                )} />
                <Controller name={`issues.${index}.detectionMethod`} control={control} render={({ field }) => (
                  <TextField className="flex-1">
                    <Input {...field} placeholder="How was it detected?" />
                  </TextField>
                )} />
                <Controller name={`issues.${index}.resolution`} control={control} render={({ field }) => (
                  <TextField className="flex-1">
                    <Input {...field} placeholder="Resolution" />
                  </TextField>
                )} />
                <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeIssue(index)} aria-label="Remove issue">
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            ))}
          </div>
        </div>
      </Card>

      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Verification & Contributions</h3>
          <Controller name="verificationMethodsText" control={control} render={({ field }) => (
            <TextField>
              <Label>Verification Methods (How did you verify AI output?)</Label>
              <TextArea {...field} />
            </TextField>
          )} />
          <Separator />
          <h4 className="text-md font-semibold">Personal Contribution (Individual Project)</h4>
          <Controller name="personalContributionText" control={control} render={({ field }) => (
            <TextField>
              <TextArea {...field} placeholder="Describe your personal work vs AI assistance..." />
            </TextField>
          )} />
          <Separator />
          <div className="flex justify-between items-center">
            <h4 className="text-md font-semibold">Group Contributions</h4>
            <Button size="sm" variant="secondary" onPress={() => appendGroup({ id: Math.random().toString(36).substr(2, 9), memberName: "", memberId: "", tasks: "", aiUsed: false, evidence: "" })}>
              <Plus className="w-4 h-4 mr-2 inline" />
              Add Member Entry
            </Button>
          </div>
          <div className="flex flex-col gap-3">
            {groupFields.map((field, index) => (
              <div key={field.id} className="flex flex-col gap-2 p-3 border border-border rounded-lg bg-surface-secondary/20">
                <div className="flex justify-between">
                  <div className="flex gap-4 flex-1 items-center">
                    <div className="w-1/2">
                      <MemberSelect
                        members={project.members || []}
                        selectedId={(() => {
                          const mName = watch(`groupContributions.${index}.memberName`);
                          const mId = watch(`groupContributions.${index}.memberId`);
                          const match = (project.members || []).find(m => m.name === mName && m.studentId === mId);
                          return match ? match.id : "";
                        })()}
                        onChange={(val) => {
                          const selected = (project.members || []).find((m) => m.id === val);
                          if (selected) {
                            setValue(`groupContributions.${index}.memberName`, selected.name, { shouldDirty: true });
                            setValue(`groupContributions.${index}.memberId`, selected.studentId, { shouldDirty: true });
                          } else {
                            setValue(`groupContributions.${index}.memberName`, "", { shouldDirty: true });
                            setValue(`groupContributions.${index}.memberId`, "", { shouldDirty: true });
                          }
                        }}
                        placeholder="Select team member..."
                      />
                    </div>
                    <Controller name={`groupContributions.${index}.aiUsed`} control={control} render={({ field: { value, onChange } }) => (
                      <Checkbox isSelected={value} onChange={onChange}>
                        <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                        <Checkbox.Content>Used AI?</Checkbox.Content>
                      </Checkbox>
                    )} />
                  </div>
                  <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeGroup(index)} aria-label="Remove member">
                    <Trash2 className="w-4 h-4" />
                  </Button>
                </div>
                <Controller name={`groupContributions.${index}.tasks`} control={control} render={({ field }) => (
                  <TextField>
                    <Input {...field} placeholder="Main Tasks" />
                  </TextField>
                )} />
                <Controller name={`groupContributions.${index}.evidence`} control={control} render={({ field }) => (
                  <TextField>
                    <Input {...field} placeholder="Evidence of contribution" />
                  </TextField>
                )} />
              </div>
            ))}
          </div>
        </div>
      </Card>

      <div className="flex justify-between items-center mt-4">
        <Button onPress={() => guardNavigation(`/project/${projectId}/workspace/step3`)} variant="secondary">
          <ArrowLeft className="w-4 h-4 mr-2 inline" />
          Back
        </Button>
        <div className="flex gap-2">
          <Button type="submit" variant={isActuallyDirty ? "secondary" : "ghost"}>
            <Save className="w-4 h-4 mr-2 inline" />
            {isActuallyDirty ? "Save Changes" : "Saved"}
          </Button>
          <Button onPress={() => guardNavigation(`/project/${projectId}/workspace/step5`)} variant="secondary">
            Next Step
            <ArrowRight className="w-4 h-4 ml-2 inline" />
          </Button>
        </div>
      </div>
      <UnsavedModal />
      <ConfirmDeleteModal
        isOpen={deleteTarget !== null}
        onClose={() => setDeleteTarget(null)}
        onConfirm={() => {
          if (!deleteTarget) return;
          if (deleteTarget.type === 'audit') removeAudit(deleteTarget.index);
          if (deleteTarget.type === 'issue') removeIssue(deleteTarget.index);
          if (deleteTarget.type === 'matrix') removeMatrix(deleteTarget.index);
          if (deleteTarget.type === 'group') removeGroup(deleteTarget.index);
          setDeleteTarget(null);
        }}
        itemName={deleteTarget?.name}
        title={`Delete ${deleteTarget?.type === 'audit' ? 'Audit Entry' : deleteTarget?.type === 'issue' ? 'Issue' : deleteTarget?.type === 'matrix' ? 'Matrix Row' : 'Contribution'}`}
      />
    </form>
  );
}
