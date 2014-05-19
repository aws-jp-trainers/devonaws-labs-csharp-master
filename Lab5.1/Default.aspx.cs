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
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.S3.Model;

namespace AwsLabs
{
    public partial class _Default : Page
    {

        #region Non-Student Code
        private static readonly ILabCode LabCode = new StudentCode();
        private static readonly IOptionalLabCode OptionalLabCode = new StudentCode();
       

        private static List<string> StatusLog { get; set; }
        private static List<string> ImageListRows { get; set; }

        private readonly AWSCredentials _credentials;

        /// <summary>
        ///     Prepare the lab exercise by creating the DynamoDB table, creating a bucket,
        ///     uploading the icons into the bucket, and storing a reference to the items in DynamoDB.
        /// </summary>
        public _Default()
        {
            StatusLog = new List<string>();
            ImageListRows = new List<string>();
            var imageNames = new List<string>
            {
                "icons\\dynamodb.png",
                "icons\\ec2.png",
                "icons\\elasticbeanstalk.png",
                "icons\\iam.png",
                "icons\\s3.png",
                "icons\\sqs.png",
                // DECOY IMAGE
                "decoy\\decoy.png"
            };
            try
            {
                _credentials = new CustomCredentialsProviderChain().GetCredentials();
            }
            catch (Exception ex)
            {
                LogMessageToPage(ex.ToString());
                return;
            }


            // Get our settings from the settings file if it's provided.
            PrepSettings();

            // Check to see that we have our table name in the settings
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["SESSIONTABLE"]))
            {
                LogMessageToPage("SESSIONTABLE wasn't defined, using default value \"imageindex.\"");
                // The value is empty, so put a default value for testing in there.
                ConfigurationManager.AppSettings["SESSIONTABLE"] = "imageindex";
            }
            string tableName = ConfigurationManager.AppSettings["SESSIONTABLE"];

