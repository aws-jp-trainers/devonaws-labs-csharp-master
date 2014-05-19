// Copyright 2013 Amazon.com, Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"). You may not 
// use this file except in compliance with the License. A copy of the License 
// is located at
// 
// 	http://aws.amazon.com/apache2.0/
// 
// or in the "LICENSE" file accompanying this file. This file is distributed 
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. See the License for the specific language governing 
// permissions and limitations under the License.

using System;
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsLabs
{
    internal class SolutionCode : ILabCode, IOptionalLabCode
    {
        public virtual void CreateBucket(AmazonS3Client s3Client, string bucketName)
        {
            // リクエストの作成
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            // バケットの作成
            s3Client.PutBucket(putBucketRequest);
        }

        public virtual void PutObject(AmazonS3Client s3Client, string bucketName, string sourceFile, string objectKey)
        {
            // リクエストの作成
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                FilePath = sourceFile
            };

            // オブジェクトのアップロード
            s3Client.PutObject(putObjectRequest);
        }

        public virtual void ListObjects(AmazonS3Client s3Client, string bucketName)
        {
            // リクエストの作成
            var listObjectsRequest = new ListObjectsRequest
            {
                BucketName = bucketName
            };

            // リクエストの送信
            ListObjectsResponse listObjectsResponse = s3Client.ListObjects(listObjectsRequest);

            // 結果の表示
            foreach (S3Object objectSummary in listObjectsResponse.S3Objects)
            {
                Console.WriteLine("{0} (size: {1})", objectSummary.Key, objectSummary.Size);
            }
        }

        public virtual void MakeObjectPublic(AmazonS3Client s3Client, string bucketName, string key)
        {
            // リクエストの作成
            var putAclRequest = new PutACLRequest {
                BucketName = bucketName,
                Key = key,
                CannedACL = S3CannedACL.PublicRead
            };

            // リクエストの送信
            s3Client.PutACL(putAclRequest);
        }

        public virtual string GeneratePreSignedUrl(AmazonS3Client s3Client, string bucketName, string key)
        {
            // リクエストの作成
            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.Now.AddHours(1.0)
            };

            // リクエストの送信
            return s3Client.GetPreSignedURL(getPreSignedUrlRequest);
        }

        public virtual void DeleteBucket(AmazonS3Client s3Client, string bucketName)
        {
            // はじめに、バケットの削除を試みる 
            var deleteBucketRequest = new DeleteBucketRequest
            {
                BucketName = bucketName
            };

            try
            {
                s3Client.DeleteBucket(deleteBucketRequest);
                // もしここに来た場合、エラーは生成されていないので、バケットは削除されているとみなして戻る
                return;
            }
            catch (AmazonS3Exception ex)
            {
                if (!ex.ErrorCode.Equals("BucketNotEmpty"))
                {
                    // 予想しないエラーが発生した場合は、再スロー
                    throw;
                }
            }

            // もしここにきた場合、バケットが空ではないため、はじめにアイテムを削除する必要がある

            DeleteObjectsRequest deleteObjectsRequest = new DeleteObjectsRequest {BucketName = bucketName};

            foreach (S3Object obj in s3Client.ListObjects(new ListObjectsRequest {BucketName = bucketName}).S3Objects)
            {
                // 削除リクエストにオブジェクトのキーを加える
                deleteObjectsRequest.AddKey(obj.Key, null);
            }

            // リクエストの送信
            s3Client.DeleteObjects(deleteObjectsRequest);

            // ここでバケットが空になったので、バケットを削除する
            s3Client.DeleteBucket(deleteBucketRequest);
        }
    }
}
