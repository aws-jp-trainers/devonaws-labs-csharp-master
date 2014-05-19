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
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.S3;

namespace AwsLabs
{
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     用意されたS3クライアントを用いて、指定されたバケットとキーに対して事前署名付きURLを生成する
        ///     URLは1分で期限が切れるようにする
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="key">リンクを用意するオブジェクトのキー</param>
        /// <param name="bucket">オブジェクトを格納するバケット</param>
        /// <returns>オブジェクトの事前署名付きURL</returns>
        public override string GetUrlForItem(AmazonS3Client s3Client, string key, string bucket)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.GetUrlForItem(s3Client, key, bucket);
        }

        /// <summary>
        ///     DynamoDBから、ページに表示するイメージの詳細を含むアイテムコレクションを返す
        ///     アイテムを含むテーブルの名前は、SeSSIONTABLEの値から特定する
        ///     PARAM3に定義されたキープリフィックスを基に結果をフィルターする。 スキャン操作を使用してアイテムを特定するようにする
        ///     アイテムコレクションは結果のオブジェクトに入る
        /// </summary>
        /// <param name="dynamoDbClient">DynamoDBクライアントオブジェクト</param>
        /// <remarks>
        ///     このタスクの目的は、DynamoDBに対してスキャン操作を行うために、ランタイムにアプリケーションに渡される構成情報を使う練習をすることです
        /// </remarks>
        /// <returns>マッチングアイテムコレクション</returns>
        public override List<Dictionary<string, AttributeValue>> GetImageItems(AmazonDynamoDBClient dynamoDbClient)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.GetImageItems(dynamoDbClient);
        }

        /// <summary>
        ///     REGION設定にリージョン制約を適用するS3クライアントオブジェクトを生成して返す
        /// </summary>
        /// <remarks>
        ///     このタスクの目的は、どのリージョンをターゲットかを特定するために、ランタイムにアプリケーションに渡される構成情報を使う練習をすることです
        /// </remarks>
        /// <returns>S3クライアントオブジェクト</returns>
        public override AmazonS3Client CreateS3Client(AWSCredentials credentials)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.CreateS3Client(credentials);
        }

        /// <summary>
        ///     REGION設定にリージョン制約を適用するDynamoDBクライアントオブジェクトを生成して返す
        /// </summary>
        /// <remarks>
        ///     このタスクの目的は、どのリージョンをターゲットかを特定するために、ランタイムにアプリケーションに渡される構成情報を使う練習をすることです
        /// </remarks>
        /// <returns>DynamoDBクライアントオブジェクト</returns>
        public override AmazonDynamoDBClient CreateDynamoDbClient(AWSCredentials credentials)
        {
            //TODO:基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.CreateDynamoDbClient(credentials);
        }

        /// <summary>
        ///     このメソッドは、DynamoDBのアイテムコレクションを、ウェブページに表示できる要素に変換するために使用する
        ///     このタスクを完了するには、以下を行う
        ///     (1) コレクション中のアイテムをループし、 "Key" および "Bucket"属性値を抽出
        ///     (2) "Key" および "Bucket"値を使用して、各オブジェクトの事前署名付きURLを生成。URLを生成するには、GetUrlForItem()メソッドの実装を呼び出し、戻り値を掴む
        ///     (3)  各アイテムについて、_Default.AddImageToPage()を呼び出し、メソッドパラメータとしてキー、バケット、およびURLの値を渡す
        /// </summary>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="items">解析するアイテムコレクション</param>
        /// <remarks>
        ///     このタスクの目的は、渡されるDynamoDBの結果オブジェクトのアイテムコレクションから、アイテムの属性を取り出す練習をすることです
          /// </remarks>
        public override void AddItemsToPage(AmazonS3Client s3Client, List<Dictionary<string, AttributeValue>> items)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.AddItemsToPage(s3Client, items);
        }

        #region Optional Tasks

        /// <summary>
        ///     DynamoDBテーブルを検査し、指定したハッシュキーにマッチするアイテムを含むかどうかを特定する
        /// </summary>
        /// <param name="dynamoDbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">検索するテーブル名</param>
        /// <param name="key">特定するアイテムのハッシュキー</param>
        /// <remarks>
        ///     このタスクの目的は、DynamoDBnoアイテムの存在を確認する正しいプロセスを特定し実装することです
        /// </remarks>
        /// <returns>アイテムが存在していれば真、なければ偽 </returns>
        public override bool IsImageInDynamo(AmazonDynamoDBClient dynamoDbClient, string tableName, string key)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.IsImageInDynamo(dynamoDbClient, tableName, key);
        }

        /// <summary>
        ///     指定されたテーブルのテーブル説明をリクエストし、呼び出し元に返す
        ///     Hint: DescribeTable操作を使用する
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">テーブル名</param>
        /// <remarks>
        ///     このタスクの目的は、DynamoDBテーブルの構成を検査すうる経験を得ることです
        /// </remarks>
        /// <returns> テーブル説明オブジェクト。テーブルがなければヌル</returns>
        public override TableDescription GetTableDescription(AmazonDynamoDBClient ddbClient, string tableName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.GetTableDescription(ddbClient, tableName);
        }

        /// <summary>
        ///     tableDescriptionパラメータで定義されるスキーマを有効化する 
        ///     テーブルは以下の特徴を持つことを求められる
        ///      スキーマ - "Key" および "Bucket"の最低でも2つの属性（双方ともに文字列型）
        ///     　ハッシュキー -  "Key"という名前の文字列型の1つの属性
        ///    　　レンジキー - "Bucket"という名前の文字列型の1つの属性
        /// </summary>
        /// <param name="tableDescription">テーブル定義</param>
        /// <remarks>
        ///     このタスクの目的は、DynamoDBを使用する上でに通常行う複雑なデータ構成を扱う経験を得ることです
        /// </remarks>
        /// <returns>スキーマが期待と一致するときは真、スキーマが無効または例外が投げられたときは偽</returns>
        public override bool ValidateSchema(TableDescription tableDescription)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.ValidateSchema(tableDescription);
        }

        /// <summary>
        ///     指定したテーブルに関連するテーブルステータスの文字列を返す
        ///     テーブルステータスはTableDescriptionオブジェクトのプロパティ
        ///     Hint: テーブル説明を得るために、GetTableDescription()メソッドを呼び出す
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">検査するテーブル名</param>
        /// <remarks>
        ///     この演習の目的は、DynamoDBのステータスを検査する練習をすることです。テーブルステータスを見ることで、他のテーブル操作が失敗する、またはテーブルステータスが期待する状態になったときのヒントを得ることがｋでいます
        /// </remarks>
        /// <returns>テーブルステータス文字列 テーブルが存在しない、または特定できない場合は "NOTFOUND" </returns>
        public override string GetTableStatus(AmazonDynamoDBClient ddbClient, string tableName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.GetTableStatus(ddbClient, tableName);
        }

        /// <summary>
        ///     テーブルステータスが、与えられたステータスの文字列とマッチするまで、このスレッドの実行を停止するstring or until a timeout
        ///     タイムアウトパラメータで設定した値に達した場合、(timeout はDateTime.Nowよりも小さい)
        ///     TimeoutExceptionを投げる
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">検査するテーブル名</param>
        /// <param name="status">望むテーブルステータス</param>
        /// <param name="timeout">待機をやめる時間。ヌルの場合、無限に待機</param>
        /// <remarks>
        ///     この演習の目的は、実践的なテーブルステータスを利用する経験をすることです(テーブルが望むステータスとなるまで待機）
        /// </remarks>
        public override void WaitForStatus(AmazonDynamoDBClient ddbClient, string tableName, string status,
            DateTime? timeout = null)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.WaitForStatus(ddbClient, tableName, status, timeout);
        }

        /// <summary>
        ///     指定されたテーブルを削除する。このメソッドは、ラボ制御コードによって、存在するテーブルがこのラボに向こうと判断した場合に呼び出されます
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">削除するテーブル名</param>
        /// <remarks>
        ///     この演習の目的は、不要な、または不適切にプロビジョンされたリソースをクリーンアップする経験を得ることです
        /// </remarks>
        public override void DeleteTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.DeleteTable(ddbClient, tableName);
        }

        /// <summary>
        ///     指定されたイメージをS3にアップロードし、DynamoDBのイメージへの参照を加える
        ///     イメージを表すDynamoDBのアイテムは、以下の2つの属性を持っているべきである
        ///     Key - S3のオブジェクトへのキー
        ///     Bucket - そのオブジェクトの存在するバケット
        ///     S3のオブジェクトには何もパーミッションの設定は何も行わず、制限されたデフォルトを保つ.
        ///     このメソッドは、ラボ制御コードがラボで使われるイメージがない、またはDynamoDBで正しく参照されないと判断された場合に呼び出される
        ///     最低1回は実行される
        /// </summary>
        /// <param name="dynamoDbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">アイテムを入れるテーブル名</param>
        /// <param name="s3Client">S3クライアントオブジェクト</param>
        /// <param name="bucketName">S3オブジェクトにつかうバケットの名前</param>
        /// <param name="imageKey">S3オブジェクトに使用するキー</param>
        /// <param name="filePath">アップロードするイメージへのフルパス</param>
        /// <remarks>
        ///     このタスクの目的は、自身のアプリケーションでAWSリソースを準備するプロセスの経験を得ることです
        /// </remarks>
        public override void AddImage(AmazonDynamoDBClient dynamoDbClient, string tableName, AmazonS3Client s3Client,
            string bucketName,
            string imageKey, string filePath)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.AddImage(dynamoDbClient, tableName, s3Client, bucketName, imageKey, filePath);
        }

        /// <summary>
        ///     ボで使われるテーブルを作成。テーブルステータスが"ACTIVE"となるまでこのメソッドから戻らない
        ///      Hint: 待機するために前に作成したWaitForStatus() メソッドを使用する
        ///     これらのパラメータにマッチするテーブルを構築する
        ///     属性 - "Key" 文字列、"Bucket" 文字列
        ///     ハッシュキー属性 - "Key"
        ///     レンジキー属性 - "Bucket"
        ///     プロビジョンドキャパシティ - 5 Reads/5 Writes
        ///     このメソッドは、ラボを準備するために最低1回はラボ制御コードから呼び出される。
        ///     また、ラボ制御コードが、テーブルを再構築する必要があると判断した場合にも呼び出される (例：スキーマが期待とマッチしない）
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">作成するテーブル名</param>
        public override void BuildTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.BuildTable(ddbClient, tableName);
        }

        #endregion Optional Tasks
    }
}
