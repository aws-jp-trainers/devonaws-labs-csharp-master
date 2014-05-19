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

using Amazon;
using Amazon.IdentityManagement;
using Amazon.S3;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace AwsLabs
{
    internal interface ILabCode
    {
        string PrepMode_GetUserArn(AmazonIdentityManagementServiceClient iamClient, string userName);

        string PrepMode_CreateRole(AmazonIdentityManagementServiceClient iamClient, string roleName, string policyText,
            string trustRelationshipText);

        Credentials AppMode_AssumeRole(AmazonSecurityTokenServiceClient stsClient, string roleArn,
            string roleSessionName);

        AmazonS3Client AppMode_CreateS3Client(Credentials credentials, RegionEndpoint regionEndpoint);
    }
}
