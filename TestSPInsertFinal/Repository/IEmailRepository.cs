namespace TestSPInsertFinal.Repository
{
    public interface IEmailRepository
    {
        void sendPassword(string toAddress, string content);

        bool IsValidEmail(string email);
    }
}