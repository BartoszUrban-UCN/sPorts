using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

using WebApplication.Controllers.RestApi;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Tests.Utils;

namespace WebApplication.Tests.Controllers
{
    public class MarinaControllerTest : IClassFixture<SharedDatabaseFixture> 
    {
        public MarinaControllerTest(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        public SharedDatabaseFixture Fixture { get; }
    }
}
