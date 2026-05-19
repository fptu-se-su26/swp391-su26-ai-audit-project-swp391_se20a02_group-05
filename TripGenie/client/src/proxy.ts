import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';
import { jwtVerify } from 'jose';
import { ROUTES } from './lib/constants/auth.constants';
import { UserRole } from './types/auth.types';
import { normalizeRole } from './lib/utils/auth-utils';

const SUPPORTED_LANGS = ['vi', 'en'];
const DEFAULT_LANG = 'vi';
const LANG_COOKIE_NAME = 'i18next';

function handleLocale(request: NextRequest, response: NextResponse): void {
  const { cookies, headers } = request;
  let locale = cookies.get(LANG_COOKIE_NAME)?.value;

  if (!locale) {
    const acceptLang = headers.get('accept-language');
    if (acceptLang) {
      const preferred = acceptLang
        .split(',')
        .map((lang) => lang.split(';')[0].trim().substring(0, 2).toLowerCase())
        .find((lang) => SUPPORTED_LANGS.includes(lang));

      if (preferred) {
        locale = preferred;
      }
    }
  }

  if (!locale || !SUPPORTED_LANGS.includes(locale)) {
    locale = DEFAULT_LANG;
  }

  if (cookies.get(LANG_COOKIE_NAME)?.value !== locale) {
    response.cookies.set(LANG_COOKIE_NAME, locale, {
      path: '/',
      maxAge: 31536000, // 1 year
      sameSite: 'lax',
    });
  }
}

