using S3.BaseClass;
using S3.Request;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S3.Contract
{
    public interface IS3Storage
    {
        Task<ApiResponse<bool>> CreateBucket(CreateBucketRequest request);
        Task<ApiResponse<IList<string>>> ListOfBucket();
        Task<ApiResponse<bool>> CreateFoldersAsync(CreateFolderRequest request);
        Task<ApiResponse<FileBase>> UploadFiles(UploadFileRequest request);

        public Task<(Stream, string)> DownloadFile(DownloadFileRequest request);
        public Task<ApiResponse<bool>> DeleteFile(DeleteFileRequest request);

        public Task<ApiResponse<IList<string>>> ListAllFiles(ListFilesRequest request);
    }
}
