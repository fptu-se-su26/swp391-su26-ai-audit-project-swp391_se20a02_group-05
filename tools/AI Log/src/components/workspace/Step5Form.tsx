"use client";

import { useProjectStore } from "@/store/projectStore";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { TextField, Input, Button, TextArea, Select, ListBox, CheckboxGroup, Checkbox, Card, Separator, Label } from "@heroui/react";
import { Save, ArrowRight, ArrowLeft, Plus, Trash2 } from "lucide-react";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { ReflectionData } from "@/types/project";

export default function Step5Form({ projectId }: { projectId: string }) {
  const { projects, updateReflection } = useProjectStore();
  const project = projects[projectId];
  const router = useRouter();

  const { control, handleSubmit, formState: { isDirty }, reset } = useForm<ReflectionData>({
    defaultValues: {
      summaryText: "",
      toolsUsed: [],
      mostUsedTool: "",
      mostUsedReason: "",
      supportAreas: [],
      supportDetails: "",
      helpfulPoints: "",
      unhelpfulPoints: "",
      dependencyLevel: "Không phụ thuộc",
      dependencyReason: "",
      verificationMethods: [],
      verificationDescription: "",
      verificationExample: { aiSuggestion: "", checkMethod: "", result: "", followUp: "" },
      wrongSuggestions: [],
      realContributionText: "",
      beforeAfter: [],
      lessonsLearnedText: "",
      responsibilityLessonsText: "",
      commitments: [],
      commitmentExplanation: "",
      improvementPlanText: "",
      selfEvaluation: [
        { id: "eval-1", criteria: "Ghi nhận việc dùng AI trung thực", score: 5, notes: "" },
        { id: "eval-2", criteria: "Prompt có mục tiêu rõ ràng", score: 5, notes: "" },
        { id: "eval-3", criteria: "Kiểm chứng kết quả AI", score: 5, notes: "" },
        { id: "eval-4", criteria: "Tự chỉnh sửa/cải tiến", score: 5, notes: "" },
        { id: "eval-5", criteria: "Hiểu nội dung đã nộp", score: 5, notes: "" },
        { id: "eval-6", criteria: "Reflection có chiều sâu", score: 5, notes: "" },
        { id: "eval-7", criteria: "Sử dụng AI có trách nhiệm", score: 5, notes: "" }
      ],
      finalQuestions: { explainable: "", canReproduce: "", coreCompetency: "", desiredSkill: "" }
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

  useEffect(() => {
    if (project && project.reflection.summaryText !== undefined) {
      if (project.reflection.selfEvaluation.length > 0) {
        reset(project.reflection);
      } else {
        reset({
          ...project.reflection,
          selfEvaluation: [
            { id: "eval-1", criteria: "Ghi nhận việc dùng AI trung thực", score: 5, notes: "" },
            { id: "eval-2", criteria: "Prompt có mục tiêu rõ ràng", score: 5, notes: "" },
            { id: "eval-3", criteria: "Kiểm chứng kết quả AI", score: 5, notes: "" },
            { id: "eval-4", criteria: "Tự chỉnh sửa/cải tiến", score: 5, notes: "" },
            { id: "eval-5", criteria: "Hiểu nội dung đã nộp", score: 5, notes: "" },
            { id: "eval-6", criteria: "Reflection có chiều sâu", score: 5, notes: "" },
            { id: "eval-7", criteria: "Sử dụng AI có trách nhiệm", score: 5, notes: "" }
          ]
        });
      }
    }
  }, [project, reset]);

  const onSubmit = (data: ReflectionData) => {
    updateReflection(data);
    reset(data);
  };

  const aiToolOptions = ["ChatGPT", "Gemini", "Claude", "GitHub Copilot", "Cursor", "Antigravity", "Microsoft Copilot", "Perplexity"];
  const supportAreaOptions = ["Hiểu yêu cầu đề bài", "Phân tích bài toán", "Tìm ý tưởng giải pháp", "Thiết kế database", "Thiết kế giao diện", "Thiết kế kiến trúc hệ thống", "Viết code mẫu", "Debug lỗi", "Viết test case", "Review code", "Tối ưu code", "Kiểm tra bảo mật", "Viết báo cáo", "Chuẩn bị thuyết trình", "Tìm hiểu công nghệ mới"];
  const verificationMethodsOptions = ["Chạy thử chương trình", "Kiểm tra output", "Viết test case", "So sánh với yêu cầu", "Đối chiếu tài liệu", "Review code", "Hỏi lại giảng viên", "Tra cứu chính thống", "Thảo luận nhóm", "Kiểm tra bằng dữ liệu mẫu", "So sánh trước/sau"];

  if (!project) return null;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6 pb-20">
      
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
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <Controller name="mostUsedTool" control={control} render={({ field }) => (
              <TextField>
                <Label>Công cụ được sử dụng nhiều nhất</Label>
                <Input {...field} />
              </TextField>
            )} />
            <Controller name="mostUsedReason" control={control} render={({ field }) => (
              <TextField>
                <Label>Lý do sử dụng công cụ đó</Label>
                <Input {...field} />
              </TextField>
            )} />
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
                    <Input {...field} placeholder="AI đã gợi ý gì?" />
                  </TextField>
                )} />
                <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeWrong(index)}>
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
          <Controller name="realContributionText" control={control} render={({ field }) => (
            <TextField>
              <Label>Mô tả rõ phần nào là đóng góp chính của sinh viên/nhóm (không copy từ AI)</Label>
              <TextArea {...field} />
            </TextField>
          )} />
          
          <Separator />
          <div className="flex justify-between items-center">
            <h4 className="text-md font-semibold">So sánh trước và sau khi dùng AI</h4>
            <Button size="sm" variant="secondary" onPress={() => appendBA({ id: Math.random().toString(36).substr(2, 9), area: "Hiểu yêu cầu", before: "", after: "", improvement: "" })}>
              <Plus className="w-4 h-4 mr-2 inline" />
              Add Area
            </Button>
          </div>
          <div className="flex flex-col gap-3">
            {baFields.map((field, index) => (
              <div key={field.id} className="flex gap-2 items-start">
                <Controller name={`beforeAfter.${index}.area`} control={control} render={({ field }) => (
                  <TextField className="w-1/4">
                    <Input {...field} placeholder="Area (e.g. Code/Testing)" />
                  </TextField>
                )} />
                <Controller name={`beforeAfter.${index}.before`} control={control} render={({ field }) => (
                  <TextField className="flex-1">
                    <Input {...field} placeholder="Trước" />
                  </TextField>
                )} />
                <Controller name={`beforeAfter.${index}.after`} control={control} render={({ field }) => (
                  <TextField className="flex-1">
                    <Input {...field} placeholder="Sau" />
                  </TextField>
                )} />
                <Controller name={`beforeAfter.${index}.improvement`} control={control} render={({ field }) => (
                  <TextField className="flex-1">
                    <Input {...field} placeholder="Cải thiện đạt được" />
                  </TextField>
                )} />
                <Button isIconOnly className="bg-danger/20 text-danger" variant="ghost" onPress={() => removeBA(index)}>
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            ))}
          </div>
        </div>
      </Card>

      {/* 11 & 12 & 14. Lessons & Plans */}
      <Card>
        <div className="flex flex-col gap-6 p-6">
          <h3 className="text-lg font-semibold">Bài học & Kế hoạch</h3>
          <Controller name="lessonsLearnedText" control={control} render={({ field }) => (
            <TextField>
              <Label>Bài học về môn học</Label>
              <TextArea {...field} />
            </TextField>
          )} />
          <Controller name="responsibilityLessonsText" control={control} render={({ field }) => (
            <TextField>
              <Label>Bài học về sử dụng AI có trách nhiệm</Label>
              <TextArea {...field} />
            </TextField>
          )} />
          <Controller name="improvementPlanText" control={control} render={({ field }) => (
            <TextField>
              <Label>Kế hoạch cải thiện lần sau</Label>
              <TextArea {...field} />
            </TextField>
          )} />
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
        <Button onPress={() => router.push(`/project/${projectId}/workspace/step4`)} variant="secondary">
          <ArrowLeft className="w-4 h-4 mr-2 inline" />
          Back
        </Button>
        <div className="flex gap-2">
          <Button type="submit" variant={isDirty ? "secondary" : "ghost"}>
            <Save className="w-4 h-4 mr-2 inline" />
            {isDirty ? "Save Changes" : "Saved"}
          </Button>
          <Button onPress={() => router.push(`/project/${projectId}/export`)} className="bg-success text-success-foreground" variant="secondary">
            Finish & Export
            <ArrowRight className="w-4 h-4 ml-2 inline" />
          </Button>
        </div>
      </div>
    </form>
  );
}
