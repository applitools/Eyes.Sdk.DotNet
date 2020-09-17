namespace Applitools.Utils.Gui
{
    using System;

    /// <summary>
    /// GUI event types.
    /// </summary>
    [Flags]
    public enum GuiEventTypes : int
    {
        /// <summary>
        /// An unknown GUI event.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Window became active.
        /// </summary>
        WindowActivated = 0x00000001,

        /// <summary>
        /// Window became inactive.
        /// </summary>
        WindowDeactivated = 0x00000002,

        /// <summary>
        /// Window got focus.
        /// </summary>
        WindowGotFocus = 0x00000004,

        /// <summary>
        /// Window lost focus.
        /// </summary>
        WindowLostFocus = 0x00000008,

        /// <summary>
        /// Window is moved.
        /// </summary>
        WindowMoved = 0x00000010,

        /// <summary>
        /// Window is resized.
        /// </summary>
        WindowResized = 0x00000020,

        /// <summary>
        /// A bitwise mask for all window events.
        /// </summary>
        Window = WindowActivated | WindowDeactivated | WindowGotFocus | WindowLostFocus |
            WindowMoved | WindowResized,

        /// <summary>
        /// Keyboard key is down.
        /// </summary>
        KeyDown = 0x00001000,

        /// <summary>
        /// Keyboard key is up.
        /// </summary>
        KeyUp = 0x00002000,

        /// <summary>
        /// Keyboard key was pressed.
        /// </summary>
        KeyPress = 0x00004000,

        /// <summary>
        /// A bitwise mask for all keyboard events.
        /// </summary>
        Keyboard = KeyDown | KeyUp | KeyPress,

        /// <summary>
        /// Mouse button is down.
        /// </summary>
        MouseButtonDown = 0x00010000,

        /// <summary>
        /// Mouse button is up.
        /// </summary>
        MouseButtonUp = 0x00020000,

        /// <summary>
        /// Mouse button clicked.
        /// </summary>
        MouseButtonClick = 0x00040000,

        /// <summary>
        /// Mouse button event.
        /// </summary>
        MouseButton = MouseButtonDown | MouseButtonUp | MouseButtonClick,

        /// <summary>
        /// Mouse wheel is scrolled.
        /// </summary>
        MouseWheelScrolled = 0x00080000,

        /// <summary>
        /// Mouse is moved.
        /// </summary>
        MouseMoved = 0x00100000,

        /// <summary>
        /// A bitwise mask for all mouse events.
        /// </summary>
        Mouse = MouseButton | MouseWheelScrolled | MouseMoved,

        /// <summary>
        /// A bitwise mask for all human interface device events (i.e., keyboard, mouse, etc.)
        /// </summary>
        Hid = Keyboard | Mouse,

        /// <summary>
        /// A bitwise mask for all events.
        /// </summary>
        All = Window | Hid,
    }
}
