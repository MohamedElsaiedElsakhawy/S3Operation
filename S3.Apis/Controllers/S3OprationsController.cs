using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using S3.BaseClass;
using S3.Contract;
using S3.Request;
using Amazon;
using System.Net.Mime;

namespace S3.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class S3OprationsController : ControllerBase
    {
        private IS3Storage s3Storage;

        public S3OprationsController(IS3Storage s3Storage)
        {
            this.s3Storage = s3Storage;
        }

        [HttpPost("buckets")]
        public Task<ApiResponse<bool>> Post(string bucketName)
        {
            var response  = s3Storage.CreateBucket(new S3.Request.CreateBucketRequest
            {
                bucketName = bucketName
            });

            return response;
        }

        [HttpGet("buckets")]
        public async Task<ApiResponse<IList<string>>> ListOfBuckets()
        {
            var response = await s3Storage.ListOfBucket();

            return response;
        }

        [HttpPost("folders")]
        public Task<ApiResponse<bool>> CreateFolder(string bucketName , string folderName)
        {
            var response = s3Storage.CreateFoldersAsync(new S3.Request.CreateFolderRequest
            {
                bucketName = bucketName , 
                folderName = folderName
            });

            return response;
        }

        [HttpPost("files")]
        public Task<ApiResponse<FileBase>> Upload(string bucketName , string filePath , List<IFormFile> files)
        {
            var response = s3Storage.UploadFiles(new S3.Request.UploadFileRequest
            {
               bucketName= bucketName , 
               filePath = filePath , 
               files = files
            });

            return response;
        }

        [HttpGet("file")]
        public async Task<IActionResult> Download(string bucketName, string filePath, string fileName)
        {
            var response = await s3Storage.DownloadFile(new S3.Request.DownloadFileRequest
            {
                bucketName= bucketName , 
                fileName =  fileName , 
                filePath = filePath
            });

            return File(response.Item1,MediaTypeNames.Application.Octet ,fileName);
            //return new Task<(Stream, string)>(x => (response.Result.Item1, response.Result.Item2),true);
        }

        [HttpGet("files")]
        public Task<ApiResponse<IList<string>>> List(string bucketName , string fullPath)
        {
            var response = s3Storage.ListAllFiles(new S3.Request.ListFilesRequest
            {
                bucketName= bucketName ,
                fullPath = fullPath
            });

            return response;
        }

        [HttpDelete("delete")]
        public Task<ApiResponse<bool>> Delete(string bucketName, string filePath)
        {
            var response = s3Storage.DeleteFile(new S3.Request.DeleteFileRequest
            {
                bucketName = bucketName,
                filePath = filePath
            });

            return response;
        }
        

    }
}
