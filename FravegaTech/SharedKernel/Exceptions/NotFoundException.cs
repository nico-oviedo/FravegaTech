namespace SharedKernel.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string value, string exceptionLocation)
            : base($"{value} was not found in {exceptionLocation}.") { }
    }
}
