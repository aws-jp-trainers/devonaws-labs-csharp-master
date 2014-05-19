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

namespace AwsLabs
{
    internal interface IOptionalLabCode
    {
        void PrepMode_RemoveRoles(AmazonIdentityManagementServiceClient iamClient, params string[] roles);
        void PrepMode_CreateBucket(AmazonS3Client s3Client, string bucketName);

        bool AppMode_TestSnsAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials);
        bool AppMode_TestSqsAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials);
        bool AppMode_TestIamAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials);

        void RemoveLabBuckets(AmazonS3Client s3Client, List<string> bucketNames);
    }
}
