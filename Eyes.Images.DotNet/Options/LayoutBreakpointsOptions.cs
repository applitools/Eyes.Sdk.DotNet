using System.Collections.Generic;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools
{
    [JsonConverter(typeof(LayoutBreakpointsConverter))]
    public class LayoutBreakpointsOptions
    {
        public LayoutBreakpointsOptions()
        {
        }

        internal LayoutBreakpointsOptions(LayoutBreakpointsOptions options)
        {
            if (options == null)
            {
                return;
            }
            foreach (var bp in options.layoutBreakpoints_)
            {
                layoutBreakpoints_.Add(bp);
            }

            isLayoutBreakpoints_ = options.isLayoutBreakpoints_;
            reload_ = options.reload_;
        }

        private bool? isLayoutBreakpoints_;

        /// <summary>
        /// explicit list of layout breakpoints
        /// </summary>
        [JsonProperty("breakpoints")]
        private readonly List<int> layoutBreakpoints_ = new List<int>();

        /// <summary>
        /// reload after layout breakpoints
        /// </summary>
        [JsonProperty("reload")]
        private bool? reload_;

        public LayoutBreakpointsOptions Breakpoints()
        {
            isLayoutBreakpoints_ = true;
            layoutBreakpoints_.Clear();
            return this;
        }

        public LayoutBreakpointsOptions Breakpoints(bool? shouldSet)
        {
            isLayoutBreakpoints_ = shouldSet;
            layoutBreakpoints_.Clear();
            return this;
        }

        public LayoutBreakpointsOptions Breakpoints(params int[] breakpoints)
        {
            isLayoutBreakpoints_ = null;
            layoutBreakpoints_.Clear();

            if (breakpoints == null || breakpoints.Length == 0)
            {
                return this;
            }

            foreach (int breakpoint in breakpoints)
            {
                ArgumentGuard.GreaterThan(breakpoint, 0, nameof(breakpoint));
                layoutBreakpoints_.Add(breakpoint);
            }

            layoutBreakpoints_.Sort();
            return this;
        }

        public LayoutBreakpointsOptions Reload()
        {
            reload_ = true;
            return this;
        }

        public LayoutBreakpointsOptions Reload(bool shouldReload)
        {
            reload_ = shouldReload;
            return this;
        }

        public bool? GetReload()
        {
            return reload_;
        }

        public bool? IsLayoutBreakpoints()
        {
            return isLayoutBreakpoints_;
        }

        public List<int> GetLayoutBreakpoints()
        {
            return layoutBreakpoints_;
        }
    }
}