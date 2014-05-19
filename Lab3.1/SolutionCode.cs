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
using Amazon.Auth.AccessControlPolicy;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AwsLabs
{
    internal class SolutionCode : IOptionalLabCode, ILabCode
    {
        public virtual string CreateQueue(AmazonSQSClient sqsClient, string queueName)
        {
            string queueUrl;

            // リクエストの作成
            var createQueueRequest = new CreateQueueRequest {QueueName = queueName};

            // リクエストの送信
            CreateQueueResponse createQueueResponse = sqsClient.CreateQueue(createQueueRequest);

            // 新たに作成したキューのURLを返す
            queueUrl = createQueueResponse.QueueUrl;
            return queueUrl;
        }


        public virtual string GetQueueArn(AmazonSQSClient sqsClient, string queueUrl)
        {
            string queueArn;
            // ARNを得るためのURLのGetQueueAttributesRequestを生成
            var getQueueAttributesRequest = new GetQueueAttributesRequest
            {
                QueueUrl = queueUrl,
                AttributeNames =
                {
                    "QueueArn"
                }
            };

            // リクエストの送信
            GetQueueAttributesResponse getQueueAttributesResponse =
                sqsClient.GetQueueAttributes(getQueueAttributesRequest);

            // 見つけたARNをqueueArnList変数に追加
            queueArn = getQueueAttributesResponse.QueueARN;
            return queueArn;
        }


        public virtual string CreateTopic(AmazonSimpleNotificationServiceClient snsClient, string topicName)
        {
            string topicArn;
            // リクエストの作成
            var createTopicRequest = new CreateTopicRequest
            {
                Name = topicName
            };
            // リクエストの送信
            CreateTopicResponse topicResponse = snsClient.CreateTopic(createTopicRequest);

            // ARNを返す
            topicArn = topicResponse.TopicArn;
            return topicArn;
        }


        public virtual void CreateSubscription(AmazonSimpleNotificationServiceClient snsClient, string queueArn,
            string topicArn)
        {
            // リクエストの作成
            var subscriptionRequest = new SubscribeRequest
            {
                Endpoint = queueArn,
                Protocol = "sqs",
                TopicArn = topicArn
            };
            // サブスクリプションの作成
            snsClient.Subscribe(subscriptionRequest);
        }


        public virtual void PublishTopicMessage(AmazonSimpleNotificationServiceClient snsClient, string topicArn,
            string subject, string message)
        {
            // リクエストの作成
            var publishRequest = new PublishRequest
            {
                Subject = subject,
                Message = message,
                TopicArn = topicArn
            };

            // リクエストの送信
            snsClient.Publish(publishRequest);
        }


        public virtual void PostToQueue(AmazonSQSClient sqsClient, string queueUrl, string messageText)
        {
            // リクエストの作成
            var sendMessageRequest = new SendMessageRequest
            {
                MessageBody = messageText,
                QueueUrl = queueUrl
            };

            //メッセージをキューに送信
            sqsClient.SendMessage(sendMessageRequest);
        }

        public virtual List<Message> ReadMessages(AmazonSQSClient sqsClient, string queueUrl)
        {
            // リクエストの作成
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10
            };
            // リクエストを送信し、レスポンスを返す
            ReceiveMessageResponse resp = sqsClient.ReceiveMessage(receiveMessageRequest);
            return resp.Messages;
        }

        public virtual void RemoveMessage(AmazonSQSClient sqsClient, string queueUrl, string receiptHandle)
        {
            // リクエストの作成
            var deleteMessageRequest = new DeleteMessageRequest
            {
                ReceiptHandle = receiptHandle,
                QueueUrl = queueUrl
            };
            // リクエストの送信
            sqsClient.DeleteMessage(deleteMessageRequest);
        }


        public virtual void DeleteSubscriptions(AmazonSimpleNotificationServiceClient snsClient, string topicArn)
        {
            var listSubscriptionsByTopicRequest = new ListSubscriptionsByTopicRequest
            {
                TopicArn = topicArn
            };

            ListSubscriptionsByTopicResponse listSubscriptionsByTopicResponse =
                snsClient.ListSubscriptionsByTopic(listSubscriptionsByTopicRequest);

            foreach (
                Subscription subscription in
                    listSubscriptionsByTopicResponse.Subscriptions)
            {
                var unsubscribeRequest = new UnsubscribeRequest
                {
                    SubscriptionArn = subscription.SubscriptionArn
                };
                snsClient.Unsubscribe(unsubscribeRequest);
            }
        }


        public virtual void DeleteTopic(AmazonSimpleNotificationServiceClient snsClient, string topicArn)
        {
            // リクエストの作成
            var deleteTopicRequest = new DeleteTopicRequest
            {
                TopicArn = topicArn
            };

            snsClient.DeleteTopic(deleteTopicRequest);
        }


        public virtual void DeleteQueue(AmazonSQSClient sqsClient, string queueUrl)
        {
            var deleteQueueRequest = new DeleteQueueRequest
            {
                QueueUrl = queueUrl
            };
            // キューの削除
            sqsClient.DeleteQueue(deleteQueueRequest);
        }

        public virtual void GrantNotificationPermission(AmazonSQSClient sqsClient, string queueArn, string queueUrl,
            string topicArn)
        {
            // SNSトピックから通知を受け取るキューを許可するためのポリシーの作成
            var policy = new Policy("SubscriptionPermission")
            {
                Statements =
                {
                    new Statement(Statement.StatementEffect.Allow)
                    {
                        Actions = {SQSActionIdentifiers.SendMessage},
                        Principals = {new Principal("*")},
                        Conditions = {ConditionFactory.NewSourceArnCondition(topicArn)},
                        Resources = {new Resource(queueArn)}
                    }
                }
            };

            var attributes = new Dictionary<string, string>();
            attributes.Add("Policy", policy.ToJson());

            // ポリシーのキュー属性を設定するリクエストを作成
            var setQueueAttributesRequest = new SetQueueAttributesRequest
            {
                QueueUrl = queueUrl,
                Attributes = attributes
            };

            // キューポリシーを設定
            sqsClient.SetQueueAttributes(setQueueAttributesRequest);
        }
    }
}
