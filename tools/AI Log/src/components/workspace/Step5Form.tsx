"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { TextField, Input, Button, TextArea, Select, ListBox, CheckboxGroup, Checkbox, Card, Separator, Label } from "@heroui/react";
import { Save, ArrowRight, ArrowLeft, Plus, Trash2, Check, HelpCircle, Sparkles } from "lucide-react";
import { useEffect, useCallback, useMemo, useRef } from "react";
import { ReflectionData } from "@/types/project";
import { useUnsavedChanges } from "@/lib/useUnsavedChanges";
import TagSelector from "./TagSelector";
import { useFormDraft } from "@/hooks/useFormDraft";

const reflectionToolsList = ["ChatGPT", "Claude", "Gemini", "GitHub Copilot", "Cursor", "Perplexity", "Midjourney", "Canva AI", "Notion AI", "Other"];

const reasonSuggestions = [
  "Tiết kiệm thời gian",
  "Hỗ trợ brainstorming",
  "Sinh code nhanh",
  "Kiểm tra lỗi",
  "Tạo tài liệu",
  "Hỗ trợ thiết kế UI/UX",
  "Tối ưu thuật toán",
  "Học công nghệ mới",
  "Refactor code",
  "Viết test",
  "Debug hệ thống",
  "Khác (Other)"
];

const areaSuggestions = [
  "Coding Speed", "Debugging", "Documentation", "UI Design", 
  "Planning", "Research", "Testing", "Communication", "Team Collaboration", "Other"
];

const ratingSuggestions = [
  "Very Slow", "Slow", "Average", "Fast", "Very Fast",
  "Poor", "Basic", "Good", "Very Good", "Excellent", "Other"
];

const lessonsSuggestions = [
  "Tầm quan trọng của làm việc nhóm",
  "Lập kế hoạch kiến trúc phần mềm tốt hơn",
  "Phân tích yêu cầu đóng vai trò then chốt",
  "Kiểm thử sớm giúp giảm thiểu lỗi",
  "Tài liệu hóa dự án rất quan trọng"
];

const responsibilitySuggestions = [
  "Cần kiểm chứng nội dung AI tạo ra",
  "Tránh sao chép mù quáng kết quả từ AI",
  "AI chỉ hỗ trợ, không thay thế tư duy",
  "Kiểm tra kỹ mã nguồn liên quan bảo mật",
  "Tôn trọng tính trung thực trong học thuật"
];

const improvementSuggestions = [
  "Nâng cao tiêu chuẩn viết code (Coding standards)",
  "Viết nhiều unit test/integration test hơn",
  "Cải thiện quy trình làm việc với Git",
  "Đảm bảo tính nhất quán của UI/UX",
  "Tìm hiểu sâu hơn về thiết kế hệ thống",
  "Tối ưu hóa cấu trúc dự án"
];

