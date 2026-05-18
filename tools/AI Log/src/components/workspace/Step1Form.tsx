"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Card, Input, TextField, Label, Button, Separator } from "@heroui/react";
import { Plus, Trash2, Save, ArrowRight } from "lucide-react";
import { useEffect, useCallback, useRef } from "react";
import { useRouter } from "next/navigation";
import { ProjectMember } from "@/types/project";

import { useUnsavedChanges } from "@/lib/useUnsavedChanges";
import { useFormDraft } from "@/hooks/useFormDraft";

const schema = z.object({
  name: z.string().min(1, "Required"),
  course: z.string().min(1, "Required"),
  courseCode: z.string().min(1, "Required"),
  class: z.string().min(1, "Required"),
  semester: z.string().min(1, "Required"),
  lecturer: z.string().min(1, "Required"),
  repoUrl: z.string().optional(),
  startDate: z.string().optional(),
  endDate: z.string().optional(),
  members: z.array(z.object({
    id: z.string(),
    name: z.string().min(1, "Name required"),
    studentId: z.string().min(1, "ID required"),
  })),
});

type FormData = z.infer<typeof schema>;

import { DEFAULT_PROJECT_METADATA, DEFAULT_PROJECT_MEMBERS } from "@/lib/defaults";

export default function Step1Form({ projectId }: { projectId: string }) {
  const { projects, updateMetadata, updateMembers } = useProjectStore();
  const project = projects[projectId];
  const router = useRouter();

  const { control, handleSubmit, formState: { errors, isDirty }, reset, watch } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      ...DEFAULT_PROJECT_METADATA,
      members: DEFAULT_PROJECT_MEMBERS,
    }
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: "members"
  });

  const originalData = {
    ...DEFAULT_PROJECT_METADATA,
    members: DEFAULT_PROJECT_MEMBERS,
  };
  if (project) {
    originalData.name = project.metadata.name;
    originalData.courseCode = project.metadata.courseCode;
    originalData.course = project.metadata.course;
    originalData.class = project.metadata.class;
    originalData.semester = project.metadata.semester;
    originalData.lecturer = project.metadata.lecturer;
    originalData.repoUrl = project.metadata.repoUrl || "";
    originalData.startDate = project.metadata.startDate || "";
    originalData.endDate = project.metadata.endDate || "";
    originalData.members = project.members || [];
  }

  const { DraftStatusIndicator, isActuallyDirty } = useFormDraft({
    projectId,
    stepKey: "step1",
    watch,
    reset,
    originalData
  });

  const onSubmit = (data: FormData) => {
    const { members, ...metadata } = data;
    updateMetadata(metadata);
    updateMembers(members as ProjectMember[]);
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

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
      <div className="flex justify-between items-center mb-2">
        <h2 className="text-xl font-bold text-default-800">1. Thông tin chung & Thành viên</h2>
        <DraftStatusIndicator />
      </div>
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <Controller name="name" control={control} render={({ field }) => (
              <TextField isInvalid={!!errors.name} className="md:col-span-2">
                <Label>Project Name</Label>
                <Input {...field} />
                {errors.name && <span className="text-danger text-xs">{errors.name.message}</span>}
              </TextField>
            )} />
            <Controller name="courseCode" control={control} render={({ field }) => (
              <TextField isInvalid={!!errors.courseCode}>
                <Label>Course Code</Label>
                <Input {...field} />
                {errors.courseCode && <span className="text-danger text-xs">{errors.courseCode.message}</span>}
              </TextField>
            )} />
            <Controller name="course" control={control} render={({ field }) => (
              <TextField isInvalid={!!errors.course}>
                <Label>Course Name</Label>
                <Input {...field} />
                {errors.course && <span className="text-danger text-xs">{errors.course.message}</span>}
              </TextField>
            )} />
            <Controller name="class" control={control} render={({ field }) => (
              <TextField isInvalid={!!errors.class}>
                <Label>Class</Label>
                <Input {...field} />
                {errors.class && <span className="text-danger text-xs">{errors.class.message}</span>}
              </TextField>
            )} />
            <Controller name="semester" control={control} render={({ field }) => (
              <TextField isInvalid={!!errors.semester}>
                <Label>Semester</Label>
                <Input {...field} />
                {errors.semester && <span className="text-danger text-xs">{errors.semester.message}</span>}
              </TextField>
            )} />
            <Controller name="lecturer" control={control} render={({ field }) => (
              <TextField isInvalid={!!errors.lecturer}>
                <Label>Lecturer</Label>
                <Input {...field} />
                {errors.lecturer && <span className="text-danger text-xs">{errors.lecturer.message}</span>}
              </TextField>
            )} />
            <Controller name="repoUrl" control={control} render={({ field }) => (
              <TextField isInvalid={!!errors.repoUrl}>
                <Label>Repository URL</Label>
                <Input {...field} />
                {errors.repoUrl && <span className="text-danger text-xs">{errors.repoUrl.message}</span>}
              </TextField>
            )} />
            <Controller name="startDate" control={control} render={({ field }) => (
              <TextField>
                <Label>Start Date</Label>
                <Input 
                  {...field} 
                  type="date" 
                  value={field.value ? new Date(field.value).toISOString().split('T')[0] : ''}
                  onChange={(e) => {
                    const val = e.target.value;
                    if (val) {
                      field.onChange(new Date(val).toISOString());
                    } else {
                      field.onChange('');
                    }
                  }}
                />
              </TextField>
            )} />
            <Controller name="endDate" control={control} render={({ field }) => (
              <TextField>
                <Label>Completion Date</Label>
                <Input 
                  {...field} 
                  type="date" 
                  value={field.value ? new Date(field.value).toISOString().split('T')[0] : ''}
                  onChange={(e) => {
                    const val = e.target.value;
                    if (val) {
                      field.onChange(new Date(val).toISOString());
                    } else {
                      field.onChange('');
                    }
                  }}
                />
              </TextField>
            )} />
          </div>
        </div>
      </Card>

      <Card>
        <div className="flex flex-col gap-6 p-6">
          <div className="flex justify-between items-center">
            <h3 className="text-lg font-semibold">Team Members</h3>
            <Button size="sm" variant="secondary" onPress={() => append({ id: Math.random().toString(36).substr(2, 9), name: "", studentId: "" })}>
              <Plus className="w-4 h-4 mr-2 inline" />
              Add Member
            </Button>
          </div>
          <Separator />

          <div className="flex flex-col gap-4">
            {fields.map((field, index) => (
              <div key={field.id} className="flex gap-4 items-start">
                <Controller
                  name={`members.${index}.studentId`}
                  control={control}
                  render={({ field }) => (
                    <TextField isInvalid={!!errors.members?.[index]?.studentId}>
                      <Label>Student ID</Label>
                      <Input {...field} />
                    </TextField>
                  )}
                />
                <Controller
                  name={`members.${index}.name`}
                  control={control}
                  render={({ field }) => (
                    <TextField isInvalid={!!errors.members?.[index]?.name} className="flex-1">
                      <Label>Full Name</Label>
                      <Input {...field} />
                    </TextField>
                  )}
                />
                <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => remove(index)}>
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            ))}
            {fields.length === 0 && (
              <p className="text-sm text-default-400 italic">No members added yet. For individual projects, you can add just yourself.</p>
            )}
          </div>
        </div>
      </Card>

      <div className="flex justify-between items-center mt-4">
        <Button
          type="submit"
          variant={isActuallyDirty ? "secondary" : "ghost"}
        >
          <Save className="w-4 h-4 mr-2 inline" />
          {isActuallyDirty ? "Save Changes" : "Saved"}
        </Button>

        <Button
          onPress={() => guardNavigation(`/project/${projectId}/workspace/step2`)}
          variant="secondary"
        >
          Next Step
          <ArrowRight className="w-4 h-4 ml-2 inline" />
        </Button>
      </div>
      <UnsavedModal />
    </form>
  );
}
