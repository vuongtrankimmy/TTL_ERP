---
name: Agent Operation Standards
description: Core principles for Antigravity's operation in the TTL ERP project, including automation, stability, and focus.
---

# Agent Operation Standards - TTL ERP

This document outlines the mandatory operational standards for the AI Coding Assistant (Antigravity) when working on the TTL ERP project.

## ⚡ 1. Automatic Command Execution (Turbo Mode)
- **Directives**: All technical commands required to validate code, sync environments, or check system status should be executed proactively.
- **Operation**:
    - Use `SafeToAutoRun: true` for all `run_command` calls that involve monitoring, status checks, or verified deployment steps.
    - Leverage the `// turbo-all` annotation in workflows to ensure seamless execution.
    - The goal is to provide results, not just proposals.

## 🛡️ 2. Zero Regression Policy (No Re-fixing)
- **Directives**: Once a bug is fixed (e.g., character encoding, port conflicts, MongoDB transactions), it must remain fixed.
- **Operation**:
    - Always double-check current configuration files (`.env`, `web.config`, `appsettings.json`) before making changes.
    - Refer to specific project patterns such as:
        - **UTF-8 Encoding**: Ensuring all strings and headers handle Vietnamese characters correctly.
        - **Mongo UnitOfWork**: Handling standalone servers without forcing transactions.
        - **Gateway Routing**: Using `localhost` for service-to-service communication in dev cycles.

## 🎯 3. Modular Focus & High-Fidelity Completion
- **Directives**: Focus exclusively on the requested module and handle it "End-to-End".
- **Operation**:
    - If the user asks for "Attendance", perform all necessary fixes across the Controller, Service, and Blazor Page layers.
    - **Stability Bugfix**: Fix all bugs completely, focusing on making the project build and run. If a fix requires cross-module changes for build integrity, do them.
    - Do not wander into unrelated modules unless explicitly asked or if they are direct dependencies.
    - Ensure all code items are production-ready and visually premium (UI/UX Pro Max) before declaring task completion.

## 🏗️ 4. Build & Execution Integrity
- **Port & Process Management**: Proactively prevent "Port Conflict" or "File Locked" (In Process) errors.
    - Whenever a "File access denied" or "Port in use" error occurs, search for and kill the offending processes (e.g., `w3wp.exe`, `iisexpress.exe`, `dotnet.exe`).
    - Use `SafeToAutoRun: true` to clear blocked ports/files immediately.
- **Clean State**: If build errors persist, perform `dotnet clean` or manually remove `bin`/`obj` folders before `dotnet build`.

## 🔍 5. Verification Checkpoint
- Before concluding a task, Antigravity MUST:
    1.  Validate connectivity through the Gateway.
    2.  Check for build/lint errors in modified files.
    3.  Confirm that the specific Vietnamese localization (RESX) is respected.
