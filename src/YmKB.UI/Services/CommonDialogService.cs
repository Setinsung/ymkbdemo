using Microsoft.AspNetCore.Components;
using MudBlazor;
using YmKB.UI.Components.Dialogs;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Services;

public class CommonDialogService : ICommonDialogService
{
    private readonly IDialogService _dialogService;

    public CommonDialogService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task ShowConfirmationDialog(
        string title,
        string contentText,
        Func<Task> onConfirm,
        Func<Task>? onCancel = null
    )
    {
        var parameters = new DialogParameters
        {
            { nameof(ConfirmationDialog.ContentText), contentText }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true
        };
        var dialog = await _dialogService.ShowAsync<ConfirmationDialog>(title, parameters, options);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
        {
            await onConfirm();
        }
        else if (onCancel != null)
        {
            await onCancel();
        }
    }

    public async Task ShowDialogAsync<T>(
        string? title = "",
        DialogParameters<T>? parameters = null,
        DialogOptions? options = null,
        Func<DialogResult, Task>? onConfirm = null,
        Func<Task>? onCancel = null
    )
        where T : IComponent
    {
        options ??= new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };
        IDialogReference dialog;
        if (parameters is null)
            dialog = await _dialogService.ShowAsync<T>(title, options);
        else
            dialog = await _dialogService.ShowAsync<T>(title, parameters, options);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled && onConfirm is not null)
        {
            await onConfirm(result);
        }
        else if (result is not null && result.Canceled && onCancel is not null)
        {
            await onCancel();
        }
    }
}
