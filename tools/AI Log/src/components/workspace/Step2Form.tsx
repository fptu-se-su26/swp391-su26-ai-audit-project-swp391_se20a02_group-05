"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { Accordion, TextField, Input, Button, Card, TextArea, Checkbox, Select, ListBox, Label } from "@heroui/react";
import { Plus, Trash2, Save, ArrowRight, ArrowLeft, ChevronDown } from "lucide-react";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { ChangelogEntry } from "@/types/project";

export default function Step2Form({ projectId }: { projectId: string }) {
  const { projects, updateChangelogs } = useProjectStore();
  const project = projects[projectId];
  const router = useRouter();

  const { control, handleSubmit, formState: { isDirty }, reset, watch } = useForm<{ changelogs: ChangelogEntry[] }>({
    defaultValues: { changelogs: [] }
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: "changelogs"
  });

  useEffect(() => {
    if (project) {
      reset({ changelogs: project.changelogs || [] });
    }
  }, [project, reset]);

  const onSubmit = (data: { changelogs: ChangelogEntry[] }) => {
    updateChangelogs(data.changelogs);
    reset(data);
  };

  const handleAddPhase = () => {
    append({
      id: Math.random().toString(36).substr(2, 9),
      phaseName: `Phase 0${fields.length + 1}`,
      date: new Date().toISOString().split('T')[0],
      status: "In Progress",
      completedChecklist: [],
      changes: [],
      aiSupport: { used: false, description: "" },
      evidenceLink: "",
      notes: ""
    });
  };

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
      <div className="flex justify-end">
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
                    {/* eslint-disable-next-line react-compiler/react-compiler */}
                    <span className="text-sm font-medium">{watch(`changelogs.${index}.phaseName`)}</span>
                    {/* eslint-disable-next-line react-compiler/react-compiler */}
                    <span className="text-xs text-default-500">{watch(`changelogs.${index}.status`)}</span>
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
                      <Controller name={`changelogs.${index}.phaseName`} control={control} render={({ field }) => (
                        <TextField>
                          <Label>Phase Name</Label>
                          <Input {...field} />
                        </TextField>
                      )} />
                      <Controller name={`changelogs.${index}.date`} control={control} render={({ field }) => (
                        <TextField>
                          <Label>Date</Label>
                          <Input {...field} type="date" />
                        </TextField>
                      )} />
                      <Controller name={`changelogs.${index}.status`} control={control} render={({ field: { value, onChange } }) => (
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
                      )} />
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

                    <Controller name={`changelogs.${index}.evidenceLink`} control={control} render={({ field }) => (
                      <TextField>
                        <Label>Evidence (Commit / Screenshot Link)</Label>
                        <Input {...field} />
                      </TextField>
                    )} />

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

      <div className="flex justify-between items-center mt-4">
        <Button onPress={() => router.push(`/project/${projectId}/workspace/step1`)} variant="secondary">
          <ArrowLeft className="w-4 h-4 mr-2 inline" />
          Back
        </Button>
        <div className="flex gap-2">
          <Button type="submit" variant={isDirty ? "secondary" : "ghost"}>
            <Save className="w-4 h-4 mr-2 inline" />
            {isDirty ? "Save Changes" : "Saved"}
          </Button>
          <Button onPress={() => router.push(`/project/${projectId}/workspace/step3`)} variant="secondary">
            Next Step
            <ArrowRight className="w-4 h-4 ml-2 inline" />
          </Button>
        </div>
      </div>
    </form>
  );
}

import { Control } from "react-hook-form";

// Sub-component to manage nested field array for changes
function ChangesListArray({ control, nestIndex }: { control: Control<{ changelogs: ChangelogEntry[] }>, nestIndex: number }) {
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
              <TextField className="sm:col-span-2">
                <Input {...field} placeholder="What changed?" />
              </TextField>
            )} />
            <Controller name={`changelogs.${nestIndex}.changes.${k}.author`} control={control} render={({ field }) => (
              <TextField>
                <Input {...field} placeholder="Author" />
              </TextField>
            )} />
            <Controller name={`changelogs.${nestIndex}.changes.${k}.evidence`} control={control} render={({ field }) => (
              <TextField>
                <Input {...field} placeholder="Evidence" />
              </TextField>
            )} />
          </div>
          <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => remove(k)}>
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
