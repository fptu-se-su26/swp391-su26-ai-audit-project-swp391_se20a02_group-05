import '@testing-library/jest-dom';

// Stable theme store for component tests
jest.mock('@/stores/use-theme-store', () => ({
  useThemeStore: (selector: (state: { theme: string }) => unknown) =>
    selector({ theme: 'dark' }),
}));
