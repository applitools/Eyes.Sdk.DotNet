namespace Applitools.Utils.Gui.Win
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    internal static class UnsafeNativeMethods
    {
        #region Delegates

        public delegate IntPtr HookProc(int nCode, UIntPtr wParam, IntPtr lParam);

        #endregion

        #region Methods

        [SecurityCritical]
        [DllImport("user32.dll", SetLastError = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability",
            "CA1901:PInvokeDeclarationsShouldBePortable",
            Justification = "Rule has a bug")]
        public static extern IntPtr SetWindowsHookEx(
            int hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [SecurityCritical]
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [SecurityCritical]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallNextHookEx(
            IntPtr hhk, int nCode, UIntPtr wParam, IntPtr lParam);

        #endregion

        #region Classes

        [StructLayout(LayoutKind.Sequential)]
        public class MouseLLHookStruct
        {
            public SafeNativeMethods.POINT pt;
            public int mouseData;
            public int flags;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public int time;
            public int dwExtraInfo;
        }

        #endregion
    }
}
