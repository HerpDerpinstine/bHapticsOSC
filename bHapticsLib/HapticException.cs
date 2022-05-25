using System;

namespace bHapticsLib
{
    public class HapticException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HapticException"/> class.
        /// </summary>
        public HapticException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HapticException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public HapticException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HapticException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public HapticException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
