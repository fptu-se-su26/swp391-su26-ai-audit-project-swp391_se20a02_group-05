"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Card, Input, TextField, Label, Button, Separator } from "@heroui/react";
import { Plus, Trash2, Save, ArrowRight } from "lucide-react";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { ProjectMember } from "@/types/project";

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

export default function Step1Form({ projectId }: { projectId: string }) {
  const { projects, updateMetadata, updateMembers } = useProjectStore();
  const project = projects[projectId];
  const router = useRouter();

  const { control, handleSubmit, formState: { errors, isDirty }, reset } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: "",
      course: "",
      courseCode: "",
      class: "",
      semester: "",
      lecturer: "",
      repoUrl: "",
      startDate: "",
      endDate: "",
      members: [],
    }
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: "members"
  });

  // Initialize form with store data
  useEffect(() => {
    if (project) {
      reset({
        ...project.metadata,
        members: project.members || [],
      });
    }
  }, [project, reset]);

  const onSubmit = (data: FormData) => {
    const { members, ...metadata } = data;
    updateMetadata(metadata);
    updateMembers(members as ProjectMember[]);
    // Reset to clear isDirty state
    reset(data);
  };

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
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
                <Input {...field} type="date" />
              </TextField>
            )} />
            <Controller name="endDate" control={control} render={({ field }) => (
              <TextField>
                <Label>Completion Date</Label>
                <Input {...field} type="date" />
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
          variant={isDirty ? "secondary" : "ghost"}
        >
          <Save className="w-4 h-4 mr-2 inline" />
          {isDirty ? "Save Changes" : "Saved"}
        </Button>
        
        <Button 
          onPress={() => router.push(`/project/${projectId}/workspace/step2`)}
          variant="secondary"
        >
          Next Step
          <ArrowRight className="w-4 h-4 ml-2 inline" />
        </Button>
      </div>
    </form>
  );
}
