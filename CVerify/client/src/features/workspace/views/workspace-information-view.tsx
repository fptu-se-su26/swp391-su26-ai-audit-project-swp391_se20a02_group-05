"use client";
import React, { useEffect, useState, useRef } from "react";
import { useWorkspaceStore } from "../store/use-workspace-store";
import { Card } from "@/components/ui/card";
import { Typography, Chip, Input, TextArea, Button, Spinner, toast } from "@heroui/react";
import { useTranslation } from "react-i18next";
import { 
  Building2, 
  Globe, 
  MapPin, 
  Briefcase, 
  Calendar, 
  ShieldCheck, 
  Edit3, 
  Save, 
  AlertTriangle, 
  Plus, 
  Phone, 
  Mail, 
  Link2, 
  X, 
  Info,
  Map,
  Users
} from "lucide-react";
import { SkeletonLoader } from "@/components/ui/states";
import { workspaceService } from "../services/workspace.service";

interface WorkspaceInformationViewProps {
  organizationSlug: string;
}

const VI_BENEFITS_LIST = [
  "Bảo hiểm y tế",
  "Thưởng tháng 13",
  "Đào tạo miễn phí",
  "Văn phòng tiện nghi",
  "Lộ trình thăng tiến"
];

const BENEFIT_TRANSLATIONS: Record<string, { vi: string; en: string }> = {
  "Bảo hiểm y tế": { vi: "Bảo hiểm y tế", en: "Health Insurance" },
  "Thưởng tháng 13": { vi: "Thưởng tháng 13", en: "13th-month Salary" },
  "Đào tạo miễn phí": { vi: "Đào tạo miễn phí", en: "Free Training" },
  "Văn phòng tiện nghi": { vi: "Văn phòng tiện nghi", en: "Modern Office" },
  "Lộ trình thăng tiến": { vi: "Lộ trình thăng tiến", en: "Career Roadmap" }
};

interface PredefinedTag {
  id: string;
  vi: string;
  en: string;
}

const PREDEFINED_TAGS: PredefinedTag[] = [
  // Software & Systems
  { id: "Web Development", vi: "Lập trình Web", en: "Web Development" },
  { id: "Mobile Development", vi: "Lập trình Di động", en: "Mobile Development" },
  { id: "Embedded Systems", vi: "Hệ thống nhúng", en: "Embedded Systems" },
  { id: "Cloud Computing", vi: "Điện toán đám mây", en: "Cloud Computing" },
  // AI & Data
  { id: "Artificial Intelligence", vi: "Trí tuệ nhân tạo", en: "Artificial Intelligence" },
  { id: "Machine Learning", vi: "Học máy", en: "Machine Learning" },
  { id: "Data Science", vi: "Khoa học dữ liệu", en: "Data Science" },
  { id: "Computer Vision", vi: "Thị giác máy tính", en: "Computer Vision" },
  // Hardware & Advanced Tech
  { id: "Semiconductor", vi: "Bán dẫn", en: "Semiconductor" },
  { id: "IC Design", vi: "Thiết kế vi mạch", en: "IC Design" },
  { id: "IoT", vi: "Internet vạn vật", en: "IoT" },
  { id: "Microchip Technology", vi: "Công nghệ vi mạch", en: "Microchip Technology" },
];

const getTagLabel = (tag: string, isEn: boolean) => {
  const found = PREDEFINED_TAGS.find(
    (t) => t.id === tag || t.vi === tag || t.en === tag
  );
  return found ? (isEn ? found.en : found.vi) : tag;
};

const sanitizeMapUrl = (input: string): { sanitizedUrl: string; error: string | null } => {
  const value = input.trim();
  if (!value) {
    return { sanitizedUrl: "", error: null };
  }

  // Case A: Check if it's an iframe snippet
  if (value.toLowerCase().includes("<iframe")) {
    const srcMatch = value.match(/src=["']([^"']+)["']/i);
    if (srcMatch && srcMatch[1]) {
      const srcUrl = srcMatch[1];
      if (srcUrl.startsWith("https://www.google.com/maps/embed")) {
        return { sanitizedUrl: srcUrl, error: null };
      }
    }
    return { sanitizedUrl: "", error: "Invalid format. Please use the 'Embed a map' link from Google Maps." };
  }

  // Case B: Check if it starts with the clean embed url prefix
  if (value.startsWith("https://www.google.com/maps/embed")) {
    return { sanitizedUrl: value, error: null };
  }

  // Case C: Standard sharing links or place links
  return { sanitizedUrl: "", error: "Invalid format. Please use the 'Embed a map' link from Google Maps." };
};

