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
    interface ILabCode
    {

        string CreateQueue(AmazonSQSClient sqsClient, string queueName);
        string GetQueueArn(AmazonSQSClient sqsClient, string queueUrl);
        string CreateTopic(AmazonSimpleNotificationServiceClient snsClient, string topicName);
        void CreateSubscription(AmazonSimpleNotificationServiceClient snsClient, string queueArn, string topicArn);
        void PublishTopicMessage(AmazonSimpleNotificationServiceClient snsClient, string topicArn, string subject, string message);
        void PostToQueue(AmazonSQSClient sqsClient, string queueUrl, string messageText);
        List<Message> ReadMessages(AmazonSQSClient sqsClient, string queueUrl);
        void RemoveMessage(AmazonSQSClient sqsClient, string queueUrl, string receiptHandle);
        void DeleteSubscriptions(AmazonSimpleNotificationServiceClient snsClient, string topicArn);
        void DeleteTopic(AmazonSimpleNotificationServiceClient snsClient, string topicArn);
        void DeleteQueue(AmazonSQSClient sqsClient, string queueUrl);
    }
}
