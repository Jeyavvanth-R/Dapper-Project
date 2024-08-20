
namespace TestSPInsertFinal.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<FileRepository> logger;
        public FileRepository(IConfiguration configuration, ILogger<FileRepository> logger) 
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public string GetContentType(string filePath)
        {
            try
            {
                var types = GetMimeTypes();
                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                return types[ext];
            }
            catch(Exception ex)
            {
                logger.LogError("FileRepository.GetContentType: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
            };
        }

    }
}
