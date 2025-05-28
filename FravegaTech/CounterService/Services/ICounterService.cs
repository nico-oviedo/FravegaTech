namespace CounterService.Services
{
    public interface ICounterService
    {
        /// <summary>
        /// Gets next sequence value
        /// </summary>
        /// <param name="sequenceName">Sequence name.</param>
        /// <returns>Integer with the next sequence value.</returns>
        Task<int> GetNextSequenceValueAsync(string sequenceName);
    }
}