export default function Step5Form({ projectId }: { projectId: string }) {
  const { projects, updateReflection } = useProjectStore();
  const project = projects[projectId];
  const groupContributions = project?.aiAudit?.groupContributions || [];

  const { control, handleSubmit, formState: { isDirty }, reset, watch, setValue } = useForm<ReflectionData>({
    defaultValues: {
      summaryText: "",
      toolsUsed: [],
      mostUsedTool: "",
      mostUsedToolCustom: "",
      mostUsedReason: "",
      mostUsedReasonsList: [],
      mostUsedReasonCustom: "",
      supportAreas: [],
      supportDetails: "",
      helpfulPoints: "",
      unhelpfulPoints: "",
      dependencyLevel: "Phụ thuộc ít",
      dependencyReason: "Sử dụng AI để tối ưu hóa thời gian nghiên cứu và tạo cấu trúc ban đầu.",
      verificationMethods: [],
      verificationDescription: "",
      verificationExample: { aiSuggestion: "", checkMethod: "", result: "", followUp: "" },
      wrongSuggestions: [],
      realContributionText: "",
      linkedContributionIds: [],
      contributionDetails: {},
      beforeAfter: [],
      lessonsLearnedText: "",
      lessonsLearnedList: [],
      lessonsLearnedCustom: "",
      responsibilityLessonsText: "",
      responsibilityLessonsList: [],
      responsibilityLessonsCustom: "",
      commitments: [
        "Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.",
        "Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.",
        "Không che giấu việc sử dụng AI trong các phần quan trọng.",
        "Không dùng AI để tạo nội dung sai lệch hoặc gian lận.",
        "Không dùng AI thay thế hoàn toàn quá trình học.",
        "Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên."
      ],
      commitmentExplanation: "",
      improvementPlanText: "",
      improvementPlanList: [],
      improvementPlanCustom: "",
      selfEvaluation: [
        { id: "eval-1", criteria: "Ghi nhận việc dùng AI trung thực", score: 5, notes: "" },
        { id: "eval-2", criteria: "Prompt có mục tiêu rõ ràng", score: 5, notes: "" },
        { id: "eval-3", criteria: "Kiểm chứng kết quả AI", score: 5, notes: "" },
        { id: "eval-4", criteria: "Tự chỉnh sửa/cải tiến", score: 5, notes: "" },
        { id: "eval-5", criteria: "Hiểu nội dung đã nộp", score: 5, notes: "" },
        { id: "eval-6", criteria: "Reflection có chiều sâu", score: 5, notes: "" },
        { id: "eval-7", criteria: "Sử dụng AI có trách nhiệm", score: 5, notes: "" }
      ],
      finalQuestions: { 
        explainable: "Có, nhóm đã đọc, kiểm tra và hiểu nội dung trước khi sử dụng.", 
        canReproduce: "Có, nhưng sẽ mất nhiều thời gian hơn để nghiên cứu và triển khai.", 
        coreCompetency: "Phần thiết kế workflow, chỉnh sửa logic và xử lý lỗi thực tế.", 
        desiredSkill: "Kỹ năng thiết kế hệ thống, viết prompt và kiểm thử phần mềm." 
      }
    }
  });

  const { fields: wrongFields, append: appendWrong, remove: removeWrong } = useFieldArray({
    control,
    name: "wrongSuggestions"
  });

  const { fields: baFields, append: appendBA, remove: removeBA } = useFieldArray({
    control,
    name: "beforeAfter"
  });

  const { fields: evalFields } = useFieldArray({
    control,
    name: "selfEvaluation"
  });

  const originalData = useMemo(() => {
    const defaultOriginal = {
      summaryText: "",
      toolsUsed: [],
      mostUsedTool: "",
      mostUsedToolCustom: "",
      mostUsedReason: "",
      mostUsedReasonsList: [],
      mostUsedReasonCustom: "",
      supportAreas: [],
      supportDetails: "",
      helpfulPoints: "",
      unhelpfulPoints: "",
      dependencyLevel: "Phụ thuộc ít",
      dependencyReason: "Sử dụng AI để tối ưu hóa thời gian nghiên cứu và tạo cấu trúc ban đầu.",
      verificationMethods: [],
      verificationDescription: "",
      verificationExample: { aiSuggestion: "", checkMethod: "", result: "", followUp: "" },
      wrongSuggestions: [],
      realContributionText: "",
      linkedContributionIds: [],
      contributionDetails: {},
      beforeAfter: [],
      lessonsLearnedText: "",
      lessonsLearnedList: [],
      lessonsLearnedCustom: "",
      responsibilityLessonsText: "",
      responsibilityLessonsList: [],
      responsibilityLessonsCustom: "",
      commitments: [
        "Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.",
        "Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.",
        "Không che giấu việc sử dụng AI trong các phần quan trọng.",
        "Không dùng AI để tạo nội dung sai lệch hoặc gian lận.",
        "Không dùng AI thay thế hoàn toàn quá trình học.",
        "Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên."
      ],
      commitmentExplanation: "",
      improvementPlanText: "",
      improvementPlanList: [],
      improvementPlanCustom: "",
      selfEvaluation: [
        { id: "eval-1", criteria: "Ghi nhận việc dùng AI trung thực", score: 5, notes: "" },
        { id: "eval-2", criteria: "Prompt có mục tiêu rõ ràng", score: 5, notes: "" },
        { id: "eval-3", criteria: "Kiểm chứng kết quả AI", score: 5, notes: "" },
        { id: "eval-4", criteria: "Tự chỉnh sửa/cải tiến", score: 5, notes: "" },
        { id: "eval-5", criteria: "Hiểu nội dung đã nộp", score: 5, notes: "" },
        { id: "eval-6", criteria: "Reflection có chiều sâu", score: 5, notes: "" },
        { id: "eval-7", criteria: "Sử dụng AI có trách nhiệm", score: 5, notes: "" }
      ],
      finalQuestions: { 
        explainable: "Có, nhóm đã đọc, kiểm tra và hiểu nội dung trước khi sử dụng.", 
        canReproduce: "Có, nhưng sẽ mất nhiều thời gian hơn để nghiên cứu và triển khai.", 
        coreCompetency: "Phần thiết kế workflow, chỉnh sửa logic và xử lý lỗi thực tế.", 
        desiredSkill: "Kỹ năng thiết kế hệ thống, viết prompt và kiểm thử phần mềm." 
      }
    };

    if (!project || !project.reflection) return defaultOriginal;

    const refl = project.reflection;
    const initialReasonsList = refl.mostUsedReasonsList 
      ? [...refl.mostUsedReasonsList]
      : (refl.mostUsedReason ? reasonSuggestions.filter(r => refl.mostUsedReason.includes(r)) : []);
    if (refl.mostUsedReason && refl.mostUsedReason.includes("Khác:") && !initialReasonsList.includes("Khác (Other)")) {
      initialReasonsList.push("Khác (Other)");
    }
    const initialReasonCustom = refl.mostUsedReasonCustom || 
      (refl.mostUsedReason && refl.mostUsedReason.includes("Khác:") ? refl.mostUsedReason.split("Khác:")[1].trim() : "");

    const initialLessonsList = refl.lessonsLearnedList
      ? [...refl.lessonsLearnedList]
      : (refl.lessonsLearnedText ? lessonsSuggestions.filter(s => refl.lessonsLearnedText.includes(s)) : []);
    const initialLessonsCustom = refl.lessonsLearnedCustom || 
      (refl.lessonsLearnedText ? refl.lessonsLearnedText.replace(/- .*\n?/g, "").trim() : "");

    const initialRespList = refl.responsibilityLessonsList
      ? [...refl.responsibilityLessonsList]
      : (refl.responsibilityLessonsText ? responsibilitySuggestions.filter(s => refl.responsibilityLessonsText.includes(s)) : []);
    const initialRespCustom = refl.responsibilityLessonsCustom || 
      (refl.responsibilityLessonsText ? refl.responsibilityLessonsText.replace(/- .*\n?/g, "").trim() : "");

    const initialImprList = refl.improvementPlanList
      ? [...refl.improvementPlanList]
      : (refl.improvementPlanText ? improvementSuggestions.filter(s => refl.improvementPlanText.includes(s)) : []);
    const initialImprCustom = refl.improvementPlanCustom || 
      (refl.improvementPlanText ? refl.improvementPlanText.replace(/- .*\n?/g, "").trim() : "");

    const selfEval = refl.selfEvaluation && refl.selfEvaluation.length > 0
      ? refl.selfEvaluation
      : defaultOriginal.selfEvaluation;

    return {
      ...defaultOriginal,
      ...refl,
      mostUsedReasonsList: initialReasonsList,
      mostUsedReasonCustom: initialReasonCustom,
      lessonsLearnedList: initialLessonsList,
      lessonsLearnedCustom: initialLessonsCustom,
      responsibilityLessonsList: initialRespList,
      responsibilityLessonsCustom: initialRespCustom,
      improvementPlanList: initialImprList,
      improvementPlanCustom: initialImprCustom,
      selfEvaluation: selfEval
    };
  }, [project]);

  const { DraftStatusIndicator, isActuallyDirty } = useFormDraft({
    projectId,
    stepKey: "step5",
    watch,
    reset,
    originalData
  });

  const onSubmit = (data: ReflectionData) => {
    updateReflection(data);
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

  const aiToolOptions = ["ChatGPT", "Gemini", "Claude", "GitHub Copilot", "Cursor", "Antigravity", "Microsoft Copilot", "Perplexity"];
  const supportAreaOptions = ["Hiểu yêu cầu đề bài", "Phân tích bài toán", "Tìm ý tưởng giải pháp", "Thiết kế database", "Thiết kế giao diện", "Thiết kế kiến trúc hệ thống", "Viết code mẫu", "Debug lỗi", "Viết test case", "Review code", "Tối ưu code", "Kiểm tra bảo mật", "Viết báo cáo", "Chuẩn bị thuyết trình", "Tìm hiểu công nghệ mới"];
  const verificationMethodsOptions = ["Chạy thử chương trình", "Kiểm tra output", "Viết test case", "So sánh với yêu cầu", "Đối chiếu tài liệu", "Review code", "Hỏi lại giảng viên", "Tra cứu chính thống", "Thảo luận nhóm", "Kiểm tra bằng dữ liệu mẫu", "So sánh trước/sau"];

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6 pb-20">
      <div className="flex justify-between items-center bg-surface-secondary/20 p-4 rounded-xl border border-border/80">
        <h3 className="text-lg font-bold">5. Tự đánh giá & Phản hồi (Reflection)</h3>
        <DraftStatusIndicator />
      </div>

      {/* 3. Summary */}
      <Card>
        <div className="flex flex-col gap-4 p-6">
          <h3 className="text-lg font-semibold">Tóm tắt quá trình sử dụng AI</h3>
          <Controller name="summaryText" control={control} render={({ field }) => (
            <TextField>
              <TextArea {...field} placeholder="Mô tả ngắn gọn quá trình sử dụng AI trong bài tập/project này..." />
            </TextField>
          )} />
        </div>
      </Card>

      {/* 4. Tools */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Công cụ AI đã sử dụng</h3>
          <Controller name="toolsUsed" control={control} render={({ field: { value, onChange } }) => (
            <CheckboxGroup value={value} onChange={onChange}>
              <div className="flex flex-wrap gap-4 mt-2">
                {aiToolOptions.map(tool => (
                  <Checkbox key={tool} value={tool}>
                    <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                    <Checkbox.Content>{tool}</Checkbox.Content>
                  </Checkbox>
                ))}
                <Checkbox value="Other">
                  <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                  <Checkbox.Content>Other</Checkbox.Content>
                </Checkbox>
              </div>
            </CheckboxGroup>
          )} />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="flex flex-col gap-4">
              <Controller
                name="mostUsedTool"
                control={control}
                render={({ field: { value, onChange } }) => (
                  <Select
                    selectedKey={reflectionToolsList.includes(value) ? value : (value ? "Other" : "")}
                    onSelectionChange={(key) => {
                      onChange(key);
                      if (key !== "Other") {
                        setValue("mostUsedToolCustom", "", { shouldDirty: true });
                      } else {
                        setValue("mostUsedTool", "Other", { shouldDirty: true });
                      }
                    }}
                  >
                    <Label>Công cụ được sử dụng nhiều nhất</Label>
                    <Select.Trigger>
                      <Select.Value />
                      <Select.Indicator />
                    </Select.Trigger>
                    <Select.Popover>
                      <ListBox>
                        {reflectionToolsList.map((t) => (
                          <ListBox.Item key={t} id={t} textValue={t}>
                            {t}
                          </ListBox.Item>
                        ))}
                      </ListBox>
                    </Select.Popover>
                  </Select>
                )}
              />

              {(watch("mostUsedTool") === "Other" || !reflectionToolsList.includes(watch("mostUsedTool") || "")) && (
                <Controller
                  name="mostUsedToolCustom"
                  control={control}
                  render={({ field }) => (
                    <TextField>
                      <Label>Nhập tên công cụ khác</Label>
                      <Input
                        {...field}
                        placeholder="Ví dụ: Midjourney, v.v."
                        onChange={(e) => {
                          field.onChange(e.target.value);
                          setValue("mostUsedTool", e.target.value, { shouldDirty: true });
                        }}
                      />
                    </TextField>
                  )}
                />
              )}
            </div>

            <div className="flex flex-col gap-4">
              <Controller
                name="mostUsedReasonsList"
                control={control}
                render={({ field: { value, onChange } }) => (
                  <TagSelector
                    options={reasonSuggestions}
                    selected={value || []}
                    onChange={(selected) => {
                      onChange(selected);
                      const customReason = watch("mostUsedReasonCustom") || "";
                      const joined = selected.filter(r => r !== "Khác (Other)").join(", ");
                      const finalReason = selected.includes("Khác (Other)")
                        ? `${joined}${joined ? " & " : ""}Khác: ${customReason}`
                        : joined;
                      setValue("mostUsedReason", finalReason, { shouldDirty: true });
                    }}
                    label="Lý do sử dụng công cụ đó"
                  />
                )}
              />

              {(watch("mostUsedReasonsList") || []).includes("Khác (Other)") && (
                <Controller
                  name="mostUsedReasonCustom"
                  control={control}
                  render={({ field }) => (
                    <TextField>
                      <Label>Lý do khác (Custom Reason)</Label>
                      <TextArea
                        {...field}
                        placeholder="Nhập lý do khác của bạn..."
                        onChange={(e) => {
                          field.onChange(e.target.value);
                          const selected = watch("mostUsedReasonsList") || [];
                          const joined = selected.filter(r => r !== "Khác (Other)").join(", ");
                          setValue("mostUsedReason", `${joined}${joined ? " & " : ""}Khác: ${e.target.value}`, { shouldDirty: true });
                        }}
                      />
                    </TextField>
                  )}
                />
              )}
            </div>
          </div>
        </div>
      </Card>

      {/* 5. Support Areas */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">AI đã hỗ trợ em/nhóm ở điểm nào?</h3>
          <Controller name="supportAreas" control={control} render={({ field: { value, onChange } }) => (
            <CheckboxGroup value={value} onChange={onChange}>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-2 mt-2">
                {supportAreaOptions.map(area => (
                  <Checkbox key={area} value={area}>
                    <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                    <Checkbox.Content className="text-sm">{area}</Checkbox.Content>
                  </Checkbox>
                ))}
              </div>
            </CheckboxGroup>
          )} />
          <Controller name="supportDetails" control={control} render={({ field }) => (
            <TextField>
              <Label>Mô tả chi tiết</Label>
              <TextArea {...field} />
            </TextField>
          )} />
        </div>
      </Card>

      {/* 6. Learning impact */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">AI có giúp em/nhóm học tốt hơn không?</h3>
          <Controller name="helpfulPoints" control={control} render={({ field }) => (
            <TextField>
              <Label>Những điểm AI giúp em/nhóm học tốt hơn</Label>
              <TextArea {...field} />
            </TextField>
          )} />
          <Controller name="unhelpfulPoints" control={control} render={({ field }) => (
            <TextField>
              <Label>Những điểm AI chưa giúp tốt hoặc gây khó khăn</Label>
              <TextArea {...field} />
            </TextField>
          )} />

          <Separator />
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <Controller name="dependencyLevel" control={control} render={({ field: { value, onChange } }) => (
              <Select selectedKey={value} onSelectionChange={onChange}>
                <Label>Em/nhóm có bị phụ thuộc vào AI không?</Label>
                <Select.Trigger>
                  <Select.Value />
                  <Select.Indicator />
                </Select.Trigger>
                <Select.Popover>
                  <ListBox>
                    {["Không phụ thuộc", "Phụ thuộc ít", "Phụ thuộc trung bình", "Phụ thuộc nhiều"].map(l => <ListBox.Item key={l} id={l} textValue={l}>{l}<ListBox.ItemIndicator /></ListBox.Item>)}
                  </ListBox>
                </Select.Popover>
              </Select>
            )} />
            <div className="md:col-span-2">
              <Controller name="dependencyReason" control={control} render={({ field }) => (
                <TextField>
                  <Label>Giải thích lý do</Label>
                  <Input {...field} />
                </TextField>
              )} />
            </div>
          </div>
        </div>
      </Card>

      {/* 7 & 8. Verification & Issues */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Kiểm chứng kết quả AI</h3>
          <Controller name="verificationMethods" control={control} render={({ field: { value, onChange } }) => (
            <CheckboxGroup value={value} onChange={onChange}>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-2 mt-2">
                {verificationMethodsOptions.map(m => (
                  <Checkbox key={m} value={m}>
                    <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                    <Checkbox.Content className="text-sm">{m}</Checkbox.Content>
                  </Checkbox>
                ))}
              </div>
            </CheckboxGroup>
          )} />
          <Controller name="verificationDescription" control={control} render={({ field }) => (
            <TextField>
              <Label>Mô tả quá trình kiểm chứng</Label>
              <TextArea {...field} />
            </TextField>
          )} />

          <Separator />
          <h4 className="text-md font-semibold">Ví dụ cụ thể về một lần kiểm chứng</h4>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <Controller name="verificationExample.aiSuggestion" control={control} render={({ field }) => (
              <TextField>
                <Label>AI đã gợi ý gì?</Label>
                <Input {...field} />
              </TextField>
            )} />
            <Controller name="verificationExample.checkMethod" control={control} render={({ field }) => (
              <TextField>
                <Label>Em/nhóm đã kiểm tra bằng cách nào?</Label>
                <Input {...field} />
              </TextField>
            )} />
            <Controller name="verificationExample.result" control={control} render={({ field }) => (
              <TextField>
                <Label>Kết quả kiểm tra (Đúng / Sai...)</Label>
                <Input {...field} />
              </TextField>
            )} />
            <Controller name="verificationExample.followUp" control={control} render={({ field }) => (
              <TextField>
                <Label>Đã xử lý tiếp như thế nào?</Label>
                <Input {...field} />
              </TextField>
            )} />
          </div>

          <Separator />
          <div className="flex justify-between items-center">
            <h4 className="text-md font-semibold">Ví dụ AI gợi ý sai hoặc chưa phù hợp</h4>
            <Button size="sm" variant="secondary" onPress={() => appendWrong({ id: Math.random().toString(36).substr(2, 9), suggestion: "", reason: "", detectionMethod: "", fixMethod: "", lesson: "" })}>
              <Plus className="w-4 h-4 mr-2 inline" />
              Add Example
            </Button>
          </div>
          {wrongFields.map((field, index) => (
            <div key={field.id} className="p-4 border border-danger/30 bg-danger/5 rounded-lg flex flex-col gap-3">
              <div className="flex justify-between">
                <Controller name={`wrongSuggestions.${index}.suggestion`} control={control} render={({ field }) => (
                  <TextField className="w-2/3">
                    <Label>AI đã gợi ý gì?</Label>
                    <Input {...field} placeholder="AI đã gợi ý gì?" />
                  </TextField>
                )} />
                <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeWrong(index)} aria-label="Remove suggestion">
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                <Controller name={`wrongSuggestions.${index}.reason`} control={control} render={({ field }) => (
                  <TextField>
                    <Input {...field} placeholder="Vì sao sai?" />
                  </TextField>
                )} />
                <Controller name={`wrongSuggestions.${index}.detectionMethod`} control={control} render={({ field }) => (
                  <TextField>
                    <Input {...field} placeholder="Phát hiện thế nào?" />
                  </TextField>
                )} />
                <Controller name={`wrongSuggestions.${index}.fixMethod`} control={control} render={({ field }) => (
                  <TextField>
                    <Input {...field} placeholder="Đã sửa như thế nào?" />
                  </TextField>
                )} />
                <Controller name={`wrongSuggestions.${index}.lesson`} control={control} render={({ field }) => (
                  <TextField>
                    <Input {...field} placeholder="Bài học rút ra" />
                  </TextField>
                )} />
              </div>
            </div>
          ))}
          {wrongFields.length === 0 && (
            <p className="text-sm text-default-400 italic">&quot;Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng.&quot; (will be generated)</p>
          )}
        </div>
      </Card>

      {/* 9 & 10. Real Contribution & Comparison */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Đóng góp thật sự & So sánh</h3>
          
          {/* Real Contribution Linking System */}
          <div className="flex flex-col gap-4 bg-surface-secondary/20 p-4 rounded-xl border border-border">
            <h4 className="text-md font-semibold flex items-center gap-2 text-primary">
              <Sparkles className="w-4 h-4" />
              Real Contribution Linking System
            </h4>
            <p className="text-xs text-default-400">
              Chọn các mục đóng góp nhóm từ Step 4 bạn đã thực hiện để liên kết trực tiếp vào phần tự đánh giá của mình.
            </p>

            {groupContributions.length === 0 ? (
              <p className="text-sm text-default-400 italic">Chưa có đóng góp nhóm nào được tạo ở Step 4.</p>
            ) : (
              <div className="flex flex-col gap-3">
                {groupContributions.map((contrib) => {
                  const isLinked = (watch("linkedContributionIds") || []).includes(contrib.id);
                  
                  // Safe real contributions compiler
                  const compileRealContributions = (
                    linkedIds: string[],
                    details: Record<string, { type: 'human' | 'assisted' | 'reviewed'; notes: string }>
                  ) => {
                    if (!linkedIds || linkedIds.length === 0) return "";
                    
                    const labelMap = {
                      human: "Main Human Contribution (Tự thực hiện hoàn toàn)",
                      assisted: "AI Assisted (Có sự hỗ trợ của AI)",
                      reviewed: "Reviewed & Modified (AI tạo và sinh viên kiểm tra, chỉnh sửa)"
                    };

                    return linkedIds.map((id) => {
                      const contribution = groupContributions.find(c => c.id === id);
                      if (!contribution) return "";
                      const detail = details[id] || { type: "human", notes: "" };
                      
                      return `### [Đóng góp] ${contribution.tasks || "Nhiệm vụ chung"}
- **Thành viên:** ${contribution.memberName} (${contribution.memberId})
- **Minh chứng:** ${contribution.evidence || "Chưa có"}
- **Đánh giá AI:** ${contribution.aiUsed ? "Có sử dụng AI" : "Không sử dụng AI"}
- **Loại đóng góp thật sự:** ${labelMap[detail.type] || "Main Human Contribution"}
- **Chi tiết thực hiện & So sánh:** ${detail.notes || "Thực hiện đúng yêu cầu đề tài."}`;
                    }).filter(Boolean).join("\n\n---\n\n");
                  };

                  return (
                    <div key={contrib.id} className="p-3 bg-surface border border-border rounded-lg flex flex-col gap-3">
                      <div className="flex items-start gap-3">
                        <Checkbox
                          isSelected={isLinked}
                          onChange={(selected) => {
                            const currentList = watch("linkedContributionIds") || [];
                            let newList;
                            if (selected) {
                              newList = [...currentList, contrib.id];
                              const currentDetails = watch("contributionDetails") || {};
                              if (!currentDetails[contrib.id]) {
                                setValue(`contributionDetails.${contrib.id}`, { type: "human", notes: "" }, { shouldDirty: true });
                              }
                            } else {
                              newList = currentList.filter(id => id !== contrib.id);
                            }
                            setValue("linkedContributionIds", newList, { shouldDirty: true });
                            
                            const details = watch("contributionDetails") || {};
                            setValue("realContributionText", compileRealContributions(newList, details), { shouldDirty: true });
                          }}
                        >
                          <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                          <Checkbox.Content>
                            <span className="font-semibold text-sm">{contrib.memberName} ({contrib.memberId})</span>
                          </Checkbox.Content>
                        </Checkbox>
                        <div className="flex-1 text-sm text-default-500">
                          <strong>Nhiệm vụ:</strong> {contrib.tasks} | <strong>Minh chứng:</strong> {contrib.evidence}
                        </div>
                      </div>

                      {isLinked && (
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pl-8 border-l border-border/80 ml-2">
                          <Controller
                            name={`contributionDetails.${contrib.id}.type`}
                            control={control}
                            render={({ field: { value, onChange } }) => (
                              <Select
                                selectedKey={value || "human"}
                                onSelectionChange={(key) => {
                                  onChange(key);
                                  const currentIds = watch("linkedContributionIds") || [];
                                  const details = watch("contributionDetails") || {};
                                  const updatedDetails = { ...details, [contrib.id]: { ...details[contrib.id], type: key as 'human' | 'assisted' | 'reviewed' } };
                                  setValue("realContributionText", compileRealContributions(currentIds, updatedDetails), { shouldDirty: true });
                                }}
                              >
                                <Label>Loại đóng góp thực tế</Label>
                                <Select.Trigger>
                                  <Select.Value />
                                  <Select.Indicator />
                                </Select.Trigger>
                                <Select.Popover>
                                  <ListBox>
                                    <ListBox.Item id="human" textValue="Main Human Contribution">Main Human Contribution (Tự viết hoàn toàn)</ListBox.Item>
                                    <ListBox.Item id="assisted" textValue="AI Assisted">AI Assisted (AI hỗ trợ viết/thiết kế)</ListBox.Item>
                                    <ListBox.Item id="reviewed" textValue="Reviewed & Modified by Student">Reviewed & Modified by Student (AI viết, sinh viên sửa)</ListBox.Item>
                                  </ListBox>
                                </Select.Popover>
                              </Select>
                            )}
                          />

                          <Controller
                            name={`contributionDetails.${contrib.id}.notes`}
                            control={control}
                            render={({ field }) => (
                              <TextField>
                                <Label>Mô tả chi tiết & So sánh phần tự thực hiện</Label>
                                <TextArea
                                  {...field}
                                  placeholder="Ví dụ: Tự thiết kế API và viết code logic chính, dùng AI hỗ trợ viết mock test..."
                                  onChange={(e) => {
                                    field.onChange(e.target.value);
                                    const currentIds = watch("linkedContributionIds") || [];
                                    const details = watch("contributionDetails") || {};
                                    const updatedDetails = { ...details, [contrib.id]: { ...details[contrib.id], notes: e.target.value } };
                                    setValue("realContributionText", compileRealContributions(currentIds, updatedDetails), { shouldDirty: true });
                                  }}
                                />
                              </TextField>
                            )}
                          />
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>
            )}
          </div>

          <Controller name="realContributionText" control={control} render={({ field }) => (
            <TextField>
              <Label>Mô tả chi tiết đóng góp chính của bạn (Tự động biên dịch từ danh sách liên kết trên, có thể sửa trực tiếp)</Label>
              <TextArea {...field} placeholder="Nội dung đóng góp sẽ tự động tạo tại đây khi bạn liên kết đóng góp..." rows={6} />
            </TextField>
          )} />

          <Separator />
          <div className="flex justify-between items-center">
            <h4 className="text-md font-semibold">So sánh trước và sau khi dùng AI</h4>
            <Button size="sm" variant="secondary" onPress={() => appendBA({ id: Math.random().toString(36).substr(2, 9), area: "Coding Speed", before: "Average", after: "Fast", improvement: "" })}>
              <Plus className="w-4 h-4 mr-2 inline" />
              Add Area
            </Button>
          </div>
          <div className="flex flex-col gap-4">
            {baFields.map((field, index) => (
              <div key={field.id} className="flex flex-col gap-2 p-3 border border-border rounded-lg bg-surface-secondary/10">
                <div className="flex gap-3 items-center w-full">
                  {/* Area Select */}
                  <div className="w-1/4 min-w-[120px]">
                    <Controller
                      name={`beforeAfter.${index}.area`}
                      control={control}
                      render={({ field: { value, onChange } }) => (
                        <Select
                          selectedKey={areaSuggestions.includes(value) ? value : (value ? "Other" : "")}
                          onSelectionChange={(key) => {
                            if (key === "Other") {
                              onChange("");
                            } else {
                              onChange(key);
                            }
                          }}
                        >
                          <Label>Area</Label>
                          <Select.Trigger>
                            <Select.Value />
                            <Select.Indicator />
                          </Select.Trigger>
                          <Select.Popover>
                            <ListBox>
                              {areaSuggestions.map(a => (
                                <ListBox.Item key={a} id={a} textValue={a}>{a}</ListBox.Item>
                              ))}
                            </ListBox>
                          </Select.Popover>
                        </Select>
                      )}
                    />
                  </div>

                  {/* Before Select */}
                  <div className="flex-1">
                    <Controller
                      name={`beforeAfter.${index}.before`}
                      control={control}
                      render={({ field: { value, onChange } }) => (
                        <Select
                          selectedKey={ratingSuggestions.includes(value) ? value : (value ? "Other" : "")}
                          onSelectionChange={(key) => {
                            if (key === "Other") {
                              onChange("");
                            } else {
                              onChange(key);
                            }
                          }}
                        >
                          <Label>Trước (Before)</Label>
                          <Select.Trigger>
                            <Select.Value />
                            <Select.Indicator />
                          </Select.Trigger>
                          <Select.Popover>
                            <ListBox>
                              {ratingSuggestions.map(r => (
                                <ListBox.Item key={r} id={r} textValue={r}>{r}</ListBox.Item>
                              ))}
                            </ListBox>
                          </Select.Popover>
                        </Select>
                      )}
                    />
                  </div>

                  {/* After Select */}
                  <div className="flex-1">
                    <Controller
                      name={`beforeAfter.${index}.after`}
                      control={control}
                      render={({ field: { value, onChange } }) => (
                        <Select
                          selectedKey={ratingSuggestions.includes(value) ? value : (value ? "Other" : "")}
                          onSelectionChange={(key) => {
                            if (key === "Other") {
                              onChange("");
                            } else {
                              onChange(key);
                            }
                          }}
                        >
                          <Label>Sau (After)</Label>
                          <Select.Trigger>
                            <Select.Value />
                            <Select.Indicator />
                          </Select.Trigger>
                          <Select.Popover>
                            <ListBox>
                              {ratingSuggestions.map(r => (
                                <ListBox.Item key={r} id={r} textValue={r}>{r}</ListBox.Item>
                              ))}
                            </ListBox>
                          </Select.Popover>
                        </Select>
                      )}
                    />
                  </div>

                  {/* Improvement text */}
                  <div className="flex-1">
                    <Controller
                      name={`beforeAfter.${index}.improvement`}
                      control={control}
                      render={({ field }) => (
                        <TextField>
                          <Label>Cải thiện (Improvement)</Label>
                          <Input {...field} placeholder="e.g. Tăng 200% tốc độ" />
                        </TextField>
                      )}
                    />
                  </div>

                  <Button isIconOnly className="bg-danger/20 text-danger mt-6 shrink-0" variant="ghost" onPress={() => removeBA(index)} aria-label="Remove area">
                    <Trash2 className="w-4 h-4" />
                  </Button>
                </div>

                {/* Render inline Custom Text Inputs if "Other" is chosen */}
                {((watch(`beforeAfter.${index}.area`) && !areaSuggestions.includes(watch(`beforeAfter.${index}.area`))) ||
                  (watch(`beforeAfter.${index}.before`) && !ratingSuggestions.includes(watch(`beforeAfter.${index}.before`))) ||
                  (watch(`beforeAfter.${index}.after`) && !ratingSuggestions.includes(watch(`beforeAfter.${index}.after`)))) && (
                  <div className="grid grid-cols-3 gap-3 bg-surface-secondary/40 p-2.5 rounded-lg border border-dashed border-border mt-1">
                    {(watch(`beforeAfter.${index}.area`) && !areaSuggestions.includes(watch(`beforeAfter.${index}.area`))) ? (
                      <Controller
                        name={`beforeAfter.${index}.area`}
                        control={control}
                        render={({ field }) => (
                          <TextField>
                            <Label>Custom Area Name</Label>
                            <Input {...field} placeholder="Enter custom area..." />
                          </TextField>
                        )}
                      />
                    ) : <div />}
                    {(watch(`beforeAfter.${index}.before`) && !ratingSuggestions.includes(watch(`beforeAfter.${index}.before`))) ? (
                      <Controller
                        name={`beforeAfter.${index}.before`}
                        control={control}
                        render={({ field }) => (
                          <TextField>
                            <Label>Custom Before Value</Label>
                            <Input {...field} placeholder="Enter custom before..." />
                          </TextField>
                        )}
                      />
                    ) : <div />}
                    {(watch(`beforeAfter.${index}.after`) && !ratingSuggestions.includes(watch(`beforeAfter.${index}.after`))) ? (
                      <Controller
                        name={`beforeAfter.${index}.after`}
                        control={control}
                        render={({ field }) => (
                          <TextField>
                            <Label>Custom After Value</Label>
                            <Input {...field} placeholder="Enter custom after..." />
                          </TextField>
                        )}
                      />
                    ) : <div />}
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      </Card>

      {/* 11 & 12 & 14. Lessons & Plans */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Bài học & Kế hoạch</h3>
          
          <div className="flex flex-col gap-6">
            {/* Academic Lessons */}
            <div className="flex flex-col gap-3">
              <Controller
                name="lessonsLearnedList"
                control={control}
                render={({ field: { value, onChange } }) => (
                  <TagSelector
                    options={lessonsSuggestions}
                    selected={value || []}
                    onChange={(selected) => {
                      onChange(selected);
                      const custom = watch("lessonsLearnedCustom") || "";
                      const joined = selected.map(s => `- ${s}`).join("\n");
                      setValue("lessonsLearnedText", `${joined}${joined && custom ? "\n\n" : ""}${custom}`, { shouldDirty: true });
                    }}
                    label="Bài học về môn học"
                  />
                )}
              />
              <Controller
                name="lessonsLearnedCustom"
                control={control}
                render={({ field }) => (
                  <TextField>
                    <Label>Góp ý / Bài học bổ sung (Nhập tự do)</Label>
                    <TextArea
                      {...field}
                      placeholder="Nhập thêm bài học tự do của bạn..."
                      onChange={(e) => {
                        field.onChange(e.target.value);
                        const selected = watch("lessonsLearnedList") || [];
                        const joined = selected.map(s => `- ${s}`).join("\n");
                        setValue("lessonsLearnedText", `${joined}${joined && e.target.value ? "\n\n" : ""}${e.target.value}`, { shouldDirty: true });
                      }}
                    />
                  </TextField>
                )}
              />
            </div>

            <Separator />

            {/* Responsible AI Lessons */}
            <div className="flex flex-col gap-3">
              <Controller
                name="responsibilityLessonsList"
                control={control}
                render={({ field: { value, onChange } }) => (
                  <TagSelector
                    options={responsibilitySuggestions}
                    selected={value || []}
                    onChange={(selected) => {
                      onChange(selected);
                      const custom = watch("responsibilityLessonsCustom") || "";
                      const joined = selected.map(s => `- ${s}`).join("\n");
                      setValue("responsibilityLessonsText", `${joined}${joined && custom ? "\n\n" : ""}${custom}`, { shouldDirty: true });
                    }}
                    label="Bài học về sử dụng AI có trách nhiệm"
                  />
                )}
              />
              <Controller
                name="responsibilityLessonsCustom"
                control={control}
                render={({ field }) => (
                  <TextField>
                    <Label>Ý kiến / Nguyên tắc bổ sung (Nhập tự do)</Label>
                    <TextArea
                      {...field}
                      placeholder="Nhập thêm bài học sử dụng AI có trách nhiệm bổ sung..."
                      onChange={(e) => {
                        field.onChange(e.target.value);
                        const selected = watch("responsibilityLessonsList") || [];
                        const joined = selected.map(s => `- ${s}`).join("\n");
                        setValue("responsibilityLessonsText", `${joined}${joined && e.target.value ? "\n\n" : ""}${e.target.value}`, { shouldDirty: true });
                      }}
                    />
                  </TextField>
                )}
              />
            </div>

            <Separator />

            {/* Future Improvement Plans */}
            <div className="flex flex-col gap-3">
              <Controller
                name="improvementPlanList"
                control={control}
                render={({ field: { value, onChange } }) => (
                  <TagSelector
                    options={improvementSuggestions}
                    selected={value || []}
                    onChange={(selected) => {
                      onChange(selected);
                      const custom = watch("improvementPlanCustom") || "";
                      const joined = selected.map(s => `- ${s}`).join("\n");
                      setValue("improvementPlanText", `${joined}${joined && custom ? "\n\n" : ""}${custom}`, { shouldDirty: true });
                    }}
                    label="Kế hoạch cải thiện lần sau"
                  />
                )}
              />
              <Controller
                name="improvementPlanCustom"
                control={control}
                render={({ field }) => (
                  <TextField>
                    <Label>Kế hoạch hành động chi tiết khác (Nhập tự do)</Label>
                    <TextArea
                      {...field}
                      placeholder="Nhập thêm kế hoạch cải thiện lần sau..."
                      onChange={(e) => {
                        field.onChange(e.target.value);
                        const selected = watch("improvementPlanList") || [];
                        const joined = selected.map(s => `- ${s}`).join("\n");
                        setValue("improvementPlanText", `${joined}${joined && e.target.value ? "\n\n" : ""}${e.target.value}`, { shouldDirty: true });
                      }}
                    />
                  </TextField>
                )}
              />
            </div>
          </div>
        </div>
      </Card>

      {/* 13. Commitments */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Cam kết khi sử dụng AI</h3>
          <Controller name="commitments" control={control} render={({ field: { value, onChange } }) => (
            <CheckboxGroup value={value} onChange={onChange}>
              <div className="flex flex-col gap-2 mt-2">
                {[
                  "Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.",
                  "Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.",
                  "Không che giấu việc sử dụng AI trong các phần quan trọng.",
                  "Không dùng AI để tạo nội dung sai lệch hoặc gian lận.",
                  "Không dùng AI thay thế hoàn toàn quá trình học.",
                  "Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên."
                ].map((item, i) => (
                  <Checkbox key={i} value={item}>
                    <Checkbox.Control><Checkbox.Indicator /></Checkbox.Control>
                    <Checkbox.Content>{item}</Checkbox.Content>
                  </Checkbox>
                ))}
              </div>
            </CheckboxGroup>
          )} />
          <Controller name="commitmentExplanation" control={control} render={({ field }) => (
            <TextField>
              <Label>Giải thích thêm (nếu có)</Label>
              <Input {...field} />
            </TextField>
          )} />
        </div>
      </Card>

      {/* 15 & 16. Self Eval & Final Questions */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Tự đánh giá & Vấn đáp</h3>

          <h4 className="text-md font-medium mt-2">Tự đánh giá mức độ hoàn thành (1-5)</h4>
          <div className="flex flex-col gap-3 bg-surface-secondary/20 p-4 rounded-xl border border-border">
            {evalFields.map((field, index) => (
              <div key={field.id} className="flex gap-4 items-center">
                <span className="w-1/2 text-sm">{field.criteria}</span>
                <Controller name={`selfEvaluation.${index}.score`} control={control} render={({ field: { value, onChange } }) => (
                  <Select selectedKey={value.toString()} onSelectionChange={(k) => onChange(Number(k))} className="w-24">
                    <Select.Trigger>
                      <Select.Value />
                      <Select.Indicator />
                    </Select.Trigger>
                    <Select.Popover>
                      <ListBox>
                        {["1", "2", "3", "4", "5"].map(n => <ListBox.Item key={n} id={n} textValue={n}>{n}<ListBox.ItemIndicator /></ListBox.Item>)}
                      </ListBox>
                    </Select.Popover>
                  </Select>
                )} />
                <Controller name={`selfEvaluation.${index}.notes`} control={control} render={({ field }) => (
                  <TextField className="flex-1">
                    <Input {...field} placeholder="Ghi chú" />
                  </TextField>
                )} />
              </div>
            ))}
          </div>

          <Separator />
          <h4 className="text-md font-medium">Câu hỏi tự vấn cuối bài</h4>
          <Controller name="finalQuestions.explainable" control={control} render={({ field }) => (
            <TextField>
              <Label>Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?</Label>
              <Input {...field} />
            </TextField>
          )} />
          <Controller name="finalQuestions.canReproduce" control={control} render={({ field }) => (
            <TextField>
              <Label>Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?</Label>
              <Input {...field} />
            </TextField>
          )} />
          <Controller name="finalQuestions.coreCompetency" control={control} render={({ field }) => (
            <TextField>
              <Label>Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?</Label>
              <Input {...field} />
            </TextField>
          )} />
          <Controller name="finalQuestions.desiredSkill" control={control} render={({ field }) => (
            <TextField>
              <Label>Em/nhóm muốn cải thiện kỹ năng nào sau bài này?</Label>
              <Input {...field} />
            </TextField>
          )} />
        </div>
      </Card>

      <div className="flex justify-between items-center mt-4 sticky bottom-6 z-10 bg-surface/80 backdrop-blur-md p-4 rounded-xl border border-border shadow-lg">
        <Button onPress={() => guardNavigation(`/project/${projectId}/workspace/step4`)} variant="secondary">
          <ArrowLeft className="w-4 h-4 mr-2 inline" />
          Back
        </Button>
        <div className="flex gap-2">
          <Button type="submit" variant={isActuallyDirty ? "secondary" : "ghost"}>
            <Save className="w-4 h-4 mr-2 inline" />
            {isActuallyDirty ? "Save Changes" : "Saved"}
          </Button>
          <Button onPress={() => guardNavigation(`/project/${projectId}/export`)} className="bg-success text-success-foreground" variant="secondary">
            Finish & Export
            <ArrowRight className="w-4 h-4 ml-2 inline" />
          </Button>
        </div>
      </div>
      <UnsavedModal />
    </form>
  );
}
