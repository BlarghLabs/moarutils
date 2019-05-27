using Amazon;
using Amazon.S3;
using System;

namespace MoarUtils.Utils.AWS.S3 {
  public class Exists {
    public static bool Execute(
      string AWSAccessKey,
      string AWSSecretKey,
      string fileKey,
      string bucketName,
      RegionEndpoint re,
      out string url
    ) {
      url = "";
      try {
        using (var s3c = new AmazonS3Client(
          awsAccessKeyId: AWSAccessKey,
          awsSecretAccessKey: AWSSecretKey,
          region: re
        )) {
          var s3FileInfo = new Amazon.S3.IO.S3FileInfo(s3c, bucketName, fileKey);
          url = "https://s3.amazonaws.com/" + bucketName + "/" + fileKey;
          return s3FileInfo.Exists;
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        return false;
      }
    }
  }
}
