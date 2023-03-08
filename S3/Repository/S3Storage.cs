using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;
using S3.BaseClass;
using S3.Contract;
using S3.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace S3.Repository
{
    public class S3Storage : IS3Storage
    {
        private IAmazonS3 client;
        private readonly S3Model _config;

        public S3Storage(IOptions<S3Model> config)
        {
            _config = config.Value;
            client = new AmazonS3Client(_config.accessKey, _config.secertKey, new AmazonS3Config
            {
                AuthenticationRegion = RegionEndpoint.USEast1.SystemName,
                ServiceURL = _config.endpoint,
                ForcePathStyle = true
            });
        }

        // Me
        public async Task<ApiResponse<bool>> CreateBucket(CreateBucketRequest request)
        {
            try
            {
                var checkBucketName = await AmazonS3Util.DoesS3BucketExistV2Async(client, request.bucketName);
                if (checkBucketName == false)
                {
                    var Request = new PutBucketRequest
                    {
                        BucketName = request.bucketName,
                        UseClientRegion = true
                    };

                    var response = await client.PutBucketAsync(Request);
                    if (response != null)
                    {
                        return new ApiResponse<bool> { message = "Save Bucket name success", container = true, responseCode = "0000" };
                    }
                    else
                    {
                        return new ApiResponse<bool> { message = "Save Bucket name failed", container = false, responseCode = "0001" };
                    }
                }
                else
                {
                    return new ApiResponse<bool> { message = "Bucket name Already exists..", container = false, responseCode = "0002" };
                }
            }
            catch (AmazonS3Exception ex)
            {
                return new ApiResponse<bool> { message = ex.Message, container = false, responseCode = "0003" };
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<ApiResponse<bool>> CreateFoldersAsync(CreateFolderRequest request)
        {
            string folderPath = request.folderName;
            char op = '/';
            folderPath = folderPath.Trim() + op;


            try
            {
                //var checkBucketName = await AmazonS3Util.DoesS3BucketExistV2Async(client, config.bucket);// if Json
                var checkBucketName = await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName: request.bucketName);
                if (checkBucketName == false)
                {
                    return new ApiResponse<bool> { message = "Bucket isn't Exist", container = false };
                }

                ListObjectsRequest findFolderRequest = new ListObjectsRequest();

                findFolderRequest.BucketName = request.bucketName;
                findFolderRequest.Delimiter = "/";
                findFolderRequest.Prefix = request.folderName;

                var findFolderResponse = await client.ListObjectsAsync(findFolderRequest);
                List<string> commonPrefixes = findFolderResponse.CommonPrefixes;

                bool folderExists = commonPrefixes.Any();

                if (folderExists)
                {
                    return new ApiResponse<bool> { message = "Folder already exist", container = false };
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Folder existence check has failed.");
                Console.WriteLine("Amazon error code: {0}",
                    string.IsNullOrEmpty(e.ErrorCode) ? "None" : e.ErrorCode);
                Console.WriteLine("Exception message: {0}", e.Message);
            }

            // add folder
            PutObjectRequest req = new PutObjectRequest()
            {
                BucketName = request.bucketName,
                Key = folderPath // <-- in S3 key represents a path  
            };

            var response = client.PutObjectAsync(req);
            if (response != null)
            {
                return new ApiResponse<bool> { message = "Save Folder name success", container = true };
            }
            else
            {
                return new ApiResponse<bool> { message = "Save Folder name failed", container = false };
            }
        }
        public async Task<ApiResponse<FileBase>> UploadFiles(UploadFileRequest request)
        {
            PutObjectResponse putResponse = null;
            List<string> filesName = new List<string>(); // files that added successfully 
            List<string> failedFilesName = new List<string>(); // files that failed to add

            try
            {
                string fileName = "";
                // get request id 
                //var AccreditationRequestObject = await repositoryBase.FirstAsync<AccreditationRequestData>(x => x.RequestIdpk == 7); //8
                //var requestId = AccreditationRequestObject.RequestidCchi;
                foreach (var item in request?.files)
                {
                    fileName = item.FileName;

                    var files = await client.ListObjectsV2Async(new ListObjectsV2Request
                    {
                        //BucketName = config.bucket, // if Json 
                        BucketName = request.bucketName,
                        Prefix = $"{request?.filePath}/{fileName}",
                    });

                    var key = files.S3Objects.Select(i => i.Key);


                    if (key.Count() > 0)
                    {
                        failedFilesName.Add(fileName);
                        continue;
                    }

                    PutObjectRequest put = null;
                    using (var stream = item.OpenReadStream())
                    {
                        put = new PutObjectRequest()
                        {
                            //BucketName = config.bucket,//if Json
                            BucketName = request.bucketName,
                            Key = $"{request.filePath}/{fileName}",
                            AutoCloseStream = true,
                            InputStream = stream,
                            ContentType = "application/json",
                        };
                        putResponse = await client.PutObjectAsync(put);
                        filesName.Add(fileName);
                    }
                }


                return new ApiResponse<FileBase>
                {
                    container = new FileBase()
                    {
                        folderName = request.filePath,
                        SuccessFilesName = filesName,
                        FailedFilesName = failedFilesName
                    },
                    message = putResponse?.HttpStatusCode.ToString()
                };
                //}
            }
            catch (Exception ex)
            {
                var e = ex.InnerException;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                }
                Console.WriteLine("UploadFileToS3: exception", ex.Message);
                Console.WriteLine("UploadFileToS3: inner exception ", e.Message);
                return new ApiResponse<FileBase> { message = e.Message, container = null };
            }
        }
        public async Task<ApiResponse<IList<string>>> ListAllFiles(ListFilesRequest request)
        {
            try
            {
                //var checkBucketName = await AmazonS3Util.DoesS3BucketExistV2Async(client, config.bucket);// if Json
                var checkBucketName = await AmazonS3Util.DoesS3BucketExistV2Async(client, request.bucketName);
                if (checkBucketName == false)
                {
                    return new ApiResponse<IList<string>> { message = "Bucket isn't Exsit .." };
                }
                if (request.fullPath is null)
                {
                    return new ApiResponse<IList<string>> { message = " Full Path is empty " };
                }

                var files = await client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    //BucketName = config.bucket,// if Json
                    BucketName = request.bucketName,
                    Prefix = request?.fullPath
                });

                return new ApiResponse<IList<string>> { container = files.S3Objects.Select(i => i.Key).ToList() };
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                //logger.LogError("Error encountered ***. Message:'{0}' when reading object", e.Message);
                return new ApiResponse<IList<string>>();
            }
            catch (Exception e)
            {
                //logger.LogError("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
                return new ApiResponse<IList<string>>();
            }
        }
        public async Task<ApiResponse<bool>> DeleteFile(DeleteFileRequest request)
        {
            //"path": "1b0c9a9a-d89a-ec11-b816-00505697564d/1.txt"
            try
            {
                var files = await client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    //BucketName = config.bucket,// if Json
                    BucketName = request.bucketName,
                    Prefix = request.filePath,
                });

                var key = files.S3Objects.Select(i => i.Key);


                if (key.Count() > 0)
                {
                    DeleteObjectRequest req = new DeleteObjectRequest
                    {
                        //BucketName = config.bucket, // if Json
                        BucketName = request.bucketName,
                        Key = request.filePath, // $"{requestId}/{fileName}"
                    };

                    var response = await client.DeleteObjectAsync(req);

                    return response.HttpStatusCode == HttpStatusCode.OK ?
                        new ApiResponse<bool> { container = true , message = "File Deleted Successfully!"} :
                        new ApiResponse<bool> { container = true };
                }
                else
                {
                    return new ApiResponse<bool> { message = "File is not exist already", container = false };
                }
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                //logger.LogError("Error encountered ***. Message:'{0}' when reading object", e.Message);
                return new ApiResponse<bool> { container = false };
            }
            catch (Exception e)
            {
                //logger.LogError("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
                return new ApiResponse<bool> { container = false };
            }
        }
        public async Task<(Stream, string)> DownloadFile(DownloadFileRequest request)
        {
            try
            {
                // check folder 


                GetObjectRequest req = new GetObjectRequest
                {
                    //BucketName = config.bucket, // if Json
                    BucketName = request.bucketName,
                    Key = /*"/" +*/ request.filePath + "/" + request.fileName
                };

                var response = await client.GetObjectAsync(req);

                return (response.ResponseStream, "/" + request.filePath + "/" + request.fileName);

            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                //logger.LogError("Error encountered ***. Message:'{0}' when reading object", e.Message);
                return (null, null);
            }
            catch (Exception e)
            {
                //logger.LogError("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
                return (null, null);
            }
        }

        public async Task<ApiResponse<IList<string>>> ListOfBucket()
        {

            ListBucketsResponse listBucketsResponse = await client.ListBucketsAsync();

            List<S3Bucket> S3Buckets = listBucketsResponse.Buckets;

            List<string> lst = new List<string>();

            foreach (S3Bucket s3Bucket in S3Buckets)
            {
                lst.Add(s3Bucket.BucketName.ToString());
            }

            if (lst.Any())
            {
                return new ApiResponse<IList<string>> { container = lst.ToList()};
            }
            else
            {
                return new ApiResponse<IList<string>> { container = null , message = "There's no bucket found!" };
            }

        }
    }
}
