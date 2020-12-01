using System;

namespace WebApplication.BusinessLogic
{
    public class BusinessException : Exception
    {
        public string Key { get; private set; }

        public BusinessException(string key, string message) : base(message)
        {
            Key = key;
        }
    }
}