            // Check to see that we have our region in the settings
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["REGION"]))
            {
                LogMessageToPage("REGION wasn't defined in settings, using default value \"us-east-1.\"");
                // The value is empty, so put a default value for testing in there.
                ConfigurationManager.AppSettings["REGION"] = "us-east-1";
            }

            // Check to see that we have our region in the settings
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["PARAM3"]))
            {
                LogMessageToPage("PARAM3 wasn't defined in settings. Setting it to an empty string.");
                // The value is empty, so put a default value for testing in there.
                ConfigurationManager.AppSettings["PARAM3"] = "";
            }

            // We have a table name and a region, so let's see if the table exists.
            using (var dynamoDbClient = LabCode.CreateDynamoDbClient(_credentials))
            {
                TableDescription tableDescription = OptionalLabCode.GetTableDescription(dynamoDbClient, tableName);
                if (tableDescription == null)
                {
                    // No table. Create it.
                    LogMessageToPage("No table found. Creating it.");
                    OptionalLabCode.BuildTable(dynamoDbClient, tableName);
                    tableDescription = OptionalLabCode.GetTableDescription(dynamoDbClient, tableName);
                }

                // We have a table, so let's see if it's valid.
                if (!OptionalLabCode.ValidateSchema(tableDescription))
                {
                    //Not valid, so we need to rebuild it.
                    LogMessageToPage("Table schema is incorrect. Dropping table and rebuilding it.");
                    OptionalLabCode.DeleteTable(dynamoDbClient, tableName);
                    OptionalLabCode.BuildTable(dynamoDbClient, tableName);
                }

                // Valid, so let's look for our images. If they're not in DynamoDB, we need to add them to DynamoDB and S3
                var missingImages = new List<string>();
                foreach (string image in imageNames)
                {
                    // We replace the slashes in the item key becasue backslashes (\) cause a signing error in the SDK.
                    if (!OptionalLabCode.IsImageInDynamo(dynamoDbClient, tableName, image.Replace("\\", "/")))
                    {
                        // It's not there, so add it to list of missing images.
                        missingImages.Add(image);
                    }
                }

                if (missingImages.Count > 0)
                {
                    StatusLog.Add("Adding images to S3 and DynamoDB");
                    // Some images are missing. Add them.
                    var bucketName = "awslabc" + Guid.NewGuid().ToString().Substring(0, 8);
                    using (var s3Client = LabCode.CreateS3Client(_credentials))
                    {
                        // Create the bucket
                        s3Client.PutBucket(new PutBucketRequest
                        {
                            BucketName = bucketName,
                            UseClientRegion = true
                        });
                        foreach (string image in missingImages)
                        {
                            string filePath = HttpRuntime.AppDomainAppPath + image;
                            OptionalLabCode.AddImage(
                                dynamoDbClient,
                                tableName,
                                s3Client,
                                bucketName,
                                // We replace the slashes in the item key becasue backslashes (\) cause a signing error in the SDK.
                                image.Replace("\\", "/"),
                                filePath);
                        }
                    }
                }
            }
        }

        private void PrepSettings()
        {
            string settingsFile = ConfigurationManager.AppSettings["runtime.settings"];
            // If the setting is specified, then find the file it references and read it into our app settings
            if (!String.IsNullOrEmpty(settingsFile))
            {
                if (File.Exists(settingsFile))
                {
                    StreamReader reader = File.OpenText(settingsFile);
                    string settingsLine = reader.ReadLine();
                    while (settingsLine != null)
                    {
                        string[] pair = settingsLine.Split('=');
                        if (pair.Length == 2)
                        {
                            ConfigurationManager.AppSettings[pair[0]] = pair[1];
                        }
                        settingsLine = reader.ReadLine();
                    }
                }
            }
        }

        /// <summary>
        ///     This is called after the class is constructed and before the page is displayed.
        ///     We use it as a trigger to populate the page elements.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Write our status messages to a list on the page.
            var statusBuilder = new StringBuilder();
            if (StatusLog.Count > 0)
            {
                statusBuilder.AppendLine("<ul>");
                foreach (var status in StatusLog)
                {
                    statusBuilder.AppendFormat("<li>{0}</li>\n", status.Contains("\n") ? FormatForPage(status) : status);
                }
                statusBuilder.AppendLine("</ul>");
                statusPlaceholder.Text = statusBuilder.ToString();

                // We've written the messages out, so remove them from the list.
                StatusLog.Clear();
            }

            // Write information about the system to the page.
            WriteSystemVariables();

            // Build our list of images from DynamoDB and S3
            BuildImageList();

            // Display the image list in a table on the page.
            var imageListBuilder = new StringBuilder();
            foreach (var imageRow in ImageListRows)
            {
                imageListBuilder.AppendLine(imageRow);
            }
            imageListPlaceholder.Text = imageListBuilder.ToString();
        }

        /// <summary>
        /// Format text for proper display on a web page. This method is used to beautify status messages.
        /// </summary>
        private string FormatForPage(string status)
        {
            return String.Format("<p>{0}</p>",
                status.Replace("  ", "&nbsp;&nbsp;")
                      .Replace("\r\n", "<br/>"));
        }

        /// <summary>
        ///     Grabs information from the local system environment and displays it on the page.
        ///     In particular, it shows the computer name and processor identifier.
        /// </summary>
        private void WriteSystemVariables()
        {
            var instanceIdentity = GetInstanceIdentityMetadata() ?? new InstanceIdentity();
            var sb = new StringBuilder();
            sb.AppendFormat("<tr><td><b>{0}</b>&nbsp;</td><td>&nbsp;{1}</td></tr>", "EC2 Instance ID",
                instanceIdentity.instanceId);

            sb.AppendFormat("<tr><td><b>{0}</b>&nbsp;</td><td>&nbsp;{1}</td></tr>", "Instance Type",
                instanceIdentity.instanceType);

            sb.AppendFormat("<tr><td><b>{0}</b>&nbsp;</td><td>&nbsp;{1}</td></tr>", "Host Instance Region",
                instanceIdentity.region);

            sb.AppendFormat("<tr><td><b>{0}</b>&nbsp;</td><td>&nbsp;{1}</td></tr>", "Availability Zone",
                instanceIdentity.availabilityZone);

            var keys = new[] {"PROCESSOR_IDENTIFIER"};
            foreach (string key in keys)
            {
                sb.AppendFormat("<tr><td><b>{0}</b>&nbsp;</td><td>&nbsp;{1}</td></tr>",
                    key, Environment.GetEnvironmentVariable(key));
            }
            sysenvPlaceholder.Text = sb.ToString();


            sb = new StringBuilder();
            sb.AppendFormat("<tr><td><b>{0}</b> <i>{1}</i>&nbsp;</td><td>&nbsp;{2}</td></tr>", "SESSIONTABLE", "(table name)",
                ConfigurationManager.AppSettings["SESSIONTABLE"]);
            sb.AppendFormat("<tr><td><b>{0}</b> <i>{1}</i>&nbsp;</td><td>&nbsp;{2}</td></tr>", "REGION", "(target region)",
                ConfigurationManager.AppSettings["REGION"]);
            sb.AppendFormat("<tr><td><b>{0}</b> <i>{1}</i>&nbsp;</td><td>&nbsp;{2}</td></tr>", "PARAM3", "(key prefix)",
                ConfigurationManager.AppSettings["PARAM3"]);
            sb.AppendFormat("<tr><td><b>{0}</b>&nbsp;</td><td>&nbsp;{1}</td></tr>", "runtime.settings", ConfigurationManager.AppSettings["runtime.settings"]);

            configPlaceholder.Text = sb.ToString();
        }

        /// <summary>
        ///     Controls the process of gathering a fresh list of images from DynamoDB and
        ///     collecting the links used to display them.
        /// </summary>
        private void BuildImageList()
        {
            ImageListRows.Clear();
            if (_credentials != null)
            {
                using (var dynamoDbClient = LabCode.CreateDynamoDbClient(_credentials))
                {
                    using (var s3Client = LabCode.CreateS3Client(_credentials))
                    {
                        var images = LabCode.GetImageItems(dynamoDbClient);
                        if (images != null)
                        {
                            LabCode.AddItemsToPage(s3Client, images);
                        }
                        else
                        {
                            LogMessageToPage("List of images from DynamoDB came back empty.");
                        }
                    }
                }
            }
            else
            {
                LogMessageToPage("Skipped building image list becasue no credentials were found.");
            }
        }

        /// <summary>
        ///     Adds an image to the web page as a row in a table. The image specified by the URL
        ///     parameter is displayed on the page, and the remaining parameters to the method are displayed
        ///     beside it.
        /// </summary>
        /// <param name="url">The URL for the image to dispaly.</param>
        /// <param name="bucket">The bucket that contains the image file.</param>
        /// <param name="key">The key of the image file in the bucket.</param>
        public static void AddImageToPage(string url, string bucket, string key)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div class=\"imageblock\"><table><tr>");
            sb.AppendFormat("<td><img src=\"{0}\" /></td>", url);
            sb.AppendFormat("<td><table>");
            sb.AppendFormat("<tr><td><b>Bucket:</b></td><td>{0}</td></tr>", bucket);
            sb.AppendFormat("<tr><td><b>Key:</b></td><td>{0}</td></tr>", key);
            sb.AppendFormat("</table></td></tr></table></div>");
            ImageListRows.Add(sb.ToString());
        }

        /// <summary>
        ///     Store a message to be logged to the screen when the page renders.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogMessageToPage(string message)
        {
            StatusLog.Add(String.Format("[<i>{0:MM/dd/yy H:mm:ss.ff}</i>] {1}", DateTime.Now, message));
        }

        /// <summary>
        ///     Store a message to be logged to the screen when the page renders.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The values to show in the formatted string.</param>
        public static void LogMessageToPage(string format, params object[] args)
        {
            LogMessageToPage(String.Format(format, args));
        }

        /// <summary>
        ///     Return the text located at the specified URL.
        /// </summary>
        /// <returns>The text, or null if there was a problem.</returns>
        private static string GetStringFromUrl(string url, string failureDefault = null)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch (WebException wex)
            {
                if (wex.Status == WebExceptionStatus.ConnectFailure)
                {
                    // We're running local (not on EC2)
                    LogMessageToPage("Unable to connect to {0}.", url);
                    return failureDefault;
                }
            }
            return null;
        }


        private static InstanceIdentity GetInstanceIdentityMetadata()
        {
            const string instanceIdentityDocumentUrl =
                "http://169.254.169.254/latest/dynamic/instance-identity/document";
            try
            {
                var instanceIdentity = GetStringFromUrl(instanceIdentityDocumentUrl, null);
                return new JavaScriptSerializer().Deserialize<InstanceIdentity>(instanceIdentity);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     This class is used as a deserialization target for the JSON describing
        ///     the instance identity details.
        /// </summary>
        private class InstanceIdentity
        {
            public InstanceIdentity()
            {
                // Initialize all of the members to "(no instance metadata)"    
                instanceId = "(no instance metadata)";
                version = "(no instance metadata)";
                region = "(no instance metadata)";
                accountId = "(no instance metadata)";
                imageId = "(no instance metadata)";
                kernelId = "(no instance metadata)";
                ramdiskId = "(no instance metadata)";
                architecture = "(no instance metadata)";
                pendingTime = "(no instance metadata)";
                instanceType = "(no instance metadata)";
                availabilityZone = "(no instance metadata)";
                privateIp = "(no instance metadata)";
            }

            public string instanceId { get; set; }

            //public string[] billingProducts { get; set; }
            public string version { get; set; }
            public string region { get; set; }
            public string accountId { get; set; }
            public string imageId { get; set; }
            public string kernelId { get; set; }
            public string ramdiskId { get; set; }
            public string architecture { get; set; }
            public string pendingTime { get; set; }
            public string instanceType { get; set; }
            public string availabilityZone { get; set; }
            public string privateIp { get; set; }
        }

        #endregion Non-Student Code
    }
}
