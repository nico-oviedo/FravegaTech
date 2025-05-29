namespace SharedKernel.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string valueName, string exceptionLocation)
            : base($"{valueName} not found in {exceptionLocation}.") { }
    }
}
