"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { Accordion, TextField, Input, Button, TextArea, Select, ListBox, CheckboxGroup, Checkbox, Card, Separator, Label } from "@heroui/react";
import { Plus, Trash2, Save, ArrowRight, ArrowLeft, ChevronDown } from "lucide-react";
import { useEffect, useState } from "react";
import { PromptLogEntry, PromptLessons } from "@/types/project";
import { useUnsavedChanges } from "@/lib/useUnsavedChanges";
import ConfirmDeleteModal from "./ConfirmDeleteModal";

export default function Step3Form({ projectId }: { projectId: string }) {
  const { projects, updatePrompts } = useProjectStore();
  const project = projects[projectId];
  const [deleteTarget, setDeleteTarget] = useState<{ index: number; name: string } | null>(null);

  const { control, handleSubmit, formState: { isDirty }, reset, watch } = useForm<{ prompts: PromptLogEntry[], promptLessons: PromptLessons }>({
    defaultValues: {
      prompts: [],
      promptLessons: { infoNeeded: "", lessonsLearned: "", futureImprovements: "" }
    }
  });

  const prompts = watch("prompts") || [];

  const { fields, append, remove } = useFieldArray({
    control,
    name: "prompts"
  });

  useEffect(() => {
    if (project) {
      reset({
        prompts: project.prompts || [],
        promptLessons: project.promptLessons || { infoNeeded: '', lessonsLearned: '', futureImprovements: '' }
      });
    }
  }, [project, reset]);

  const onSubmit = (data: { prompts: PromptLogEntry[], promptLessons: PromptLessons }) => {
    updatePrompts(data.prompts, data.promptLessons);
    reset(data);
  };

  const { UnsavedModal, guardNavigation } = useUnsavedChanges({
    isDirty,
    onSave: handleSubmit(onSubmit),
  });

  const handleAddPrompt = () => {
    append({
      id: Math.random().toString(36).substr(2, 9),
      date: new Date().toISOString().split('T')[0],
      aiTool: "ChatGPT",
      purpose: "",
      category: "Coding",
      usageLevel: "Hỏi sinh code",
      promptText: "",
      context: "",
      aiResponse: "",
      appliedResult: "",
      improvements: "",
      evaluationChecklist: [],
      evidence: [],
      notes: "",
      isMostImportant: false,
      isIneffective: false,
      importanceExplanation: ""
    });
  };

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
      <div className="flex justify-between items-center">
        <h3 className="text-xl font-semibold">Logged Prompts</h3>
        <Button size="sm" variant="secondary" onPress={handleAddPrompt}>
          <Plus className="w-4 h-4 mr-2 inline" />
          Add Prompt
        </Button>
      </div>

      {fields.length === 0 ? (
        <Card className="bg-surface-secondary/50 border-dashed border-2 border-border shadow-none">
          <div className="p-12 flex flex-col items-center justify-center text-center">
            <p className="text-default-500 mb-4">No prompts logged yet.</p>
            <Button size="sm" onPress={handleAddPrompt}>Create First Prompt Entry</Button>
          </div>
        </Card>
      ) : (
        <Accordion className="w-full">
          {fields.map((field, index) => (
            <Accordion.Item key={field.id}>
              <Accordion.Heading>
                <Accordion.Trigger className="bg-surface-secondary px-4 py-3 rounded-lg mb-2">
                  <div className="flex flex-col text-left">
                    <span className="text-sm font-medium">{prompts[index]?.purpose || `Prompt ${index + 1}`}</span>
                    <span className="text-xs text-default-500">{prompts[index]?.aiTool} - {prompts[index]?.date}</span>
                  </div>
                  <Accordion.Indicator>
                    <ChevronDown className="w-4 h-4" />
                  </Accordion.Indicator>
                </Accordion.Trigger>
              </Accordion.Heading>
              <Accordion.Panel>
                <Accordion.Body className="bg-surface border border-border p-4 rounded-lg mb-4">
                  <div className="flex flex-col gap-6 pb-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <Controller name={`prompts.${index}.date`} control={control} render={({ field }) => (
                        <TextField>
                          <Label>Date</Label>
                          <Input {...field} type="date" />
                        </TextField>
                      )} />
                      <Controller name={`prompts.${index}.aiTool`} control={control} render={({ field: { value, onChange } }) => (
                        <Select selectedKey={value} onSelectionChange={onChange}>
                          <Label>AI Tool</Label>
                          <Select.Trigger>
                            <Select.Value />
                            <Select.Indicator />
                          </Select.Trigger>
                          <Select.Popover>
                            <ListBox>
                              <ListBox.Item id="ChatGPT" textValue="ChatGPT">ChatGPT<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Gemini" textValue="Gemini">Gemini<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Claude" textValue="Claude">Claude<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="GitHub Copilot" textValue="GitHub Copilot">GitHub Copilot<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Cursor" textValue="Cursor">Cursor<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Other" textValue="Other">Other<ListBox.ItemIndicator /></ListBox.Item>
                            </ListBox>
                          </Select.Popover>
                        </Select>
                      )} />
                      <Controller name={`prompts.${index}.category`} control={control} render={({ field: { value, onChange } }) => (
                        <Select selectedKey={value} onSelectionChange={onChange}>
                          <Label>Category / Task Area</Label>
                          <Select.Trigger>
                            <Select.Value />
                            <Select.Indicator />
                          </Select.Trigger>
                          <Select.Popover>
                            <ListBox>
                              <ListBox.Item id="Requirement" textValue="Requirement">Requirement<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Design" textValue="Design">Design<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Database" textValue="Database">Database<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Coding" textValue="Coding">Coding<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Testing" textValue="Testing">Testing<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Debug" textValue="Debug">Debug<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Report" textValue="Report">Report / Documentation<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Other" textValue="Other">Other<ListBox.ItemIndicator /></ListBox.Item>
                            </ListBox>
                          </Select.Popover>
                        </Select>
                      )} />
                      <Controller name={`prompts.${index}.usageLevel`} control={control} render={({ field: { value, onChange } }) => (
                        <Select selectedKey={value} onSelectionChange={onChange}>
                          <Label>Usage Intent</Label>
                          <Select.Trigger>
                            <Select.Value />
                            <Select.Indicator />
                          </Select.Trigger>
                          <Select.Popover>
                            <ListBox>
                              <ListBox.Item id="Hỏi ý tưởng" textValue="Hỏi ý tưởng (Ideas)">Hỏi ý tưởng (Ideas)<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Hỏi giải thích" textValue="Hỏi giải thích (Explain)">Hỏi giải thích (Explain)<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Hỏi review" textValue="Hỏi review (Review)">Hỏi review (Review)<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Hỏi debug" textValue="Hỏi debug (Debug)">Hỏi debug (Debug)<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Hỏi sinh code" textValue="Hỏi sinh code (Generate)">Hỏi sinh code (Generate)<ListBox.ItemIndicator /></ListBox.Item>
                              <ListBox.Item id="Hỏi tối ưu" textValue="Hỏi tối ưu (Optimize)">Hỏi tối ưu (Optimize)<ListBox.ItemIndicator /></ListBox.Item>
                            </ListBox>
                          </Select.Popover>
                        </Select>
                      )} />
                    </div>

                    <Controller name={`prompts.${index}.purpose`} control={control} render={({ field }) => (
                      <TextField>
                        <Label>Purpose of this prompt</Label>
                        <Input {...field} />
                      </TextField>
                    )} />

                    <Controller name={`prompts.${index}.promptText`} control={control} render={({ field }) => (
                      <TextField className="font-mono text-sm">
                        <Label>Exact Prompt Used</Label>
                        <TextArea {...field} />
                      </TextField>
                    )} />

                    <Controller name={`prompts.${index}.context`} control={control} render={({ field }) => (
                      <TextField>
                        <Label>Context provided to AI</Label>
                        <TextArea {...field} />
                      </TextField>
                    )} />

                    <Controller name={`prompts.${index}.aiResponse`} control={control} render={({ field }) => (
                      <TextField>
                        <Label>AI Response Summary</Label>
                        <TextArea {...field} />
                      </TextField>
                    )} />

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <Controller name={`prompts.${index}.appliedResult`} control={control} render={({ field }) => (
                        <TextField>
                          <Label>What part was used?</Label>
                          <TextArea {...field} />
                        </TextField>
                      )} />
                      <Controller name={`prompts.${index}.improvements`} control={control} render={({ field }) => (
                        <TextField>
                          <Label>How did you modify/improve it?</Label>
                          <TextArea {...field} />
                        </TextField>
                      )} />
                    </div>

                    <div className="border border-border rounded-lg p-4 bg-surface-secondary/20">
                      <h4 className="text-sm font-semibold mb-3">Evaluation Checklist</h4>
                      <Controller name={`prompts.${index}.evaluationChecklist`} control={control} render={({ field: { value, onChange } }) => (
                        <CheckboxGroup value={value} onChange={onChange}>
                          <div className="flex flex-col gap-2">
                            {[
                              { id: "Prompt rõ ràng", label: "Clear prompt" },
                              { id: "Prompt có đủ bối cảnh", label: "Enough context provided" },
                              { id: "Prompt còn thiếu thông tin", label: "Missing information" },
                              { id: "Prompt tạo ra kết quả tốt", label: "Good result" },
                              { id: "Prompt tạo ra kết quả chưa phù hợp", label: "Poor result" },
                              { id: "Cần hỏi lại AI nhiều lần", label: "Needed multiple tries" },
                              { id: "Cần tự kiểm tra và chỉnh sửa nhiều", label: "Required heavy edits" }
                            ].map(item => (
                              <Checkbox key={item.id} value={item.id}>
                                <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                                <Checkbox.Content><Label>{item.label}</Label></Checkbox.Content>
                              </Checkbox>
                            ))}
                          </div>
                        </CheckboxGroup>
                      )} />
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <Controller name={`prompts.${index}.isMostImportant`} control={control} render={({ field: { value, onChange } }) => (
                        <Checkbox isSelected={value} onChange={onChange}>
                          <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                          <Checkbox.Content><Label>Mark as Most Important Prompt (Section 6)</Label></Checkbox.Content>
                        </Checkbox>
                      )} />
                      <Controller name={`prompts.${index}.isIneffective`} control={control} render={({ field: { value, onChange } }) => (
                        <Checkbox isSelected={value} onChange={onChange}>
                          <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                          <Checkbox.Content><Label>Mark as Ineffective Prompt (Section 7)</Label></Checkbox.Content>
                        </Checkbox>
                      )} />
                    </div>

                    {watch(`prompts.${index}.isMostImportant`) && (
                      <div className="border-l-4 border-primary pl-4 py-2 flex flex-col gap-3">
                        <h4 className="text-sm font-semibold text-primary">Why is this prompt important? (Section 6.2)</h4>
                        <Controller name={`prompts.${index}.importanceExplanation`} control={control} render={({ field }) => (
                          <TextField>
                            <Label>Explain why this prompt was important, what impact it had, and how it improved your workflow</Label>
                            <TextArea {...field} />
                          </TextField>
                        )} />
                      </div>
                    )}

                    {watch(`prompts.${index}.isIneffective`) && (
                      <div className="border-l-4 border-danger pl-4 py-2 flex flex-col gap-3">
                        <h4 className="text-sm font-semibold text-danger">Ineffective Details</h4>
                        <Controller name={`prompts.${index}.ineffectiveDetails.reason`} control={control} render={({ field }) => (
                          <TextField>
                            <Label>Why was it ineffective?</Label>
                            <Input {...field} />
                          </TextField>
                        )} />
                        <Controller name={`prompts.${index}.ineffectiveDetails.improvedPrompt`} control={control} render={({ field }) => (
                          <TextField className="font-mono text-sm">
                            <Label>Improved Prompt</Label>
                            <TextArea {...field} />
                          </TextField>
                        )} />
                      </div>
                    )}

                    <div className="flex justify-end mt-2">
                      <Button size="sm" variant="ghost" className="text-danger" onPress={() => remove(index)}>
                        <Trash2 className="w-4 h-4 mr-2 inline" />
                        Delete Prompt
                      </Button>
                    </div>
                  </div>
                </Accordion.Body>
              </Accordion.Panel>
            </Accordion.Item>
          ))}
        </Accordion>
      )}

      <Separator className="my-4" />

      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Prompt Lessons Learned</h3>
          <Controller name="promptLessons.infoNeeded" control={control} render={({ field }) => (
            <TextField>
              <Label>What info should be provided to get better AI answers?</Label>
              <TextArea {...field} />
            </TextField>
          )} />
          <Controller name="promptLessons.lessonsLearned" control={control} render={({ field }) => (
            <TextField>
              <Label>What did you learn about prompting?</Label>
              <TextArea {...field} />
            </TextField>
          )} />
          <Controller name="promptLessons.futureImprovements" control={control} render={({ field }) => (
            <TextField>
              <Label>How will you improve next time?</Label>
              <TextArea {...field} />
            </TextField>
          )} />
        </div>
      </Card>

      <div className="flex justify-between items-center mt-4">
        <Button onPress={() => guardNavigation(`/project/${projectId}/workspace/step2`)} variant="secondary">
          <ArrowLeft className="w-4 h-4 mr-2 inline" />
          Back
        </Button>
        <div className="flex gap-2">
          <Button type="submit" variant={isDirty ? "secondary" : "ghost"}>
            <Save className="w-4 h-4 mr-2 inline" />
            {isDirty ? "Save Changes" : "Saved"}
          </Button>
          <Button onPress={() => guardNavigation(`/project/${projectId}/workspace/step4`)} variant="secondary">
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
        title="Delete Prompt Entry"
      />
    </form>
  );
}