export async function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const isDev = process.env.NODE_ENV === 'development';
  
  // Extract tokens from cookies, aligned with C# snake_case cookie naming
  const accessToken = request.cookies.get('access_token')?.value;

  // Define route classifications
  const isDashboardRoute = pathname.startsWith('/dashboard');
  const isAuthRoute = ([
    ROUTES.LOGIN,
    ROUTES.REGISTER,
    ROUTES.FORGOT_PASSWORD,
    ROUTES.RESET_PASSWORD,
    ROUTES.VERIFY_EMAIL,
  ] as string[]).includes(pathname);

  let userRole: UserRole = 'USER';
  let isTokenValid = false;
  let isEmailVerified = false;

  if (accessToken) {
    try {
      const secret = new TextEncoder().encode(process.env.JWT_SECRET || 'DbqDgBM1u2H5lNnUFBgYrRaotpSP9Wda8jASgjIbFh6');
      // Verify JWT signature cryptographically at the edge
      const { payload } = await jwtVerify(accessToken, secret);
      
      isTokenValid = true;
      isEmailVerified = payload.isEmailVerified === 'true' || payload.isEmailVerified === true;
      
      // Extract role handling both standard and .NET ClaimTypes.Role serialized formats
      const rolesRaw = (
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        payload.role ||
        payload.roles
      ) as string | string[] | undefined | null;

      // Centralized role parsing and normalization
      userRole = normalizeRole(rolesRaw);
    } catch (error) {
      if (isDev) {
        console.warn('[Security Proxy] Cryptographic JWT verification failed or token expired:', (error as Error).message);
      }
      isTokenValid = false;
    }
  }

  // Development environment gated edge logging to prevent production data leakage
  if (isDev) {
    console.log(
      `[Security Proxy] Route: ${pathname} | Token Present: ${!!accessToken} | Valid: ${isTokenValid} | Role: ${userRole}`
    );
  }

  // 1. Gating Auth Pages (Prevent logged-in users from seeing /login or /register)
  if (isAuthRoute && isTokenValid) {
    // Unverified users accessing login or register should be redirected to verify-email,
    // but they should be allowed to view the verify-email page itself!
    if (!isEmailVerified) {
      if (pathname !== ROUTES.VERIFY_EMAIL) {
        if (isDev) {
          console.log(`[Security Proxy] Logged-in unverified user accessing auth route. Redirecting to verify-email.`);
        }
        const response = NextResponse.redirect(new URL(ROUTES.VERIFY_EMAIL, request.url));
        handleLocale(request, response);
        return response;
      }
      const response = NextResponse.next();
      handleLocale(request, response);
      return response;
    }

    if (isDev) {
      console.log(`[Security Proxy] Logged-in verified user accessing auth route. Redirecting to: /dashboard`);
    }
    const response = NextResponse.redirect(new URL('/dashboard', request.url));
    handleLocale(request, response);
    return response;
  }

  // 2. Root Dashboard Redirect (Exact match for '/dashboard' or '/dashboard/')
  if (pathname === '/dashboard' || pathname === '/dashboard/') {
    if (!isTokenValid) {
      const redirectUrl = new URL(ROUTES.LOGIN, request.url);
      if (isDev) {
        console.log(`[Security Proxy] Unauthenticated root dashboard access. Redirecting to: ${ROUTES.LOGIN}`);
      }
      const response = NextResponse.redirect(redirectUrl);
      handleLocale(request, response);
      return response;
    }

    // Force unverified authenticated users to verify their email
    if (!isEmailVerified) {
      if (isDev) {
        console.log(`[Security Proxy] Authenticated unverified root dashboard access. Redirecting to verify-email.`);
      }
      const response = NextResponse.redirect(new URL(ROUTES.VERIFY_EMAIL, request.url));
      handleLocale(request, response);
      return response;
    }

    let targetDashboard: string = ROUTES.DASHBOARD.USER;
    if (userRole === 'ADMIN') {
      targetDashboard = ROUTES.DASHBOARD.ADMIN;
    } else if (userRole === 'BUSINESS') {
      targetDashboard = ROUTES.DASHBOARD.BUSINESS;
    }

    if (isDev) {
      console.log(`[Security Proxy] Root dashboard redirect match. Routing to: ${targetDashboard}`);
    }
    const response = NextResponse.redirect(new URL(targetDashboard, request.url));
    handleLocale(request, response);
    return response;
  }

  // 3. Protecting Dashboard Sub-Routes
  if (isDashboardRoute) {
    // If not authenticated, redirect to /login with callback URL
    if (!isTokenValid) {
      const callbackUrl = encodeURIComponent(pathname + request.nextUrl.search);
      const redirectUrl = new URL(`${ROUTES.LOGIN}?callbackUrl=${callbackUrl}`, request.url);
      
      if (isDev) {
        console.log(`[Security Proxy] Unauthorized dashboard route access. Redirecting to login: ${redirectUrl.toString()}`);
      }

      // Clean cookies on redirect to clear potentially broken sessions
      const response = NextResponse.redirect(redirectUrl);
      response.cookies.delete('access_token');
      response.cookies.delete('refresh_token');
      handleLocale(request, response);
      return response;
    }

    // Force unverified authenticated users to verify their email
    if (!isEmailVerified) {
      if (isDev) {
        console.warn(`[Security Proxy] Access denied for dashboard. Email unverified. Redirecting to verify-email.`);
      }
      const response = NextResponse.redirect(new URL(ROUTES.VERIFY_EMAIL, request.url));
      handleLocale(request, response);
      return response;
    }

    // Role Gating Logic
    if (pathname.startsWith('/dashboard/admin') && userRole !== 'ADMIN') {
      if (isDev) {
        console.warn(`[Security Proxy] Access denied for /dashboard/admin. Role: ${userRole}. Redirecting to unauthorized.`);
      }
      const response = NextResponse.redirect(new URL(ROUTES.UNAUTHORIZED, request.url));
      handleLocale(request, response);
      return response;
    }

    if (pathname.startsWith('/dashboard/business') && !['BUSINESS', 'ADMIN'].includes(userRole)) {
      if (isDev) {
        console.warn(`[Security Proxy] Access denied for /dashboard/business. Role: ${userRole}. Redirecting to unauthorized.`);
      }
      const response = NextResponse.redirect(new URL(ROUTES.UNAUTHORIZED, request.url));
      handleLocale(request, response);
      return response;
    }

    if (pathname.startsWith('/dashboard/user') && !['USER', 'BUSINESS', 'ADMIN'].includes(userRole)) {
      if (isDev) {
        console.warn(`[Security Proxy] Access denied for /dashboard/user. Role: ${userRole}. Redirecting to unauthorized.`);
      }
      const response = NextResponse.redirect(new URL(ROUTES.UNAUTHORIZED, request.url));
      handleLocale(request, response);
      return response;
    }
  }

  const response = NextResponse.next();
  handleLocale(request, response);
  return response;
}

// Next.js Proxy matcher configuration
export const config = {
  matcher: [
    /*
     * Match all request paths except for the ones starting with:
     * - api (API routes)
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     * - public (public folder items)
     */
    '/((?!api|_next/static|_next/image|favicon.ico|public).*)',
  ],
};
