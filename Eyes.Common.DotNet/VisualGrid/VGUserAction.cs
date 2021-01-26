using System.Drawing;

namespace Applitools.VisualGrid
{
    public abstract class VGUserAction : IUserAction
    {
        public VGUserAction(object control)
        {
            Control = control;
        }

        public object Control { get; }
    }

    public class VGTextTrigger : VGUserAction
    {
        public VGTextTrigger(object control, string text) : base(control)
        {
            Text = text;
        }
        
        public string Text { get; }
    }

    public class VGMouseTrigger : VGUserAction
    {
        public VGMouseTrigger(MouseAction action, object control, Point cursor) : base(control)
        {
            Action = action;
            Cursor = cursor;
        }

        public MouseAction Action { get; }
        public Point Cursor { get; }
    }
}