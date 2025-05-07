using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace YmKB.UI.Services.Contracts;

/// <summary>
/// 对话服务的帮助接口
/// </summary>
public interface ICommonDialogService
{
    IDialogService DialogService { get; set; }
    
    /// <summary>
    /// 显示一个带有指定标题和内容文本的确认对话框。
    /// </summary>
    /// <param name="title">确认对话框的标题。</param>
    /// <param name="contentText">确认对话框的内容文本。</param>
    /// <param name="onConfirm">确认操作被确认时要执行的操作。</param>
    /// <param name="onCancel">确认操作被取消时要执行的可选操作。</param>
    Task ShowConfirmationDialog(
        string title,
        string contentText,
        Func<Task> onConfirm,
        Func<Task>? onCancel = null
    );

    /// <summary>
    /// 显示一个带有指定标题和组件的对话框。
    /// </summary>
    /// <param name="title">对话框的标题。</param>
    /// <param name="parameters">对话框组件的参数。</param>
    /// <param name="options">对话框的选项。</param>
    /// <param name="onConfirm">确认操作被确认时要执行的操作。</param>
    /// <param name="onCancel">确认操作被取消时要执行的可选操作。</param>
    /// <typeparam name="T">对话框组件的类型。</typeparam>
    /// <returns>一个表示对话框的 <see cref="Task"/>。</returns>
    Task ShowDialogAsync<T>(
        string? title = "",
        DialogParameters<T>? parameters = null,
        DialogOptions? options = null,
        Func<DialogResult, Task>? onConfirm = null,
        Func<Task>? onCancel = null
    )
        where T : IComponent;
}