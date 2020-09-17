using System;

namespace Applitools.VisualGrid
{
    public class EyesListener
    {
        public EyesListener(Action<VisualGridTask, IVisualGridEyes> onTaskComplete, Action onRenderComplete)
        {
            OnTaskComplete = onTaskComplete;
            OnRenderComplete = onRenderComplete;
        }

        public Action<VisualGridTask, IVisualGridEyes> OnTaskComplete { get; }
        public Action OnRenderComplete { get; }
    }
}