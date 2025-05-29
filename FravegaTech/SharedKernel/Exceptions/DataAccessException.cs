namespace SharedKernel.Exceptions
{
    public class DataAccessException : Exception
    {
        public DataAccessException(string exceptionLocation, Exception inner)
            : base($"Data access error in {exceptionLocation}.", inner) { }
    }
}
