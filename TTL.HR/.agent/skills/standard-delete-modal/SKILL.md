---
name: Standard Delete Confirmation
description: Implements a double-confirmation delete modal that requires users to type the item name. This is the project standard for deleting critical resources.
---

# Standard Delete Confirmation Pattern

This skill guides you through implementing the standardized `DeleteConfirmationModal` component. This component enhances security by forcing users to type the exact name of the item they wish to delete, preventing accidental deletions.

## 1. Setup

First, ensure you have the necessary using directive in your Razor component or `_Imports.razor`:

```razor
@using TTL.HR.Shared.Components.Common
```

## 2. Component Implementation (Razor Markup)

Add the `<DeleteConfirmationModal>` component at the end of your Razor file (outside the main layout loops/tables). It is controlled by a boolean flag (`IsVisible`).

```razor
<!-- Delete Confirmation Modal -->
<DeleteConfirmationModal 
    IsVisible="@IsDeleteModalOpen"
    Title="Delete Resource"
    Message="You are requesting to delete"
    ItemName="@(ItemToDelete?.Name ?? "")"
    OnConfirmed="ConfirmDelete"
    OnCancelled="CloseDeleteModal" />
```

### Parameters:
- **IsVisible**: (`bool`) Controls the modal's visibility.
- **Title**: (`string`) The modal title (e.g., "Delete Employee").
- **Message**: (`string`) The warning message prefix (e.g., "You are requesting to delete").
- **ItemName**: (`string`) The name of the item to be typed for confirmation.
- **OnConfirmed**: (`EventCallback`) Method to call when deletion is confirmed.
- **OnCancelled**: (`EventCallback`) Method to call when deletion is cancelled.

## 3. Backend Implementation (C# Code Block)

Implement the logic to handle opening, closing, and confirming the deletion.

```csharp
@code {
    // STATE - Modal Control
    private bool IsDeleteModalOpen = false;
    private MyEntity ItemToDelete; // Replace 'MyEntity' with your ViewModel/Entity class

    // 1. TRIGGER - Open Modal
    private void PromptDelete(MyEntity item)
    {
        ItemToDelete = item;
        IsDeleteModalOpen = true;
    }

    // 2. ACTION - Close Modal
    private void CloseDeleteModal()
    {
        IsDeleteModalOpen = false;
        ItemToDelete = null;
    }

    // 3. ACTION - Confirm Delete (Execute Logic)
    private async Task ConfirmDelete()
    {
        if (ItemToDelete != null)
        {
            // TODO: Call your service/API to delete data
            // await MyService.DeleteAsync(ItemToDelete.Id);
            
            // Update UI list locally
            MyList.Remove(ItemToDelete);
            
            // Close modal
            CloseDeleteModal();
            
            // Optional: Show success toast
        }
    }
}
```

## 4. Usage in Table/List

Bind the delete button in your list to the `PromptDelete` method.

```razor
<button class="btn btn-icon btn-active-light-danger w-30px h-30px" 
        title="Delete" 
        @onclick="() => PromptDelete(item)">
    <i class="ki-outline ki-trash fs-3"></i>
</button>
```

## Checklist

- [ ] Added `@using TTL.HR.Shared.Components.Common`
- [ ] Added `<DeleteConfirmationModal>` to markup
- [ ] Implemented `PromptDelete`, `CloseDeleteModal`, and `ConfirmDelete` methods
- [ ] Validated that the modal requires typing the item name to enable the delete button
