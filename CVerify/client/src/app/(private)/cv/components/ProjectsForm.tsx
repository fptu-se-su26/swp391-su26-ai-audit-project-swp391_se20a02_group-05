import React from "react";
import { useTranslation } from "react-i18next";
import { AlertCircle, ArrowLeft } from "lucide-react";
import { Card, Button } from "@heroui/react";
import { useRouter } from "next/navigation";

export const ProjectsForm: React.FC = () => {
  const { t } = useTranslation(["common"]);
  const router = useRouter();

  return (
    <div className="flex flex-col gap-6 text-left">
      <Card className="p-5 border border-warning/40 bg-warning/5 flex flex-row gap-3 items-start">
        <AlertCircle className="size-5 text-warning shrink-0 mt-0.5" />
        <div className="flex flex-col gap-1 text-xs">
          <span className="font-bold text-foreground">
            {t("common:cvManagement.labels.projectsSyncedInfo")}
          </span>
        </div>
      </Card>

      <div className="flex flex-col gap-3 py-10 items-center justify-center border-2 border-dashed border-border/40 rounded-2xl p-6">
        <span className="text-muted-foreground text-xs font-bold">
          {t("common:cvManagement.labels.noProjects")}
        </span>
        <Button
          size="sm"
          className="rounded-xl font-bold bg-accent text-accent-foreground border-none mt-2 text-xs"
          onPress={() => router.push("/settings")}
        >
          {t("common:cvManagement.viewDigitalProfileDescMain") || "Connect Repositories"}
        </Button>
      </div>
    </div>
  );
};
