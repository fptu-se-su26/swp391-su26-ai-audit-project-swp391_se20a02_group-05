/**
 * Black-box E2E — company verification 3-step wizard (user-visible behavior only).
 */
describe('Company Verification (black-box)', () => {
  beforeEach(() => {
    cy.visit('/company-verification');
  });

  describe('Step 1 — Legal Identity', () => {
    it('loads wizard with step 1 heading "Register Your Company"', () => {
      cy.contains('Register Your Company').should('be.visible');
    });

    it('shows all three step labels in the stepper header', () => {
      cy.contains('Legal Identity').should('be.visible');
      cy.contains('Owner Ownership').should('be.visible');
      cy.contains('Workspace Setup').should('be.visible');
    });

    it('displays brand logo (not broken image)', () => {
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

    it('has a Company Name input field', () => {
      cy.get('input[name="companyName"]').should('exist');
    });

    it('has a Tax Code input field', () => {
      cy.get('input[name="taxCode"]').should('exist');
    });

    it('"Verify Registry" button is disabled when fields are empty', () => {
      cy.contains('button', 'Verify Registry').should('be.disabled');
    });

    it('shows tax code error after typing an invalid format and blurring', () => {
      cy.get('input[name="taxCode"]').type('123').blur();
      cy.contains('Tax code format is invalid').should('be.visible');
    });

    it('shows company name error when fewer than 2 characters are typed', () => {
      cy.get('input[name="companyName"]').type('A').blur();
      cy.contains('Company name must be at least 2 characters').should('be.visible');
    });

    it('accepts a valid 10-digit tax code format', () => {
      cy.get('input[name="taxCode"]').type('0312345678').blur();
      cy.contains('Tax code format is invalid').should('not.exist');
    });

    it('accepts a valid 13-digit branch code format (10+3)', () => {
      cy.get('input[name="taxCode"]').type('0312345678-001').blur();
      cy.contains('Tax code format is invalid').should('not.exist');
    });

    it('"Back to Sign In" button navigates away from the page', () => {
      cy.contains('button', 'Back to Sign In').click();
      cy.url().should('not.include', '/company-verification');
    });
  });

  describe('Step 1 — already registered company flow', () => {
    it('shows recovery card when company is already registered (mocked)', () => {
      cy.intercept('POST', '**/api/auth/company-onboarding/verify-registry', {
        statusCode: 200,
        body: {
          organizationExists: true,
          organizationDisplayName: 'FPT Software',
          organizationSlug: 'fpt-software',
          officialCompanyName: 'FPT Software JSC',
          taxCode: '0101689711',
          signedToken: '',
        },
      }).as('verifyRegistry');

      cy.get('input[name="companyName"]').type('FPT Software');
      cy.get('input[name="taxCode"]').type('0101689711');
      cy.contains('button', 'Verify Registry').click();
      cy.wait('@verifyRegistry');

      cy.contains('This company has already been registered').should('be.visible');
      cy.contains('Return to Login').should('be.visible');
    });
  });

  describe('Step 2 — Owner Identity Link', () => {
    beforeEach(() => {
      cy.intercept('POST', '**/api/auth/company-onboarding/verify-registry', {
        statusCode: 200,
        body: {
          organizationExists: false,
          officialCompanyName: 'FPT Software JSC',
          taxCode: '0101689711',
          signedToken: 'mock-step1-token',
        },
      }).as('verifyRegistry');

      cy.get('input[name="companyName"]').type('FPT Software');
      cy.get('input[name="taxCode"]').type('0101689711');
      cy.contains('button', 'Verify Registry').click();
      cy.wait('@verifyRegistry');
      cy.contains('button', 'Confirm & Continue').click();
    });

    it('advances to step 2 after registry confirmation', () => {
      cy.contains('Link Owner Profile').should('be.visible');
    });

    it('shows Email Verification tab', () => {
      cy.contains('Email Verification').should('be.visible');
    });

    it('shows Continue with Google tab', () => {
      cy.contains('Continue with Google').should('be.visible');
    });

    it('shows email input on the Email Verification tab', () => {
      cy.get('input[name="email"]').should('exist');
    });

    it('"Send Verification Code" button is disabled when email is empty', () => {
      cy.contains('button', 'Send Verification Code').should('be.disabled');
    });

    it('shows email error for invalid email format', () => {
      cy.get('input[name="email"]').type('not-an-email').blur();
      cy.contains('Please enter a valid business email').should('be.visible');
    });

    it('"Back to step 1" button returns to step 1', () => {
      cy.contains('button', 'Back to step 1').click();
      cy.contains('Register Your Company').should('be.visible');
    });
  });
});
