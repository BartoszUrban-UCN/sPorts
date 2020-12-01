using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication.BusinessLogic
{
    public class MockPayProvider : Controller
    {
        public  String ProcessPayment()
        {
            String processed = null;

            Thread t = new Thread(new ThreadStart(wait));

            t.Start();

            processed = "OK";

            return processed;
        }

        public void wait()
        {
            Thread.Sleep(10000);
        }
    }
}
