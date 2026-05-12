"use client";

import { Card, Separator, Button, Dropdown } from "@heroui/react";
import { MoreVertical, FolderOpen, Download, Trash2, Calendar } from "lucide-react";
import { Project } from "@/types/project";
import { useProjectStore } from "@/store/projectStore";

export default function ProjectCard({ project }: { project: Project }) {
  const deleteProject = useProjectStore(state => state.deleteProject);

  const handleExport = () => {
    const dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(project, null, 2));
    const downloadAnchorNode = document.createElement('a');
    downloadAnchorNode.setAttribute("href", dataStr);
    downloadAnchorNode.setAttribute("download", `${project.metadata.name.replace(/\s+/g, '-').toLowerCase()}-export.json`);
    document.body.appendChild(downloadAnchorNode); // required for firefox
    downloadAnchorNode.click();
    downloadAnchorNode.remove();
  };

  return (
    <Card className="max-w-[400px]">
      <Card.Header className="flex gap-3 justify-between">
        <div className="flex flex-col">
          <p className="text-md font-bold text-foreground">{project.metadata.name}</p>
          <p className="text-small text-default-500">{project.metadata.courseCode} - {project.metadata.semester}</p>
        </div>
        <Dropdown>
          <Dropdown.Trigger>
            <Button isIconOnly variant="ghost" size="sm">
              <MoreVertical className="h-4 w-4" />
            </Button>
          </Dropdown.Trigger>
          <Dropdown.Menu aria-label="Project Actions">
            <Dropdown.Item id="open" href={`/project/${project.id}/workspace`}>
              <div className="flex items-center gap-2">
                <FolderOpen className="w-4 h-4" />
                <span>Open Workspace</span>
              </div>
            </Dropdown.Item>
            <Dropdown.Item id="export" onAction={handleExport}>
              <div className="flex items-center gap-2">
                <Download className="w-4 h-4" />
                <span>Export JSON</span>
              </div>
            </Dropdown.Item>
            <Dropdown.Item id="delete" className="text-danger" onAction={() => deleteProject(project.id)}>
              <div className="flex items-center gap-2">
                <Trash2 className="w-4 h-4" />
                <span>Delete Project</span>
              </div>
            </Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
      </Card.Header>
      <Separator />
      <div className="p-3">
        <p className="text-sm text-default-600 mb-2">Lecturer: {project.metadata.lecturer}</p>
        <div className="flex items-center gap-2 text-small text-default-400">
          <Calendar className="h-4 w-4" />
          <span>Last updated: {new Date(project.updatedAt).toLocaleDateString()}</span>
        </div>
      </div>
      <Separator />
      <Card.Footer>
        <Button onPress={() => window.location.href = `/project/${project.id}/workspace`} variant="secondary" className="w-full">
          Continue Workflow
        </Button>
      </Card.Footer>
    </Card>
  );
}
