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
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     与えられたS3クライアントオブジェクトを使用して、指定したバケットを作成
        ///     Hint: クライアントオブジェクトのPutBucket()メソッドを使用する
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">作成するバケットの名前</param>
        /// <remarks>このタスクの目的は、S3をプログラムから使用する経験を得ることです</remarks>
        public override void CreateBucket(AmazonS3Client s3Client, string bucketName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.CreateBucket(s3Client, bucketName);
        }

        /// <summary>
        ///     与えられたアイテムを指定したバケットにアップロードする
        ///     Hint: PutObject()のクライアントオブジェクトを使用する
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">ターゲットのバケットの名前</param>
        /// <param name="sourceFile">アップロードするファイルの名前</param>
        /// <param name="objectKey">新しいS3オブジェクトに割りあえるキー</param>
        /// <remarks>このタスクの目的は、S3をプログラムから使用する経験を得ることです</remarks>
        public override void PutObject(AmazonS3Client s3Client, string bucketName, string sourceFile, string objectKey)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.PutObject(s3Client, bucketName, sourceFile, objectKey);
        }

        /// <summary>
        ///     オブジェクトキーとアイテムサイズをコンソールに書き込みすることで指定したバケットの中身をリストする
        ///     Hint: ListObjects()のクライアントオブジェクトを使用する
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">リストするオブジェクトを含むバケットの名前</param>
        /// <remarks>このタスクの目的は、S3をプログラムから使用する経験を得ることです</remarks>
        public override void ListObjects(AmazonS3Client s3Client, string bucketName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.ListObjects(s3Client, bucketName);
        }

        /// <summary>
        ///     指定したオブジェクトのACLを変更して、パブリックにアクセスできるように設定する
        ///     Hint: クライアントオブジェクトのSetACL()メソッドを呼び出す。 S3CannedACL列挙型を用いてオブジェクトのACLを規定ACL"PublicRead"に設定
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">オブジェクトを格納しているバケットの名前</param>
        /// <param name="key">オブジェクトを特定するために使用するキー</param>
        /// <remarks>このタスクの目的は、S3をプログラムから使用する経験を得ることです</remarks>
        public override void MakeObjectPublic(AmazonS3Client s3Client, string bucketName, string key)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.MakeObjectPublic(s3Client, bucketName, key);
        }

        /// <summary>
        ///     指定されたアイテムの事前署名付きURLを作成し、返す。生成から1時間でURLの有効期限が切れるように設定する
        ///     Hint: クライアントオブジェクトのGetPreSignedURL()メソッドを使用する
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">オブジェクトを格納しているバケットの名前</param>
        /// <param name="key">オブジェクトを特定するために使用するキー</param>
        /// <remarks>このタスクの目的は、S3をプログラムから使用する経験を得ることです</remarks>
        /// <returns>オブジェクトの事前署名付きURL</returns>
        public override string GeneratePreSignedUrl(AmazonS3Client s3Client, string bucketName, string key)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.GeneratePreSignedUrl(s3Client, bucketName, key);
        }

        #region Optional Tasks

        /// <summary>
        ///     指定したバケットを削除する。クライアントオブジェクトのDeleteBucket() を使用してバケットを削除するが、その前にバケットの中身を削除する必要がある
        ///     中身を削除するには、オブジェクトをリストし、それぞれ削除する(DeleteObject() メソッド)か、バッチで削除する(DeleteObjects() メソッド).
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">削除するバケットの名前</param>
        /// <remarks>
        ///     このタスクの目的は、使用していないAWSリソースの削除を自動科するようなプログラムを作成する経験を得ることです
        /// </remarks>
        public override void DeleteBucket(AmazonS3Client s3Client, string bucketName)
        {
            base.DeleteBucket(s3Client, bucketName);
        }

        #endregion
    }
}
