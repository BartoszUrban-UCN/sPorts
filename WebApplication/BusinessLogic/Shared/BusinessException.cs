using System;

namespace WebApplication.BusinessLogic
{
    public class BusinessException : Exception
    {
        /// <summary>
        /// Business Exceptions are meant to be added to the ModelState errors
        /// in Controllers in a similar fashion as model validation attributes perform
        /// </summary>
        /// <param name="key">
        /// Model object that the validation is about. ex. "Email" will put the
        /// error under the email field. string.Empty will put the error message
        /// in a separate error message
        /// </param>
        /// <param name="message">The user friendly error message</param>
        public BusinessException(string key, string message) : base(message)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}
