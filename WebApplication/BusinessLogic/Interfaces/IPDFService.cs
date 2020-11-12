using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    internal interface IPDFService<T>
    {
        void CreatePDFFile(T fileToMakePDFFrom);
    }
}
