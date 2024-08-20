using Microsoft.AspNetCore.Mvc;

namespace TestSPInsertFinal.Repository
{
    public interface IFileRepository
    {
        string GetContentType(string filePath);
    }
}
