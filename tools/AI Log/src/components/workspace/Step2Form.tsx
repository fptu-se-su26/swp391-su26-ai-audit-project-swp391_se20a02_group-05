"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { Accordion, TextField, Input, Button, Card, TextArea, Checkbox, Select, ListBox, Label, Separator } from "@heroui/react";
import { Plus, Trash2, Save, ArrowRight, ArrowLeft, ChevronDown, CheckCircle, XCircle, Rocket, ClipboardList, Sparkles } from "lucide-react";
import { useEffect, useState, useCallback, useRef } from "react";
import { ChangelogEntry, ChangelogSummary } from "@/types/project";
import { useUnsavedChanges } from "@/lib/useUnsavedChanges";
import ConfirmDeleteModal from "./ConfirmDeleteModal";
import EvidenceSection from "./EvidenceSection";
import { useFormDraft } from "@/hooks/useFormDraft";

interface Step2FormData {
  changelogs: ChangelogEntry[];
  changelogSummary: ChangelogSummary;
}

const phaseDefaults: Record<string, string> = {
  "Phase 01": "Khởi tạo project",
  "Phase 02": "Phân tích yêu cầu",
  "Phase 03": "Thiết kế hệ thống",
  "Phase 04": "Implementation",
  "Phase 05": "Testing & Debug",
  "Phase 06": "Hoàn thiện báo cáo và demo",
};

