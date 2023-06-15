using System;

namespace Applitools.Playwright.Universal
{
    public class PlaywrightStaleElementReferenceException : EyesException 
    {
        /// <summary>
        /// Creates an <see cref="PlaywrightStaleElementReferenceException"/> instance.
        /// </summary>
        public PlaywrightStaleElementReferenceException()
        {
        }
        
        /// <summary>
        /// Creates an <see cref="PlaywrightStaleElementReferenceException"/> instance.
        /// </summary>
        public PlaywrightStaleElementReferenceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an <see cref="PlaywrightStaleElementReferenceException"/> instance.
        /// </summary>
        public PlaywrightStaleElementReferenceException(string message, Exception ex)
            : base(message, ex)
        {
        }
        
        public void ThrowException(string message)
        {
            throw new PlaywrightStaleElementReferenceException(message);
        }
    }
}