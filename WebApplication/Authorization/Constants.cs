using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace WebApplication.Authorization
{
    public static class Operation
    {
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement { Name = Constants.Create };
        public static OperationAuthorizationRequirement Read =
          new OperationAuthorizationRequirement { Name = Constants.Read };
        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement { Name = Constants.Update };
        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = Constants.Delete };

        public static OperationAuthorizationRequirement Book =
          new OperationAuthorizationRequirement { Name = Constants.Book };
        public static OperationAuthorizationRequirement Confirm =
          new OperationAuthorizationRequirement { Name = Constants.Confirm };
        public static OperationAuthorizationRequirement Approve =
          new OperationAuthorizationRequirement { Name = Constants.Approve };
        public static OperationAuthorizationRequirement Reject =
          new OperationAuthorizationRequirement { Name = Constants.Reject };
    }

    public class Constants
    {
        // Operation constant names
        public static readonly string Create = "Create";
        public static readonly string Read = "Read";
        public static readonly string Update = "Update";
        public static readonly string Delete = "Delete";

        public static readonly string Book = "Book";
        public static readonly string Confirm = "Confirm";
        public static readonly string Approve = "Approve";
        public static readonly string Reject = "Reject";
    }

    // Role constant namings
    public class RoleName
    {
        public static readonly string Administrator = "Admin";
        public static readonly string Manager = "Manager";
        public static readonly string BoatOwner = "BoatOwner";
        public static readonly string MarinaOwner = "MarinaOwner";
    }
}
