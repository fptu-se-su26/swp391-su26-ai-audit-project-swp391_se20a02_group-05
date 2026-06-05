"use client";

import React, { useEffect } from "react";
import { useForm, FormProvider, useWatch, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Card } from "@/components/ui/card";
import { SelectDropdown } from "@/components/ui/select-dropdown";
import { SettingsSection } from "./SettingsSection";
import { TagChipMultiSelect } from "./TagChipMultiSelect";
import { PreferenceCard } from "./PreferenceCard";
import { Typography, Switch, Checkbox, toast, Spinner, Input, TextArea, Label, FieldError } from "@heroui/react";
import {
  UnsavedChangesBar,
  isDeepEqual,
} from "@/components/ui/unsaved-changes-bar";
import { useCareerPreferences } from "@/hooks/use-career-preferences";
import { type UpdateCareerPreferenceRequest } from "@/types/profile.types";
import { useProfileStore } from "@/stores/use-profile-store";

const WORK_ENVIRONMENT_OPTIONS = [
  "Professional environment",
  "Supportive workplace",
  "Growth-oriented environment",
  "Dynamic and fast-paced environment",
  "Creative environment",
  "International environment",
  "Remote-friendly environment",
  "Collaborative workplace",
  "Structured and process-driven workplace",
  "Mentorship-focused environment",
];

const WORK_STYLE_OPTIONS = [
  "Team-oriented",
  "Independent",
  "Responsible",
  "Detail-oriented",
  "Problem-solving focused",
  "Result-oriented",
  "Creative",
  "Self-learning",
  "Feedback-oriented",
  "Organized",
  "Proactive",
  "Flexible",
];

const COMPANY_VALUES_OPTIONS = [
  "Employee growth",
  "Transparency",
  "Innovation",
  "Teamwork",
  "Professionalism",
  "Work-life balance",
  "Customer focus",
  "Product quality",
  "Continuous improvement",
  "Respectful culture",
  "Knowledge sharing",
];

const CURRENCY_OPTIONS = [
  { value: "VND", label: "VND" },
  { value: "USD", label: "USD" },
];

const SALARY_TYPE_OPTIONS = [
  { value: "Monthly", label: "Monthly" },
  { value: "Hourly", label: "Hourly" },
  { value: "Project-based", label: "Project-based" },
];

const salarySchema = z.preprocess((val) => {
  if (val === "" || val === null || val === undefined) return null;
  const num = Number(val);
  return isNaN(num) ? null : num;
}, z.number().nonnegative().nullable().optional());

// 1. Zod career schema definition
const careerSchema = z
  .object({
    availableForHire: z.boolean(),
    employmentPreferences: z.array(z.string()),
    preferredLanguage: z.enum(["en", "vi", "ja", "ko", "zh"]),
    version: z.number(),

    preferredWorkEnvironments: z.array(z.string()),
    workStyles: z.array(z.string()),
    companyValues: z.array(z.string()),
    expectedSalaryMin: salarySchema,
    expectedSalaryMax: salarySchema,
    expectedSalaryCurrency: z.enum(["VND", "USD"]),
    expectedSalaryType: z.enum(["Monthly", "Hourly", "Project-based"]),
    expectedSalaryNegotiable: z.boolean(),
    isExpectedSalaryVisible: z.boolean(),
    workPreferenceNotes: z.string().nullable().optional(),
  })
  .refine(
    (data) => {
      if (
        data.expectedSalaryMin !== null &&
        data.expectedSalaryMin !== undefined &&
        data.expectedSalaryMax !== null &&
        data.expectedSalaryMax !== undefined
      ) {
        return data.expectedSalaryMax >= data.expectedSalaryMin;
      }
      return true;
    },
    {
      message: "Max salary must be greater than or equal to min salary.",
      path: ["expectedSalaryMax"],
    }
  )
  .refine(
    (data) => {
      if (data.isExpectedSalaryVisible && !data.expectedSalaryNegotiable) {
        const hasMin = data.expectedSalaryMin !== null && data.expectedSalaryMin !== undefined;
        const hasMax = data.expectedSalaryMax !== null && data.expectedSalaryMax !== undefined;
        return hasMin || hasMax;
      }
      return true;
    },
    {
      message: "Please enter expected salary or mark it as negotiable before showing it publicly.",
      path: ["isExpectedSalaryVisible"],
    }
  );

