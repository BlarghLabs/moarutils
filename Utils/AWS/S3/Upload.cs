using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.IO;

namespace MoarUtils.Utils.AWS.S3 {
  public class Upload {
    public static bool Exists(
      string AWSAccessKey,
      string AWSSecretKey,
      string fileKey,
      string bucketName,
      out string url
    ) {
      url = "";
      try {
        using (var s3c = new AmazonS3Client(AWSAccessKey, AWSSecretKey, RegionEndpoint.USEast1)) {
          var gomr = s3c.GetObjectMetadata(new GetObjectMetadataRequest {
            BucketName = bucketName,
            Key = fileKey
          });
          url = "https://s3.amazonaws.com/" + bucketName + "/" + fileKey;
          return gomr.ContentLength != 0;
          //return true;
        }
      } catch (Amazon.S3.AmazonS3Exception ex) {
        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
          return false;

        //status wasn't not found, so throw the exception
        throw;
      }
    }

    public static bool Execute(
      string AWSAccessKey,
      string AWSSecretKey,
      string bucketName,
      string filePath,
      string toPath,
      out string url
    ) {
      url = "";
      try {
        using (var tu = new TransferUtility(AWSAccessKey, AWSSecretKey, RegionEndpoint.USEast1)) {
          var tuur = new TransferUtilityUploadRequest {
            FilePath = filePath,
            BucketName = bucketName,
            Key = toPath,
            CannedACL = S3CannedACL.PublicRead
          };
          tu.Upload(tuur);
          url = "https://s3.amazonaws.com/" + bucketName + "/" + toPath;
        }
        return true;
      } catch (Exception ex) {
        LogIt.E(ex);
        return false;
      }
    }

    public static bool Execute(
      string AWSAccessKey,
      string AWSSecretKey,
      string bucketName, 
      byte[] ba, 
      string toPath, 
      out string url
    ) {
      url = "";
      try {
        using (var ms = new MemoryStream(ba)) {
          var uploadMultipartRequest = new TransferUtilityUploadRequest {
            BucketName = bucketName,
            Key = toPath,
            CannedACL = S3CannedACL.PublicRead,
            InputStream = ms,
            //PartSize = 123?
          };

          using (var tu = new TransferUtility(AWSAccessKey, AWSSecretKey, RegionEndpoint.USEast1)) {
            tu.Upload(uploadMultipartRequest);
          }
          url = "https://s3.amazonaws.com/" + bucketName + "/" + toPath;
          return true;
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        return false;
      }
    }
  }
}


