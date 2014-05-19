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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AwsLabs
{
    internal class SolutionCode : IOptionalLabCode, ILabCode
    {
        virtual public  string PrepMode_GetUserArn(AmazonIdentityManagementServiceClient iamClient, string userName)
        {
            var userArn = String.Empty;
            var getUserRequest = new GetUserRequest {UserName = userName};
            // リクエストを送信して、ユーザーARNを保存
            userArn = iamClient.GetUser(getUserRequest).User.Arn;

            return userArn;
        }

        virtual public  string PrepMode_CreateRole(AmazonIdentityManagementServiceClient iamClient, string roleName,
            string policyText, string trustRelationshipText)
        {
            var roleArn = String.Empty;

            // CreateRoleRequestオブジェクトを使用してロールを定義。 trustRelationshipTextパラメータの値として、AssumeRolePolicyDocumentプロパティが設定されるべき 
  
            var createRoleRequest = new CreateRoleRequest
            {
                AssumeRolePolicyDocument = trustRelationshipText,
                RoleName = roleName
            };
            roleArn = iamClient.CreateRole(createRoleRequest).Role.Arn;

            // リクエストを定義するために、PutRolePolicyRequestオブジェクトを使用する。任意のポリシー名を選択
            // PolicyDocumentプロパティがポリシーを説明
            var putRolePolicyRequest = new PutRolePolicyRequest
            {
                RoleName = roleName,
                PolicyName = String.Format("{0}_policy", roleName),
                PolicyDocument = policyText
            };
            iamClient.PutRolePolicy(putRolePolicyRequest);

            return roleArn;
        }

        virtual public  void PrepMode_RemoveRoles(AmazonIdentityManagementServiceClient iamClient, params string[] roles)
        {
            foreach (var roleName in roles)
            {
                try
                {
                    iamClient.GetRole(new GetRoleRequest {RoleName = roleName});
                    Console.WriteLine("Removing old role {0}.", roleName);
                    // 既存のポリシーを削除する
                    var listRolePoliciesResponse =
                        iamClient.ListRolePolicies(new ListRolePoliciesRequest {RoleName = roleName});
                    foreach (var policyName in listRolePoliciesResponse.PolicyNames)
                    {
                        var deleteRolePolicyRequest = new DeleteRolePolicyRequest
                        {
                            PolicyName = policyName,
                            RoleName = roleName
                        };
                        iamClient.DeleteRolePolicy(deleteRolePolicyRequest);
                    }
                    iamClient.DeleteRole(new DeleteRoleRequest {RoleName = roleName});
                }
                catch (NoSuchEntityException)
                {
                    
                    // ロールが存在しないので何もしない
                    // 例外を握りつぶしてループを抜ける
                    break;
                }
            }
        }

        virtual public  void PrepMode_CreateBucket(AmazonS3Client s3Client, string bucketName)
        {
            var putBucketRequest = new PutBucketRequest {BucketName = bucketName, UseClientRegion = true};
            s3Client.PutBucket(putBucketRequest);
        }

        virtual public  Credentials AppMode_AssumeRole(AmazonSecurityTokenServiceClient stsClient, string roleArn,
            string roleSessionName)
        {
            Credentials credentials = null;

            var assumeRoleRequest = new AssumeRoleRequest
            {
                RoleArn = roleArn,
                RoleSessionName = roleSessionName
            };

            bool retry;
            int sleepSeconds = 3;

            DateTime startTime = DateTime.Now;
            do
            {
                try
                {
                    AssumeRoleResponse assumeRoleResponse = stsClient.AssumeRole(assumeRoleRequest);
                    credentials = assumeRoleResponse.Credentials;
                    
                    retry = false;
                }
                catch (AmazonServiceException ase)
                {
                    if (ase.ErrorCode.Equals("AccessDenied"))
                    {
                        if (sleepSeconds > 20)
                        {
                            // ここに到達した場合、何度か再試行して、同じエラーを受け取り続けているということ
                            // 待機をやめるため、エラーを再スロー。例外は握りつぶされる
                            Console.WriteLine(" [Aborted AssumeRole Operation]");
                            retry = false;
                        }
                        else
                        {
                            // 画面に期間を表示し、再試行の仕組みが動いていることを明示する 
                            Console.Write(".");
                            // 再試行の前にスリープ
                            Thread.Sleep(TimeSpan.FromSeconds(sleepSeconds));
                            // 再試行間隔をインクリメント
                            sleepSeconds = sleepSeconds*3;
                            retry = true;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (retry);

            return credentials;
        }

        virtual public  AmazonS3Client AppMode_CreateS3Client(Credentials credentials, RegionEndpoint regionEndpoint)
        {
            AmazonS3Client s3Client;
            var sessionCredentials = new SessionAWSCredentials(
                credentials.AccessKeyId,
                credentials.SecretAccessKey,
                credentials.SessionToken);

            s3Client = new AmazonS3Client(sessionCredentials, regionEndpoint);

            return s3Client;
        }

        virtual public  bool AppMode_TestSnsAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            try
            {
                var snsClient = new AmazonSimpleNotificationServiceClient(credentials, regionEndpoint);
                snsClient.ListTopics(new ListTopicsRequest());
                return true;
            }
            catch
            {
                return false;
            }
        }

        virtual public  bool AppMode_TestSqsAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            try
            {
                var sqsClient = new AmazonSQSClient(credentials, regionEndpoint);
                sqsClient.ListQueues(new ListQueuesRequest());
                return true;
            }
            catch
            {
                return false;
            }
        }

        virtual public  bool AppMode_TestIamAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            try
            {
                var iamClient = new AmazonIdentityManagementServiceClient(credentials, regionEndpoint);
                iamClient.ListUsers(new ListUsersRequest());
                return true;
            }
            catch
            {
                return false;
            }
        }

        virtual public  void RemoveLabBuckets(AmazonS3Client s3Client, List<string> bucketNames)
        {
            foreach (var bucketName in bucketNames)
            {
                try
                {
                    ListObjectsResponse listObjectsResponse =
                        s3Client.ListObjects(new ListObjectsRequest {BucketName = bucketName});
                    foreach (var s3Object in listObjectsResponse.S3Objects)
                    {
                        var deleteObjectRequest = new DeleteObjectRequest
                        {
                            BucketName = bucketName,
                            Key = s3Object.Key
                        };
                        s3Client.DeleteObject(deleteObjectRequest);
                    }

                    s3Client.DeleteBucket(new DeleteBucketRequest {BucketName = bucketName});
                }
                catch (AmazonS3Exception s3E)
                {
                    if (!s3E.ErrorCode.Equals("NoSuchBucket"))
                    {
                        // このエラーは期待されないため、再スロー
                        throw;
                    }
                }
            }
        }
    }
}