export const WorkspaceInformationView: React.FC<WorkspaceInformationViewProps> = ({
  organizationSlug,
}) => {
  const { i18n } = useTranslation();
  const isEn = (i18n.language || "vi").startsWith("en");

  const fetchWorkspace = useWorkspaceStore((s) => s.fetchWorkspace);
  const workspaceDetails = useWorkspaceStore((s) => s.workspaces[organizationSlug]);
  const isDetailsLoading = useWorkspaceStore((s) => s.loading[organizationSlug]);
  const detailsError = useWorkspaceStore((s) => s.errors[organizationSlug]);
  const updateWorkspaceDetails = useWorkspaceStore((s) => s.updateWorkspaceDetails);

  const [isEditing, setIsEditing] = useState(false);
  const [description, setDescription] = useState("");
  const [website, setWebsite] = useState("");
  const [companyType, setCompanyType] = useState("");
  const [companySize, setCompanySize] = useState("");
  const [branchCount, setBranchCount] = useState(0);
  const [industryTags, setIndustryTags] = useState<string[]>([]);
  const [benefitTags, setBenefitTags] = useState<string[]>([]);
  const [contactName, setContactName] = useState("");
  const [contactPhone, setContactPhone] = useState("");
  const [contactEmail, setContactEmail] = useState("");
  const [city, setCity] = useState("");
  const [detailAddress, setDetailAddress] = useState("");
  const [googleMapsEmbedUrl, setGoogleMapsEmbedUrl] = useState("");
  const [linkedinUrl, setLinkedinUrl] = useState("");
  const [facebookUrl, setFacebookUrl] = useState("");
  const [twitterUrl, setTwitterUrl] = useState("");

  // UI state
  const [isSaving, setIsSaving] = useState(false);
  const [newTagInput, setNewTagInput] = useState("");
  const [newBenefitInput, setNewBenefitInput] = useState("");
  const [mapInputVal, setMapInputVal] = useState("");
  const [mapValidationError, setMapValidationError] = useState<string | null>(null);

  useEffect(() => {
    if (organizationSlug) {
      fetchWorkspace(organizationSlug);
    }
  }, [organizationSlug, fetchWorkspace]);

  useEffect(() => {
    if (workspaceDetails) {
      setDescription(workspaceDetails.description || "");
      setWebsite(workspaceDetails.website || "");
      setCompanyType(workspaceDetails.companyType || "");
      setCompanySize(workspaceDetails.companySize || "");
      setBranchCount(workspaceDetails.branchCount || 0);
      setIndustryTags(workspaceDetails.industryTags || []);
      setBenefitTags(workspaceDetails.benefitTags || []);
      setContactName(workspaceDetails.contactName || "");
      setContactPhone(workspaceDetails.contactPhone || "");
      setContactEmail(workspaceDetails.contactEmail || "");
      setCity(workspaceDetails.city || "");
      setDetailAddress(workspaceDetails.detailAddress || "");
      setGoogleMapsEmbedUrl(workspaceDetails.googleMapsEmbedUrl || "");
      setMapInputVal(workspaceDetails.googleMapsEmbedUrl || "");
      setMapValidationError(null);
      setLinkedinUrl(workspaceDetails.linkedinUrl || "");
      setFacebookUrl(workspaceDetails.facebookUrl || "");
      setTwitterUrl(workspaceDetails.twitterUrl || "");
    }
  }, [workspaceDetails]);

  if (isDetailsLoading) {
    return (
      <div className="space-y-6 max-w-7xl mx-auto p-4 font-outfit text-foreground">
        <div className="h-10 w-48 bg-separator/50 animate-pulse rounded-lg mb-4" />
        <Card className="p-0 overflow-hidden">
          <SkeletonLoader rows={6} columns={4} />
        </Card>
      </div>
    );
  }

  if (detailsError || !workspaceDetails) {
    const isAccessDenied = detailsError?.toLowerCase().includes("forbidden") || detailsError?.toLowerCase().includes("forbid") || detailsError?.includes("403");
    return (
      <div className="max-w-xl mx-auto py-20 font-outfit text-foreground">
        <Card className="p-8 border border-border bg-surface text-center">
          <div className="size-16 rounded-2xl bg-danger/10 flex items-center justify-center border border-danger/20 mx-auto mb-5 text-danger">
            <AlertTriangle size={28} />
          </div>
          <Typography type="h4" className="font-bold text-foreground mb-2">
            {isAccessDenied ? "Access Denied" : "Workspace Loading Error"}
          </Typography>
          <Typography type="body-xs" className="text-muted leading-relaxed mb-6">
            {isAccessDenied 
              ? "You do not have permission to access this organization workspace. Please verify your membership credentials or switch accounts."
              : detailsError || "Organization not found"}
          </Typography>
        </Card>
      </div>
    );
  }

  const userRole = workspaceDetails.userRole;
  const canEdit = userRole === "OWNER" || userRole === "REPRESENTATIVE";

  const handleSave = async () => {
    setIsSaving(true);
    const success = await updateWorkspaceDetails(organizationSlug, {
      description,
      website,
      companyType,
      companySize,
      branchCount,
      industryTags,
      benefitTags,
      contactName,
      contactPhone,
      contactEmail,
      city,
      detailAddress,
      googleMapsEmbedUrl,
      linkedinUrl,
      facebookUrl,
      twitterUrl
    });
    setIsSaving(false);
    if (success) {
      toast.success(isEn ? "Profile updated successfully." : "Cập nhật hồ sơ thành công.");
      setIsEditing(false);
    } else {
      toast.danger(isEn ? "Failed to update profile." : "Không thể cập nhật hồ sơ.");
    }
  };

  const handleAddIndustryTag = () => {
    if (newTagInput.trim() && !industryTags.includes(newTagInput.trim())) {
      setIndustryTags([...industryTags, newTagInput.trim()]);
      setNewTagInput("");
    }
  };

  const handleRemoveIndustryTag = (tag: string) => {
    setIndustryTags(industryTags.filter((t) => t !== tag));
  };

  const handleTogglePredefinedBenefit = (benefit: string) => {
    if (benefitTags.includes(benefit)) {
      setBenefitTags(benefitTags.filter((b) => b !== benefit));
    } else {
      setBenefitTags([...benefitTags, benefit]);
    }
  };

  const handleAddCustomBenefit = () => {
    if (newBenefitInput.trim() && !benefitTags.includes(newBenefitInput.trim())) {
      setBenefitTags([...benefitTags, newBenefitInput.trim()]);
      setNewBenefitInput("");
    }
  };

  const handleRemoveBenefitTag = (benefit: string) => {
    setBenefitTags(benefitTags.filter((b) => b !== benefit));
  };



  const getBenefitLabel = (benefit: string) => {
    for (const [key, val] of Object.entries(BENEFIT_TRANSLATIONS)) {
      if (key === benefit || val.vi === benefit || val.en === benefit) {
        return isEn ? val.en : val.vi;
      }
    }
    return benefit;
  };

  const getCompanySizeLabel = (size: string) => {
    if (!size) return "";
    if (isEn) {
      return size.replace("người", "employees");
    }
    return size;
  };

  const getCompanyTypeLabel = (type: string) => {
    if (!type) return "";
    const typesMap: Record<string, string> = {
      "Cổ phần": isEn ? "Joint Stock" : "Cổ phần",
      "TNHH": isEn ? "LLC" : "TNHH",
      "Tập đoàn": isEn ? "Corporation" : "Tập đoàn",
      "Startup": "Startup"
    };
    return typesMap[type] || type;
  };

  return (
    <div className="space-y-6 font-outfit max-w-7xl mx-auto text-foreground">
      {/* 1. Header Banner */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-surface border border-border text-foreground select-none">
        <div className="space-y-1">
          <Typography
            type="h2"
            className="text-2xl font-bold flex items-center gap-2 text-foreground"
          >
            <Building2 size={24} className="text-accent" />
            {workspaceDetails.organizationName}
          </Typography>
          <Typography type="body-xs" className="text-muted font-light mt-0.5">
            {isEn ? "Workspace context:" : "Không gian làm việc:"} <span className="font-mono text-accent">@{workspaceDetails.organizationSlug}</span> • {isEn ? "My Role:" : "Vai trò:"} <span className="font-semibold text-foreground">{workspaceDetails.userRole}</span>
          </Typography>
        </div>
        <div className="flex gap-2">
          <Chip color="success" variant="soft" size="sm" className="font-semibold text-xs py-1">
            <ShieldCheck size={12} className="inline mr-1" />
            {isEn ? "Verified Enterprise" : "Doanh nghiệp đã xác minh"}
          </Chip>
        </div>
      </div>

      {/* 2. Top Title and Edit Button */}
      <div className="flex justify-between items-center pb-2">
        <Typography type="h3" className="font-bold text-foreground text-xl">
          {isEn ? "Organization Settings" : "Thiết lập tổ chức"}
        </Typography>
        {canEdit && !isEditing && (
          <Button
            size="sm"
            variant="outline"
            onClick={() => setIsEditing(true)}
            className="font-bold text-xs cursor-pointer border-border text-foreground bg-surface hover:bg-card/40"
          >
            <Edit3 size={14} className="mr-1.5" />
            {isEn ? "Edit Profile" : "Chỉnh sửa hồ sơ"}
          </Button>
        )}
      </div>

      {/* 3. Main Form or Read Mode (2 columns) */}
      {isEditing ? (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
          {/* Column 1 (Left 2-colspan) */}
          <div className="lg:col-span-2 space-y-6">
            {/* Card: Thông tin hành chính */}
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl">
              <div className="space-y-6">
                <Typography type="h4" className="font-bold text-foreground flex items-center gap-2 pb-2 border-b border-separator/40">
                  <Building2 size={18} className="text-accent" />
                  {isEn ? "Administrative Details" : "Thông tin hành chính"}
                </Typography>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Tax Code (Fixed)" : "Mã số thuế (Cố định)"}</span>
                    <Input
                      type="text"
                      value={workspaceDetails.taxCode || ""}
                      disabled
                      className="w-full text-xs opacity-70"
                    />
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Company Type" : "Loại hình công ty"}</span>
                    <select
                      value={companyType}
                      onChange={(e) => setCompanyType(e.target.value)}
                      className="w-full text-xs font-semibold px-3.5 py-2.5 rounded-xl border border-border bg-surface text-foreground focus:outline-hidden cursor-pointer hover:border-accent/50 transition-colors"
                    >
                      <option value="">{isEn ? "Select Company Type..." : "Chọn loại hình công ty..."}</option>
                      <option value="Cổ phần">{isEn ? "Joint Stock" : "Cổ phần"}</option>
                      <option value="TNHH">{isEn ? "LLC" : "TNHH"}</option>
                      <option value="Tập đoàn">{isEn ? "Corporation" : "Tập đoàn"}</option>
                      <option value="Startup">{isEn ? "Startup" : "Startup"}</option>
                    </select>
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Company Size" : "Quy mô công ty"}</span>
                    <select
                      value={companySize}
                      onChange={(e) => setCompanySize(e.target.value)}
                      className="w-full text-xs font-semibold px-3.5 py-2.5 rounded-xl border border-border bg-surface text-foreground focus:outline-hidden cursor-pointer hover:border-accent/50 transition-colors"
                    >
                      <option value="">{isEn ? "Select Company Size..." : "Chọn quy mô..."}</option>
                      <option value="1-9 người">{isEn ? "1-9 employees" : "1-9 người"}</option>
                      <option value="10-49 người">{isEn ? "10-49 employees" : "10-49 người"}</option>
                      <option value="50-99 người">{isEn ? "50-99 employees" : "50-99 người"}</option>
                      <option value="100-499 người">{isEn ? "100-499 employees" : "100-499 người"}</option>
                      <option value="500+ người">{isEn ? "500+ employees" : "500+ người"}</option>
                    </select>
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Branch Count" : "Số lượng chi nhánh"}</span>
                    <Input
                      type="number"
                      value={branchCount.toString()}
                      onChange={(e) => setBranchCount(Math.max(0, parseInt(e.target.value) || 0))}
                      className="w-full text-xs"
                      min="0"
                    />
                  </div>
                </div>
              </div>
            </Card>

            {/* Card: Lĩnh vực & Sứ mệnh */}
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl">
              <div className="space-y-6">
                <Typography type="h4" className="font-bold text-foreground flex items-center gap-2 pb-2 border-b border-separator/40">
                  <Briefcase size={18} className="text-accent" />
                  {isEn ? "Focus Areas & Industry Tags" : "Lĩnh vực hoạt động & Từ khóa"}
                </Typography>
                <div className="space-y-4">
                  <div className="space-y-1.5">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Primary Industries" : "Lĩnh vực chính"}</span>
                    
                    {/* Predefined Tag Recommendations */}
                    <div className="space-y-3 pb-3 border-b border-separator/20">
                      <span className="text-[10px] font-bold text-muted uppercase block">{isEn ? "Recommended Focus Areas" : "Lĩnh vực gợi ý"}</span>
                      <div className="space-y-2.5">
                        {[
                          {
                            category: isEn ? "Software & Systems" : "Phần mềm & Hệ thống",
                            tags: PREDEFINED_TAGS.slice(0, 4)
                          },
                          {
                            category: isEn ? "AI & Data" : "AI & Dữ liệu",
                            tags: PREDEFINED_TAGS.slice(4, 8)
                          },
                          {
                            category: isEn ? "Hardware & Advanced Tech" : "Phần cứng & Công nghệ vi mạch",
                            tags: PREDEFINED_TAGS.slice(8, 12)
                          }
                        ].map((group) => (
                          <div key={group.category} className="space-y-1">
                            <span className="text-[9px] font-semibold text-muted/80 block">{group.category}</span>
                            <div className="flex flex-wrap gap-1.5">
                              {group.tags.map((tag) => {
                                const isChecked = industryTags.some(t => t === tag.id || t === tag.vi || t === tag.en);
                                return (
                                  <div
                                    key={tag.id}
                                    onClick={() => {
                                      if (isChecked) {
                                        setIndustryTags(industryTags.filter(t => t !== tag.id && t !== tag.vi && t !== tag.en));
                                      } else {
                                        setIndustryTags([...industryTags, tag.id]);
                                      }
                                    }}
                                    className={`px-2.5 py-1 rounded-lg border text-[11px] font-semibold cursor-pointer transition-all select-none ${
                                      isChecked
                                        ? "bg-accent/15 border-accent text-accent shadow-xs"
                                        : "border-border bg-card/10 text-muted hover:border-muted/50"
                                    }`}
                                  >
                                    {isEn ? tag.en : tag.vi}
                                  </div>
                                );
                              })}
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>

                    <div className="flex gap-2 pt-2">
                      <Input
                        type="text"
                        placeholder={isEn ? "Type a sector and press Enter..." : "Nhập lĩnh vực và nhấn Enter..."}
                        value={newTagInput}
                        onChange={(e) => setNewTagInput(e.target.value)}
                        onKeyDown={(e) => e.key === "Enter" && (e.preventDefault(), handleAddIndustryTag())}
                        className="w-full text-xs"
                      />
                      <Button 
                        onClick={handleAddIndustryTag}
                        variant="outline"
                        size="sm"
                        className="cursor-pointer border-border px-3 font-semibold text-xs h-10"
                      >
                        <Plus size={14} /> {isEn ? "Add" : "Thêm"}
                      </Button>
                    </div>
                    <div className="flex flex-wrap gap-1.5 pt-2">
                      {industryTags.length === 0 ? (
                        <span className="text-xs text-muted font-light">{isEn ? "No industry tags configured." : "Chưa cấu hình từ khóa lĩnh vực."}</span>
                      ) : (
                        industryTags.map((tag) => (
                          <Chip
                            key={tag}
                            variant="soft"
                            color="accent"
                            size="sm"
                            className="text-xs font-semibold pr-1.5 flex items-center gap-1 select-none"
                          >
                            <span>{getTagLabel(tag, isEn)}</span>
                            <button
                              type="button"
                              onClick={() => handleRemoveIndustryTag(tag)}
                              className="opacity-70 hover:opacity-100 transition-opacity cursor-pointer p-0 border-none bg-transparent inline-flex items-center"
                            >
                              <X size={10} />
                            </button>
                          </Chip>
                        ))
                      )}
                    </div>
                  </div>

                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Short Introduction" : "Giới thiệu ngắn"}</span>
                    <TextArea
                      value={description}
                      onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setDescription(e.target.value)}
                      className="w-full text-xs font-outfit"
                      rows={4}
                      placeholder={isEn ? "Provide a brief summary about your company..." : "Nhập mô tả giới thiệu về công ty của bạn..."}
                    />
                  </div>
                </div>
              </div>
            </Card>

            {/* Card: Quyền lợi công việc */}
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl">
              <div className="space-y-6">
                <Typography type="h4" className="font-bold text-foreground flex items-center gap-2 pb-2 border-b border-separator/40">
                  <GiftIcon size={18} className="text-accent" />
                  {isEn ? "Employee Benefits & Perks" : "Phúc lợi & Quyền lợi nhân viên"}
                </Typography>
                <div className="space-y-5">
                  <div className="space-y-1.5">
                    <span className="text-[10px] font-bold text-muted uppercase block">{isEn ? "Select Common Benefits" : "Chọn phúc lợi phổ biến"}</span>
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-2">
                      {VI_BENEFITS_LIST.map((benefit) => {
                        const isChecked = benefitTags.includes(benefit);
                        return (
                          <div
                            key={benefit}
                            onClick={() => handleTogglePredefinedBenefit(benefit)}
                            className={`p-2.5 rounded-xl border text-xs font-semibold cursor-pointer transition-all select-none text-center ${
                              isChecked
                                ? "bg-accent/10 border-accent text-accent"
                                : "border-border bg-card/20 text-muted hover:border-muted/50"
                            }`}
                          >
                            {getBenefitLabel(benefit)}
                          </div>
                        );
                      })}
                    </div>
                  </div>

                  <div className="space-y-1.5">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Add Custom Benefits" : "Thêm phúc lợi tùy chỉnh"}</span>
                    <div className="flex gap-2">
                      <Input
                        type="text"
                        placeholder={isEn ? "Type custom benefit and add..." : "Nhập phúc lợi tự chọn..."}
                        value={newBenefitInput}
                        onChange={(e) => setNewBenefitInput(e.target.value)}
                        onKeyDown={(e) => e.key === "Enter" && (e.preventDefault(), handleAddCustomBenefit())}
                        className="w-full text-xs"
                      />
                      <Button
                        onClick={handleAddCustomBenefit}
                        variant="outline"
                        size="sm"
                        className="cursor-pointer border-border px-3 font-semibold text-xs h-10"
                      >
                        <Plus size={14} /> {isEn ? "Add" : "Thêm"}
                      </Button>
                    </div>
                  </div>

                  <div className="flex flex-wrap gap-1.5 pt-2 border-t border-separator/20">
                    {benefitTags.length === 0 ? (
                      <span className="text-xs text-muted font-light">{isEn ? "No benefits configured." : "Chưa cấu hình phúc lợi."}</span>
                    ) : (
                      benefitTags.map((benefit) => (
                        <Chip
                          key={benefit}
                          variant="soft"
                          color="success"
                          size="sm"
                          className="text-xs font-semibold pr-1.5 flex items-center gap-1 select-none"
                          title={benefit}
                        >
                          <span>{getBenefitLabel(benefit)}</span>
                          <button
                            type="button"
                            onClick={() => handleRemoveBenefitTag(benefit)}
                            className="opacity-70 hover:opacity-100 transition-opacity cursor-pointer p-0 border-none bg-transparent inline-flex items-center"
                          >
                            <X size={10} />
                          </button>
                        </Chip>
                      ))
                    )}
                  </div>
                </div>
              </div>
            </Card>
          </div>

          {/* Column 2 (Right 1-colspan) */}
          <div className="space-y-6">
            {/* Card: Thông tin liên hệ */}
            <Card className="p-6 bg-surface border border-border rounded-2xl">
              <div className="space-y-4">
                <Typography type="h4" className="font-bold text-foreground flex items-center gap-2 pb-2 border-b border-separator/40">
                  <Mail size={16} className="text-accent" />
                  {isEn ? "Hiring Representative" : "Người đại diện tuyển dụng"}
                </Typography>
                <div className="space-y-3">
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Contact Name" : "Tên liên hệ"}</span>
                    <Input
                      type="text"
                      value={contactName}
                      onChange={(e) => setContactName(e.target.value)}
                      placeholder="e.g. HR Team"
                      className="w-full text-xs"
                    />
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Contact Phone" : "Số điện thoại"}</span>
                    <Input
                      type="tel"
                      value={contactPhone}
                      onChange={(e) => setContactPhone(e.target.value)}
                      placeholder="e.g. 0912345678"
                      className="w-full text-xs"
                    />
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Contact Email" : "Email nhận CV"}</span>
                    <Input
                      type="email"
                      value={contactEmail}
                      onChange={(e) => setContactEmail(e.target.value)}
                      placeholder="e.g. careers@fpt.com"
                      className="w-full text-xs"
                    />
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Website" : "Trang web"}</span>
                    <Input
                      type="url"
                      value={website}
                      onChange={(e) => setWebsite(e.target.value)}
                      placeholder="e.g. https://fpt.com"
                      className="w-full text-xs"
                    />
                  </div>
                </div>
              </div>
            </Card>

            {/* Card: Địa chỉ và bản đồ */}
            <Card className="p-6 bg-surface border border-border rounded-2xl">
              <div className="space-y-4">
                <Typography type="h4" className="font-bold text-foreground flex items-center gap-2 pb-2 border-b border-separator/40">
                  <MapPin size={16} className="text-accent" />
                  {isEn ? "Office Location" : "Địa điểm văn phòng"}
                </Typography>
                <div className="space-y-3">
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "City/Province" : "Tỉnh/Thành phố"}</span>
                    <Input
                      type="text"
                      value={city}
                      onChange={(e) => setCity(e.target.value)}
                      placeholder="e.g. Hanoi"
                      className="w-full text-xs"
                    />
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">{isEn ? "Detailed Address" : "Địa chỉ chi tiết"}</span>
                    <Input
                      type="text"
                      value={detailAddress}
                      onChange={(e) => setDetailAddress(e.target.value)}
                      placeholder="e.g. FPT Tower, Cau Giay, Hanoi"
                      className="w-full text-xs"
                    />
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase flex items-center gap-1">
                      <span>{isEn ? "Map Embed URL" : "Link nhúng bản đồ"}</span>
                      <span title={isEn ? "Go to Google Maps -> Share -> Embed Map and copy the URL from src attribute in the iframe tag" : "Nhúng bản đồ Google Maps của bạn và lấy liên kết liên quan từ thẻ src"} className="cursor-help text-muted">
                        <Info size={12} />
                      </span>
                    </span>
                    <Input
                      type="url"
                      value={mapInputVal}
                      onChange={(e) => {
                        const val = e.target.value;
                        const { sanitizedUrl, error } = sanitizeMapUrl(val);
                        setMapValidationError(error);
                        if (sanitizedUrl) {
                          setGoogleMapsEmbedUrl(sanitizedUrl);
                          setMapInputVal(sanitizedUrl);
                        } else {
                          setMapInputVal(val);
                          setGoogleMapsEmbedUrl("");
                        }
                      }}
                      placeholder="https://www.google.com/maps/embed?pb=..."
                      className={`w-full text-xs ${mapValidationError ? "border-danger focus:border-danger focus:ring-1 focus:ring-danger/20" : ""}`}
                    />
                    {mapValidationError && (
                      <span className="text-[11px] font-semibold text-danger block mt-1">
                        {mapValidationError}
                      </span>
                    )}
                  </div>

                  <div className="w-full h-[250px] rounded-xl overflow-hidden border border-border mt-2 relative">
                    {googleMapsEmbedUrl ? (
                      <iframe
                        src={googleMapsEmbedUrl}
                        className="w-full h-full border-0"
                        allowFullScreen
                        loading="lazy"
                        referrerPolicy="no-referrer-when-downgrade"
                        title="Google Maps Location Preview"
                        style={{ border: 0 }}
                      />
                    ) : (
                      <div className="w-full h-full flex flex-col items-center justify-center bg-card/10 text-muted p-4 text-center select-none">
                        <Map size={36} className="text-muted/60 mb-2" />
                        <span className="text-xs font-semibold text-muted">
                          Map preview will appear here once a valid Embed URL is provided.
                        </span>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </Card>

            {/* Card: Mạng xã hội */}
            <Card className="p-6 bg-surface border border-border rounded-2xl">
              <div className="space-y-4">
                <Typography type="h4" className="font-bold text-foreground flex items-center gap-2 pb-2 border-b border-separator/40">
                  <Link2 size={16} className="text-accent" />
                  {isEn ? "Social Networks" : "Mạng xã hội"}
                </Typography>
                <div className="space-y-3">
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">LinkedIn URL</span>
                    <Input
                      type="url"
                      value={linkedinUrl}
                      onChange={(e) => setLinkedinUrl(e.target.value)}
                      placeholder="https://linkedin.com/company/..."
                      className="w-full text-xs"
                    />
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">Facebook URL</span>
                    <Input
                      type="url"
                      value={facebookUrl}
                      onChange={(e) => setFacebookUrl(e.target.value)}
                      placeholder="https://facebook.com/..."
                      className="w-full text-xs"
                    />
                  </div>
                  <div className="space-y-1">
                    <span className="text-[10px] font-bold text-muted uppercase">X (Twitter) URL</span>
                    <Input
                      type="url"
                      value={twitterUrl}
                      onChange={(e) => setTwitterUrl(e.target.value)}
                      placeholder="https://x.com/..."
                      className="w-full text-xs"
                    />
                  </div>
                </div>
              </div>
            </Card>
          </div>

          {/* Action buttons footer */}
          <div className="col-span-full flex gap-2 justify-end pt-4 border-t border-separator/40">
            <Button
              size="sm"
              variant="outline"
              onClick={() => setIsEditing(false)}
              className="font-bold text-xs cursor-pointer border-border text-foreground hover:bg-card/40"
            >
              {isEn ? "Cancel" : "Hủy"}
            </Button>
            <Button
              size="sm"
              variant="primary"
              onClick={handleSave}
              isDisabled={isSaving}
              className="font-bold text-xs bg-accent text-background border-none hover:bg-accent/90 shrink-0 cursor-pointer"
            >
              {isSaving ? (isEn ? "Saving..." : "Đang lưu...") : (
                <>
                  <Save size={14} className="mr-1.5" />
                  {isEn ? "Save Changes" : "Lưu thay đổi"}
                </>
              )}
            </Button>
          </div>
        </div>
      ) : (
        /* Read Only Mode view */
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
          {/* Column 1 (Left 2-colspan) */}
          <div className="lg:col-span-2 space-y-6">
            {/* Overview Detail Card */}
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl">
              <div className="space-y-6">
                <Typography type="h3" className="font-bold text-foreground text-lg">
                  {isEn ? "About the Company" : "Về công ty"}
                </Typography>
                <Typography type="body-xs" className="text-muted leading-relaxed text-sm">
                  {description || (isEn ? "No description configured yet." : "Chưa cấu hình phần giới thiệu.")}
                </Typography>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 pt-4 border-t border-separator/40">
                  <div className="flex items-center gap-3">
                    <div className="w-9 h-9 rounded-xl bg-accent/10 text-accent flex items-center justify-center">
                      <Globe size={16} />
                    </div>
                    <div>
                      <span className="text-[10px] text-muted font-bold uppercase block">{isEn ? "Website" : "Trang web"}</span>
                      {website ? (
                        <a href={website} target="_blank" rel="noopener noreferrer" className="text-xs font-bold text-accent hover:underline">
                          {website.replace("https://", "").replace("http://", "")}
                        </a>
                      ) : (
                        <span className="text-xs font-bold text-muted-foreground">{isEn ? "Not set" : "Chưa liên kết"}</span>
                      )}
                    </div>
                  </div>

                  <div className="flex items-center gap-3">
                    <div className="w-9 h-9 rounded-xl bg-accent/10 text-accent flex items-center justify-center">
                      <MapPin size={16} />
                    </div>
                    <div>
                      <span className="text-[10px] text-muted font-bold uppercase block">{isEn ? "Headquarters" : "Trụ sở chính"}</span>
                      <span className="text-xs font-bold text-foreground">
                        {detailAddress ? `${detailAddress}, ${city}` : city || workspaceDetails.location || (isEn ? "Not set" : "Chưa thiết lập")}
                      </span>
                    </div>
                  </div>

                  <div className="flex items-center gap-3">
                    <div className="w-9 h-9 rounded-xl bg-accent/10 text-accent flex items-center justify-center">
                      <Briefcase size={16} />
                    </div>
                    <div>
                      <span className="text-[10px] text-muted font-bold uppercase block">{isEn ? "Company Type & Size" : "Loại hình & Quy mô công ty"}</span>
                      <span className="text-xs font-bold text-foreground">
                        {companyType ? getCompanyTypeLabel(companyType) : (isEn ? "Not set" : "Chưa thiết lập")} {companySize ? `(${getCompanySizeLabel(companySize)})` : ""}
                      </span>
                    </div>
                  </div>

                  <div className="flex items-center gap-3">
                    <div className="w-9 h-9 rounded-xl bg-accent/10 text-accent flex items-center justify-center">
                      <Building2 size={16} />
                    </div>
                    <div>
                      <span className="text-[10px] text-muted font-bold uppercase block">{isEn ? "Branch Count" : "Số lượng chi nhánh"}</span>
                      <span className="text-xs font-bold text-foreground">
                        {branchCount} {isEn ? "office branches" : "chi nhánh văn phòng"}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </Card>

            {/* Card: Lĩnh vực & Tags */}
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl">
              <div className="space-y-4">
                <Typography type="h3" className="font-bold text-foreground text-lg">
                  {isEn ? "Focus Areas & Industry Tags" : "Lĩnh vực hoạt động & Từ khóa"}
                </Typography>
                <div className="flex flex-wrap gap-1.5 pt-2">
                  {industryTags.length === 0 ? (
                    <span className="text-xs text-muted font-light">{isEn ? "No industry tags configured." : "Chưa cấu hình từ khóa lĩnh vực."}</span>
                  ) : (
                    industryTags.map((tag) => (
                      <Chip key={tag} variant="soft" color="accent" size="sm" className="text-xs font-semibold">
                        {getTagLabel(tag, isEn)}
                      </Chip>
                    ))
                  )}
                </div>
              </div>
            </Card>

            {/* Card: Phúc lợi */}
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl">
              <div className="space-y-4">
                <Typography type="h3" className="font-bold text-foreground text-lg">
                  {isEn ? "Employee Benefits & Perks" : "Phúc lợi & Quyền lợi nhân viên"}
                </Typography>
                <div className="flex flex-wrap gap-1.5 pt-2">
                  {benefitTags.length === 0 ? (
                    <span className="text-xs text-muted font-light">{isEn ? "No benefits configured." : "Chưa cấu hình phúc lợi."}</span>
                  ) : (
                    benefitTags.map((benefit) => (
                      <Chip key={benefit} variant="soft" color="success" size="sm" className="text-xs font-semibold">
                        {getBenefitLabel(benefit)}
                      </Chip>
                    ))
                  )}
                </div>
              </div>
            </Card>
          </div>

          {/* Column 2 (Right 1-colspan) */}
          <div className="space-y-6">
            {/* Recruitment Contact Card */}
            <Card className="p-6 bg-surface border border-border rounded-2xl">
              <div className="space-y-4">
                <Typography type="h4" className="font-bold text-foreground flex items-center gap-2 border-b border-separator/40 pb-2">
                  <Users size={16} className="text-accent" />
                  {isEn ? "Hiring Representative" : "Người đại diện tuyển dụng"}
                </Typography>
                <div className="space-y-3 text-xs select-none">
                  {contactName && (
                    <div className="flex items-center gap-2">
                      <span className="font-bold text-foreground">{contactName}</span>
                    </div>
                  )}
                  {contactPhone && (
                    <div className="flex items-center gap-2 text-muted">
                      <Phone size={13} className="text-accent" />
                      <span>{contactPhone}</span>
                    </div>
                  )}
                  {contactEmail && (
                    <div className="flex items-center gap-2 text-muted">
                      <Mail size={13} className="text-accent" />
                      <a href={`mailto:${contactEmail}`} className="text-accent hover:underline">{contactEmail}</a>
                    </div>
                  )}
                  {!contactName && !contactPhone && !contactEmail && (
                    <span className="text-xs text-muted font-light">{isEn ? "No hiring contact info provided." : "Chưa cung cấp thông tin liên hệ."}</span>
                  )}
                </div>
              </div>
            </Card>

            {/* Map Address Card */}
            <Card className="p-6 bg-surface border border-border rounded-2xl">
              <div className="space-y-4">
                <Typography type="h4" className="font-bold text-foreground flex items-center gap-2 border-b border-separator/40 pb-2">
                  <Map size={16} className="text-accent" />
                  {isEn ? "Office Location" : "Địa điểm văn phòng"}
                </Typography>
                <div className="space-y-3">
                  <div className="text-xs">
                    <span className="font-bold block text-muted-foreground uppercase text-[9px] mb-0.5">{isEn ? "Address" : "Địa chỉ"}</span>
                    <span className="font-semibold text-foreground">{detailAddress || (isEn ? "Address not set." : "Chưa thiết lập địa chỉ.")}</span>
                    {city && <span className="block text-foreground mt-0.5">{city}</span>}
                  </div>

                  {googleMapsEmbedUrl && (
                    <div className="aspect-video w-full rounded-xl overflow-hidden border border-border mt-2">
                      <iframe
                        src={googleMapsEmbedUrl}
                        className="w-full h-full border-none"
                        allowFullScreen
                        loading="lazy"
                        referrerPolicy="no-referrer-when-downgrade"
                        title="Google Maps Location"
                      />
                    </div>
                  )}
                </div>
              </div>
            </Card>

            {/* Social Coordinates Card */}
            <Card className="p-6 bg-surface border border-border rounded-2xl">
              <div className="space-y-4">
                <Typography type="h4" className="font-bold text-foreground border-b border-separator/40 pb-2">
                  {isEn ? "Social Networks" : "Mạng xã hội"}
                </Typography>
                <div className="flex flex-col gap-2">
                  {linkedinUrl && (
                    <a
                      href={linkedinUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-2.5 px-3 py-2.5 rounded-xl border border-border bg-card/10 hover:bg-card/50 transition-colors text-xs font-semibold text-muted hover:text-foreground"
                    >
                      <span className="text-accent">LinkedIn</span>
                    </a>
                  )}
                  {facebookUrl && (
                    <a
                      href={facebookUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-2.5 px-3 py-2.5 rounded-xl border border-border bg-card/10 hover:bg-card/50 transition-colors text-xs font-semibold text-muted hover:text-foreground"
                    >
                      <span className="text-accent">Facebook</span>
                    </a>
                  )}
                  {twitterUrl && (
                    <a
                      href={twitterUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-2.5 px-3 py-2.5 rounded-xl border border-border bg-card/10 hover:bg-card/50 transition-colors text-xs font-semibold text-muted hover:text-foreground"
                    >
                      <span className="text-accent">Twitter (X)</span>
                    </a>
                  )}
                  {!linkedinUrl && !facebookUrl && !twitterUrl && (
                    <span className="text-xs text-muted font-light text-center py-2">{isEn ? "No social networks linked." : "Chưa liên kết mạng xã hội."}</span>
                  )}
                </div>
              </div>
            </Card>
          </div>
        </div>
      )}
    </div>
  );
};

// Inline helper Gift icon for Lucide compatibility
const GiftIcon = (props: React.SVGProps<SVGSVGElement> & { size?: number }) => (
  <svg
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
    className={props.className}
    style={{ width: props.size || 24, height: props.size || 24 }}
  >
    <rect x="3" y="8" width="18" height="4" rx="1" />
    <path d="M12 8v13" />
    <path d="M19 12v7a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2v-7" />
    <path d="M7.5 8a2.5 2.5 0 0 1 0-5A4.8 8 0 0 1 12 8a4.8 8 0 0 1 4.5-5a2.5 2.5 0 0 1 0 5" />
  </svg>
);

export default WorkspaceInformationView;
