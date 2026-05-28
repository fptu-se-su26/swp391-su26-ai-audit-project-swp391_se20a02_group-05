/**
 * Black-box E2E — auth gateway routes and login form behavior.
 */
describe('Auth Gateway (black-box)', () => {
  describe('page routing', () => {
    it('gateway page loads', () => {
      cy.visit('/gateway');
      cy.url().should('include', '/gateway');
    });

    it('login page loads', () => {
      cy.visit('/login');
      cy.url().should('include', '/login');
    });
  });

  describe('login page UI', () => {
    beforeEach(() => {
      cy.visit('/login');
    });

    it('displays brand logo (not broken)', () => {
      cy.get('img[alt="CVerify Logo"]')
        .should('be.visible')
        .and(($img) => {
          const el = $img[0] as HTMLImageElement;
          expect(el.naturalWidth).to.be.greaterThan(0);
        });
    });

    it('shows protocol version badge', () => {
      cy.contains('PROTOCOL V1.0.0').should('be.visible');
    });

    it('has an email input field', () => {
      cy.get('input[type="email"], input[name="email"]').should('exist');
    });

    it('has a password input field', () => {
      cy.get('input[type="password"]').should('exist');
    });

    it('submit button is disabled when fields are empty', () => {
      cy.get('button[type="submit"]').should('be.disabled');
    });
  });

  describe('company verification page link', () => {
    it('company-verification page is accessible from gateway area', () => {
      cy.visit('/company-verification');
      cy.url().should('include', '/company-verification');
    });
  });
});
