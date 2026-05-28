# CVerify Client — Testing Guide

## Strategy

| Type | Tool | Scope |
|------|------|--------|
| **White-box (unit)** | Jest | Pure functions, branches, boundaries |
| **White-box (component)** | Jest + **React Testing Library** | React components, hooks, DOM |
| **Black-box (logic)** | **Mocha** + **Chai** | Pure TS modules without JSDOM |
| **Black-box (E2E)** | **Cypress** | Full pages, user-visible behavior |
| **Backend white-box** | xUnit + FluentAssertions | `CVerify.Core/tests/CVerify.API.UnitTests` |
| **Backend black-box** | xUnit + Testcontainers | `CVerify.Core/tests/CVerify.API.IntegrationTests` |

### Not used (and why)

| Tool | Reason |
|------|--------|
| **Enzyme** | Deprecated; incompatible with React 19 |
| **Jasmine** | Redundant — Jest provides the same `describe` / `it` / `expect` API |

## Commands

```bash
cd client

# Install
npm install

# White-box: Jest + RTL
npm test
npm run test:coverage

# Black-box: Mocha + Chai (pure logic)
npm run test:mocha

# Black-box: Cypress E2E (requires dev server on :3000)
npm run dev          # terminal 1
npm run test:e2e     # terminal 2
npm run test:e2e:open  # interactive UI

# All fast tests (no E2E)
npm run test:all
```

## Backend

```bash
cd CVerify.Core
dotnet test
```

## Test locations

```
client/
├── src/**/__tests__/*.test.ts(x)     # Jest white-box
├── tests/mocha/*.mocha.ts            # Mocha + Chai
└── cypress/e2e/*.cy.ts               # Cypress black-box E2E

CVerify.Core/tests/
├── CVerify.API.UnitTests/            # White-box (.NET)
└── CVerify.API.IntegrationTests/     # Black-box API + DB
```

## Edge-case coverage highlights

- Password policy: empty, partial, enterprise length, unicode, unknown policy id
- Workspace slug: reserved names, typosquatting, Vietnamese diacritics, length bounds
- Error normalizer: validation 400, 429, timeout, unknown errors
- Auth logo: theme-aware asset paths
- OTP / password: backend policy services (xUnit)
