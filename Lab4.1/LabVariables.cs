﻿// Copyright 2013 Amazon.com, Inc. or its affiliates. All Rights Reserved.
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

namespace AwsLabs
{
    public class LabVariables
    {
        public string ProductionRoleArn { get; set; }
        public string DevelopmentRoleArn { get; set; }

        private List<string> _bucketNames = new List<string>(); 
        public List<string> BucketNames
        {
            get
            {
                return _bucketNames;
            }
            set
            {
                _bucketNames = value;
            }
        }
    }
}
