"use client";

import React, { useEffect } from "react";
import { useWorkspaceStore } from "../store/use-workspace-store";
import { Card } from "@/components/ui/card";
import { Typography, Chip } from "@heroui/react";
import {
  Building2,
  Globe,
  MapPin,
  Briefcase,
  Calendar,
  ShieldCheck,
  AlertTriangle,
  Info,
  Heart,
  Target,
  Eye,
  Phone,
  Mail
} from "lucide-react";
import { SkeletonLoader } from "@/components/ui/states";
import { useTranslation } from "react-i18next";

// Inline brand SVGs to bypass Lucide member mismatch errors
const LinkedInIcon = (props: React.SVGProps<SVGSVGElement>) => (
  <svg viewBox="0 0 24 24" fill="currentColor" className="size-4" {...props}>
    <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.779-1.75-1.75s.784-1.75 1.75-1.75 1.75.779 1.75 1.75-.784 1.75-1.75 1.75zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z" />
  </svg>
);

const TwitterIcon = (props: React.SVGProps<SVGSVGElement>) => (
  <svg viewBox="0 0 24 24" fill="currentColor" className="size-4" {...props}>
    <path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z" />
  </svg>
);

const FacebookIcon = (props: React.SVGProps<SVGSVGElement>) => (
  <svg viewBox="0 0 24 24" fill="currentColor" className="size-4" {...props}>
    <path d="M22 12c0-5.52-4.48-10-10-10S2 6.48 2 12c0 4.84 3.44 8.87 8 9.8V15H8v-3h2V9.5C10 7.57 11.57 6 13.5 6H16v3h-2c-.55 0-1 .45-1 1v2h3v3h-3v6.95c4.56-.93 8-4.96 8-9.95z" />
  </svg>
);

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

interface WorkspacePublicProfileViewProps {
  organizationSlug: string;
}

