---
name: ttl-frontend-premium-page
description: Building high-fidelity Blazor pages with Skeleton loading, Search UI, and Premium Aesthetics.
---

# TTL Frontend Premium Page

All Blazor pages must follow the **Premium Standard** to ensure a "WOW" effect.

## 1. Page Layout
- **Toolbar**: Include Breadcrumbs and primary Action buttons (Create/Export).
- **Filters**: Card-based search panel with `Collapse` capability.
- **Content**: Data Grid or Card-List wrapped in `<SkeletonLoading>`.

## 2. Loading & Interaction
- **Skeleton Standard**: Use `_isLoading` flag in `OnInitializedAsync`.
- **Async Operations**: 
    - Use `_ = Swal.fire` (Fire & Forget) for loading indicators.
    - NEVER `await` `Swal.showLoading()` to avoid deadlocks.
- **Confirmation**: Use `DeleteConfirmationModal` for all destructive actions.

## 3. UI/UX Heuristics
- Use **Metronic/Keenthemes** CSS classes (`btn-light-primary`, `fs-6`, `fw-bold`).
- Ensure empty states are handled with "No data found" illustrations.
- All forms must have client-side validation using `EditForm` and `FluentValidationValidator`.

## 4. Global Exceptions
- Use `Try-Catch` blocks in Page components to catch API errors and display **Swal.fire** error messages instead of console logs.
