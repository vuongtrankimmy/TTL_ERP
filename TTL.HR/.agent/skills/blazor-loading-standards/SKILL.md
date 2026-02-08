---
name: Blazor Loading Standards
description: Guidelines for implementing consistent loading states and skeleton UI in Blazor components across the application.
---

# Blazor Loading Standards

This skill outlines the mandatory process for handling data loading and visual feedback in Blazor components. All pages, particularly those displaying lists or detail views, must implement Skeleton Loading to provide a polished UX.

## 1. State Management

Every component that fetches data must manage a loading state.

### Implementation Checklist:
- [ ] Define a private boolean field `_isLoading` initialized to `true`.
- [ ] Implement `OnInitializedAsync` to handle data fetching.
- [ ] Ensure `_isLoading` is set to `false` **after** data is fully loaded (or simulated).

```csharp
public partial class ExamplePage
{
    private bool _isLoading = true; // Default to loading state

    protected override async Task OnInitializedAsync()
    {
        try 
        {
            // Simulate API latency (Replace with actual service call later)
            await Task.Delay(1300); 
            
            // fetch data...
            // _data = await _service.GetDataAsync();
        }
        finally 
        {
            _isLoading = false; // Ensure loading state is cleared
        }
    }
}
```

## 2. Skeleton Component Usage

Wrap the main content area (e.g., tables, cards, lists) with the `<SkeletonLoading>` component.

### Parameters:
- `IsLoading`: Should bind to your `_isLoading` variable.
- `Type`: 
    - `SkeletonLoading.SkeletonType.Table`: For data grids/tables.
    - `SkeletonLoading.SkeletonType.Card`: For detail views or card layouts.
    - `SkeletonLoading.SkeletonType.List`: For simple lists.
- `Rows`: (Optional) Number of rows to simulate (default is usually 5).
- `Columns`: (Optional, Table only) Number of columns to simulate. match your table headers.

### Examples:

#### Table Lists
Wrap the `<table>` or its container (`.table-responsive`) within the skeleton component.

```razor
<div class="card-body">
    <SkeletonLoading IsLoading="_isLoading" Type="SkeletonLoading.SkeletonType.Table" Rows="10" Columns="6">
        <div class="table-responsive">
            <table class="table ...">
                <!-- Table Content -->
            </table>
        </div>
        <!-- Pagination (optional inside or outside) -->
    </SkeletonLoading>
</div>
```

#### Detail Views / Cards
Wrap the detail content card.

```razor
<div class="card mb-5">
    <div class="card-body">
        <SkeletonLoading IsLoading="_isLoading" Type="SkeletonLoading.SkeletonType.Card">
            <!-- Header / Detail Content -->
             <div class="d-flex ...">
                 ...
             </div>
        </SkeletonLoading>
    </div>
</div>
```

## 3. Best Practices

- **Simulation**: During frontend development without backend APIs, use `await Task.Delay(1000-1500)` to simulate network latency. This verifies the skeleton UI works correctly.
- **Consistency**: Apply this pattern to ALL list pages (e.g., Department List, Employee List) and detail pages.
- **Placement**: Place the `<SkeletonLoading>` component logically around the content that is being fetched. Static headers or filters can remain outside if they don't depend on the fetched data.