export default function Step2Form({ projectId }: { projectId: string }) {
  const { projects, updateChangelogs, updateChangelogSummary } = useProjectStore();
  const project = projects[projectId];
  const [deleteTarget, setDeleteTarget] = useState<{ index: number; name: string } | null>(null);

  const { control, handleSubmit, formState: { isDirty }, reset, watch, setValue } = useForm<Step2FormData>({
    defaultValues: {
      changelogs: [],
      changelogSummary: {
        completedFeatures: "",
        unfinishedFeatures: "",
        majorImprovements: "",
        overallSummary: "",
        futureImprovements: "",
      }
    }
  });

  const changelogs = watch("changelogs") || [];

  const { fields, append, remove } = useFieldArray({
    control,
    name: "changelogs"
  });

  const originalData = {
    changelogs: (project?.changelogs || []).map((c) => ({
      ...c,
      startDate: c.startDate || c.date || new Date().toISOString().split("T")[0],
      endDate: c.endDate || c.date || new Date().toISOString().split("T")[0],
      phaseDescription: c.phaseDescription || c.notes.split("\n")[0] || phaseDefaults[c.phaseName] || ""
    })),
    changelogSummary: project?.changelogSummary || {
      completedFeatures: "",
      unfinishedFeatures: "",
      majorImprovements: "",
      overallSummary: "",
      futureImprovements: "",
    }
  };

  const { DraftStatusIndicator, isActuallyDirty } = useFormDraft({
    projectId,
    stepKey: "step2",
    watch,
    reset,
    originalData
  });

  const onSubmit = (data: Step2FormData) => {
    updateChangelogs(data.changelogs);
    updateChangelogSummary(data.changelogSummary);
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

  const handleAddPhase = () => {
    const nextPhaseNum = fields.length + 1;
    const nextPhaseName = nextPhaseNum <= 6 ? `Phase 0${nextPhaseNum}` : `Phase ${nextPhaseNum}`;
    append({
      id: Math.random().toString(36).substr(2, 9),
      phaseName: nextPhaseName,
      startDate: new Date().toISOString().split('T')[0],
      endDate: new Date().toISOString().split('T')[0],
      phaseDescription: phaseDefaults[nextPhaseName] || "",
      date: `${new Date().toISOString().split('T')[0]} ~ ${new Date().toISOString().split('T')[0]}`,
      status: "In Progress",
      completedChecklist: [],
      changes: [],
      aiSupport: { used: false, description: "" },
      evidence: [],
      evidenceLink: "",
      notes: ""
    });
  };

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
      <div className="flex justify-between items-center bg-surface-secondary/20 p-4 rounded-xl border border-border/80">
        <div className="flex items-center gap-3">
          <h3 className="text-lg font-bold">Project Phases</h3>
          <DraftStatusIndicator />
        </div>
        <Button size="sm" variant="secondary" onPress={handleAddPhase}>
          <Plus className="w-4 h-4 mr-2 inline" />
          Add Phase
        </Button>
      </div>

      {fields.length === 0 ? (
        <Card className="bg-surface-secondary/50 border-dashed border-2 border-border shadow-none">
          <div className="p-12 flex flex-col items-center justify-center text-center">
            <p className="text-default-500 mb-4">No phases logged yet.</p>
            <Button size="sm" onPress={handleAddPhase}>Create First Phase</Button>
          </div>
        </Card>
      ) : (
        <Accordion className="w-full">
          {fields.map((field, index) => (
            <Accordion.Item key={field.id}>
              <Accordion.Heading>
                <Accordion.Trigger className="bg-surface-secondary px-4 py-3 rounded-lg mb-2">
                  <div className="flex flex-col text-left">
                    <span className="text-sm font-medium">{changelogs[index]?.phaseName}</span>
                    <span className="text-xs text-default-500">{changelogs[index]?.status}</span>
                  </div>
                  <Accordion.Indicator>
                    <ChevronDown className="w-4 h-4" />
                  </Accordion.Indicator>
                </Accordion.Trigger>
              </Accordion.Heading>
              <Accordion.Panel>
                <Accordion.Body className="bg-surface border border-border p-4 rounded-lg mb-4">
                  <div className="flex flex-col gap-6 pb-4">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                      <Controller
                        name={`changelogs.${index}.phaseName`}
                        control={control}
                        render={({ field: { value, onChange } }) => (
                          <Select
                            selectedKey={value}
                            onSelectionChange={(key) => {
                              onChange(key);
                              const desc = phaseDefaults[key as string];
                              const currentDesc = watch(`changelogs.${index}.phaseDescription`);
                              if (!currentDesc || Object.values(phaseDefaults).includes(currentDesc)) {
                                setValue(`changelogs.${index}.phaseDescription`, desc || "", { shouldDirty: true });
                              }
                            }}
                          >
                            <Label>Phase</Label>
                            <Select.Trigger>
                              <Select.Value />
                              <Select.Indicator />
                            </Select.Trigger>
                            <Select.Popover>
                              <ListBox>
                                {Object.keys(phaseDefaults).map((p) => (
                                  <ListBox.Item key={p} id={p} textValue={p}>
                                    {p}
                                  </ListBox.Item>
                                ))}
                              </ListBox>
                            </Select.Popover>
                          </Select>
                        )}
                      />

                      <div className="flex gap-2">
                        <Controller
                          name={`changelogs.${index}.startDate`}
                          control={control}
                          render={({ field }) => (
                            <TextField className="flex-1">
                              <Label>Start Date</Label>
                              <Input
                                {...field}
                                type="date"
                                value={field.value ? field.value.split("T")[0] : ""}
                                onChange={(e) => {
                                  field.onChange(e.target.value);
                                  const end = watch(`changelogs.${index}.endDate`) || "";
                                  setValue(`changelogs.${index}.date`, `${e.target.value} ~ ${end}`, { shouldDirty: true });
                                }}
                              />
                            </TextField>
                          )}
                        />
                        <Controller
                          name={`changelogs.${index}.endDate`}
                          control={control}
                          render={({ field }) => (
                            <TextField className="flex-1">
                              <Label>End Date</Label>
                              <Input
                                {...field}
                                type="date"
                                value={field.value ? field.value.split("T")[0] : ""}
                                onChange={(e) => {
                                  field.onChange(e.target.value);
                                  const start = watch(`changelogs.${index}.startDate`) || "";
                                  setValue(`changelogs.${index}.date`, `${start} ~ ${e.target.value}`, { shouldDirty: true });
                                }}
                              />
                            </TextField>
                          )}
                        />
                      </div>

                      <Controller
                        name={`changelogs.${index}.status`}
                        control={control}
                        render={({ field: { value, onChange } }) => (
                          <Select selectedKey={value} onSelectionChange={onChange}>
                            <Label>Status</Label>
                            <Select.Trigger>
                              <Select.Value />
                              <Select.Indicator />
                            </Select.Trigger>
                            <Select.Popover>
                              <ListBox>
                                <ListBox.Item id="Not Started" textValue="Not Started">Not Started<ListBox.ItemIndicator /></ListBox.Item>
                                <ListBox.Item id="In Progress" textValue="In Progress">In Progress<ListBox.ItemIndicator /></ListBox.Item>
                                <ListBox.Item id="Completed" textValue="Completed">Completed<ListBox.ItemIndicator /></ListBox.Item>
                              </ListBox>
                            </Select.Popover>
                          </Select>
                        )}
                      />

                      <Controller
                        name={`changelogs.${index}.phaseDescription`}
                        control={control}
                        render={({ field }) => (
                          <TextField className="md:col-span-3">
                            <Label>Phase Description</Label>
                            <Input {...field} placeholder="Description of this phase..." />
                          </TextField>
                        )}
                      />
                    </div>

                    <div className="border border-border rounded-lg p-4 bg-surface-secondary/20">
                      <div className="flex justify-between items-center mb-4">
                        <h4 className="text-sm font-semibold">Changes / Tasks</h4>
                        <ChangesListArray control={control} nestIndex={index} />
                      </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <Controller name={`changelogs.${index}.aiSupport.used`} control={control} render={({ field: { value, onChange } }) => (
                        <Checkbox isSelected={value} onChange={onChange}>
                          <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                          <Checkbox.Content><Label>AI assisted in this phase?</Label></Checkbox.Content>
                        </Checkbox>
                      )} />
                      {watch(`changelogs.${index}.aiSupport.used`) && (
                        <Controller name={`changelogs.${index}.aiSupport.description`} control={control} render={({ field }) => (
                          <TextField className="md:col-span-2">
                            <Label>Describe AI Support</Label>
                            <Input {...field} />
                          </TextField>
                        )} />
                      )}
                    </div>

                    <Controller 
                      name={`changelogs.${index}.evidence`} 
                      control={control} 
                      render={({ field: { value, onChange } }) => (
                        <EvidenceSection 
                          evidence={value || []} 
                          onChange={onChange} 
                        />
                      )} 
                    />

                    <Controller name={`changelogs.${index}.notes`} control={control} render={({ field }) => (
                      <TextField>
                        <Label>Notes</Label>
                        <TextArea {...field} />
                      </TextField>
                    )} />

                    <div className="flex justify-end mt-2">
                      <Button size="sm" variant="ghost" className="text-danger" onPress={() => remove(index)}>
                        <Trash2 className="w-4 h-4 mr-2 inline" />
                        Delete Phase
                      </Button>
                    </div>
                  </div>
                </Accordion.Body>
              </Accordion.Panel>
            </Accordion.Item>
          ))}
        </Accordion>
      )}

      {/* ─── Final Project Change Summary ──────────────────── */}
      <Separator className="my-2" />

      <Card>
        <div className="flex flex-col gap-4 p-6">
          <h3 className="text-lg font-semibold">Final Project Change Summary</h3>
          <p className="text-xs text-default-400">Summarize the overall project changes for the CHANGELOG.md final section.</p>

          <Accordion className="w-full">
            <Accordion.Item>
              <Accordion.Heading>
                <Accordion.Trigger className="bg-surface-secondary px-4 py-3 rounded-lg mb-1">
                  <div className="flex items-center gap-2 text-left">
                    <CheckCircle className="w-4 h-4 text-success shrink-0" />
                    <span className="text-sm font-medium">Completed Features</span>
                  </div>
                  <Accordion.Indicator><ChevronDown className="w-4 h-4" /></Accordion.Indicator>
                </Accordion.Trigger>
              </Accordion.Heading>
              <Accordion.Panel>
                <Accordion.Body className="p-4">
                  <Controller name="changelogSummary.completedFeatures" control={control} render={({ field }) => (
                    <TextField aria-label="Completed Features">
                      <TextArea {...field} placeholder="List features that were completed successfully..." />
                    </TextField>
                  )} />
                </Accordion.Body>
              </Accordion.Panel>
            </Accordion.Item>

            <Accordion.Item>
              <Accordion.Heading>
                <Accordion.Trigger className="bg-surface-secondary px-4 py-3 rounded-lg mb-1">
                  <div className="flex items-center gap-2 text-left">
                    <XCircle className="w-4 h-4 text-danger shrink-0" />
                    <span className="text-sm font-medium">Unfinished Features</span>
                  </div>
                  <Accordion.Indicator><ChevronDown className="w-4 h-4" /></Accordion.Indicator>
                </Accordion.Trigger>
              </Accordion.Heading>
              <Accordion.Panel>
                <Accordion.Body className="p-4">
                  <Controller name="changelogSummary.unfinishedFeatures" control={control} render={({ field }) => (
                    <TextField aria-label="Unfinished Features">
                      <TextArea {...field} placeholder="List features that were not completed, with reasons..." />
                    </TextField>
                  )} />
                </Accordion.Body>
              </Accordion.Panel>
            </Accordion.Item>

            <Accordion.Item>
              <Accordion.Heading>
                <Accordion.Trigger className="bg-surface-secondary px-4 py-3 rounded-lg mb-1">
                  <div className="flex items-center gap-2 text-left">
                    <Rocket className="w-4 h-4 text-primary shrink-0" />
                    <span className="text-sm font-medium">Major Improvements</span>
                  </div>
                  <Accordion.Indicator><ChevronDown className="w-4 h-4" /></Accordion.Indicator>
                </Accordion.Trigger>
              </Accordion.Heading>
              <Accordion.Panel>
                <Accordion.Body className="p-4">
                  <Controller name="changelogSummary.majorImprovements" control={control} render={({ field }) => (
                    <TextField aria-label="Major Improvements">
                      <TextArea {...field} placeholder="Describe major improvements made during the project..." />
                    </TextField>
                  )} />
                </Accordion.Body>
              </Accordion.Panel>
            </Accordion.Item>

            <Accordion.Item>
              <Accordion.Heading>
                <Accordion.Trigger className="bg-surface-secondary px-4 py-3 rounded-lg mb-1">
                  <div className="flex items-center gap-2 text-left">
                    <ClipboardList className="w-4 h-4 text-secondary shrink-0" />
                    <span className="text-sm font-medium">Overall Project Summary</span>
                  </div>
                  <Accordion.Indicator><ChevronDown className="w-4 h-4" /></Accordion.Indicator>
                </Accordion.Trigger>
              </Accordion.Heading>
              <Accordion.Panel>
                <Accordion.Body className="p-4">
                  <Controller name="changelogSummary.overallSummary" control={control} render={({ field }) => (
                    <TextField aria-label="Overall Project Summary">
                      <TextArea {...field} placeholder="Provide an overall summary of the project..." />
                    </TextField>
                  )} />
                </Accordion.Body>
              </Accordion.Panel>
            </Accordion.Item>

            <Accordion.Item>
              <Accordion.Heading>
                <Accordion.Trigger className="bg-surface-secondary px-4 py-3 rounded-lg mb-1">
                  <div className="flex items-center gap-2 text-left">
                    <Sparkles className="w-4 h-4 text-warning shrink-0" />
                    <span className="text-sm font-medium">Future Improvements</span>
                  </div>
                  <Accordion.Indicator><ChevronDown className="w-4 h-4" /></Accordion.Indicator>
                </Accordion.Trigger>
              </Accordion.Heading>
              <Accordion.Panel>
                <Accordion.Body className="p-4">
                  <Controller name="changelogSummary.futureImprovements" control={control} render={({ field }) => (
                    <TextField aria-label="Future Improvements">
                      <TextArea {...field} placeholder="What would you improve in the next iteration?" />
                    </TextField>
                  )} />
                </Accordion.Body>
              </Accordion.Panel>
            </Accordion.Item>
          </Accordion>
        </div>
      </Card>

      <div className="flex justify-between items-center mt-4">
        <Button onPress={() => guardNavigation(`/project/${projectId}/workspace/step1`)} variant="secondary">
          <ArrowLeft className="w-4 h-4 mr-2 inline" />
          Back
        </Button>
        <div className="flex gap-2">
          <Button type="submit" variant={isActuallyDirty ? "secondary" : "ghost"}>
            <Save className="w-4 h-4 mr-2 inline" />
            {isActuallyDirty ? "Save Changes" : "Saved"}
          </Button>
          <Button onPress={() => guardNavigation(`/project/${projectId}/workspace/step3`)} variant="secondary">
            Next Step
            <ArrowRight className="w-4 h-4 ml-2 inline" />
          </Button>
        </div>
      </div>
      <UnsavedModal />
      <ConfirmDeleteModal
        isOpen={deleteTarget !== null}
        onClose={() => setDeleteTarget(null)}
        onConfirm={() => { if (deleteTarget) { remove(deleteTarget.index); setDeleteTarget(null); } }}
        itemName={deleteTarget?.name}
        title="Delete Phase"
      />
    </form>
  );
}

