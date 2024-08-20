namespace TestSPInsertFinal.Repository
{
    public interface IPasswordRepository
    {
        string DecryptString(string cipherText);
        string EncryptString(string text);
    }
}