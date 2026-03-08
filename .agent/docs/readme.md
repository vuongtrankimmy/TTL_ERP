# AI Software Factory - COMPLETE PACK 

This directory (`.agent`) contains the configurations, prompts, and patterns designed specifically to be executed with Claude / Antigravity / Local AI Agents.

## Structure Highlights

- `agents/`: Contains the purpose, skills, and prompts for specialized agents:
  - Product Manager
  - Architect
  - Backend Developer
  - Frontend Developer
  - QA Automation
  - DevOps Engineer
- `patterns/`: Contains reusable templates for Architecture, Caching, Databases, Events, etc.
- `prompts/`: General prompts that can be fed into agents for specific knowledge drops (e.g., System Design).
- `templates/`: Foundational code generated for the most common paradigms (e.g., .NET Microservices, Kafka events).
- `workflows/`: Orchestration flows describing how these agents interact autonomously.

## Typical Autonomous Loop
You can execute `workflows/autonomous_dev_loop.md` or instruct Antigravity via Slash Commands (`/autonomous_dev_loop`) to engage all layers automatically from an idea to scaffolding the infrastructure.

If executing manually: Start with Product Manager -> Architect -> Backend/Frontend -> QA -> DevOps.

## Updates
Loaded with the Enterprise / MEGA & ULTRA Pack extensions (10M user traffic simulation, Kafka, Distributed Caching).