import { Control } from "react-hook-form";

// Sub-component to manage nested field array for changes
function ChangesListArray({ control, nestIndex }: { control: Control<Step2FormData>, nestIndex: number }) {
  const { fields, append, remove } = useFieldArray({
    control,
    name: `changelogs.${nestIndex}.changes`
  });

  return (
    <div className="flex flex-col gap-3 w-full">
      {fields.map((item, k) => (
        <div key={item.id} className="flex gap-2 items-start">
          <div className="flex-1 grid grid-cols-1 sm:grid-cols-4 gap-2">
            <Controller name={`changelogs.${nestIndex}.changes.${k}.content`} control={control} render={({ field }) => (
              <TextField aria-label="What changed?" className="sm:col-span-2">
                <Input {...field} placeholder="What changed?" />
              </TextField>
            )} />
            <Controller name={`changelogs.${nestIndex}.changes.${k}.author`} control={control} render={({ field }) => (
              <TextField aria-label="Author">
                <Input {...field} placeholder="Author" />
              </TextField>
            )} />
            <Controller name={`changelogs.${nestIndex}.changes.${k}.evidence`} control={control} render={({ field }) => (
              <TextField aria-label="Evidence">
                <Input {...field} placeholder="Evidence" />
              </TextField>
            )} />
          </div>
          <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => remove(k)} aria-label="Remove change item">
            <Trash2 className="w-4 h-4" />
          </Button>
        </div>
      ))}
      <div className="flex justify-end mt-1">
        <Button size="sm" variant="secondary" onPress={() => append({ id: Math.random().toString(36).substr(2, 9), content: "", author: "", files: "", evidence: "" })}>
          <Plus className="w-4 h-4 mr-2 inline" />
          Add Change Item
        </Button>
      </div>
    </div>
  );
}
