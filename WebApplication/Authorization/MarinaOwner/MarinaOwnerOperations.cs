using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Sports.Authorization.MarinaOwner
{
    public static class MarinaOwnerOperations
    {

        public static OperationAuthorizationRequirement CreateMarina =   
          new OperationAuthorizationRequirement {Name=Constants.CreateMarinaOperationName};
        public static OperationAuthorizationRequirement CreateSpot =   
          new OperationAuthorizationRequirement {Name=Constants.CreateSpotOperationName};
        public static OperationAuthorizationRequirement Read = 
          new OperationAuthorizationRequirement {Name=Constants.ReadOperationName};  
        public static OperationAuthorizationRequirement Update = 
          new OperationAuthorizationRequirement {Name=Constants.UpdateOperationName}; 
        public static OperationAuthorizationRequirement Delete = 
          new OperationAuthorizationRequirement {Name=Constants.DeleteOperationName};
        public static OperationAuthorizationRequirement Confirm = 
          new OperationAuthorizationRequirement {Name=Constants.ConfirmOperationName};
        public static OperationAuthorizationRequirement Reject = 
          new OperationAuthorizationRequirement {Name=Constants.RejectOperationName};
    }

    public class Constants
    {
        public static readonly string CreateMarinaOperationName = "Create";
        public static readonly string CreateSpotOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ConfirmOperationName = "Confirm";
        public static readonly string RejectOperationName = "Reject";
    }
}