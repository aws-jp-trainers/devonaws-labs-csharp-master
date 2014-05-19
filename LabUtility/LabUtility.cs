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
    public static class LabUtility
    {
        public static void DumpError(Exception ex)
        {
            if (ex is AmazonServiceException)
            {
                var ase = ex as AmazonServiceException;
                Console.WriteLine("\nCaught an AmazonServiceException, which means your request made it AWS,");
                Console.WriteLine("but was rejected with an error response for some reason.");
                Console.WriteLine("Error Message:    {0}", ase.Message);
                Console.WriteLine("HTTP Status Code: {0}", ase.StatusCode);
                Console.WriteLine("AWS Error Code:   {0}", ase.ErrorCode);
                Console.WriteLine("Error Type:       {0}", ase.ErrorType);
                Console.WriteLine("Request ID:       {0}", ase.RequestId);
                Console.WriteLine("Stack trace:\n{0}", ase.StackTrace);
            }
            else if (ex is AmazonClientException)
            {
                var ace = ex as AmazonClientException;
                Console.WriteLine("\nCaught an AmazonClientException, which means the client encountered");
                Console.WriteLine("a problem without before communicating with the service.");
                Console.WriteLine("Error Message: {0}", ace.Message);
                Console.WriteLine("Stack trace:\n{0}", ace.StackTrace);
            }
            else
            {
                Console.WriteLine("\nCaught exception [{0}].", ex.GetType());
                Console.WriteLine("Error Message: {0}", ex.Message);
                Console.WriteLine("Inner exception: {0}", ex.InnerException);
                Console.WriteLine("Stack trace:\n{0}", ex.StackTrace);
            }
        }
    }
}
