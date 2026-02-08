---
name: Enterprise Mock Data Standards
description: Guidelines for ensuring high-quality, diverse, and sufficient mock data in the application for testing and demonstration purposes.
---

# Enterprise Mock Data Standards

This skill defines the requirements for mock data generation within the application. High-quality mock data is essential for accurate UI/UX validation, performance testing, and client demonstrations.

## 1. Data Quantity Standard

To ensure all UI components (lists, tables, filters, pagination) can be verified effectively:

### Mandatory Requirement:
- **Minimum 10 Items**: Every list or table displaying mock data MUST contain at least 10 unique, diverse entries.
- **Diversity**: Data should cover all possible states, categories, and edge cases (e.g., different statuses, long text, empty fields where applicable).

## 2. Realistic Data Attributes

Mock data must simulate real-world scenarios as closely as possible:

### Implementation Guidelines:
- **Diverse Statuses**: Include items for every status in the business logic (e.g., "Tiếp nhận", "Phỏng vấn", "Đã tuyển", "Từ chối", "Đang tuyển", "Đã đóng").
- **Detailed History**: For entities with state transitions, mock a `StatusHistory` or `Timeline` with at least 2-3 past events.
- **Metadata**: Use realistic sounding names, emails, dates, and codes (e.g., "DEV-001", "anh.nv@gmail.com").
- **Avatars**: Use varied paths from the existing asset library (e.g., `assets/media/avatars/300-x.jpg`).

## 3. Visual & Interaction Validation

Sufficient data quantity allows for verifying:
- **Scroll Behavior**: Ensuring the page layout remains stable with multiple items.
- **Search & Filter Accuracy**: Providing enough data points to test search results across different criteria.
- **Conditional Formatting**: Verifying that badges, colors, and icons update correctly for each data state.

## 4. Example Pattern (C# Mock List)

```csharp
private List<ItemType> _items = new()
{
    new() { Id = 1, Title = "Standard Case", Status = "Active", ... },
    new() { Id = 2, Title = "Warning Case", Status = "Pending", ... },
    new() { Id = 3, Title = "Error/Closed Case", Status = "Closed", ... },
    // ... add at least 7 more diverse items ...
    new() { Id = 10, Title = "Edge Case", Status = "Draft", ... }
};
```

## 5. Maintenance
When adding new features, always update mock data to reflect new fields or business logic changes immediately.
