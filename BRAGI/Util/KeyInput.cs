using Avalonia.Input;
using SharpHook;
using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRAGI.Util;

public static class KeyInput
{
    private static readonly HashSet<KeyCode> PressedKeys = new();
    private static TaskPoolGlobalHook? hook;

    public static void InitializeHook()
    {
        hook = new();
        hook.KeyPressed += Hook_KeyPressed;
        hook.KeyReleased += Hook_KeyReleased;
        hook.RunAsync();
    }
    private static void Hook_KeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        PressedKeys.Remove(e.Data.KeyCode);
    }
    private static void Hook_KeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        PressedKeys.Add(e.Data.KeyCode);
    }
    public static bool IsKeyPressed(KeyCode key)
    {
        return PressedKeys.Contains(key);
    }
}
