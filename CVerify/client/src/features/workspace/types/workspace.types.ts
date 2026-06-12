export interface WorkspaceMember {
  name: string;
  email: string;
  role: 'OWNER' | 'REPRESENTATIVE' | 'HR' | 'MEMBER';
  status: string;
}

export interface LinkedOrganization {
  name: string;
  slug: string;
}

export interface WorkspaceDetails {
  organizationName: string;
  organizationSlug: string;
  userRole: 'OWNER' | 'REPRESENTATIVE' | 'HR' | 'MEMBER';
  linkedOrganizations: LinkedOrganization[];
<<<<<<< Updated upstream
=======
  permissions?: string[];
  workspaces?: LinkedWorkspace[];
  description?: string;
  website?: string;
  location?: string;
  industry?: string;
  founded?: string;
  companySize?: string;
  mission?: string;
  vision?: string;
  coreValues?: string;
  bannerUrl?: string;
  logoUrl?: string;
  followersCount?: number;
  isFollowing?: boolean;
  companyType?: string;
  branchCount?: number;
  industryTags?: string[];
  benefitTags?: string[];
  galleryUrls?: string[];
  contactName?: string;
  contactPhone?: string;
  contactEmail?: string;
  city?: string;
  detailAddress?: string;
  googleMapsEmbedUrl?: string;
  linkedinUrl?: string;
  facebookUrl?: string;
  twitterUrl?: string;
  taxCode?: string;
>>>>>>> Stashed changes
}

export interface PaginatedWorkspaceMembers {
  items: WorkspaceMember[];
  totalCount: number;
  page: number;
  pageSize: number;
}
