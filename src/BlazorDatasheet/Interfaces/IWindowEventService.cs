using BlazorDatasheet.Model.Events;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorDatasheet.Interfaces;

public interface IWindowEventService : IDisposable
{
    Task Init();
    event Func<KeyboardEventArgs, Task<bool?>> OnKeyDown;
    event Func<MouseEventArgs, bool>? OnMouseDown;
    event Func<PasteEventArgs, Task>? OnPaste;
}