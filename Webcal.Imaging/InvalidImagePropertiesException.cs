using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TachographReader.Imaging
{
    public class InvalidImagePropertiesException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
        /// </summary>
        public InvalidImagePropertiesException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public InvalidImagePropertiesException(string message) :
            base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="paramName">Name of the invalid parameter.</param>
        /// 
        public InvalidImagePropertiesException(string message, string paramName) :
            base(message, paramName) { }
    }
}
