import { 
  LayoutDashboard, 
  Sparkles, 
  Building2, 
  ShieldAlert, 
  Users, 
  Shield, 
  FileText 
} from 'lucide-react';
import { NavigationNode } from '../types/navigation.types';

export const navigationConfig: NavigationNode[] = [
  {
    id: 'user-hub',
    type: 'item',
    label: 'Traveler Hub',
    translationKey: 'common:dashboard.travelerHub',
    href: '/user',
    icon: LayoutDashboard,
  },
  {
    id: 'ai-planner',
    type: 'item',
    label: 'AI Chat',
    translationKey: 'common:dashboard.aiPlanner',
    href: '/chat',
    icon: Sparkles,
  },
  {
    id: 'partner-console',
    type: 'item',
    label: 'Partner Console',
    translationKey: 'common:dashboard.partnerConsole',
    href: '/business',
    icon: Building2,
    requiredRoles: ['BUSINESS', 'ADMIN'],
  },
  {
    id: 'admin-section',
    type: 'section',
    label: 'Administration',
    translationKey: 'common:dashboard.systemAdmin',
    requiredRoles: ['ADMIN'],
    children: [
      {
        id: 'admin-group',
        type: 'group',
        label: 'System Admin',
        translationKey: 'common:dashboard.systemAdmin',
        icon: ShieldAlert,
        children: [
          {
            id: 'admin-users',
            type: 'item',
            label: 'Users',
            translationKey: 'common:admin.users',
            href: '/admin/users',
            icon: Users,
            requiredPermissions: ['users:view:list'],
          },
          {
            id: 'admin-roles',
            type: 'item',
            label: 'Roles Matrix',
            translationKey: 'common:admin.rolesMatrix',
            href: '/admin/roles',
            icon: Shield,
            requiredPermissions: ['roles:view:list'],
          },
          {
            id: 'admin-audit-logs',
            type: 'item',
            label: 'Audit Trail',
            translationKey: 'common:admin.auditTrail',
            href: '/admin/audit-logs',
            icon: FileText,
            requiredPermissions: ['ai:audit:view'],
          },
        ],
      },
    ],
  },
];
