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
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace AwsLabs
{
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     アカウントパラメータに指定された値からDynamoDBのアイテムを作成する。アイテムの属性の名前は、アカウントの対応するプロパティの名前と一致しなければならない。
        ///     アカウントオブジェクトのフィールドで空のものに属性を追加しない (hint: String.IsNullOrEmpty() メソッドを値に使用する).
        ///     CompanyおよりEmail属性はテーブルキーの一部であるため、メソッドが呼び出されたときにに、これらは常にアカウントオブジェクトに与えられる
        ///     重要：Account.Age property が文字列で渡された場合であっても、DynamoDBのアイテムには数値として格納すること
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">アイテムを追加するテーブルの名前</param>
        /// <param name="account">加えるデータを含むアカウントオブジェクト</param>
        /// <remarks>このタスクの目的は、DynamoDBとやりとりをするリクエストオブジェクト生成の経験を得ることです</remarks>
        public override void CreateAccountItem(AmazonDynamoDBClient ddbClient, string tableName, Account account)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.CreateAccountItem(ddbClient, tableName, account);
        }

        /// <summary>
        ///     指定した基準を使用してクエリリクエストを生成し、結果のオブジェクトを返す
        ///     Hint: クライアントオブジェクトのQuery() メソッドを使用する
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">クエリするテーブル名</param>
        /// <param name="company">検索する会社名</param>
        /// <remarks>このタスクの目的は、DynamoDBとやりとりをするリクエストオブジェクト生成の経験を得ることです</remarks>
        /// <returns>クエリ結果を含むオブジェクト</returns>
        public override QueryResponse LookupByHashKey(AmazonDynamoDBClient ddbClient, string tableName, string company)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.LookupByHashKey(ddbClient, tableName, company);
        }

        /// <summary>
        ///     company nameおよびemailパラメータ値にマッチするアイテムをテーブルから探す。
        ///     属性値がfirstNameMatch パラメータにマッチする場合に限り、First属性のfirstNameTargetパラメータに値をセットする
        /// 	Hint: これは、クライアントオブジェクトのUpdateItem()メソッドを使用した1つのリクエストで実現可能です
　　　　　   /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">アイテムを含むテーブル名</param>
        /// <param name="email">Email属性にマッチする値</param>
        /// <param name="company">Company属性にマッチする値</param>
        /// <param name="firstNameTarget">マッチが見つかった場合のFirst属性の新しい値</param>
        /// <param name="firstNameMatch">First属性にマッチする値</param>
        /// <remarks>このタスクの目的は、DynamoDBとやりとりをするリクエストオブジェクト生成の経験を得ることです</remarks>
        public override void UpdateIfMatch(AmazonDynamoDBClient ddbClient, string tableName, string email,
            string company,
            string firstNameTarget, string firstNameMatch)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.UpdateIfMatch(ddbClient, tableName, email, company, firstNameTarget, firstNameMatch);
        }

        #region Optional Tasks

        /// <summary>
        ///     指定されたテーブルのテーブル説明をリクエストし、呼び出し元に返す
        ///     Hint: DescribeTable 操作を使用する
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">テーブル名</param>
        /// <remarks>
        ///     このタスクの目的は、DynamoDBテーブルの構造を検査する経験を得ることです
        /// </remarks>
        /// <returns>テーブル説明オブジェクト。テーブルが見つからない場合はヌル</returns>
        public override TableDescription GetTableDescription(AmazonDynamoDBClient ddbClient, string tableName)
        {
            return base.GetTableDescription(ddbClient, tableName);
        }

        /// <summary>
        ///     指定したテーブルに関連付けられたテーブルステータスの文字列を返す。テーブルステータスは、TableDescriptionオブジェクトのプロパティ
        ///     Hint: GetTableDescription() メソッドを呼び出して、テーブル説明を得る
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">検査するテーブル名</param>
        /// <remarks>
        ///     この演習の目的は、DynamoDBテーブルのステータスを検査する練習の経験を得ることです。テーブルステータスを確認することで、 Looking at the table
        ///     なぜ他のテーブル操作が失敗するのか、またテーブルが予想するステータスにどのようにして到達するのかのヒントを得ることができます
        /// </remarks>
        /// <returns>テーブルステータス文字列。 テーブルが存在しない、または特定できない場合には"NOTFOUND" </returns>
        public override string GetTableStatus(AmazonDynamoDBClient ddbClient, string tableName)
        {
            return base.GetTableStatus(ddbClient, tableName);
        }

        /// <summary>
        ///     与えられたテーブルステータス文字列にマッチするか、タイムアウトに達するまで、このスレッドの実行を停止する。timeoutパラメータの値に達した場合、(タイムアウトはDateTime.Nowより小さい), 
        ///     TimeoutExceptionを投げる
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">検査するテーブルの名前</param>
        /// <param name="status">望むテーブルステータス</param>
        /// <param name="timeout">待機を停止する時間。ヌルの場合、無限に待機</param>
        /// <remarks>
        ///     この演習の目的は、実践的な目的でテーブルステータスを利用する経験を得ることです(期待するステータスにテーブルが達するまで待機
        /// </remarks>
        /// <exception cref="System.TimeoutException">タイムアウトに達した場合にスロー</exception>
        public override void WaitForStatus(AmazonDynamoDBClient ddbClient, string tableName, string status,
            DateTime? timeout = null)
        {
            base.WaitForStatus(ddbClient, tableName, status, timeout);
        }

        /// <summary>
        ///     このラボで使用するテーブルを作成する。テーブルステータスが"ACTIVE"となるまでこのメソッドから戻らない
        ///     Hint: 待機するために、上で実装したWaitForStatus()メソッドを呼び出す
        ///     これらのパラメータにマッチするテーブルを構築する
        ///     --Attributes - "Company" 文字列, "Email" 文字列
        ///     --Hash Key Attribute - "Company"
        ///     --Range Key Attribute - "Email"
        ///     --Provisioned Capacity - 5 Reads/5 Writes
        ///     このメソッドは、ラボコントローラーが、テーブルを再構築する必要があると判断した場合に呼び出される
        ///     (ex. スキーマが期待とマッチしない).
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">作成するテーブル名</param>
        /// <remarks>
        ///     このタスクの目的は、講義内で議論しなかった複雑な演習を完了することに挑戦する機会を提供することです
        /// </remarks>
        public override void BuildTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            base.BuildTable(ddbClient, tableName);
        }

        /// <summary>
        ///     指定されたテーブルを削除。このメソッドは、ラボコントローラーが、存在するテーブルがラボに無効と判断した場合に呼び出される
        /// </summary>
        /// <param name="ddbClient">DynamoDBクライアントオブジェクト</param>
        /// <param name="tableName">削除するテーブル名</param>
        /// <remarks>
        ///     この演習の目的は、必要のない、または不適切にプロビジョンされたリソースをクリーンナップする経験を得ることです
        /// </remarks>
        public override void DeleteTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            base.DeleteTable(ddbClient, tableName);
        }

        #endregion
    }
}
