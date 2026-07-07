"use client";

import { Modal, Button, Input, TextField, Label } from "@heroui/react";
import { useForm, Controller } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useProjectStore } from "@/store/projectStore";
import { useRouter } from "next/navigation";
import { DEFAULT_PROJECT_METADATA } from "@/lib/defaults";

const schema = z.object({
  name: z.string().min(1, "Project name is required"),
  course: z.string().min(1, "Course is required"),
  courseCode: z.string().min(1, "Course Code is required"),
  class: z.string().min(1, "Class is required"),
  semester: z.string().min(1, "Semester is required"),
  lecturer: z.string().min(1, "Lecturer is required"),
  repoUrl: z.string().url("Must be a valid URL").optional().or(z.literal("")),
});

type FormData = z.infer<typeof schema>;

export default function CreateProjectModal({ isOpen, onClose }: { isOpen: boolean; onClose: () => void }) {
  const { control, handleSubmit, formState: { errors }, reset } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      ...DEFAULT_PROJECT_METADATA,
    }
  });

  const createProject = useProjectStore(state => state.createProject);
  const router = useRouter();

  const onSubmit = (data: FormData) => {
    const projectId = createProject({
      ...data,
      repoUrl: data.repoUrl || "",
    });
    reset();
    onClose();
    router.push(`/project/${projectId}/workspace`);
  };

  return (
    <Modal isOpen={isOpen} onOpenChange={(open) => !open && onClose()}>
      <Modal.Backdrop>
        <Modal.Container>
          <Modal.Dialog className="sm:max-w-[500px]">
            <Modal.CloseTrigger />
            <Modal.Header>
              <Modal.Heading>Create New Project</Modal.Heading>
            </Modal.Header>
            <form onSubmit={handleSubmit(onSubmit)}>
              <Modal.Body className="flex flex-col gap-4">
                <Controller
                  name="name"
                  control={control}
                  render={({ field }) => (
                    <TextField isInvalid={!!errors.name} className="w-full">
                      <Label>Project Name</Label>
                      <Input {...field} placeholder="e.g. AI Workflow System" />
                      {errors.name && <span className="text-danger text-xs">{errors.name.message}</span>}
                    </TextField>
                  )}
                />
                <div className="flex gap-4">
                  <Controller
                    name="courseCode"
                    control={control}
                    render={({ field }) => (
                      <TextField isInvalid={!!errors.courseCode} className="w-full">
                        <Label>Course Code</Label>
                        <Input {...field} placeholder="e.g. SWP391" />
                        {errors.courseCode && <span className="text-danger text-xs">{errors.courseCode.message}</span>}
                      </TextField>
                    )}
                  />
                  <Controller
                    name="class"
                    control={control}
                    render={({ field }) => (
                      <TextField isInvalid={!!errors.class} className="w-full">
                        <Label>Class</Label>
                        <Input {...field} placeholder="e.g. SE1601" />
                        {errors.class && <span className="text-danger text-xs">{errors.class.message}</span>}
                      </TextField>
                    )}
                  />
                </div>
                <Controller
                  name="course"
                  control={control}
                  render={({ field }) => (
                    <TextField isInvalid={!!errors.course} className="w-full">
                      <Label>Course Name</Label>
                      <Input {...field} placeholder="e.g. Software Development Project" />
                      {errors.course && <span className="text-danger text-xs">{errors.course.message}</span>}
                    </TextField>
                  )}
                />
                <div className="flex gap-4">
                  <Controller
                    name="semester"
                    control={control}
                    render={({ field }) => (
                      <TextField isInvalid={!!errors.semester} className="w-full">
                        <Label>Semester</Label>
                        <Input {...field} placeholder="e.g. SU26" />
                        {errors.semester && <span className="text-danger text-xs">{errors.semester.message}</span>}
                      </TextField>
                    )}
                  />
                  <Controller
                    name="lecturer"
                    control={control}
                    render={({ field }) => (
                      <TextField isInvalid={!!errors.lecturer} className="w-full">
                        <Label>Lecturer</Label>
                        <Input {...field} placeholder="e.g. Nguyen Van A" />
                        {errors.lecturer && <span className="text-danger text-xs">{errors.lecturer.message}</span>}
                      </TextField>
                    )}
                  />
                </div>
              </Modal.Body>
              <Modal.Footer>
                <Button variant="secondary" onPress={onClose}>
                  Cancel
                </Button>
                <Button type="submit">
                  Create Project
                </Button>
              </Modal.Footer>
            </form>
          </Modal.Dialog>
        </Modal.Container>
      </Modal.Backdrop>
    </Modal>
  );
}
