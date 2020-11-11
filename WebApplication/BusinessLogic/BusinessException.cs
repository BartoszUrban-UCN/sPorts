using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.BusinessLogic
{
    public class BusinessException : Exception
    {
        public string Key { get; }

        public BusinessException(string key, string message) : base(message)
        {
            Key = key;
        }
    }
}
