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
using System.IO;
using System.Windows.Forms;
using Amazon;
using Amazon.S3;

namespace AwsLabs
{
    internal class Program
    {
        #region Student Tasks

        // 使用するリージョンのエンドポイント。使用しているテーブルと同じリージョンを選択
        private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;

        #endregion Student Tasks

        #region Non-Student Code
        private static readonly ILabCode LabCode = new StudentCode();
        private static readonly IOptionalLabCode OptionalLabCode = new StudentCode();

        private const string TEST_IMAGE_PNG = "test-image.png";
        private const string PUBLIC_TEST_IMAGE_PNG = "public-test-image.png";
        private const string TEST_IMAGE2_PNG = "test-image2.png";

        [STAThread]
        private static void Main()
        {
            try
            {
                // S3クライアントの作成
                var s3Client = new AmazonS3Client(RegionEndpoint);

                // 一意のバケット名を作成
                var bucketName = "awslab" + Guid.NewGuid().ToString().Substring(0, 8);
                Console.WriteLine("Creating bucket: {0}", bucketName);
                LabCode.CreateBucket(s3Client, bucketName);
                Console.WriteLine("Bucket created.\n");

                // 確実にここにファイルがあるようにし、S3へアップロードするメソッドを呼び出す
                if (!File.Exists(TEST_IMAGE_PNG))
                {
                    Console.WriteLine("The file {0} was not found in the application directory.", TEST_IMAGE_PNG);
                    Console.WriteLine("Please add it to your project and set its \"Build Action\" property");
                    Console.WriteLine("to \"Content\" and its \"Copy to Output Directory\" property to \"Copy Always.\"");
                    return;
                }
                Console.WriteLine("Uploading object: {0}", TEST_IMAGE_PNG);
                LabCode.PutObject(s3Client, bucketName, TEST_IMAGE_PNG, TEST_IMAGE_PNG);
                Console.WriteLine("Upload complete.\n");

                // もう一つのコピーをアップロードする。後のACL変更のデモの際にこれを使用する
                if (!File.Exists(TEST_IMAGE_PNG))
                {
                    Console.WriteLine("The file {0} was not found in the application directory.", TEST_IMAGE2_PNG);
                    Console.WriteLine("Please add it to your project and set its \"Build Action\" property");
                    Console.WriteLine("to \"Content\" and its \"Copy to Output Directory\" property to \"Copy Always.\"");
                    return;
                }
                Console.WriteLine("Uploading a similar object (will be made publicly available later)");
                LabCode.PutObject(s3Client, bucketName, TEST_IMAGE2_PNG, PUBLIC_TEST_IMAGE_PNG);

                Console.WriteLine("Upload complete.\n");

                // バケット内のオブジェクトをリスト
                Console.WriteLine("Listing items in bucket: {0}", bucketName);
                LabCode.ListObjects(s3Client, bucketName);
                Console.WriteLine("Listing complete.\n");

                // オブジェクトの1つのACLを変更して、パブリックにする
                Console.WriteLine("Changing the ACL to make an object public");
                LabCode.MakeObjectPublic(s3Client, bucketName, PUBLIC_TEST_IMAGE_PNG);
                Console.WriteLine("Done the object should be publicly available now.");
                Console.WriteLine("The URL below has been copied into your clippboard. Test it.");
                string publicUrl = String.Format("http://{0}.s3.amazonaws.com/{1}\n", bucketName, PUBLIC_TEST_IMAGE_PNG);
                Console.WriteLine(publicUrl);
                Clipboard.SetText(publicUrl);

                Console.WriteLine("Press <enter> to continue to the next step.");
                Console.ReadLine();

                // ファイルへの一時的なアクセスを許可するために、1つのオブジェクトの事前署名付きURLを生成する
                Console.WriteLine("Generating presigned URL.");
                string presignedUrl = LabCode.GeneratePreSignedUrl(s3Client, bucketName, TEST_IMAGE_PNG);
                Console.WriteLine("Done. {0}\n", presignedUrl);
                Console.WriteLine("The URL has been copied into your clippboard. Test it.");
                Clipboard.SetText(presignedUrl);
                Console.WriteLine("Press <enter> to continue to the next step.");
                Console.ReadLine();
                Console.Write("Deleting lab bucket.");
                OptionalLabCode.DeleteBucket(s3Client, bucketName);
                Console.WriteLine(" Done.");
            }
            catch (Exception ex)
            {
                LabUtility.DumpError(ex);
            }
            finally
            {
                Console.WriteLine("\n\nPress <enter> to end.");
                Console.ReadLine();
            }
        }

        #endregion
    }
}
