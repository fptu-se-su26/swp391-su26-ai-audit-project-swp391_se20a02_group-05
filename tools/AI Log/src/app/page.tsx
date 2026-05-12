"use client";

import { useProjectStore } from "@/store/projectStore";
import { Button } from "@heroui/react";
import { Plus, FolderOpen, Upload } from "lucide-react";
import ProjectCard from "@/components/dashboard/ProjectCard";
import CreateProjectModal from "@/components/dashboard/CreateProjectModal";
import Navbar from "@/components/layout/Navbar";
import { useRef, useState } from "react";
import { Project } from "@/types/project";
import { useRouter } from "next/navigation";

export default function DashboardPage() {
  const { projects, importProject, openProject } = useProjectStore();
  const [isOpen, setIsOpen] = useState(false);
  const [opening, setOpening] = useState(false);
  const [openError, setOpenError] = useState<string | null>(null);
  const onOpen = () => setIsOpen(true);
  const onClose = () => setIsOpen(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const router = useRouter();

  const projectList = Object.values(projects).sort((a, b) => 
    new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
  );

  const handleImport = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      try {
        const json = JSON.parse(e.target?.result as string) as Project;
        if (json.id && json.metadata) {
          importProject(json);
        } else {
          alert("Invalid project file format");
        }
      } catch {
        alert("Error parsing JSON file");
      }
    };
    reader.readAsText(file);
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const handleOpenProject = async () => {
    setOpening(true);
    setOpenError(null);
    try {
      const projectId = await openProject();
      if (projectId) {
        router.push(`/project/${projectId}/workspace`);
      }
    } catch (err) {
      setOpenError(err instanceof Error ? err.message : "Failed to open project file");
    } finally {
      setOpening(false);
    }
  };

  return (
    <div className="min-h-screen bg-background flex flex-col">
      <Navbar />
      <main className="flex-1 container mx-auto px-6 py-8 max-w-7xl">
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-8 gap-4">
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-foreground">Projects</h1>
            <p className="text-default-500 mt-1">Manage your AI audit logs and workflows.</p>
          </div>
          <div className="flex gap-3">
            <input 
              type="file" 
              accept=".json,.data.json" 
              className="hidden" 
              ref={fileInputRef} 
              onChange={handleImport}
            />
            <Button 
              variant="ghost" 
              onPress={() => fileInputRef.current?.click()}
            >
              <Upload className="h-4 w-4 mr-2 inline" />
              Import
            </Button>
            <Button 
              variant="secondary" 
              onPress={handleOpenProject}
              isDisabled={opening}
            >
              <FolderOpen className="h-4 w-4 mr-2 inline" />
              {opening ? "Opening..." : "Open Project"}
            </Button>
            <Button 
              onPress={onOpen}
            >
              <Plus className="h-4 w-4 mr-2 inline" />
              New Project
            </Button>
          </div>
        </div>

        {openError && (
          <div className="mb-6 p-4 rounded-lg bg-danger/10 border border-danger/30 text-danger text-sm">
            {openError}
          </div>
        )}

        {projectList.length === 0 ? (
          <div className="flex flex-col items-center justify-center p-12 border border-dashed border-border rounded-xl bg-surface-secondary/30">
            <div className="p-4 rounded-full bg-surface-secondary mb-4">
              <Plus className="h-8 w-8 text-default-400" />
            </div>
            <h2 className="text-xl font-semibold mb-2">No projects yet</h2>
            <p className="text-default-500 mb-6 text-center max-w-md">
              Create a new project or open an existing <code>.data.json</code> file to start documenting your AI workflow.
            </p>
            <div className="flex gap-3">
              <Button variant="secondary" onPress={handleOpenProject}>
                <FolderOpen className="h-4 w-4 mr-2 inline" />
                Open File
              </Button>
              <Button onPress={onOpen}>
                Create your first project
              </Button>
            </div>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {projectList.map(project => (
              <ProjectCard key={project.id} project={project} />
            ))}
          </div>
        )}

        <CreateProjectModal isOpen={isOpen} onClose={onClose} />
      </main>
    </div>
  );
}
