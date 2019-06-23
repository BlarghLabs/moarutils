using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using MoarUtils.commands.logging;
using System;

namespace MoarUtils.Utils.AWS.S3 {
  public class S3Copy {
    public static bool Execute(
      string AWSAccessKey,
      string AWSSecretKey,
      RegionEndpoint regionEndpoint,
      string sourceBucketName,
      string sourceKey,
      string destBucketName,
      string destKey
    //out long? fileLengthBytes
    ) {
      //fileLengthBytes = null;
      try {
        //validate exists
        string sourceUrl;
        if (!Exists.Execute(
          fileKey: sourceKey,
          bucketName: sourceBucketName,
          url: out sourceUrl,
          re: regionEndpoint,
          AWSAccessKey: AWSAccessKey,
          AWSSecretKey: AWSSecretKey
        )) {
          LogIt.W("source did not exist:" + sourceBucketName + "|" + sourceKey);
          return false;
        }

        //validate dest doesn't already exist, fail if it does bc we aren't validating that it is different? maybe shar eeach in future?
        string destUrl;
        if (Exists.Execute(
          fileKey: destKey,
          bucketName: destBucketName,
          url: out destUrl,
          re: regionEndpoint,
          AWSAccessKey: AWSAccessKey,
          AWSSecretKey: AWSSecretKey
        )) {
          LogIt.W("dest existed already:" + sourceBucketName + "|" + sourceKey);
          return false;
        }

        //copy 
        using (var s3c = new AmazonS3Client(
          awsAccessKeyId: AWSAccessKey,
          awsSecretAccessKey: AWSSecretKey,
          region: regionEndpoint
        )) {
          var cor = s3c.CopyObject(new CopyObjectRequest {
            SourceBucket = sourceBucketName,
            SourceKey = sourceKey,
            DestinationBucket = destBucketName,
            DestinationKey = destKey
          });
          //fileLengthBytes = cor.
          return cor.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        return false;
      }
    }
  }
}


