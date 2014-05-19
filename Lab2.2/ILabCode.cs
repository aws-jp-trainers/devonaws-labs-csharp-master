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

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace AwsLabs
{
    interface ILabCode
    {
        void CreateAccountItem(AmazonDynamoDBClient ddbClient, string tableName, Account account);
        QueryResponse LookupByHashKey(AmazonDynamoDBClient ddbClient, string tableName, string company);
        void UpdateIfMatch(AmazonDynamoDBClient ddbClient, string tableName, string email, string company, string firstNameTarget, string firstNameMatch);
    }
}
