using Amazon;
using Amazon.S3;
using MoarUtils.commands.logging;
using System;

namespace MoarUtils.Utils.AWS.S3 {
  public class S3FileLength {
    public static long Execute(
      string AWSAccessKey,
      string AWSSecretKey,
      string fileKey,
      RegionEndpoint re,
      string bucketName
    ) {
      try {
        using (var s3c = new AmazonS3Client(AWSAccessKey, AWSSecretKey, re)) {
          var s3FileInfo = new Amazon.S3.IO.S3FileInfo(s3c, bucketName, fileKey);
          return s3FileInfo.Length;
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        return 0;
      }
    }
  }
}
