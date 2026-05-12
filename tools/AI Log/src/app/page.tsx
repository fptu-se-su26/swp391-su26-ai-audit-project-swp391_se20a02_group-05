"use client";

import { useProjectStore } from "@/store/projectStore";
import { Button } from "@heroui/react";
import { Plus, Upload } from "lucide-react";
import ProjectCard from "@/components/dashboard/ProjectCard";
import CreateProjectModal from "@/components/dashboard/CreateProjectModal";
import Navbar from "@/components/layout/Navbar";
import { useRef, useState } from "react";
import { Project } from "@/types/project";

export default function DashboardPage() {
  const { projects, importProject } = useProjectStore();
  const [isOpen, setIsOpen] = useState(false);
  const onOpen = () => setIsOpen(true);
  const onClose = () => setIsOpen(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

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
              accept=".json" 
              className="hidden" 
              ref={fileInputRef} 
              onChange={handleImport}
            />
            <Button 
              variant="secondary" 
              onPress={() => fileInputRef.current?.click()}
            >
              <Upload className="h-4 w-4 mr-2 inline" />
              Import
            </Button>
            <Button 
              onPress={onOpen}
            >
              <Plus className="h-4 w-4 mr-2 inline" />
              New Project
            </Button>
          </div>
        </div>

        {projectList.length === 0 ? (
          <div className="flex flex-col items-center justify-center p-12 border border-dashed border-border rounded-xl bg-surface-secondary/30">
            <div className="p-4 rounded-full bg-surface-secondary mb-4">
              <Plus className="h-8 w-8 text-default-400" />
            </div>
            <h2 className="text-xl font-semibold mb-2">No projects yet</h2>
            <p className="text-default-500 mb-6 text-center max-w-md">
              Create a new project to start documenting your AI workflow and generating audit logs.
            </p>
            <Button onPress={onOpen}>
              Create your first project
            </Button>
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
