namespace SharedKernel.Exceptions
{
    public class BusinessValidationException : Exception
    {
        public BusinessValidationException(string value, string exceptionLocation)
        : base($"{value} does not meet business validations. - Exception location: {exceptionLocation}")
        {
        }
    }
}