export const WorkspacePublicProfileView: React.FC<WorkspacePublicProfileViewProps> = ({
  organizationSlug,
}) => {
  const fetchWorkspace = useWorkspaceStore((s) => s.fetchWorkspace);
  const workspaceDetails = useWorkspaceStore((s) => s.workspaces[organizationSlug]);
  const isDetailsLoading = useWorkspaceStore((s) => s.loading[organizationSlug]);
  const detailsError = useWorkspaceStore((s) => s.errors[organizationSlug]);
  const { i18n } = useTranslation();
  const isEn = (i18n.language || "vi").startsWith("en");

  const BENEFIT_TRANSLATIONS: Record<string, { vi: string; en: string }> = {
    "Bảo hiểm y tế": { vi: "Bảo hiểm y tế", en: "Health Insurance" },
    "Thưởng tháng 13": { vi: "Thưởng tháng 13", en: "13th-month Salary" },
    "Đào tạo miễn phí": { vi: "Đào tạo miễn phí", en: "Free Training" },
    "Văn phòng tiện nghi": { vi: "Văn phòng tiện nghi", en: "Modern Office" },
    "Lộ trình thăng tiến": { vi: "Lộ trình thăng tiến", en: "Career Roadmap" }
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

  useEffect(() => {
    if (organizationSlug) {
      fetchWorkspace(organizationSlug);
    }
  }, [organizationSlug, fetchWorkspace]);

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
            {isAccessDenied ? (isEn ? "Access Denied" : "Truy cập bị từ chối") : (isEn ? "Workspace Loading Error" : "Lỗi tải không gian làm việc")}
          </Typography>
          <Typography type="body-xs" className="text-muted leading-relaxed mb-6">
            {isAccessDenied 
              ? (isEn ? "You do not have permission to access this organization workspace. Please verify your membership credentials or switch accounts." : "Bạn không có quyền truy cập vào không gian làm việc của tổ chức này. Vui lòng xác minh thông tin đăng nhập thành viên của bạn hoặc chuyển đổi tài khoản.")
              : detailsError || (isEn ? "Organization not found" : "Không tìm thấy tổ chức")}
          </Typography>
        </Card>
      </div>
    );
  }

  const showAddress = workspaceDetails.detailAddress || workspaceDetails.city;

  // Build Social Links dynamic array
  const socials = [];
  if (workspaceDetails.linkedinUrl) {
    socials.push({ icon: <LinkedInIcon className="size-4" />, label: "LinkedIn", href: workspaceDetails.linkedinUrl });
  }
  if (workspaceDetails.facebookUrl) {
    socials.push({ icon: <FacebookIcon className="size-4" />, label: "Facebook", href: workspaceDetails.facebookUrl });
  }
  if (workspaceDetails.twitterUrl) {
    socials.push({ icon: <TwitterIcon className="size-4" />, label: "X (Twitter)", href: workspaceDetails.twitterUrl });
  }

  // Combined headquarters display text
  const headquartersAddress = workspaceDetails.detailAddress
    ? `${workspaceDetails.detailAddress}${workspaceDetails.city ? `, ${workspaceDetails.city}` : ""}`
    : workspaceDetails.location || "";

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
            {isEn ? "Workspace context:" : "Không gian làm việc:"} <span className="font-mono text-accent">@{workspaceDetails.organizationSlug}</span>
          </Typography>
        </div>
        <div className="flex gap-2">
          <Chip color="success" variant="soft" size="sm" className="font-semibold text-xs py-1">
            <ShieldCheck size={12} className="inline mr-1" />
            {isEn ? "Verified Enterprise" : "Doanh nghiệp đã xác minh"}
          </Chip>
        </div>
      </div>

      {/* 2. Public profile details view */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
        {/* Main Column */}
        <div className="lg:col-span-2 space-y-6">
          {/* Detailed Description */}
          <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl space-y-4">
            <div className="flex justify-between items-center pb-4 border-b border-separator/40">
              <Typography type="h3" className="font-bold text-foreground">
                {isEn ? "About the Company" : "Về công ty"}
              </Typography>
            </div>
            <Typography type="body-xs" className="text-muted leading-relaxed text-sm">
              {workspaceDetails.description || (isEn ? "No description provided." : "Chưa có phần giới thiệu.")}
            </Typography>
          </Card>

          {/* Mission, Vision, and Values */}
          {(workspaceDetails.mission || workspaceDetails.vision || workspaceDetails.coreValues) && (
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl space-y-6">
              <Typography type="h3" className="font-bold text-foreground">
                {isEn ? "Corporate Pillars" : "Trụ cột doanh nghiệp"}
              </Typography>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {workspaceDetails.mission && (
                  <div className="space-y-2">
                    <div className="w-10 h-10 rounded-xl bg-accent/10 text-accent flex items-center justify-center select-none">
                      <Target size={18} />
                    </div>
                    <Typography type="body-sm" className="font-bold text-foreground text-sm">
                      {isEn ? "Mission" : "Sứ mệnh"}
                    </Typography>
                    <Typography type="body-xs" className="text-muted text-xs leading-relaxed">
                      {workspaceDetails.mission}
                    </Typography>
                  </div>
                )}

                {workspaceDetails.vision && (
                  <div className="space-y-2">
                    <div className="w-10 h-10 rounded-xl bg-accent/10 text-accent flex items-center justify-center select-none">
                      <Eye size={18} />
                    </div>
                    <Typography type="body-sm" className="font-bold text-foreground text-sm">
                      {isEn ? "Vision" : "Tầm nhìn"}
                    </Typography>
                    <Typography type="body-xs" className="text-muted text-xs leading-relaxed">
                      {workspaceDetails.vision}
                    </Typography>
                  </div>
                )}

                {workspaceDetails.coreValues && (
                  <div className="space-y-2">
                    <div className="w-10 h-10 rounded-xl bg-accent/10 text-accent flex items-center justify-center select-none">
                      <Heart size={18} />
                    </div>
                    <Typography type="body-sm" className="font-bold text-foreground text-sm">
                      {isEn ? "Core Values" : "Giá trị cốt lõi"}
                    </Typography>
                    <Typography type="body-xs" className="text-muted text-xs leading-relaxed">
                      {workspaceDetails.coreValues}
                    </Typography>
                  </div>
                )}
              </div>
            </Card>
          )}

          {/* Industry Tags */}
          {workspaceDetails.industryTags && workspaceDetails.industryTags.length > 0 && (
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl space-y-4">
              <Typography type="h3" className="font-bold text-foreground">
                {isEn ? "Focus Areas & Industry Tags" : "Lĩnh vực hoạt động & Từ khóa"}
              </Typography>
              <div className="flex flex-wrap gap-2">
                {workspaceDetails.industryTags.map((tag, idx) => (
                  <Chip key={idx} variant="soft" color="accent" size="sm" className="font-semibold text-xs py-1 px-2.5">
                    {getTagLabel(tag, isEn)}
                  </Chip>
                ))}
              </div>
            </Card>
          )}

          {/* Benefit Tags */}
          {workspaceDetails.benefitTags && workspaceDetails.benefitTags.length > 0 && (
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl space-y-4">
              <Typography type="h3" className="font-bold text-foreground">
                {isEn ? "Employee Benefits & Perks" : "Phúc lợi & Quyền lợi nhân viên"}
              </Typography>
              <div className="flex flex-wrap gap-2">
                {workspaceDetails.benefitTags.map((benefit, idx) => (
                  <Chip key={idx} variant="soft" color="success" size="sm" className="font-semibold text-xs py-1 px-2.5">
                    {getBenefitLabel(benefit)}
                  </Chip>
                ))}
              </div>
            </Card>
          )}



          {/* Office Locations & Maps */}
          {(headquartersAddress || workspaceDetails.googleMapsEmbedUrl) && (
            <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl space-y-4">
              <Typography type="h3" className="font-bold text-foreground">
                {isEn ? "Office Location" : "Địa điểm văn phòng"}
              </Typography>
              <div className="space-y-4">
                {headquartersAddress && (
                  <div className="p-4 rounded-xl border border-border bg-card/10 space-y-1 select-none">
                    <div className="flex items-center gap-2 font-bold text-xs text-foreground">
                      <MapPin size={14} className="text-accent" />
                      {isEn ? "Headquarters" : "Trụ sở chính"}
                    </div>
                    <Typography type="body-xs" className="text-muted text-xs leading-relaxed pl-5">
                      {headquartersAddress}
                    </Typography>
                  </div>
                )}

                {workspaceDetails.googleMapsEmbedUrl && (
                  <div className="rounded-xl overflow-hidden border border-border h-64 w-full">
                    <iframe
                      src={workspaceDetails.googleMapsEmbedUrl}
                      width="100%"
                      height="100%"
                      style={{ border: 0 }}
                      allowFullScreen={false}
                      loading="lazy"
                      referrerPolicy="no-referrer-when-downgrade"
                      title="Office Location Map"
                    />
                  </div>
                )}
              </div>
            </Card>
          )}
        </div>

        {/* Side Widget column */}
        <div className="space-y-6">
          {/* Specifications */}
          <Card className="p-6 bg-surface border border-border rounded-2xl space-y-4">
            <Typography type="h4" className="font-bold text-foreground flex items-center gap-2">
              <Info size={16} className="text-accent" />
              {isEn ? "Company details" : "Thông tin chi tiết"}
            </Typography>

            <div className="space-y-4 text-xs select-none">
              {workspaceDetails.website && (
                <div>
                  <span className="text-[10px] text-muted-foreground font-bold uppercase block">{isEn ? "Website" : "Trang web"}</span>
                  <a
                    href={workspaceDetails.website}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="font-semibold text-accent hover:underline break-all"
                  >
                    {workspaceDetails.website}
                  </a>
                </div>
              )}

              {workspaceDetails.industry && (
                <div>
                  <span className="text-[10px] text-muted-foreground font-bold uppercase block">{isEn ? "Industry" : "Lĩnh vực hoạt động"}</span>
                  <span className="font-semibold text-foreground">{workspaceDetails.industry}</span>
                </div>
              )}

              {workspaceDetails.companyType && (
                <div>
                  <span className="text-[10px] text-muted-foreground font-bold uppercase block">{isEn ? "Company type" : "Loại hình công ty"}</span>
                  <span className="font-semibold text-foreground">{getCompanyTypeLabel(workspaceDetails.companyType)}</span>
                </div>
              )}

              {workspaceDetails.companySize && (
                <div>
                  <span className="text-[10px] text-muted-foreground font-bold uppercase block">{isEn ? "Company Size" : "Quy mô công ty"}</span>
                  <span className="font-semibold text-foreground">{getCompanySizeLabel(workspaceDetails.companySize)}</span>
                </div>
              )}

              {workspaceDetails.branchCount !== undefined && workspaceDetails.branchCount > 0 && (
                <div>
                  <span className="text-[10px] text-muted-foreground font-bold uppercase block">{isEn ? "Branch Count" : "Số lượng chi nhánh"}</span>
                  <span className="font-semibold text-foreground">{workspaceDetails.branchCount} {isEn ? "office branches" : "chi nhánh văn phòng"}</span>
                </div>
              )}

              {headquartersAddress && (
                <div>
                  <span className="text-[10px] text-muted-foreground font-bold uppercase block">{isEn ? "Headquarters" : "Trụ sở chính"}</span>
                  <span className="font-semibold text-foreground">{headquartersAddress}</span>
                </div>
              )}

              {workspaceDetails.founded && (
                <div>
                  <span className="text-[10px] text-muted-foreground font-bold uppercase block">{isEn ? "Founded" : "Năm thành lập"}</span>
                  <span className="font-semibold text-foreground">{workspaceDetails.founded}</span>
                </div>
              )}

              {/* Recruitment Contact Details */}
              {(workspaceDetails.contactName || workspaceDetails.contactEmail || workspaceDetails.contactPhone) && (
                <div className="pt-3 border-t border-separator/40 space-y-2">
                  <span className="text-[10px] text-muted-foreground font-bold uppercase block">{isEn ? "Hiring Representative" : "Người đại diện tuyển dụng"}</span>
                  {workspaceDetails.contactName && (
                    <div className="font-semibold text-foreground">{workspaceDetails.contactName}</div>
                  )}
                  {workspaceDetails.contactEmail && (
                    <div className="flex items-center gap-1.5 text-muted-foreground font-medium">
                      <Mail size={12} className="text-accent shrink-0" />
                      <a href={`mailto:${workspaceDetails.contactEmail}`} className="text-accent hover:underline break-all">
                        {workspaceDetails.contactEmail}
                      </a>
                    </div>
                  )}
                  {workspaceDetails.contactPhone && (
                    <div className="flex items-center gap-1.5 text-muted-foreground font-medium">
                      <Phone size={12} className="text-accent shrink-0" />
                      <span>{workspaceDetails.contactPhone}</span>
                    </div>
                  )}
                </div>
              )}
            </div>
          </Card>

          {/* Social Links */}
          {socials.length > 0 && (
            <Card className="p-6 bg-surface border border-border rounded-2xl space-y-4">
              <Typography type="h4" className="font-bold text-foreground">
                {isEn ? "Social Networks" : "Mạng xã hội"}
              </Typography>
              <div className="flex flex-col gap-2">
                {socials.map((social, idx) => (
                  <a
                    key={idx}
                    href={social.href}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex items-center gap-2.5 px-3 py-2.5 rounded-xl border border-border bg-card/10 hover:bg-card/50 transition-colors text-xs font-semibold text-muted hover:text-foreground"
                  >
                    <span className="text-accent">{social.icon}</span>
                    {social.label}
                  </a>
                ))}
              </div>
            </Card>
          )}

          {/* Side Widget card */}
          <Card className="p-6 bg-surface border border-border rounded-2xl space-y-4">
            <Typography type="h4" className="font-bold text-foreground flex items-center gap-2">
              <ShieldCheck size={18} className="text-accent" />
              {isEn ? "Public Authority" : "Quyền hạn công khai"}
            </Typography>
            <Typography type="body-xs" className="text-muted leading-relaxed">
              {isEn 
                ? "This organization profile is publicly visible to candidates applying for active job postings and collaborating on shared evidence boards."
                : "Hồ sơ tổ chức này hiển thị công khai cho các ứng viên đang nộp đơn ứng tuyển vào các vị trí tuyển dụng và cộng tác trên bảng bằng chứng chia sẻ."}
            </Typography>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default WorkspacePublicProfileView;
