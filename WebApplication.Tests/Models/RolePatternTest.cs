using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Models;

namespace WebApplication.Tests.Models
{
    [TestClass]
    class RolePatternTest
    {
        private Person person;
        //private Role marinaRole;
        //private Role boatRole;

        [TestMethod]
        public void TestPerson()
        {
            //Act
            Person person = new Person();

            //Assert
            Assert.IsNotNull(person);
        }

        [TestMethod]
        public void TestAdd()
        {
            //Arrange

            //Act
            //person.AddRole(marinaRole);
            //person.AddRole(boatRole);

            //Assert
            //Assert.IsTrue(person.Roles.Contains(marinaRole));
            //Assert.IsTrue(person.Roles.Contains(boatRole));
        }

        [TestMethod]
        public void TestDelete()
        {
            //Act
            //person.RemoveRole(boatRole);
            //person.RemoveRole(marinaRole);

            //Assert
            //Assert.IsFalse(person.Roles.Contains(marinaRole));
            //Assert.IsFalse(person.Roles.Contains(boatRole));
        }
    }
}
