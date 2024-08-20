using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TestSPInsertFinal.Repository;

namespace TestSPInsertFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {

        private readonly ILogger<FileController> logger;
        private readonly IFileRepository fileRepository;

        public FileController( ILogger<FileController> logger,IFileRepository fileRepository)
        {
            this.logger = logger;
            this.fileRepository = fileRepository;
        }

 
        [HttpPost("upload-file"), Authorize(Roles = "User")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                   return Ok(new
                    {
                        status = "200",
                        message = "file has been uploaded successfully",
                    });
                }

                return BadRequest(new
                {
                    status = "404",
                    message = "File not uploaded",
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Error at FileRepository.UploadFile: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("download-file"), Authorize(Roles = "User")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;

                    return File(memory, fileRepository.GetContentType(filePath), Path.GetFileName(filePath));
                }

                return NotFound(new
                {
                    status = "404",
                    message = "File not found",
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Error at FileRepository.DownloadFile: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

    }
}
