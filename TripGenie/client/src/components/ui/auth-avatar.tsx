"use client";

import React from 'react';
import { useAuth } from '../../features/auth/hooks/use-auth';
import { useRouter } from 'next/navigation';
import { Dropdown, Avatar, Label, Separator } from '@heroui/react';
import { LogOut, LayoutDashboard, Settings, Check } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { setCookie } from '../../services/axios-client';

export function AuthAvatar() {
  const { user, logout } = useAuth();
  const router = useRouter();
  const { t, i18n } = useTranslation(['navbar', 'common']);

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
        <button
          aria-label={t('navbar:menu.userMenu')}
          className="outline-none focus:ring-2 focus:ring-zinc-500 rounded-full transition-all duration-200 select-none shrink-0"
        >
          <Avatar className="cursor-pointer size-10 select-none hover:opacity-90 active:scale-95 transition-all bg-linear-to-tr from-indigo-500 to-emerald-500">
            {user.avatarUrl && (
              <Avatar.Image src={user.avatarUrl} alt={user.fullName} />
            )}
            <Avatar.Fallback className="text-white font-bold text-sm">
              {initials}
            </Avatar.Fallback>
          </Avatar>
        </button>
      </Dropdown.Trigger>

      <Dropdown.Popover className="min-w-[240px] bg-white/95 dark:bg-zinc-950/95 backdrop-blur-xl border border-zinc-200/80 dark:border-zinc-900 shadow-2xl rounded-2xl p-1.5 animate-in fade-in slide-in-from-top-2 duration-200 z-50">
        <Dropdown.Menu onAction={handleAction} className="outline-none">
          {/* Header custom item (non-clickable info panel) */}
          <Dropdown.Item id="user-info" textValue={user.fullName} className="px-3 py-2.5 pointer-events-none select-none">
            <div className="flex flex-col">
              <span className="font-bold text-zinc-950 dark:text-zinc-50 text-sm font-outfit truncate">
                {user.fullName}
              </span>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs truncate">
                {user.email}
              </span>
              <span className="mt-1.5 inline-flex w-fit items-center px-2.5 py-0.5 rounded-full text-[9px] font-extrabold tracking-wider uppercase bg-zinc-100 dark:bg-zinc-900 text-zinc-800 dark:text-zinc-300 border border-zinc-200/50 dark:border-zinc-800/50">
                {user.role}
              </span>
            </div>
          </Dropdown.Item>

          <Dropdown.Item id="separator-1" textValue="Separator" className="p-0 pointer-events-none select-none">
            <Separator className="my-1.5 bg-zinc-200/50 dark:bg-zinc-900/50" />
          </Dropdown.Item>

          {/* Action items */}
          <Dropdown.Item
            id="dashboard"
            textValue={t('navbar:menu.dashboard')}
            className="flex items-center gap-2.5 px-3 py-2 rounded-xl text-sm font-medium text-zinc-700 dark:text-zinc-300 hover:bg-zinc-100 dark:hover:bg-zinc-900 hover:text-zinc-950 dark:hover:text-zinc-50 transition-all duration-150 cursor-pointer"
          >
            <div className="flex items-center gap-2.5 w-full">
              <LayoutDashboard size={16} />
              <Label className="cursor-pointer font-semibold text-zinc-700 dark:text-zinc-300">{t('navbar:menu.dashboard')}</Label>
            </div>
          </Dropdown.Item>

          <Dropdown.Item
            id="settings"
            textValue={t('navbar:menu.settings')}
            className="flex items-center gap-2.5 px-3 py-2 rounded-xl text-sm font-medium text-zinc-700 dark:text-zinc-300 hover:bg-zinc-100 dark:hover:bg-zinc-900 hover:text-zinc-950 dark:hover:text-zinc-50 transition-all duration-150 cursor-pointer"
          >
            <div className="flex items-center gap-2.5 w-full">
              <Settings size={16} />
              <Label className="cursor-pointer font-semibold text-zinc-700 dark:text-zinc-300">{t('navbar:menu.settings')}</Label>
            </div>
          </Dropdown.Item>

          <Dropdown.Item id="separator-2" textValue="Separator" className="p-0 pointer-events-none select-none">
            <Separator className="my-1.5 bg-zinc-200/50 dark:bg-zinc-900/50" />
          </Dropdown.Item>

          {/* Language Selection Section */}
          <Dropdown.Item id="lang-section-title" textValue="Language" className="px-3 py-1 pointer-events-none select-none">
            <span className="text-[10px] font-bold uppercase tracking-wider text-zinc-400 dark:text-zinc-500">
              {i18n.language === 'vi' ? 'Ngôn ngữ' : 'Language'}
            </span>
          </Dropdown.Item>

          <Dropdown.Item
            id="lang-vi"
            textValue="Tiếng Việt (VI)"
            className={`flex items-center justify-between px-3 py-2 rounded-xl text-sm transition-all duration-150 cursor-pointer ${i18n.language === 'vi'
              ? 'bg-zinc-100 dark:bg-zinc-900 text-zinc-950 dark:text-zinc-50 font-semibold'
              : 'text-zinc-600 dark:text-zinc-400 hover:bg-zinc-50 dark:hover:bg-zinc-900/60 font-medium'
              }`}
          >
            <div className="flex items-center justify-between w-full">
              <div className="flex items-center gap-2">
                <span className="select-none text-base">🇻🇳</span>
                <span>Tiếng Việt</span>
              </div>
              <div className="flex items-center gap-2">
                <span className="text-[10px] font-bold font-mono opacity-60">VI</span>
                {i18n.language === 'vi' && <Check size={12} className="text-zinc-900 dark:text-zinc-50 stroke-3" />}
              </div>
            </div>
          </Dropdown.Item>

          <Dropdown.Item
            id="lang-en"
            textValue="English (EN)"
            className={`flex items-center justify-between px-3 py-2 rounded-xl text-sm transition-all duration-150 cursor-pointer ${i18n.language === 'en'
              ? 'bg-zinc-100 dark:bg-zinc-900 text-zinc-950 dark:text-zinc-50 font-semibold'
              : 'text-zinc-600 dark:text-zinc-400 hover:bg-zinc-50 dark:hover:bg-zinc-900/60 font-medium'
              }`}
          >
            <div className="flex items-center justify-between w-full">
              <div className="flex items-center gap-2">
                <span className="select-none text-base">🇺🇸</span>
                <span>English</span>
              </div>
              <div className="flex items-center gap-2">
                <span className="text-[10px] font-bold font-mono opacity-60">EN</span>
                {i18n.language === 'en' && <Check size={12} className="text-zinc-900 dark:text-zinc-50 stroke-3" />}
              </div>
            </div>
          </Dropdown.Item>

          <Dropdown.Item id="separator-3" textValue="Separator" className="p-0 pointer-events-none select-none">
            <Separator className="my-1.5 bg-zinc-200/50 dark:bg-zinc-900/50" />
          </Dropdown.Item>

          <Dropdown.Item
            id="logout"
            textValue={t('navbar:menu.logout')}
            className="flex items-center gap-2.5 px-3 py-2 rounded-xl text-sm font-semibold text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-950/20 transition-all duration-150 cursor-pointer"
          >
            <div className="flex items-center gap-2.5 w-full">
              <LogOut size={16} className="text-red-600 dark:text-red-400" />
              <Label className="text-red-600 dark:text-red-400 cursor-pointer font-bold">{t('navbar:menu.logout')}</Label>
            </div>
          </Dropdown.Item>
        </Dropdown.Menu>
      </Dropdown.Popover>
    </Dropdown>
  );
}
