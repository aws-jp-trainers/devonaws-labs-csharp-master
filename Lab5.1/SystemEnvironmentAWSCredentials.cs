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
using Amazon.Runtime;

namespace AwsLabs
{
    /// <summary>
    ///     環境変数AWS_ACCESS_KEY_ID および AWS_SECRET_KEYClassから認証情報を取り出すために使用するクラス
    /// </summary>
    public class SystemEnvironmentAWSCredentials : AWSCredentials
    {
        private readonly ImmutableCredentials _credentials;

        public SystemEnvironmentAWSCredentials()
        {
            string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");

            if (String.IsNullOrEmpty(accessKey) || String.IsNullOrEmpty(secretKey))
            {
                throw new Exception("No credentials found in the system environment.");
            }
            _credentials = new ImmutableCredentials(accessKey, secretKey, "");
        }

        public override ImmutableCredentials GetCredentials()
        {
            return _credentials != null ? _credentials.Copy() : null;
        }
    }
}
