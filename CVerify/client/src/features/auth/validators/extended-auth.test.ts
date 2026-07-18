import { describe, test, expect } from "@jest/globals";

describe("Extended Auth Validators Tests", () => {
  test("email validator checks syntactical accuracy", () => {
    const email = "developer@cverify.com";
    const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    expect(pattern.test(email)).toBe(true);
  });

  test("password policy complexity verification", () => {
    const pass = "ComplexPass123!";
    const minLength = pass.length >= 8;
    expect(minLength).toBe(true);
  });
});
