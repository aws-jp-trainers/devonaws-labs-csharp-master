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

using System.Collections.Generic;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace AwsLabs
{
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     指定されたユーザのARNを検索して返す
        ///     Hint: クライアントオブジェクトのGetUser()メソッドを使用する。
        /// </summary>
        /// <param name="iamClient">IAMクライアントオブジェクト</param>
        /// <param name="userName">検索するユーザー名</param>
        /// <returns>指定されたユーザーのARN</returns>
        public override string PrepMode_GetUserArn(AmazonIdentityManagementServiceClient iamClient, string userName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.PrepMode_GetUserArn(iamClient, userName);
        }

        /// <summary>
        ///     指定されたポリシーと信頼関係テキストを使用し、指定されたロールを作成。ロールARNを返す
        /// </summary>
        /// <param name="iamClient">IAMクライアントオブジェクト</param>
        /// <param name="roleName">作成するロールの名前</param>
        /// <param name="policyText">ロールに付加するポリシー</param>
        /// <param name="trustRelationshipText">だれがロールを引き継ぐことができるかを定義するポリシー</param>
        /// <returns>新規に作成したロールのARN</returns>
        public override string PrepMode_CreateRole(AmazonIdentityManagementServiceClient iamClient, string roleName,
            string policyText,
            string trustRelationshipText)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.PrepMode_CreateRole(iamClient, roleName, policyText, trustRelationshipText);
        }

        /// <summary>
        ///     指定したロールを引き継ぐ
        ///     Hint: クライアントオブジェクトのAssumeRole()メソッドを使用する
        ///     オプション: ここで、結果整合性の問題をみる可能性があります。. AssumeRoleパーミッションは、システム全体に浸透していない可能性があり、これによりロールを引き継ぐことが阻害される可能性があります
        ///     "AmazonServiceException"のエラーコード"AccessDenied" を確認し、少し待機した後にロール操作の引き継ぎを再試行する(再試行でexponential back-offを使用）
        ///     再試行をやめると判断した場合は、ヌルを返す
        /// </summary>
        /// <param name="stsClient">STSクライアントオブジェクト</param>
        /// <param name="roleArn">引き継ぐロールのARN</param>
        /// <param name="roleSessionName">ロールセッション名として使用する名前</param>
        /// <returns>ロール認証情報、または問題がある場合はヌル</returns>
        public override Credentials AppMode_AssumeRole(AmazonSecurityTokenServiceClient stsClient, string roleArn,
            string roleSessionName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.AppMode_AssumeRole(stsClient, roleArn, roleSessionName);
        }

        /// <summary>
        ///     与えられた認証情報（前にassumeRole()メソッドの呼び出しで返されたもの）を使用して、セッション/一時認証情報を作成する
        ///     そして、セッション認証情報を使用してS3クライアントオブジェクトを作成する
        /// </summary>
        /// <param name="credentials">セッション認証情報を作成するために使用する認証情報</param>
        /// <param name="regionEndpoint">クライアントに使用するリージョンのエンドポイント</param>
        /// <returns>S3クライアントオブジェクト</returns>
        public override AmazonS3Client AppMode_CreateS3Client(Credentials credentials, RegionEndpoint regionEndpoint)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.AppMode_CreateS3Client(credentials, regionEndpoint);
        }

        #region Optional Tasks

        /// <summary>
        ///     IAMユーザーのリストをリクエストすることで、与えられた認証情報を使ってIAMサービスにアクセスできるかをテストする
        ///     テストの仕方は問いません。なんらかのリクエストを送信して、実行を確認してください
        /// </summary>
        /// <param name="regionEndpoint">クライアント接続に使用するリージョンエンドポイント</param>
        /// <param name="credentials">使用する認証情報</param>
        /// <returns>サービスがアクセス可能な場合はTrue。認証情報が拒否された場合はFalse</returns>
        public override bool AppMode_TestIamAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.AppMode_TestIamAccess(regionEndpoint, credentials);
        }

        /// <summary>
        ///     SNSトピックのリストをリクエストすることで、与えられた認証情報を使ってSNSサービスにアクセスできるかをテストする
        ///     テストの仕方は問いません。なんらかのリクエストを送信して、実行を確認してください
        /// </summary>
        /// <param name="regionEndpoint">クライアント接続に使用するリージョンエンドポイント</param>
        /// <param name="credentials">使用する認証情報</param>
        /// <returns>サービスがアクセス可能な場合はTrue。認証情報が拒否された場合はFalse</returns>
        public override bool AppMode_TestSnsAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.AppMode_TestSnsAccess(regionEndpoint, credentials);
        }

        /// <summary>
        ///     SQSキューのリストをリクエストすることで、与えられた認証情報を使ってSQSサービスにアクセスできるかをテストする
        ///     テストの仕方は問いません。なんらかのリクエストを送信して、実行を確認してください
        /// </summary>
        /// <param name="regionEndpoint">クライアント接続に使用するリージョンエンドポイント</param>
        /// <param name="credentials">使用する認証情報</param>
        /// <returns>サービスがアクセス可能な場合はTrue。認証情報が拒否された場合はFalse</returns>
        public override bool AppMode_TestSqsAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.AppMode_TestSqsAccess(regionEndpoint, credentials);
        }

        /// <summary>
        ///     このラボで後で使用されるバケットを作成する。ラボ演習の環境を準備するためのコード
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">作成するバケットの名前</param>
        public override void PrepMode_CreateBucket(AmazonS3Client s3Client, string bucketName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.PrepMode_CreateBucket(s3Client, bucketName);
        }

        /// <summary>
        ///     作成しようとしているロール名とマッチするロールを削除。これはラボ制御コードから呼び出され、ラボ実行に支障をきたす可能性があるリソースをクリーンアップするために呼び出される
        /// </summary>
        /// <param name="iamClient">IAMクライアントオブジェクト</param>
        /// <param name="roles">ロール名のリスト</param>
        public override void PrepMode_RemoveRoles(AmazonIdentityManagementServiceClient iamClient, params string[] roles)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.PrepMode_RemoveRoles(iamClient, roles);
        }

        /// <summary>
        ///     このラボで作成されたバケットのクリーンアップと削除
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketNames">削除するバケット</param>
        public override void RemoveLabBuckets(AmazonS3Client s3Client, List<string> bucketNames)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.RemoveLabBuckets(s3Client, bucketNames);
        }

        #endregion
    }
}
