# 🤖 AI Workflow Logger

[![Next.js](https://img.shields.io/badge/Next.js-16.2.6-black?style=for-the-badge&logo=next.js)](https://nextjs.org/)
[![HeroUI](https://img.shields.io/badge/HeroUI-v3-0070F3?style=for-the-badge&logo=react)](https://heroui.com/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-v4-38B2AC?style=for-the-badge&logo=tailwind-css)](https://tailwindcss.com/)
[![Zustand](https://img.shields.io/badge/Zustand-State_Mgmt-orange?style=for-the-badge)](https://github.com/pmndrs/zustand)

A premium, local-first workflow management tool designed for auditing AI interactions and documenting project evolution. Built with a focus on privacy, comprehensive logging, and deep reflection.

---

## 🌟 Overview

**AI Workflow Logger** is a structured environment for developers and students to track their collaborative journey with Artificial Intelligence. It moves beyond simple prompt history, providing a multi-dimensional view of how AI influences project architecture, code quality, and personal growth.

### Why AI Workflow Logger?

- **Audit-Ready**: Generate comprehensive reports on how AI was used in your project.
- **Structured Reflection**: Analyze the effectiveness of your prompts and the accuracy of AI suggestions.
- **Evidence-Based**: Link commits, screenshots, and documentation directly to your AI logs.
- **Privacy First**: All data is stored locally in your project workspace as `.data.json`.

---

## 🚀 Key Features

### 📅 Workflow Changelog

Track project phases with precision. Log completed features, major improvements, and AI support levels for every development cycle.

### 📝 Prompt Analytics & Logging

- **Detailed Context**: Record not just the prompt, but the purpose, category, and usage level.
- **Evaluation System**: Flag "Most Important" prompts or "Ineffective" responses with detailed post-mortems.
- **Prompt Lessons**: Extract actionable insights from your AI interactions to improve future communication.

### 🛡️ AI Audit System

- **Usage Matrix**: Visualize which AI tools are supporting which parts of your project.
- **Issue Tracking**: Record AI-generated bugs or logic errors and how they were resolved.
- **Contribution Audit**: Distinguish between human effort and AI-generated content in team projects.

### 🧠 Deep Reflection

- **Dependency Analysis**: Honestly evaluate your level of reliance on AI tools.
- **Before/After Metrics**: Document specific areas where AI significantly improved (or hindered) performance.
- **Self-Evaluation**: Score yourself against core criteria to track competency development.

---

## 🛠️ Tech Stack

- **Framework**: [Next.js 16 (App Router)](https://nextjs.org/)
- **UI Library**: [HeroUI v3](https://heroui.com/) - Utilizing Compound Components and React Aria.
- **Styling**: [Tailwind CSS v4](https://tailwindcss.com/) with OKLCH color variables.
- **State Management**: [Zustand](https://github.com/pmndrs/zustand) for robust local state.
- **Forms**: [React Hook Form](https://react-hook-form.com/) + [Zod](https://zod.dev/) for strict validation.
- **Icons**: [Lucide React](https://lucide.dev/)

---

## 📁 Data Architecture

This project uses a **local-first, file-based architecture**.

Projects are stored as standalone `.data.json` files within the workspace. This ensures:

1.  **Version Control Compatibility**: Your AI logs live alongside your code in Git.
2.  **No Backend Required**: Zero-config setup and 100% data ownership.
3.  **Portability**: Easily share project files with team members or instructors.

---

## 🏁 Getting Started

### Prerequisites

- Node.js 18.x or later
- npm / yarn / pnpm

### Installation

1.  Clone the repository:

    ```bash
    git clone https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05
    cd "tools/AI Log"
    ```

2.  Install dependencies:

    ```bash
    npm install
    ```

3.  Run the development server:

    ```bash
    npm run dev
    ```

4.  Open [http://localhost:3000](http://localhost:3000) in your browser.

---

## 📝 License

Part of the SWP391 AI Audit Project - FPT University.
Group-05 | SE20A02
