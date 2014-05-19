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
using System.Configuration;
using System.IO;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace AwsLabs
{
    internal class Program
    {
        // 最も近いリージョン、または講師に指示されたリージョンのエンドポイント
        private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;

        #region Non-Student Code
        private static readonly ILabCode LabCode = new StudentCode();
        private static readonly IOptionalLabCode OptionalLabCode = new StudentCode();

        private const string LAB_USER_NAME = "LabAppUser";

        public static void Main(string[] args)
        {
            LabVariables labVariables = null;
            var program = new Program();
            try
            {
                //  リソースが期待する状態かを確認するための"prep"モード運用 
                Console.WriteLine("Starting up in \"prep\" mode.");
                labVariables = program.PrepMode_Run();

                Console.WriteLine("\nPrep complete. Transitioning to \"app\" mode.");
                program.AppMode_Run(labVariables);
            }
            catch (Exception ex)
            {
                LabUtility.DumpError(ex);
            }
            finally
            {
                try
                {
                    
                    if (labVariables != null)
                    {
                        Console.Write("\nLab run completed. Cleaning up buckets.");
                        
                        AWSCredentials credentials =
                            new BasicAWSCredentials(ConfigurationManager.AppSettings["prepModeAWSAccessKey"],
                                ConfigurationManager.AppSettings["prepModeAWSSecretKey"]);

                        var s3Client = new AmazonS3Client(credentials, RegionEndpoint);

                        OptionalLabCode.RemoveLabBuckets(s3Client, labVariables.BucketNames);
                        Console.WriteLine(" Done.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nAttempt to clean up buckets failed. {0}", ex.Message);
                }

                Console.WriteLine("\nPress <enter> to end.");
                Console.ReadLine();
            }
        }


        /// <summary>
        ///     ラボで使用するリソースの準備
        /// </summary>
        /// <returns>作成したリソースの詳細</returns>
        public LabVariables PrepMode_Run()
        {
            AWSCredentials credentials =
                new BasicAWSCredentials(ConfigurationManager.AppSettings["prepModeAWSAccessKey"],
                    ConfigurationManager.AppSettings["prepModeAWSSecretKey"]);


            var labVariables = new LabVariables();
            using (var iamClient = new AmazonIdentityManagementServiceClient(credentials, RegionEndpoint))
            {
                string trustRelationship = File.OpenText("TrustRelationship.txt").ReadToEnd();
                string developmentPolicyText = File.OpenText("development_role.txt").ReadToEnd();
                string productionPolicyText = File.OpenText("production_role.txt").ReadToEnd();

                // もし既存のロールがあれば削除し、環境をクリーンアップ
                OptionalLabCode.PrepMode_RemoveRoles(iamClient, "development_role", "production_role");

                // ロールの信頼関係（ロールの使用方法）には、ユーザーのARNが必要
                string userArn = LabCode.PrepMode_GetUserArn(iamClient, LAB_USER_NAME);
                Console.WriteLine("ARN for {0} is {1}", LAB_USER_NAME, userArn);
                trustRelationship = trustRelationship.Replace("{userArn}", userArn);
                Console.WriteLine("Trust relationship policy:\n{0}", trustRelationship);

                // ロールを作成し、ロールARNを格納する
                labVariables.DevelopmentRoleArn = LabCode.PrepMode_CreateRole(iamClient, "development_role",
                    developmentPolicyText, trustRelationship);
                labVariables.ProductionRoleArn = LabCode.PrepMode_CreateRole(iamClient, "production_role",
                    productionPolicyText, trustRelationship);

                Console.WriteLine("Created development policy role: {0}", labVariables.DevelopmentRoleArn);
                Console.WriteLine("Created production policy role: {0}", labVariables.ProductionRoleArn);

                // バケット名を作成
                string identifier = Guid.NewGuid().ToString().Substring(0, 8);
                labVariables.BucketNames.Add("dev" + identifier);
                labVariables.BucketNames.Add("prod" + identifier);

                // バケットの作成
                using (var s3Client = new AmazonS3Client(credentials, RegionEndpoint))
                {
                    foreach (string bucketName in labVariables.BucketNames)
                    {
                        OptionalLabCode.PrepMode_CreateBucket(s3Client, bucketName);
                        Console.WriteLine("Created bucket: {0}", bucketName);
                    }
                }
            }

            return labVariables;
        }

        /// <summary>
        ///    devおよびprodロールと仮定し、パーミッションをチェックしてAppModeの操作を実行
        /// </summary>
        /// <param name="labVariables">データ</param>
        public void AppMode_Run(LabVariables labVariables)
        {
            var credentials = new BasicAWSCredentials(
                ConfigurationManager.AppSettings["appModeAWSAccessKey"],
                ConfigurationManager.AppSettings["appModeAWSSecretKey"]);

            Credentials devCredentials = null, prodCredentials = null;

            using (var stsClient = new AmazonSecurityTokenServiceClient(credentials, RegionEndpoint))
            {
                Console.WriteLine("Assuming developer role to retrieve developer session credentials.");
                devCredentials = LabCode.AppMode_AssumeRole(stsClient, labVariables.DevelopmentRoleArn, "dev_session");
                if (devCredentials == null)
                {
                    Console.WriteLine("No developer credentials returned. AccessDenied.");
                    return;
                }


                Console.WriteLine("\nAssuming production role to retrieve production session credentials.");

                prodCredentials = LabCode.AppMode_AssumeRole(stsClient, labVariables.ProductionRoleArn, "prod_session");
                if (prodCredentials == null)
                {
                    Console.WriteLine("No production credentials returned. AccessDenied.");
                    return;
                }
            }

            using (var devS3Client = LabCode.AppMode_CreateS3Client(devCredentials, RegionEndpoint))
            {
                using (var prodS3Client = LabCode.AppMode_CreateS3Client(prodCredentials, RegionEndpoint))
                {
                    Console.WriteLine("\nTesting Developer Session...");
                    var devSession = new SessionAWSCredentials(
                        devCredentials.AccessKeyId,
                        devCredentials.SecretAccessKey,
                        devCredentials.SessionToken);

                    Console.WriteLine("  IAM: {0}",
                        OptionalLabCode.AppMode_TestIamAccess(RegionEndpoint, devSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  SQS: {0}",
                        OptionalLabCode.AppMode_TestSqsAccess(RegionEndpoint, devSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  SNS: {0}",
                        OptionalLabCode.AppMode_TestSnsAccess(RegionEndpoint, devSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  S3:");
                    foreach (string bucketName in labVariables.BucketNames)
                    {
                        TestS3Client(devS3Client, bucketName);
                    }

                    Console.WriteLine("\nTesting Production Session...");
                    var prodSession = new SessionAWSCredentials(
                        prodCredentials.AccessKeyId,
                        prodCredentials.SecretAccessKey,
                        prodCredentials.SessionToken);

                    Console.WriteLine("  IAM: {0}",
                        OptionalLabCode.AppMode_TestIamAccess(RegionEndpoint, prodSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  SQS: {0}",
                        OptionalLabCode.AppMode_TestSqsAccess(RegionEndpoint, prodSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  SNS: {0}",
                        OptionalLabCode.AppMode_TestSnsAccess(RegionEndpoint, prodSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  S3:");
                    foreach (string bucketName in labVariables.BucketNames)
                    {
                        TestS3Client(prodS3Client, bucketName);
                    }
                }
            }
        }

        /// <summary>
        ///     オブジェクトを追加して、指定されたS3バケットへのアクセスをテストする
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">The bucket name</param>
        public void TestS3Client(AmazonS3Client s3Client, string bucketName)
        {
            const string fileName = "test-image.png";

            Console.Write("    Uploading to bucket {0}. ", bucketName);
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                FilePath = new FileInfo(fileName).FullName,
                Key = fileName
            };

            try
            {
                s3Client.PutObject(putObjectRequest);
                Console.WriteLine("Succeeded.");
            }
            catch (AmazonS3Exception ase)
            {
                Console.WriteLine("Failed. {0}.", ase.ErrorCode);
            }
            catch
            {
                Console.WriteLine("Failed.");
            }
        }

        #endregion
    }
}
