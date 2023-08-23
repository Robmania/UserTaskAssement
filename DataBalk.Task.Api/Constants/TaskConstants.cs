namespace DataBalk.Task.Api.Constants;

public static class TaskConstants
{
    public static class ValidationMessages
    {
        public static string RequiredField(string fieldName) => $"{fieldName} is required.";
        public const string OperationDoesNotMatchId =
            "'{0}', you are executing the incorrect operation.";
        public const string DoesExist = "{0} already exists for : {1}.";
        public const string DoesNotExist = "{0} does not exist for Id: {1}.";
    }
}