type CareerFormValues = z.infer<typeof careerSchema>;

interface CareerTabProps {
  onDirtyChange: (isDirty: boolean) => void;
  onSaveSuccess: () => void;
}

export const CareerTab: React.FC<CareerTabProps> = ({
  onDirtyChange,
  onSaveSuccess,
}) => {
  const { career, isLoading, updateCareer } = useCareerPreferences();

  const methods = useForm<CareerFormValues>({
    resolver: zodResolver(careerSchema) as any,
    defaultValues: {
      availableForHire: true,
      employmentPreferences: [],
      preferredLanguage: "en",
      version: 0,
      preferredWorkEnvironments: [],
      workStyles: [],
      companyValues: [],
      expectedSalaryMin: null,
      expectedSalaryMax: null,
      expectedSalaryCurrency: "VND",
      expectedSalaryType: "Monthly",
      expectedSalaryNegotiable: false,
      isExpectedSalaryVisible: false,
      workPreferenceNotes: "",
    },
    mode: "onChange",
  });

  const {
    handleSubmit,
    reset,
    setValue,
    control,
    formState: { errors },
  } = methods;

  const currentValues = useWatch({ control });

  // Reset form when database preferences finish loading
  useEffect(() => {
    if (career && !methods.formState.isDirty) {
      reset({
        availableForHire: career.availableForHire,
        employmentPreferences: career.employmentPreferences || [],
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        preferredLanguage: (career.preferredLanguage as any) || "en",
        version: career.version || 0,
        preferredWorkEnvironments: career.preferredWorkEnvironments || [],
        workStyles: career.workStyles || [],
        companyValues: career.companyValues || [],
        expectedSalaryMin: career.expectedSalaryMin ?? null,
        expectedSalaryMax: career.expectedSalaryMax ?? null,
        expectedSalaryCurrency: (career.expectedSalaryCurrency as any) || "VND",
        expectedSalaryType: (career.expectedSalaryType as any) || "Monthly",
        expectedSalaryNegotiable: career.expectedSalaryNegotiable ?? false,
        isExpectedSalaryVisible: career.isExpectedSalaryVisible ?? false,
        workPreferenceNotes: career.workPreferenceNotes || "",
      });
    }
  }, [career, reset, methods.formState.isDirty]);

  useEffect(() => {
    const hasChanges = !isDeepEqual(
      currentValues,
      methods.formState.defaultValues,
    );
    onDirtyChange(hasChanges);
  }, [currentValues, methods.formState.defaultValues, onDirtyChange]);

  const handleReset = () => {
    reset();
  };

  const handleFormSubmit = async (data: CareerFormValues) => {
    try {
      const request: UpdateCareerPreferenceRequest = {
        availableForHire: data.availableForHire,
        preferredLanguage: data.preferredLanguage,
        jobTitlePreferences: career?.jobTitlePreferences || null,
        salaryExpectations: career?.salaryExpectations || null,
        remotePreference: career?.remotePreference || null,
        openToWorkStatus: career?.openToWorkStatus || null,
        skills: career?.skills || [],
        preferredLocations: career?.preferredLocations || [],
        employmentPreferences: data.employmentPreferences,
        version:
          useProfileStore.getState().career?.version ||
          data.version ||
          career?.version ||
          0,
        preferredWorkEnvironments: data.preferredWorkEnvironments,
        workStyles: data.workStyles,
        companyValues: data.companyValues,
        expectedSalaryMin: data.expectedSalaryMin,
        expectedSalaryMax: data.expectedSalaryMax,
        expectedSalaryCurrency: data.expectedSalaryCurrency,
        expectedSalaryType: data.expectedSalaryType,
        expectedSalaryNegotiable: data.expectedSalaryNegotiable,
        isExpectedSalaryVisible: data.isExpectedSalaryVisible,
        workPreferenceNotes: data.workPreferenceNotes || null,
      };

      const updated = await updateCareer(request);

      reset({
        ...data,
        version: updated.version,
      });
      onSaveSuccess();
    } catch (error: unknown) {
      console.error("Failed to save career preferences:", error);
      const axiosError = error as {
        response?: { data?: { message?: string } };
        message?: string;
      };
      const errMsg =
        axiosError.response?.data?.message ||
        axiosError.message ||
        "Failed to save career preferences.";
      toast.danger(errMsg);
    }
  };

  if (isLoading && !career) {
    return (
      <div className="flex items-center justify-center py-20 w-full h-full">
        <Spinner size="lg" color="accent" />
      </div>
    );
  }

  const employmentOptions = [
    { value: "full_time", label: "Full-time" },
    { value: "part_time", label: "Part-time" },
    { value: "contract", label: "Contract" },
    { value: "freelance", label: "Freelance" },
    { value: "internship", label: "Internship" },
  ];

  const languageOptions = [
    { value: "en", label: "English" },
    { value: "vi", label: "Vietnamese" },
    { value: "ja", label: "Japanese" },
    { value: "ko", label: "Korean" },
    { value: "zh", label: "Chinese" },
  ];

  const handleCheckboxChange = (value: string, isSelected: boolean) => {
    const current = currentValues.employmentPreferences || [];
    let updated;
    if (isSelected) {
      updated = [...current, value];
    } else {
      updated = current.filter((v) => v !== value);
    }
    setValue("employmentPreferences", updated, {
      shouldDirty: true,
      shouldValidate: true,
    });
  };

  return (
    <FormProvider {...methods}>
      <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-10">
        <input type="hidden" {...methods.register("version")} />
        
        {/* Hidden inputs to preserve Hiring/Employment arrangements state silently in form payload */}
        <input type="hidden" {...methods.register("availableForHire")} />

        {/* Localization preferences */}
        <SettingsSection
          title="Localization Settings"
          description="Choose your primary spoken language for profile indexing and system notifications."
        >
          <Card className="text-left gap-4 flex flex-col">
            <div className="flex flex-col gap-1.5 w-full md:max-w-md">
              <SelectDropdown
                label="Preferred Spoken Language"
                value={currentValues.preferredLanguage || "en"}
                onChange={(val: string) =>
                  setValue(
                    "preferredLanguage",
                    val as "en" | "vi" | "ja" | "ko" | "zh",
                    { shouldDirty: true },
                  )
                }
                options={languageOptions}
                placeholder="Select language"
              />
            </div>
          </Card>
        </SettingsSection>

        {/* Ideal Work Preferences section */}
        <SettingsSection
          title="Career Preferences"
          description="Customize the working styles, company values, and compensation details displayed on your public profile."
        >
          <div className="flex flex-col gap-6">
            {/* Preferred Work Environment Card */}
            <PreferenceCard
              title="Preferred Work Environment"
              description="Select the type of workplace where you perform best."
            >
              <Controller
                control={control}
                name="preferredWorkEnvironments"
                render={({ field: { value, onChange } }) => (
                  <TagChipMultiSelect
                    options={WORK_ENVIRONMENT_OPTIONS}
                    value={value || []}
                    onChange={onChange}
                    error={errors.preferredWorkEnvironments?.message}
                  />
                )}
              />
            </PreferenceCard>

            {/* Work Style Card */}
            <PreferenceCard
              title="Work Style"
              description="Choose the working styles that describe how you collaborate and deliver work."
            >
              <Controller
                control={control}
                name="workStyles"
                render={({ field: { value, onChange } }) => (
                  <TagChipMultiSelect
                    options={WORK_STYLE_OPTIONS}
                    value={value || []}
                    onChange={onChange}
                    error={errors.workStyles?.message}
                  />
                )}
              />
            </PreferenceCard>

            {/* Company Values Card */}
            <PreferenceCard
              title="Company Values"
              description="Select the company values that matter most to you."
            >
              <Controller
                control={control}
                name="companyValues"
                render={({ field: { value, onChange } }) => (
                  <TagChipMultiSelect
                    options={COMPANY_VALUES_OPTIONS}
                    value={value || []}
                    onChange={onChange}
                    error={errors.companyValues?.message}
                  />
                )}
              />
            </PreferenceCard>

            {/* Expected Salary Card */}
            <PreferenceCard
              title="Expected Salary"
              description="Set your expected salary and choose whether it should appear on your public profile."
            >
              <div className="flex flex-col gap-4">
                <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4 items-end">
                  {/* Min Salary */}
                  <div className="flex flex-col gap-2">
                    <Label htmlFor="expectedSalaryMin">Min Salary</Label>
                    <Controller
                      control={control}
                      name="expectedSalaryMin"
                      render={({ field: { value, onChange } }) => (
                        <Input
                          id="expectedSalaryMin"
                          aria-label="Minimum Expected Salary"
                          type="number"
                          placeholder="e.g. 10000000"
                          value={value === null || value === undefined ? "" : value.toString()}
                          onChange={onChange}
                        />
                      )}
                    />
                    {errors.expectedSalaryMin && (
                      <FieldError className="text-danger text-xs mt-1 block">
                        {errors.expectedSalaryMin.message}
                      </FieldError>
                    )}
                  </div>

                  {/* Max Salary */}
                  <div className="flex flex-col gap-2">
                    <Label htmlFor="expectedSalaryMax">Max Salary</Label>
                    <Controller
                      control={control}
                      name="expectedSalaryMax"
                      render={({ field: { value, onChange } }) => (
                        <Input
                          id="expectedSalaryMax"
                          aria-label="Maximum Expected Salary"
                          type="number"
                          placeholder="e.g. 20000000"
                          value={value === null || value === undefined ? "" : value.toString()}
                          onChange={onChange}
                        />
                      )}
                    />
                    {errors.expectedSalaryMax && (
                      <FieldError className="text-danger text-xs mt-1 block">
                        {errors.expectedSalaryMax.message}
                      </FieldError>
                    )}
                  </div>

                  {/* Currency Select */}
                  <div className="flex flex-col gap-2">
                    <Controller
                      control={control}
                      name="expectedSalaryCurrency"
                      render={({ field: { value, onChange } }) => (
                        <SelectDropdown
                          label="Currency"
                          value={value || "VND"}
                          onChange={onChange}
                          options={CURRENCY_OPTIONS}
                        />
                      )}
                    />
                  </div>

                  {/* Salary Type Select */}
                  <div className="flex flex-col gap-2">
                    <Controller
                      control={control}
                      name="expectedSalaryType"
                      render={({ field: { value, onChange } }) => (
                        <SelectDropdown
                          label="Salary Type"
                          value={value || "Monthly"}
                          onChange={onChange}
                          options={SALARY_TYPE_OPTIONS}
                        />
                      )}
                    />
                  </div>
                </div>

                {/* Salary Toggles */}
                <div className="flex flex-col md:flex-row gap-4 mt-2">
                  {/* Negotiable Toggle */}
                  <div className="flex items-center justify-between gap-4 py-2 border border-border/40 bg-surface-secondary/5 rounded-xl px-4 flex-1 select-none">
                    <div className="flex flex-col gap-0.5">
                      <Typography type="body-xs" className="font-bold text-foreground font-outfit">
                        Salary Negotiable
                      </Typography>
                      <Typography type="body-xs" className="text-muted max-w-[200px]">
                        Check if you are open to salary negotiation.
                      </Typography>
                    </div>
                    <Controller
                      control={control}
                      name="expectedSalaryNegotiable"
                      render={({ field: { value, onChange } }) => (
                        <Switch
                          isSelected={value}
                          onChange={onChange}
                          aria-label="Salary Negotiable Toggle"
                          className="cursor-pointer"
                        >
                          {({ isSelected }) => (
                            <Switch.Control
                              className={`w-10 h-5.5 rounded-full relative flex items-center transition-colors duration-200 ${isSelected ? "bg-success" : "bg-separator"}`}
                            >
                              <Switch.Thumb
                                className={`w-4 h-4 bg-foreground rounded-full absolute transition-all duration-200 ${isSelected ? "left-[20px]" : "left-0.5"}`}
                              />
                            </Switch.Control>
                          )}
                        </Switch>
                      )}
                    />
                  </div>

                  {/* Salary Visibility Toggle */}
                  <div className="flex items-center justify-between gap-4 py-2 border border-border/40 bg-surface-secondary/5 rounded-xl px-4 flex-1 select-none">
                    <div className="flex flex-col gap-0.5">
                      <Typography type="body-xs" className="font-bold text-foreground font-outfit">
                        Show expected salary on public profile
                      </Typography>
                      <Typography type="body-xs" className="text-muted max-w-[250px]">
                        Your expected salary may be visible on your public profile.
                      </Typography>
                    </div>
                    <Controller
                      control={control}
                      name="isExpectedSalaryVisible"
                      render={({ field: { value, onChange } }) => (
                        <Switch
                          isSelected={value}
                          onChange={onChange}
                          aria-label="Salary Visibility Toggle"
                          className="cursor-pointer"
                        >
                          {({ isSelected }) => (
                            <Switch.Control
                              className={`w-10 h-5.5 rounded-full relative flex items-center transition-colors duration-200 ${isSelected ? "bg-success" : "bg-separator"}`}
                            >
                              <Switch.Thumb
                                className={`w-4 h-4 bg-foreground rounded-full absolute transition-all duration-200 ${isSelected ? "left-[20px]" : "left-0.5"}`}
                              />
                            </Switch.Control>
                          )}
                        </Switch>
                      )}
                    />
                  </div>
                </div>
                {errors.isExpectedSalaryVisible && (
                  <FieldError className="text-danger text-xs mt-1 block">
                    {errors.isExpectedSalaryVisible.message}
                  </FieldError>
                )}

                <Typography type="body-xs" className="text-muted mt-2 block">
                  Only show your expected salary publicly if you are comfortable sharing it on your CV.
                </Typography>
              </div>
            </PreferenceCard>

            {/* Work Preference Notes Card */}
            <PreferenceCard
              title="Work Preference Notes"
              description="Write a short professional summary about your ideal working environment or career expectations."
            >
              <div className="flex flex-col gap-2">
                <Controller
                  control={control}
                  name="workPreferenceNotes"
                  render={({ field: { value, onChange } }) => (
                    <TextArea
                      id="workPreferenceNotes"
                      aria-label="Work Preference Notes"
                      placeholder="Describe your ideal working environment, career expectations, or what helps you perform at your best..."
                      value={value || ""}
                      onChange={onChange}
                      rows={4}
                    />
                  )}
                />
                {errors.workPreferenceNotes && (
                  <FieldError className="text-danger text-xs mt-1 block">
                    {errors.workPreferenceNotes.message}
                  </FieldError>
                )}
              </div>
            </PreferenceCard>
          </div>
        </SettingsSection>

        {/* Sticky Actions Bar */}
        <UnsavedChangesBar
          message="You have unsaved career setting changes."
          onReset={handleReset}
        />
      </form>
    </FormProvider>
  );
};

export default CareerTab;
