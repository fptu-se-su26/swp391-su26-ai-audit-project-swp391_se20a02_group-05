/**
 * White-box component tests — AuthContent logo & layout (React Testing Library).
 */
import { render, screen, waitFor } from '@testing-library/react';
import { AuthContent } from '../auth-content';

describe('AuthContent (RTL / white-box)', () => {
  it('renders children', () => {
    render(
      <AuthContent>
        <p>Form body</p>
      </AuthContent>,
    );
    expect(screen.getByText('Form body')).toBeInTheDocument();
  });

  it('shows protocol badge', () => {
    render(<AuthContent><span /></AuthContent>);
    expect(screen.getByText('PROTOCOL V1.0.0')).toBeInTheDocument();
  });

  it('uses dark-theme logo asset after mount', async () => {
    // jest.setup.ts mocks the theme store to return 'dark'
    render(<AuthContent><span /></AuthContent>);
    const logo = await waitFor(() =>
      screen.getByRole('img', { name: 'CVerify Logo' }),
    );
    expect(logo).toHaveAttribute('src', '/brand/logo&name-white.png');
  });

  it('renders white logo on initial (pre-mount) render for SSR hydration safety', () => {
    // Before useEffect fires the component defaults to the white logo
    // regardless of theme — this prevents hydration mismatch on first paint.
    const { container } = render(<AuthContent><span /></AuthContent>);
    const logo = container.querySelector('img[alt="CVerify Logo"]');
    expect(logo).toHaveAttribute('src', '/brand/logo&name-white.png');
  });
});
