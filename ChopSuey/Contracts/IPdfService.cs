namespace ChopSuey.Contracts
{
    public interface IPdfService
    {
        Task<byte[]> CreatePdfAsync(string userId);
        Task<byte[]> GenerateUserListPdfAsync();
     
    }
}
