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
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AwsLabs
{
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     与えられたキュー名を用いてSQSキューを作成し、新しいキューのURLを返す
        ///     Hint: クライアントオブジェクトのCreateQueue() メソッドを使用する。URLは戻り値
        /// </summary>
        /// <param name="sqsClient">SQSクライアントオブジェクト</param>
        /// <param name="queueName">作成するキューの名前</param>
        /// <returns>新たに作成したキューのURL</returns>
        /// <remarks>このタスクの目的は、キュー作成に最も関連する項目の経験を得ることです</remarks>
        public override string CreateQueue(AmazonSQSClient sqsClient, string queueName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.CreateQueue(sqsClient, queueName);
        }

        /// <summary>
        ///     指定されたキューのARNに対しSQSサービスをクエリして返す
        ///     Hint: クライアントオブジェクトのGetQueueAttributes() メソッドを使用する。リクエストする属性の名前はQueueArn
        /// </summary>
        /// <param name="sqsClient">SQSクライアントオブジェクト</param>
        /// <param name="queueUrl">検査するキューのURL</param>
        /// <returns>キューのARNを含む文字列</returns>
        /// <remarks>
        ///     このタスクの目的は、キューの特定の属性をリクエストすることと、そのためのANRを特定する経験を得ることです
        /// </remarks>
        public override string GetQueueArn(AmazonSQSClient sqsClient, string queueUrl)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.GetQueueArn(sqsClient, queueUrl);
        }

        /// <summary>
        ///     SNSトピックを作成し、新たに作成したトピックのARNを返す。ヒント：クライアントオブジェクトのcreateTopic() メソッドを使用する
        ///     Hint:戻り値にトピックのARNが含まれる
        /// </summary>
        /// <param name="snsClient">SNSクライアントオブジェクト</param>
        /// <param name="topicName">作成するトピックの名前</param>
        /// <returns>新規作成したトピックのARN</returns>
        public override string CreateTopic(AmazonSimpleNotificationServiceClient snsClient, string topicName)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.CreateTopic(snsClient, topicName);
        }

        /// <summary>
        ///     SQSキューへ通知を発行するSNSサブスクリプションを作成する。ヒント：クライアントオブジェクトの subscribe()メソッドを使用する
        ///     サブスクリプションエンドポイントとはqueueArnパラメータで与えられる
        /// </summary>
        /// <param name="snsClient">SNSクライアントオブジェクト</param>
        /// <param name="queueArn">サブスクリプションエンドポイントで使用するキューのARN</param>
        /// <param name="topicArn">サブスクライブするトピックのARN</param>
        /// <remarks>このタスクの目的は、サブスクリプションリクエストの構成項目に慣れて頂くことです</remarks>
        public override void CreateSubscription(AmazonSimpleNotificationServiceClient snsClient, string queueArn,
            string topicArn)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.CreateSubscription(snsClient, queueArn, topicArn);
        }

        /// <summary>
        ///     指定されたSNSトピックへのメッセージを発行。ヒント：クライアントオブジェクトのpublish() メソッドを使用する
        /// </summary>
        /// <param name="snsClient">SNSクライアントオブジェクト</param>
        /// <param name="topicArn">メッセージを送るトピックのARN</param>
        /// <param name="subject">発行するメッセージの件名</param>
        /// <param name="message">発行するメッセージの中身</param>
        public override void PublishTopicMessage(AmazonSimpleNotificationServiceClient snsClient, string topicArn,
            string subject,
            string message)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.PublishTopicMessage(snsClient, topicArn, subject, message);
        }

        /// <summary>
        ///     指定されたキューにメッセージを送る。ヒント：クライアントオブジェクトのsendMessage() メソッドを使用する
        /// </summary>
        /// <param name="sqsClient">SQSクライアントオブジェクト</param>
        /// <param name="queueUrl">メッセージを置くキューのURL</param>
        /// <param name="messageText">キューに置くメッセージの中身</param>
        public override void PostToQueue(AmazonSQSClient sqsClient, string queueUrl, string messageText)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.PostToQueue(sqsClient, queueUrl, messageText);
        }

        /// <summary>
        ///     1つのリクエストで指定されたSQSキューから10メッセージくらいまで読み込む。ヒント：クライアントオブジェクトのReceiveMessage()メソッドを使用する
        ///     リクエストでは、最大メッセージ数を10に設定
        /// </summary>
        /// <param name="sqsClient">SQSクライアントオブジェクト</param>
        /// <param name="queueUrl">メッセージを含むキューのURL</param>
        /// <returns>キューからのメッセージのリスト</returns>
        /// <remarks>
        ///     このタスクの目的は、キューからメッセージを読みこみが、デフォルトでは1メッセージずつであることを確認することです
        /// </remarks>
        public override List<Message> ReadMessages(AmazonSQSClient sqsClient, string queueUrl)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            return base.ReadMessages(sqsClient, queueUrl);
        }

        /// <summary>
        ///     指定されたキューから指定されたメッセージを削除する。ヒント：クライアントオブジェクトのdeleteMessage()メソッドを使用する
        /// </summary>
        /// <param name="sqsClient">SQSクライアントオブジェクト</param>
        /// <param name="queueUrl">メッセージを含むキューURL</param>
        /// <param name="receiptHandle">削除するメッセージのreceipt handle</param>
        /// <remarks>このタスクの目的は、キューからメッセージを削除することを経験することです</remarks>
        public override void RemoveMessage(AmazonSQSClient sqsClient, string queueUrl, string receiptHandle)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.RemoveMessage(sqsClient, queueUrl, receiptHandle);
        }

        /// <summary>
        ///     指定されたSNSトピックのサブスクリプションをすべて削除する
        ///     Hint: クライアントオブジェクトの ListSubscriptionsByTopic() メソッドですべてのサブスクリプションを呼び出し、クライアントオブジェクトのunsubscribe()メソッドを用いて詳細とともに繰り返す
        /// </summary>
        /// <param name="snsClient">SNSクライアントオブジェクト</param>
        /// <param name="topicArn">サブスクリプションを削除するSNSトピック</param>
        /// <remarks>
        ///     このタスクの目的は、使用していないリソースをクリーンアップする経験を得ることです。通知を削除する前にサブスクリプションを削除する必要があります
        /// </remarks>
        public override void DeleteSubscriptions(AmazonSimpleNotificationServiceClient snsClient, string topicArn)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.DeleteSubscriptions(snsClient, topicArn);
        }

        /// <summary>
        ///     指定したSNSトピックを削除する
        ///     Hint: クライアントオブジェクトのDeleteTopic() メソッドを使用する
        /// </summary>
        /// <param name="snsClient">SNSクライアントオブジェクト</param>
        /// <param name="topicArn">The ARN of the topic to delete.</param>
        /// <remarks>The purpose of this task is to give you experience cleaning up unused resources.</remarks>
        public override void DeleteTopic(AmazonSimpleNotificationServiceClient snsClient, string topicArn)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.DeleteTopic(snsClient, topicArn);
        }

        /// <summary>
        ///     指定したSQSキューを削除する
        ///     Hint: クライアントオブジェクトのDeleteQueue() メソッドを使用する
        /// </summary>
        /// <param name="sqsClient">SQSクライアントオブジェクト</param>
        /// <param name="queueUrl">削除するキューのURL</param>
        /// <remarks>このタスクの目的は、使用していないリソースをクリーンアップすることを経験することです</remarks>
        public override void DeleteQueue(AmazonSQSClient sqsClient, string queueUrl)
        {
            //TODO: 基本クラスの呼び出しを、自分の実装メソッドに置き換える
            base.DeleteQueue(sqsClient, queueUrl);
        }

        #region Optional Tasks

        /// <summary>
        ///     与えられたSNSトピックが、自身のキューにメッセージを発行するパーミッションを付与する。これを実現するためには、
        ///     適切に指定されたポリシーステートメントを作成し、キューのPolicy属性にそれを割り当てる必要がある
        ///     （正しくこのタスクを実行するためには調査が必要です）
        /// </summary>
        /// <param name="sqsClient">SQSクライアントオブジェクト</param>
        /// <param name="queueArn">キューを定義するARN。ポリシーステートメントにおいて、Resourceとして用いられる</param>
        /// <param name="queueUrl">
        ///     キューのURL。ポリシー属性を更新する目的でキューを特定するのに使用される
        /// </param>
        /// <param name="topicArn">
        ///     キューに発行するトピックのARN。 ポリシーステートメントにおいてARN Conditionのソースとして用いられる
        /// </param>
        /// <remarks>
        ///     このタスクの目的は、講義では触れなかった部分を、安全な演習環境で試して自由に議論をして頂くことです
            /// </remarks>
        public override void GrantNotificationPermission(AmazonSQSClient sqsClient, string queueArn, string queueUrl,
            string topicArn)
        {
            base.GrantNotificationPermission(sqsClient, queueArn, queueUrl, topicArn);
        }

        #endregion
    }
}
