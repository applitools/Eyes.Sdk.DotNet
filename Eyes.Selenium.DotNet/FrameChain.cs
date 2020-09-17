namespace Applitools.Selenium
{
    using Applitools.Utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class FrameChain : IEnumerable<Frame>
    {
        private List<Frame> frames_ = new List<Frame>();
        private Logger logger_;

        public int Count { get { return frames_.Count; } }

        public FrameChain(Logger logger)
        {
            logger_ = logger;
        }

        public Frame this[int index] { get { return frames_[index]; } }

        #region Enumerable
        public IEnumerator<Frame> GetEnumerator()
        {
            return frames_.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return frames_.GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Compares two frame chains.
        /// </summary>
        /// <param name="c1">Frame chain to be compared against c2.</param>
        /// <param name="c2">Frame chain to be compared against c1.</param>
        /// <returns>True if both frame chains represent the same frame, or false otherwise.</returns>
        public static bool IsSameFrameChain(FrameChain c1, FrameChain c2)
        {
            if (c1 == null) return c2 == null || c2.frames_.Count == 0;
            if (c2 == null) return c1.frames_.Count == 0;

            int lc1 = c1.frames_.Count;
            int lc2 = c2.frames_.Count;

            // different chains size means different frames
            if (lc1 != lc2)
            {
                return false;
            }

            //noinspection ForLoopReplaceableByForEach
            for (int i = 0; i < lc1; ++i)
            {
                if (!c1.frames_[i].Reference.Equals(c2.frames_[i].Reference))
                {
                    return false;
                }
            }

            return true;
        }

        public static int CountCommonFrames(FrameChain c1, FrameChain c2)
        {
            ArgumentGuard.NotNull(c1, nameof(c1));
            ArgumentGuard.NotNull(c2, nameof(c2));
            int commonFrames = 0;
            int lc1 = c1.frames_.Count;
            int lc2 = c2.frames_.Count;
            int lc = Math.Min(lc1, lc2);
            for (int i = 0; i < lc; ++i)
            {
                if (c1.frames_[i].Reference.Equals(c2.frames_[i].Reference))
                {
                    commonFrames++;
                    continue;
                }
                break;
            }
            return commonFrames;
        }

        /// <summary>
        /// Removes all current frames in the frame chain.
        /// </summary>
        public void Clear()
        {
            frames_.Clear();
        }

        /// <summary>
        /// Removes the last inserted frame element. Practically means we switched
        /// back to the parent of the current frame
        /// </summary>
        public Frame Pop()
        {
            if (frames_.Count == 0) return null;
            Frame frame = Peek();
            frames_.RemoveAt(frames_.Count - 1);
            return frame;
        }

        public Frame Peek()
        {
            if (frames_.Count == 0) return null;
            return frames_[frames_.Count - 1];
        }

        /// <summary>
        /// Appends a frame to the frame chain.
        /// </summary>
        /// <param name="frame">The frame to be added.</param>
        public void Push(Frame frame)
        {
            frames_.Add(frame);
        }

        /// <summary>
        /// Returns the size of the current frame.
        /// </summary>
        /// <returns>The size of the current frame.</returns>
        public Size GetCurrentFrameSize()
        {
            logger_.Verbose("enter");
            Size result = frames_[frames_.Count - 1].OuterSize;
            logger_.Verbose("Done!");
            return result;
        }

        /// <summary>
        /// Returns the inner size of the current frame.
        /// </summary>
        /// <returns>The inner size of the current frame.</returns>
        public Size GetCurrentFrameInnerSize()
        {
            logger_.Verbose("enter");
            Size result = frames_[frames_.Count - 1].InnerSize;
            logger_.Verbose(result.ToString());
            return result;
        }

        public Point GetCurrentFrameOffset()
        {
            logger_.Verbose("enter");
            Point result = new Point();
            foreach (Frame frame in frames_)
            {
                result.Offset(frame.Location);
            }
            logger_.Verbose("{0} (number of frames: {1})", result, frames_.Count);
            return result;
        }

        /// <returns>The outermost frame's location, or NoFramesException.</returns>
        public Point GetDefaultContentScrollPosition()
        {
            if (frames_.Count == 0)
            {
                throw new Exception("No frames in frame chain");
            }
            return frames_[0].OriginalLocation;
        }


        public FrameChain Clone()
        {
            return new FrameChain(logger_) { frames_ = new List<Frame>(this.frames_) };
        }
    }
}
