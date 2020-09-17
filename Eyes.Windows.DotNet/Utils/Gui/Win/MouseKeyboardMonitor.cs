namespace Applitools.Utils.Gui.Win
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Windows.Forms;
    using Applitools.Utils.Async;

    /// <summary>
    /// Monitors global mouse and keyboard activity.
    /// </summary>
    public class MouseKeyboardMonitor : IGuiMonitor
    {
        #region Fields

        private readonly SerialDequeue<HookCallback_> callbacks_;
        private readonly int doubleClickTicks_;

        private UnsafeNativeMethods.HookProc mouseHookProcedure_;
        private UnsafeNativeMethods.HookProc keyboardHookProcedure_;

        private GuiEventTypes eventMask_;
        private DateTime lastClick_;
        private MouseButtons lastButton_;
        private int lastDownClickCount_;
        private StringBuilder charBuffer_;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability", 
            "CA2006:UseSafeHandleToEncapsulateNativeResources",
            Justification = "Nothing special to do with a function pointer")]
        private IntPtr mouseHook_ = IntPtr.Zero;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2006:UseSafeHandleToEncapsulateNativeResources",
            Justification = "Nothing special to do with a function pointer")]
        private IntPtr keyboardHook_ = IntPtr.Zero;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="MouseKeyboardMonitor"/> instance.
        /// </summary>
        public MouseKeyboardMonitor()
        {
            EventMask = GuiEventTypes.All;
            doubleClickTicks_ = SafeNativeMethods.GetDoubleClickTime();
            charBuffer_ = new StringBuilder(5);
            callbacks_ = new SerialDequeue<HookCallback_>(FireGuiEvent_);
        }

        #endregion

        #region Finalizers

        /// <summary>
        /// Uninstalls all hooks.
        /// </summary>
        ~MouseKeyboardMonitor()
        {
            Dispose(false);
        }

        #endregion

        #region Events

        /// <inheritdoc />
        public event EventHandler<GuiEventArgs> GuiEvent;

        #endregion

        #region Methods

        #region Public 

        /// <inheritdoc />
        public GuiEventTypes EventMask
        {
            get { return eventMask_; }
            set { eventMask_ = value; }
        }

        /// <inheritdoc />
        [SecurityCritical]
        public void StartMonitoring()
        {
            this.StartMonitoring(true, true);
        }

        /// <summary>
        /// Installs a mouse and / or keyboard hooks and starts monitoring.
        /// </summary>
        /// <param name="installMouseHook">
        /// Whether to install the global mouse hook (required for mouse events monitoring).
        /// </param>
        /// <param name="installKeyboardHook">
        /// Whether to install the keyboard hook (required for keyboard events monitoring).
        /// </param>
        /// <exception cref="Win32Exception">Windows Api error occurred</exception>
        [SecurityCritical]
        public void StartMonitoring(bool installMouseHook, bool installKeyboardHook)
        {
            if (mouseHook_ == IntPtr.Zero && installMouseHook)
            {
                mouseHookProcedure_ = new UnsafeNativeMethods.HookProc(MouseHookProc_);
                mouseHook_ = UnsafeNativeMethods.SetWindowsHookEx(
                    SafeNativeMethods.WH_MOUSE_LL,
                    mouseHookProcedure_,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);

                if (mouseHook_ == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    StopMonitoring(true, false, false);
                    throw new Win32Exception(errorCode);
                }
            }

            if (keyboardHook_ == IntPtr.Zero && installKeyboardHook)
            {
                keyboardHookProcedure_ = new UnsafeNativeMethods.HookProc(KeyboardHookProc_);
                keyboardHook_ = UnsafeNativeMethods.SetWindowsHookEx(
                    SafeNativeMethods.WH_KEYBOARD_LL,
                    keyboardHookProcedure_,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);

                if (keyboardHook_ == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    StopMonitoring(false, true, false);
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <inheritdoc />
        public void StopMonitoring()
        {
            this.StopMonitoring(true, true, false);
        }

        /// <summary>
        /// Stop monitoring keyboard and / or mouse events.
        /// </summary>
        /// <param name="uninstallMouseHook">Whether to uninstall the mouse hook</param>
        /// <param name="uninstallKeyboardHook">Whether to uninstall the keyboard hook</param>
        /// <param name="throwEx">Whether to throw exceptions on errors</param>
        /// <exception cref="Win32Exception">Windows API error</exception>
        public void StopMonitoring(
            bool uninstallMouseHook, 
            bool uninstallKeyboardHook, 
            bool throwEx)
        {
            if (mouseHook_ != IntPtr.Zero && uninstallMouseHook)
            {
                bool retMouse = UnsafeNativeMethods.UnhookWindowsHookEx(mouseHook_);
                mouseHook_ = IntPtr.Zero;
                if (!retMouse && throwEx)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            if (keyboardHook_ != IntPtr.Zero && uninstallKeyboardHook)
            {
                bool retKeyboard = UnsafeNativeMethods.UnhookWindowsHookEx(keyboardHook_);
                keyboardHook_ = IntPtr.Zero;
                if (!retKeyboard && throwEx)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected

        /// <summary>
        /// Disposes of managed and unmanaged resources.
        /// </summary>
        protected virtual void Dispose(bool disposeManaged)
        {
            StopMonitoring(true, true, false);
            callbacks_.Dispose();
        }

        #endregion

        #region Private

        #region GuiEvents dispatching

        private static Keys GetModifiers_()
        {
            Keys modifiers = Keys.None;
            if (0 != (0x8000 & SafeNativeMethods.GetKeyState(SafeNativeMethods.VK_SHIFT)))
            {
                modifiers |= Keys.Shift;
            }

            if (0 != (0x8000 & SafeNativeMethods.GetKeyState(SafeNativeMethods.VK_CONTROL)))
            {
                modifiers |= Keys.Control;
            }

            if (0 != (0x8000 & SafeNativeMethods.GetKeyState(SafeNativeMethods.VK_MENU)))
            {
                modifiers |= Keys.Alt;
            }

            return modifiers;
        }

        /// <summary>
        /// Fires a the gui event that corresponds to the input keyboard or mouse hook callback.
        /// </summary>
        private void FireGuiEvent_(HookCallback_ callback)
        {
            if (callback.IsKeyboardCallback)
            {
                FireKeyboardGuiEvent_(callback);
            }
            else
            {
                FireMouseGuiEvent_(callback);
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Interop method signature")]
        private void FireKeyboardGuiEvent_(HookCallback_ callback)
        {
            uint wParam = callback.wParam.ToUInt32();

            Keys key = (Keys)callback.vkCode;
            Keys modifiers = GetModifiers_();
            bool isKeyDown = wParam == SafeNativeMethods.WM_KEYDOWN ||
                wParam == SafeNativeMethods.WM_SYSKEYDOWN;
            bool isKeyUp = wParam == SafeNativeMethods.WM_KEYUP ||
                wParam == SafeNativeMethods.WM_SYSKEYUP;

            var timestamp = DateTimeOffset.Now;

            if (isKeyDown)
            {
                if (IsEventEnabled_(GuiEventTypes.KeyDown))
                {
                    var args = new KeyboardEventArgs(
                        timestamp, GuiEventTypes.KeyDown, key, modifiers);
                    GuiEvent(this, args);
                }

                if (IsEventEnabled_(GuiEventTypes.KeyPress))
                {
                    IntPtr fwh = SafeNativeMethods.GetForegroundWindow();
                    uint tid = SafeNativeMethods.GetWindowThreadProcessId(fwh, out uint pid);

                    byte[] keyState = new byte[256];
                    SafeNativeMethods.GetKeyboardState(keyState);

                    KeyboardEventArgs args;
                    if (SafeNativeMethods.ToUnicodeEx(
                        (uint)key,
                        callback.scanCode,
                        keyState,
                        charBuffer_,
                        charBuffer_.Capacity,
                        callback.flags,
                        SafeNativeMethods.GetKeyboardLayout(tid)) > 0)
                    {
                        args = new KeyboardEventArgs(
                            timestamp, 
                            GuiEventTypes.KeyPress, 
                            key,
                            modifiers, 
                            charBuffer_[0]);
                    }
                    else
                    {
                        args = new KeyboardEventArgs(
                            timestamp, GuiEventTypes.KeyPress, key, modifiers);
                    }

                    GuiEvent(this, args);
                }
            }
            else if (isKeyUp)
            {
                if (IsEventEnabled_(GuiEventTypes.KeyUp))
                {
                    var args = new KeyboardEventArgs(
                        timestamp, GuiEventTypes.KeyUp, key, modifiers);
                    GuiEvent(this, args);
                }
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules","SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Interop method signature")]
        private void FireMouseGuiEvent_(HookCallback_ callback)
        {
            uint wParam = callback.wParam.ToUInt32();

            MouseButtons button = MouseButtons.None;
            bool buttonDown = false;
            bool buttonUp = false;
            short mouseDelta = 0;
            switch (wParam)
            {
                case SafeNativeMethods.WM_LBUTTONDOWN:
                    buttonDown = true;
                    button = MouseButtons.Left;
                    break;
                case SafeNativeMethods.WM_LBUTTONUP:
                    buttonUp = true;
                    button = MouseButtons.Left;
                    break;
                case SafeNativeMethods.WM_RBUTTONDOWN:
                    buttonDown = true;
                    button = MouseButtons.Right;
                    break;
                case SafeNativeMethods.WM_RBUTTONUP:
                    buttonUp = true;
                    button = MouseButtons.Right;
                    break;
                case SafeNativeMethods.WM_MOUSEWHEEL:
                    // If the message is WM_MOUSEWHEEL, the high-order word of mouseData
                    // member is the wheel delta.
                    // One wheel click is defined as WHEEL_DELTA, which is 120.
                    mouseDelta = (short)((callback.mouseData >> 16) & 0xffff);
                    break;
            }

            // count clicks
            int clickCount = 0;
            if (buttonDown)
            {
                DateTime now = DateTime.Now;
                if (button == lastButton_ &&
                    (now.Subtract(lastClick_).TotalMilliseconds <= doubleClickTicks_))
                {
                    clickCount = 2;
                }
                else
                {
                    clickCount = 1;
                    lastClick_ = now;
                }

                lastDownClickCount_ = clickCount;
            }

            if (buttonUp)
            {
                clickCount = lastDownClickCount_;
            }

            lastButton_ = button;

            var eventType = GuiEventTypes.None;

            if (buttonUp)
            {
                if (IsEventEnabled_(GuiEventTypes.MouseButtonUp))
                {
                    eventType = GuiEventTypes.MouseButtonUp;
                }
            }
            else if (buttonDown)
            {
                if (IsEventEnabled_(GuiEventTypes.MouseButtonDown))
                {
                    eventType = GuiEventTypes.MouseButtonDown;
                }
            }
            else if (wParam == SafeNativeMethods.WM_MOUSEWHEEL)
            {
                if (IsEventEnabled_(GuiEventTypes.MouseWheelScrolled))
                {
                    eventType = GuiEventTypes.MouseWheelScrolled;
                }
            }
            else if (IsEventEnabled_(GuiEventTypes.MouseMoved))
            {
                eventType = GuiEventTypes.MouseMoved;
            }

            if (eventType != GuiEventTypes.None)
            {
                var args = new Applitools.Utils.Gui.MouseEventArgs(
                    DateTimeOffset.Now,
                    eventType,
                    button,
                    clickCount,
                    callback.ptX,
                    callback.ptY,
                    mouseDelta);
                GuiEvent(this, args);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if and only if the input event is masked by the event mask.
        /// </summary>
        private bool IsEventEnabled_(GuiEventTypes e)
        {
            return e == (e & eventMask_);
        }

        #endregion

        #region Low level callbacks

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.StyleCop.CSharp.NamingRules",
            "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Interop method signature")]
        private IntPtr KeyboardHookProc_(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && GuiEvent != null)
            {
                UnsafeNativeMethods.KeyboardHookStruct keyboardHookStruct =
                    (UnsafeNativeMethods.KeyboardHookStruct)Marshal.PtrToStructure(
                        lParam, typeof(UnsafeNativeMethods.KeyboardHookStruct));

                var callback = new HookCallback_(true);
                callback.wParam = wParam;
                callback.vkCode = keyboardHookStruct.vkCode;
                callback.scanCode = keyboardHookStruct.scanCode;
                callback.flags = keyboardHookStruct.flags;

                callbacks_.Enqueue(callback);
            }

            return UnsafeNativeMethods.CallNextHookEx(keyboardHook_, nCode, wParam, lParam);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.StyleCop.CSharp.NamingRules",
            "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Interop method signature")]
        private IntPtr MouseHookProc_(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && GuiEvent != null)
            {
                UnsafeNativeMethods.MouseLLHookStruct mouseHookStruct =
                    (UnsafeNativeMethods.MouseLLHookStruct)Marshal.PtrToStructure(
                        lParam, typeof(UnsafeNativeMethods.MouseLLHookStruct));

                var callback = new HookCallback_(false);
                callback.wParam = wParam;
                callback.mouseData = mouseHookStruct.mouseData;
                callback.ptX = mouseHookStruct.pt.X;
                callback.ptY = mouseHookStruct.pt.Y;

                callbacks_.Enqueue(callback);
            }

            return UnsafeNativeMethods.CallNextHookEx(mouseHook_, nCode, wParam, lParam);
        }

        #endregion

        #endregion

        #endregion

        #region Classes

        /// <summary>
        /// Keyboard and mouse hook callback parameters.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Match native name")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Match native name")]
        [SuppressMessage("Style", "IDE1006", Justification = "Match native name")]
        private class HookCallback_
        {
            public HookCallback_(bool isKeyboardCallback)
            {
                IsKeyboardCallback = isKeyboardCallback;
            }

            public bool IsKeyboardCallback
            {
                get;
                set;
            }

            public UIntPtr wParam
            {
                get;
                set;
            }

            public int mouseData
            {
                get;
                set;
            }

            public int ptX
            {
                get;
                set;
            }

            public int ptY
            {
                get;
                set;
            }

            public uint vkCode
            {
                get;
                set;
            }

            public uint scanCode
            {
                get;
                set;
            }

            public uint flags
            {
                get;
                set;
            }
        }

        #endregion
    }
}
