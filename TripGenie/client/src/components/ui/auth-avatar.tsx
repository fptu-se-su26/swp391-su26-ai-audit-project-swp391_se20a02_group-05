"use client";

import React from 'react';
import { useAuth } from '../../features/auth/hooks/use-auth';
import { useRouter } from 'next/navigation';
import { Dropdown, Avatar, Label, Separator, Typography, Button } from '@heroui/react';
import { LogOut, LayoutDashboard, Settings, Check, Sun, Moon, Waves } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { setCookie } from '../../services/axios-client';
import { useThemeStore } from '../../hooks/use-theme-store';

export function AuthAvatar() {
  const { user, logout } = useAuth();
  const router = useRouter();
  const { t, i18n } = useTranslation(['navbar', 'common']);
  const { theme, setTheme } = useThemeStore();

  if (!user) return null;

  const initials = user.fullName
    ? user.fullName
      .split(' ')
      .map((n) => n[0])
      .join('')
      .slice(0, 2)
      .toUpperCase()
    : 'U';

  const handleAction = async (key: React.Key) => {
    switch (key) {
      case 'dashboard':
        const role = user.role?.toLowerCase() || 'user';
        router.push(`/${role}`);
        break;
      case 'settings':
        router.push('/user'); // Fallback setting routing to dashboard traveler page
        break;
      case 'lang-vi':
        setCookie('i18next', 'vi');
        if (typeof window !== 'undefined') {
          localStorage.setItem('i18nextLng', 'vi');
        }
        i18n.changeLanguage('vi');
        break;
      case 'lang-en':
        setCookie('i18next', 'en');
        if (typeof window !== 'undefined') {
          localStorage.setItem('i18nextLng', 'en');
        }
        i18n.changeLanguage('en');
        break;
      case 'theme-light':
        setTheme('light');
        break;
      case 'theme-dark':
        setTheme('dark');
        break;
      case 'theme-ocean':
        setTheme('ocean');
        break;
      case 'logout':
        await logout(true);
        router.push('/login');
        break;
      default:
        break;
    }
  };

  return (
    <Dropdown>
      <Dropdown.Trigger>
        <Button
          aria-label={t('navbar:menu.userMenu')}
          className="p-0 bg-transparent hover:bg-transparent border-none min-w-0 min-h-0 size-10 rounded-full outline-hidden focus-visible:ring-2 focus-visible:ring-focus select-none shrink-0 cursor-pointer"
        >
          <Avatar className="cursor-pointer size-10 select-none hover:opacity-90 active:scale-95 transition-all bg-linear-to-tr from-indigo-500 to-emerald-500">
            {user.avatarUrl && (
              <Avatar.Image src={user.avatarUrl} alt={user.fullName} />
            )}
            <Avatar.Fallback className="text-background font-bold text-sm">
              {initials}
            </Avatar.Fallback>
          </Avatar>
        </Button>
      </Dropdown.Trigger>

      <Dropdown.Popover className="min-w-[240px] bg-overlay border border-border shadow-overlay rounded-xl p-1.5 z-50">
        <Dropdown.Menu onAction={handleAction} className="outline-hidden">
          {/* Header custom item (non-clickable info panel) */}
          <Dropdown.Item id="user-info" textValue={user.fullName} className="px-3 py-2.5 pointer-events-none select-none">
            <div className="flex flex-col">
              <Typography type="body-sm" className="font-bold text-foreground font-display truncate">
                {user.fullName}
              </Typography>
              <Typography type="body-xs" className="text-muted truncate">
                {user.email}
              </Typography>
              <span className="mt-1.5 inline-flex w-fit items-center px-2.5 py-0.5 rounded-full text-[9px] font-extrabold tracking-wider uppercase bg-surface-secondary text-foreground border border-border">
                {user.role}
              </span>
            </div>
          </Dropdown.Item>

          <Dropdown.Item id="separator-1" textValue="Separator" className="p-0 pointer-events-none select-none">
            <Separator className="my-1.5 bg-separator" />
          </Dropdown.Item>

          {/* Action items */}
          <Dropdown.Item
            id="dashboard"
            textValue={t('navbar:menu.dashboard')}
            className="flex items-center gap-2.5 px-3 py-2 rounded-xl text-sm font-medium text-foreground hover:bg-surface-secondary transition-all duration-150 cursor-pointer"
          >
            <div className="flex items-center gap-2.5 w-full">
              <LayoutDashboard size={16} />
              <Label className="cursor-pointer font-semibold text-foreground">{t('navbar:menu.dashboard')}</Label>
            </div>
          </Dropdown.Item>

          <Dropdown.Item
            id="settings"
            textValue={t('navbar:menu.settings')}
            className="flex items-center gap-2.5 px-3 py-2 rounded-xl text-sm font-medium text-foreground hover:bg-surface-secondary transition-all duration-150 cursor-pointer"
          >
            <div className="flex items-center gap-2.5 w-full">
              <Settings size={16} />
              <Label className="cursor-pointer font-semibold text-foreground">{t('navbar:menu.settings')}</Label>
            </div>
          </Dropdown.Item>

          <Dropdown.Item id="separator-2" textValue="Separator" className="p-0 pointer-events-none select-none">
            <Separator className="my-1.5 bg-separator" />
          </Dropdown.Item>

          {/* Language Selection Section */}
          <Dropdown.Item id="lang-section-title" textValue="Language" className="px-3 py-1 pointer-events-none select-none">
            <Typography type="body-xs" className="font-bold uppercase tracking-wider text-muted">
              {i18n.language === 'vi' ? 'Ngôn ngữ' : 'Language'}
            </Typography>
          </Dropdown.Item>

          <Dropdown.Item
            id="lang-vi"
            textValue="Tiếng Việt (VI)"
            className={`flex items-center justify-between px-3 py-2 rounded-xl text-sm transition-all duration-150 cursor-pointer ${i18n.language === 'vi'
              ? 'bg-surface-secondary text-foreground font-semibold'
              : 'text-muted hover:bg-surface-secondary font-medium'
              }`}
          >
            <div className="flex items-center justify-between w-full">
              <div className="flex items-center gap-2">
                <span>Tiếng Việt</span>
              </div>
              <div className="flex items-center gap-2">
                <span className="text-[10px] font-bold font-mono opacity-60">VI</span>
                {i18n.language === 'vi' && <Check size={12} className="text-foreground stroke-3" />}
              </div>
            </div>
          </Dropdown.Item>

          <Dropdown.Item
            id="lang-en"
            textValue="English (EN)"
            className={`flex items-center justify-between px-3 py-2 rounded-xl text-sm transition-all duration-150 cursor-pointer ${i18n.language === 'en'
              ? 'bg-surface-secondary text-foreground font-semibold'
              : 'text-muted hover:bg-surface-secondary font-medium'
              }`}
          >
            <div className="flex items-center justify-between w-full">
              <div className="flex items-center gap-2">
                <span>English</span>
              </div>
              <div className="flex items-center gap-2">
                <span className="text-[10px] font-bold font-mono opacity-60">EN</span>
                {i18n.language === 'en' && <Check size={12} className="text-foreground stroke-3" />}
              </div>
            </div>
          </Dropdown.Item>

          <Dropdown.Item id="separator-3" textValue="Separator" className="p-0 pointer-events-none select-none">
            <Separator className="my-1.5 bg-separator" />
          </Dropdown.Item>

          {/* Theme Selection Section */}
          <Dropdown.Item id="theme-section-title" textValue="Theme" className="px-3 py-1 pointer-events-none select-none">
            <Typography type="body-xs" className="font-bold uppercase tracking-wider text-muted">
              {i18n.language === 'vi' ? 'Giao diện' : 'Theme'}
            </Typography>
          </Dropdown.Item>

          <Dropdown.Item
            id="theme-light"
            textValue="Light Theme"
            className={`flex items-center justify-between px-3 py-2 rounded-xl text-sm transition-all duration-150 cursor-pointer ${theme === 'light'
              ? 'bg-surface-secondary text-foreground font-semibold'
              : 'text-muted hover:bg-surface-secondary font-medium'
              }`}
          >
            <div className="flex items-center justify-between w-full">
              <div className="flex items-center gap-2">
                <Sun size={14} className="opacity-80" />
                <span>{i18n.language === 'vi' ? 'Sáng' : 'Light'}</span>
              </div>
              <div>
                {theme === 'light' && <Check size={12} className="text-foreground stroke-3" />}
              </div>
            </div>
          </Dropdown.Item>

          <Dropdown.Item
            id="theme-dark"
            textValue="Dark Theme"
            className={`flex items-center justify-between px-3 py-2 rounded-xl text-sm transition-all duration-150 cursor-pointer ${theme === 'dark'
              ? 'bg-surface-secondary text-foreground font-semibold'
              : 'text-muted hover:bg-surface-secondary font-medium'
              }`}
          >
            <div className="flex items-center justify-between w-full">
              <div className="flex items-center gap-2">
                <Moon size={14} className="opacity-80" />
                <span>{i18n.language === 'vi' ? 'Tối' : 'Dark'}</span>
              </div>
              <div>
                {theme === 'dark' && <Check size={12} className="text-foreground stroke-3" />}
              </div>
            </div>
          </Dropdown.Item>

          <Dropdown.Item
            id="theme-ocean"
            textValue="Ocean Theme"
            className={`flex items-center justify-between px-3 py-2 rounded-xl text-sm transition-all duration-150 cursor-pointer ${theme === 'ocean'
              ? 'bg-surface-secondary text-foreground font-semibold'
              : 'text-muted hover:bg-surface-secondary font-medium'
              }`}
          >
            <div className="flex items-center justify-between w-full">
              <div className="flex items-center gap-2">
                <Waves size={14} className="opacity-80" />
                <span>{i18n.language === 'vi' ? 'Đại dương' : 'Ocean'}</span>
              </div>
              <div>
                {theme === 'ocean' && <Check size={12} className="text-foreground stroke-3" />}
              </div>
            </div>
          </Dropdown.Item>

          <Dropdown.Item id="separator-4" textValue="Separator" className="p-0 pointer-events-none select-none">
            <Separator className="my-1.5 bg-separator" />
          </Dropdown.Item>

          <Dropdown.Item
            id="logout"
            textValue={t('navbar:menu.logout')}
            className="flex items-center gap-2.5 px-3 py-2 rounded-xl text-sm font-semibold text-danger hover:bg-danger/10 transition-all duration-150 cursor-pointer"
          >
            <div className="flex items-center gap-2.5 w-full">
              <LogOut size={16} className="text-danger" />
              <Label className="text-danger cursor-pointer font-bold">{t('navbar:menu.logout')}</Label>
            </div>
          </Dropdown.Item>
        </Dropdown.Menu>
      </Dropdown.Popover>
    </Dropdown>
  );
}
