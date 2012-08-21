using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLoExceptions
{
    /// <summary>
    /// Class for representing errors with MyLo
    /// </summary>
    public class MyLoException : Exception
    {
        /// <summary>
        /// Creates a new MyLoException Exception with the given Message
        /// </summary>
        /// <param name="errorMsg">Error Message</param>
        public MyLoException(String errorMsg)
            : base(errorMsg) { }

        /// <summary>
        /// Creates a new MyLoException Exception with the given Message and Inner Exception
        /// </summary>
        /// <param name="errorMsg">Error Message</param>
        /// <param name="cause">Inner Exception</param>
        public MyLoException(String errorMsg, Exception cause)
            : base(errorMsg, cause) { }
    }

    /// <summary>
    /// Class for representing Account Name errors with MyLo
    /// </summary>
    public class MyLoAccountIdException : MyLoException
    {
        /// <summary>
        /// Creates a new Account Name Exception
        /// </summary>
        /// <param name="errorMsg">Error Message</param>
        public MyLoAccountIdException(String errorMsg)
            : base(errorMsg) { }

        /// <summary>
        /// Creates a new Account Name Exception
        /// </summary>
        /// <param name="errorMsg">Error Message</param>
        /// <param name="cause">Exception that caused this Exception</param>
        public MyLoAccountIdException(String errorMsg, Exception cause)
            : base(errorMsg, cause) { }
    }

    /// <summary>
    /// Class for representing Database errors with MyLo
    /// </summary>
    public class MyLoDataStoreException : MyLoException
    {
        /// <summary>
        /// Creates a new Database errors Exception
        /// </summary>
        /// <param name="errorMsg">Error Message</param>
        public MyLoDataStoreException(String errorMsg)
            : base(errorMsg) { }

        /// <summary>
        /// Creates a new Database errors Exception
        /// </summary>
        /// <param name="errorMsg">Error Message</param>
        /// <param name="cause">Exception that caused this Exception</param>
        public MyLoDataStoreException(String errorMsg, Exception cause)
            : base(errorMsg, cause) { }
    }

    /// <summary>
    /// Class for representing CRC calculation errors with MyLo
    /// </summary>
    public class MyLoCRCException : MyLoException
    {
        /// <summary>
        /// Creates a new CRC calculation Exception
        /// </summary>
        /// <param name="errorMsg">Error Message</param>
        public MyLoCRCException(String errorMsg)
            : base(errorMsg) { }

        /// <summary>
        /// Creates a new CRC calculation Exception
        /// </summary>
        /// <param name="errorMsg">Error Message</param>
        /// <param name="cause">Exception that caused this Exception</param>
        public MyLoCRCException(String errorMsg, Exception cause)
            : base(errorMsg, cause) { }
    }
}
