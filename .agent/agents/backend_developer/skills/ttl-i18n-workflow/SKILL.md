---
name: ttl-i18n-workflow
description: Managing multi-language support from Database translations to RESX files.
---

# TTL i18n Workflow

Standardized process for adding and translating content.

## 1. Dynamic Data (Database)
- Entities requiring translation must have a secondary collection: `{{EntityName}}_translate`.
- Always query with a filter for `LanguageCode`.
- Fallback to `vi-VN` if the requested translation is missing.

## 2. Static UI (Resources)
- Use `.resx` files in the `Infrastructure.Localization` or `Shared` project.
- Keys must follow `Module_Component_Action` naming (e.g., `Employee_Create_Title`).
- In Blazor, use `IStringLocalizer<T>` to fetch strings.

## 3. Synchronization
- When adding a new translatable field:
    1. Update the Domain Entity.
    2. Add the `_translate` DTO and Repository logic.
    3. Add the corresponding keys to `.resx` for UI labels.

## 4. Frontend Selector
- The active language must be stored in `LocalStorage`.
- Changing language should trigger a page reload or state refresh to update the `LanguageCode` in the API Header.
