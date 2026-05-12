"use client";

import { useProjectStore } from "@/store/projectStore";
import { Card, Button, Tabs } from "@heroui/react";
import { Download, Copy, ChevronLeft } from "lucide-react";
import { generateChangelog, generatePrompts, generateAiAudit, generateReflection } from "@/lib/markdown/generators";
import React, { useState } from "react";
import Navbar from "@/components/layout/Navbar";

export default function ExportCenter({ projectId }: { projectId: string }) {
  const { projects } = useProjectStore();
  const project = projects[projectId];
  
  const [copied, setCopied] = useState<string | null>(null);

  const markdowns = React.useMemo(() => {
    if (!project) return { changelog: "", prompts: "", aiAudit: "", reflection: "" };
    return {
      changelog: generateChangelog(project),
      prompts: generatePrompts(project),
      aiAudit: generateAiAudit(project),
      reflection: generateReflection(project)
    };
  }, [project]);

  if (!project) return null;

  const handleCopy = (text: string, id: string) => {
    navigator.clipboard.writeText(text);
    setCopied(id);
    setTimeout(() => setCopied(null), 2000);
  };

  const handleDownload = (text: string, filename: string) => {
    const element = document.createElement("a");
    const file = new Blob([text], {type: 'text/markdown'});
    element.href = URL.createObjectURL(file);
    element.download = filename;
    document.body.appendChild(element); // Required for this to work in FireFox
    element.click();
  };

  return (
    <div className="min-h-screen bg-background flex flex-col">
      <Navbar />
      <main className="flex-1 container mx-auto px-6 py-8 max-w-5xl">
        <div className="flex justify-between items-center mb-6">
          <div className="flex items-center gap-4">
            <Button variant="ghost" isIconOnly onPress={() => window.location.href = `/project/${projectId}/workspace/step1`}>
              <ChevronLeft className="w-5 h-5" />
            </Button>
            <div>
              <h1 className="text-3xl font-bold tracking-tight">Export Center</h1>
              <p className="text-default-500">Preview and download your generated markdown audit logs.</p>
            </div>
          </div>
        </div>

        <Tabs>
          <Tabs.ListContainer>
            <Tabs.List aria-label="Markdown Files">
              <Tabs.Tab id="ai_audit">AI_AUDIT_LOG.md<Tabs.Indicator /></Tabs.Tab>
              <Tabs.Tab id="prompts">PROMPTS.md<Tabs.Indicator /></Tabs.Tab>
              <Tabs.Tab id="changelog">CHANGELOG.md<Tabs.Indicator /></Tabs.Tab>
              <Tabs.Tab id="reflection">REFLECTION.md<Tabs.Indicator /></Tabs.Tab>
            </Tabs.List>
          </Tabs.ListContainer>

          <Tabs.Panel id="ai_audit">
            <Card className="overflow-hidden">
              <div className="bg-surface-secondary/50 p-4 border-b border-border flex justify-end gap-2">
                <Button size="sm" variant="secondary" onPress={() => handleCopy(markdowns.aiAudit, 'audit')}>
                  <Copy className="w-4 h-4 mr-2 inline"/>
                  {copied === 'audit' ? "Copied!" : "Copy Markdown"}
                </Button>
                <Button size="sm" onPress={() => handleDownload(markdowns.aiAudit, 'AI_AUDIT_LOG.md')}>
                  <Download className="w-4 h-4 mr-2 inline"/>
                  Download
                </Button>
              </div>
              <div className="p-6 bg-[#0d1117] text-[#c9d1d9] font-mono text-sm overflow-x-auto max-h-[600px] overflow-y-auto whitespace-pre-wrap">
                {markdowns.aiAudit}
              </div>
            </Card>
          </Tabs.Panel>
          
          <Tabs.Panel id="prompts">
            <Card className="overflow-hidden">
              <div className="bg-surface-secondary/50 p-4 border-b border-border flex justify-end gap-2">
                <Button size="sm" variant="secondary" onPress={() => handleCopy(markdowns.prompts, 'prompts')}>
                  <Copy className="w-4 h-4 mr-2 inline"/>
                  {copied === 'prompts' ? "Copied!" : "Copy Markdown"}
                </Button>
                <Button size="sm" onPress={() => handleDownload(markdowns.prompts, 'PROMPTS.md')}>
                  <Download className="w-4 h-4 mr-2 inline"/>
                  Download
                </Button>
              </div>
              <div className="p-6 bg-[#0d1117] text-[#c9d1d9] font-mono text-sm overflow-x-auto max-h-[600px] overflow-y-auto whitespace-pre-wrap">
                {markdowns.prompts}
              </div>
            </Card>
          </Tabs.Panel>

          <Tabs.Panel id="changelog">
            <Card className="overflow-hidden">
              <div className="bg-surface-secondary/50 p-4 border-b border-border flex justify-end gap-2">
                <Button size="sm" variant="secondary" onPress={() => handleCopy(markdowns.changelog, 'changelog')}>
                  <Copy className="w-4 h-4 mr-2 inline"/>
                  {copied === 'changelog' ? "Copied!" : "Copy Markdown"}
                </Button>
                <Button size="sm" onPress={() => handleDownload(markdowns.changelog, 'CHANGELOG.md')}>
                  <Download className="w-4 h-4 mr-2 inline"/>
                  Download
                </Button>
              </div>
              <div className="p-6 bg-[#0d1117] text-[#c9d1d9] font-mono text-sm overflow-x-auto max-h-[600px] overflow-y-auto whitespace-pre-wrap">
                {markdowns.changelog}
              </div>
            </Card>
          </Tabs.Panel>

          <Tabs.Panel id="reflection">
            <Card className="overflow-hidden">
              <div className="bg-surface-secondary/50 p-4 border-b border-border flex justify-end gap-2">
                <Button size="sm" variant="secondary" onPress={() => handleCopy(markdowns.reflection, 'reflection')}>
                  <Copy className="w-4 h-4 mr-2 inline"/>
                  {copied === 'reflection' ? "Copied!" : "Copy Markdown"}
                </Button>
                <Button size="sm" onPress={() => handleDownload(markdowns.reflection, 'REFLECTION.md')}>
                  <Download className="w-4 h-4 mr-2 inline"/>
                  Download
                </Button>
              </div>
              <div className="p-6 bg-[#0d1117] text-[#c9d1d9] font-mono text-sm overflow-x-auto max-h-[600px] overflow-y-auto whitespace-pre-wrap">
                {markdowns.reflection}
              </div>
            </Card>
          </Tabs.Panel>
        </Tabs>
      </main>
    </div>
  );
}
