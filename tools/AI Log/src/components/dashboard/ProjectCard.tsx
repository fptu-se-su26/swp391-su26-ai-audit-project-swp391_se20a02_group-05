"use client";

import { Card, Separator, Button } from "@heroui/react";
import { FolderOpen, Download, Trash2, Calendar, Clock } from "lucide-react";
import { useRouter } from "next/navigation";
import { Project } from "@/types/project";
import { useProjectStore } from "@/store/projectStore";
import { useState } from "react";
import ConfirmDeleteModal from "@/components/workspace/ConfirmDeleteModal";
import { serializeProject, generateFileName } from "@/lib/fileService";

export default function ProjectCard({ project }: { project: Project }) {
  const { deleteProject, lastSavedAt } = useProjectStore();
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const router = useRouter();
  const lastSaved = lastSavedAt[project.id];

  const handleExport = () => {
    const content = serializeProject(project);
    const fileName = generateFileName(project.metadata.name);
    const blob = new Blob([content], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(url);
  };

  return (
    <>
      <Card className="max-w-[400px]">
        <Card.Header className="flex gap-3 justify-between">
          <div className="flex flex-col">
            <p className="text-md font-bold text-foreground">{project.metadata.name}</p>
            <p className="text-small text-default-500">{project.metadata.courseCode} - {project.metadata.semester}</p>
          </div>
          <div className="flex flex-col gap-1 min-w-[140px]">
            <Button variant="tertiary" size="sm" fullWidth className="justify-start" aria-label="Open Workspace" onPress={() => router.push(`/project/${project.id}/workspace`)}>
              <FolderOpen className="w-4 h-4 mr-2" />Open Workspace
            </Button>
            <Button variant="tertiary" size="sm" fullWidth className="justify-start" aria-label="Export .data.json" onPress={handleExport}>
              <Download className="w-4 h-4 mr-2" />Export
            </Button>
            <Button variant="tertiary" size="sm" fullWidth className="text-danger justify-start" aria-label="Delete Project" onPress={() => setShowDeleteModal(true)}>
              <Trash2 className="w-4 h-4 mr-2" />Delete Project
            </Button>
          </div>
        </Card.Header>
        <Separator />
        <div className="p-3">
          <p className="text-sm text-default-600 mb-2">Lecturer: {project.metadata.lecturer}</p>
          <div className="flex items-center gap-2 text-small text-default-400">
            <Calendar className="h-4 w-4" />
            <span>Updated: {new Date(project.updatedAt).toLocaleDateString()}</span>
          </div>
          {lastSaved && (
            <div className="flex items-center gap-2 text-small text-default-400 mt-1">
              <Clock className="h-4 w-4" />
              <span>Saved: {new Date(lastSaved).toLocaleDateString()}</span>
            </div>
          )}
        </div>
        <Separator />
        <Card.Footer>
          <Button onPress={() => window.location.href = `/project/${project.id}/workspace`} variant="secondary" className="w-full">
            Continue Workflow
          </Button>
        </Card.Footer>
      </Card>
      <ConfirmDeleteModal
        isOpen={showDeleteModal}
        onClose={() => setShowDeleteModal(false)}
        onConfirm={() => deleteProject(project.id)}
        itemName={project.metadata.name}
        title="Delete Project"
        description="This will permanently delete this project and all its data. This action cannot be undone."
      />
    </>
  );
}
