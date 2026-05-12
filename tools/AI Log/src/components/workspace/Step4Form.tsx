"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { Accordion, TextField, Input, Button, TextArea, Select, ListBox, CheckboxGroup, Checkbox, Card, Separator, Label } from "@heroui/react";
import { Plus, Trash2, Save, ArrowRight, ArrowLeft, ChevronDown } from "lucide-react";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { AiAuditData } from "@/types/project";

export default function Step4Form({ projectId }: { projectId: string }) {
  const { projects, updateAiAudit } = useProjectStore();
  const project = projects[projectId];
  const router = useRouter();

  const { control, handleSubmit, formState: { isDirty }, reset, watch } = useForm<AiAuditData>({
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

  useEffect(() => {
    if (project) {
      reset(project.aiAudit);
    }
  }, [project, reset]);

  const onSubmit = (data: AiAuditData) => {
    updateAiAudit(data);
    reset(data);
  };

  const aiToolOptions = ["ChatGPT", "Gemini", "Claude", "GitHub Copilot", "Cursor", "Antigravity", "Microsoft Copilot", "Perplexity"];
  const usageLevelOptions = ["Không dùng AI", "AI hỗ trợ ít", "AI hỗ trợ nhiều", "AI sinh chính"];

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
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
              lessonsLearned: ""
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
                        {/* eslint-disable-next-line react-compiler/react-compiler */}
                        <span className="text-sm font-medium">{watch(`auditEntries.${index}.purpose`) || `Entry ${index + 1}`}</span>
                        {/* eslint-disable-next-line react-compiler/react-compiler */}
                        <span className="text-xs text-default-500">{watch(`auditEntries.${index}.date`)}</span>
                      </div>
                      <Accordion.Indicator>
                        <ChevronDown className="w-4 h-4" />
                      </Accordion.Indicator>
                    </Accordion.Trigger>
                  </Accordion.Heading>
                  <Accordion.Panel>
                    <Accordion.Body className="bg-surface border border-border p-4 rounded-lg mb-4">
                      <div className="flex flex-col gap-4 pb-4">
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
                <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeMatrix(index)}>
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
                <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeIssue(index)}>
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
                  <div className="flex gap-2 flex-1 items-end">
                    <Controller name={`groupContributions.${index}.memberName`} control={control} render={({ field }) => (
                      <TextField className="w-1/3">
                        <Input {...field} placeholder="Name" />
                      </TextField>
                    )} />
                    <Controller name={`groupContributions.${index}.memberId`} control={control} render={({ field }) => (
                      <TextField className="w-1/4">
                        <Input {...field} placeholder="Student ID" />
                      </TextField>
                    )} />
                    <Controller name={`groupContributions.${index}.aiUsed`} control={control} render={({ field: { value, onChange } }) => (
                      <Checkbox isSelected={value} onChange={onChange} className="mb-2 ml-4">
                        <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                        <Checkbox.Content>Used AI?</Checkbox.Content>
                      </Checkbox>
                    )} />
                  </div>
                  <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeGroup(index)}>
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
        <Button onPress={() => router.push(`/project/${projectId}/workspace/step3`)} variant="secondary">
          <ArrowLeft className="w-4 h-4 mr-2 inline" />
          Back
        </Button>
        <div className="flex gap-2">
          <Button type="submit" variant={isDirty ? "secondary" : "ghost"}>
            <Save className="w-4 h-4 mr-2 inline" />
            {isDirty ? "Save Changes" : "Saved"}
          </Button>
          <Button onPress={() => router.push(`/project/${projectId}/workspace/step5`)} variant="secondary">
            Next Step
            <ArrowRight className="w-4 h-4 ml-2 inline" />
          </Button>
        </div>
      </div>
    </form>
  );
}
