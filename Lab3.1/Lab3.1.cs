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
using System.Threading;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AwsLabs
{
    internal class Program
    {
        #region Student Tasks

        /// <summary>
        ///     使用するリージョンのエンドポイント。講師に指示された場合は変更
        /// </summary>
        private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;

        #endregion

        #region Non-Student Code

        private static readonly ILabCode LabCode = new StudentCode();
        private static readonly IOptionalLabCode OptionalLabCode = new StudentCode();

        public static void Main(string[] args)
        {
            try
            {
                using (var snsClient = new AmazonSimpleNotificationServiceClient(RegionEndpoint))
                {
                    using (var sqsClient = new AmazonSQSClient(RegionEndpoint))
                    {
                        const string queueName = "Notifications";
                        const string topicName = "ClassroomEvent";

                        // 単に削除して再作成すると、キューの作成は失敗する。もしコードエラーをトラックしている場合、これが原因の可能性がある
                        // この場合、1分停止して再試行する 
                        Console.WriteLine("Creating {0} queue.", queueName);

                        bool retry = true, notified = false;
                        DateTime start = DateTime.Now;
                        string queueUrl = "";

                        while (retry)
                        {
                            try
                            {
                                // SQSキューの作成
                                queueUrl = LabCode.CreateQueue(sqsClient, queueName);
                                retry = false;
                            }
                            catch (AmazonSQSException ex)
                            {
                                if (!ex.ErrorCode.Equals("AWS.SimpleQueueService.QueueDeletedRecently"))
                                {
                                    // これは予期しないエラーであり、待機し再試行することでは解決しない
                                    // 単に再スロー
                                    throw;
                                }

                                if (DateTime.Now < (start + TimeSpan.FromSeconds(60)))
                                {
                                    if (!notified)
                                    {
                                        Console.WriteLine(
                                            "The attempt to recreate the queue failed because the queue was deleted too\nrecently. Waiting and retrying for up to 1 minute.");
                                        notified = true;
                                    }
                                    // タイムアウトに達していないので、待機して5秒後に再試行
                                    Console.Write(".");
                                    Thread.Sleep(TimeSpan.FromSeconds(5));
                                }
                                else
                                {
                                    Console.WriteLine("Retry timeout expired. Aborting.");
                                    throw;
                                }
                            }
                        }
                        if (notified)
                        {
                            Console.WriteLine("Recovered.");
                        }
                        Console.WriteLine("URL for new queue:\n    {0}", queueUrl);
                        // SQSキューをリスト
                        Console.WriteLine("Getting ARN for {0} queue.", queueName);
                        string queueArn = LabCode.GetQueueArn(sqsClient, queueUrl);
                        Console.WriteLine("ARN for queue: {0}", queueArn);

                        // SNSトピックを作成し、ARNを得る
                        Console.WriteLine("Creating {0} topic.", topicName);
                        string topicArn = LabCode.CreateTopic(snsClient, topicName);
                        Console.WriteLine("New topic ARN: {0}", topicArn);

                        Console.WriteLine("Granting the notification topic permission to post in the queue.");
                        OptionalLabCode.GrantNotificationPermission(sqsClient, queueArn, queueUrl, topicArn);
                        Console.WriteLine("Permission granted.");

                        // SNSサブスクリプションの作成
                        Console.WriteLine("Creating SNS subscription.");
                        LabCode.CreateSubscription(snsClient, queueArn, topicArn);
                        Console.WriteLine("Subscription created.");

                        // トピックにメッセージを発行
                        var messageText = "This is the SNS topic notification body.";
                        var messageSubject = "SNSTopicNotification";

                        Console.WriteLine("Publishing SNS topic notification.");
                        LabCode.PublishTopicMessage(snsClient, topicArn, messageSubject, messageText);
                        Console.WriteLine("Notification published.");

                        // "Notifications"キューにメッセージを送信
                        messageText = "This is the message posted to the queue directly.";
                        Console.WriteLine("Posting message to queue directly.");
                        LabCode.PostToQueue(sqsClient, queueUrl, messageText);
                        Console.WriteLine("Message posted.");

                        //Console.WriteLine(">> PAUSING FOR A MOMENT <<");
                        //Thread.Sleep(TimeSpan.FromSeconds(5));

                        // キューからメッセージを読む
                        Console.WriteLine("Reading messages from queue.");

                        List<Message> messages = LabCode.ReadMessages(sqsClient, queueUrl);
                        // ここで2つのメッセージが期待される
                        if (messages.Count < 2)
                        {
                            // 再度読み込みをためし、無くなったメッセージを選択していないか確認する.
                            messages.AddRange(LabCode.ReadMessages(sqsClient, queueUrl));
                            if (messages.Count < 2)
                            {
                                Console.WriteLine(
                                    ">>WARNING<< We didn't receive the expected number of messages. Investigate.");
                            }
                            else
                            {
                                Console.WriteLine(
                                    "\n===============================================================================");
                                Console.WriteLine(
                                    "PROBLEM: ReadMessages() had to be called twice to collect all the messages.");
                                Console.WriteLine(
                                    "         Did you remember to set the MaxNumberOfMessages property in the ");
                                Console.WriteLine("         ReceiveMessageRequest object?");
                                Console.WriteLine(
                                    "===============================================================================\n");
                            }
                        }
                        PrintAndRemoveMessagesInResponse(sqsClient, messages, queueUrl);

                        // SNSサブスクリプションを特定して削除
                        Console.WriteLine("Removing provisioned resources.");
                        LabCode.DeleteSubscriptions(snsClient, topicArn);
                        Console.WriteLine("Subscriptions removed.");

                        // SNSトピックの削除
                        LabCode.DeleteTopic(snsClient, topicArn);
                        Console.WriteLine("Topic deleted.");
                        // 前に作成したキューを特定して削除
                        LabCode.DeleteQueue(sqsClient, queueUrl);
                        Console.WriteLine("Queue deleted.");
                    }
                }
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


        // コンソールウィンドウにメッセージ内容を表示
        private static void PrintAndRemoveMessagesInResponse(AmazonSQSClient sqsClient, List<Message> messages,
            string queueUrl)
        {
            foreach (var message in messages)
            {
                Console.WriteLine("\nQueue Message:");

                Console.WriteLine("\tMessageId : {0}", message.MessageId);
                Console.WriteLine("\tMD5OfBody : {0}", message.MD5OfBody);
                // newline文字を呼び出して、表示を改善
                message.Body = message.Body.Replace("\n", "\\n");
                if (message.Body.Length > 50)
                {
                    Console.WriteLine("\tBody : {0}...", message.Body.Substring(0, 50));
                }
                else
                {
                    Console.WriteLine("\tBody : {0}", message.Body);
                }

                if (message.Attributes.Count > 0)
                {
                    Console.WriteLine("\tMessage Attributes");
                    foreach (var entry in message.Attributes)
                    {
                        Console.WriteLine("\t\t{0} : {1}", entry.Key, entry.Value);
                    }
                }

                Console.WriteLine("\nDeleting message.");
                LabCode.RemoveMessage(sqsClient, queueUrl, message.ReceiptHandle);
                Console.WriteLine("Message deleted.");
            }
        }

        #endregion
    }
}
