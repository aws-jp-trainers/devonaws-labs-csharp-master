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
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace AwsLabs
{
    class Program
    {
        #region Student Tasks
        /// <summary>
        /// 使用するリージョンのエンドポイント。使用しているテーブルと同じリージョンを選択
        /// </summary>
        private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;


        #endregion

        #region Non-Student Code (Lab plumbing)

        private static readonly ILabCode LabCode = new StudentCode();
        private static readonly IOptionalLabCode OptionalLabCode = new StudentCode();

        /// <summary>
        /// このラボの制御メソッド
        /// </summary>
        public static void Main()
        {
            using (var ddbClient = new AmazonDynamoDBClient(RegionEndpoint))
            {
                const string tableName = "Accounts";
                var accountItems = new List<Account>
                {
                    new Account
                    {
                        Company = "Amazon.com",
                        Email = "johndoe@amazon.com",
                        First = "John",
                        Last = "Doe",
                        Age = "33"
                    },
                    new Account
                    {
                        Company = "Asperatus Tech",
                        Email = "janedoe@amazon.com",
                        First = "Jane",
                        Last = "Doe",
                        Age = "24"
                    },
                    new Account
                    {
                        Company = "Amazon.com",
                        Email = "jimjohnson@amazon.com",
                        First = "Jim",
                        Last = "Johnson"
                    }
                };

                try
                {
                    // テーブルスキーマが期待しているものであることを確認し、問題があれば修正
                    if (!ConfirmTableSchema(ddbClient, tableName))
                    {
                        OptionalLabCode.DeleteTable(ddbClient, tableName);
                        OptionalLabCode.BuildTable(ddbClient, tableName);
                    }

                    Console.WriteLine("Adding items to table.");
                    // アカウントアイテムを作成
                    foreach (Account account in accountItems)
                    {
                        LabCode.CreateAccountItem(ddbClient, tableName, account);
                        Console.WriteLine("Added item: {0}/{1}", account.Company, account.Email);
                    }

                    Console.WriteLine("Requesting matches for Company == Amazon.com");
                    QueryResponse response = LabCode.LookupByHashKey(ddbClient, tableName, "Amazon.com");
                    if (response != null && response.Count > 0)
                    {
                        // レコードの発見
                        foreach (var item in response.Items)
                        {
                            Console.WriteLine("Item Found-");
                            foreach (var attr in item)
                            {
                                Console.WriteLine("    {0} : {1}", attr.Key, attr.Key == "Age" ? attr.Value.N : attr.Value.S);
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No matches found.");
                    }

                    // 条件付きでレコードを更新
                    Console.Write("Attempting update. ");
                    LabCode.UpdateIfMatch(ddbClient, tableName, "jimjohnson@amazon.com", "Amazon.com", "James", "Jim");
                    Console.WriteLine("Done.");
                }
                catch (Exception ex)
                {
                    LabUtility.DumpError(ex);
                }
                finally
                {
                    Console.WriteLine("Press <enter> to end.");
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// 指定されたテーブルを検査し、ラボのターゲットとしてサービス可能であるかを確認する。ラボのテーブルは手作業で構築されているため、予期せぬ状態である可能性がある 
        /// </summary>
        /// <param name="ddbClient">クライアントオブジェクト</param>
        /// <param name="tableName">検査するテーブル名</param>
        /// <returns>スキーマが正しければtrue。正しくない、存在しない、または使用可能でなければfalse</returns>
        private static bool ConfirmTableSchema(AmazonDynamoDBClient ddbClient, string tableName)
        {
            Console.WriteLine("Confirming table schema.");
            TableDescription tableDescription = OptionalLabCode.GetTableDescription(ddbClient, tableName);

            if (tableDescription==null)
            {
                Console.WriteLine("Table does not exist.");
                // テーブルがなければスキーマはマッチできない
                return false;
            }
            if (!tableDescription.TableStatus.Equals("ACTIVE"))
            {
                Console.WriteLine("Table is not active.");
                return false;
            }

            if (tableDescription.AttributeDefinitions == null || tableDescription.KeySchema == null)
            {
                Console.WriteLine("Schema doesn't match.");
                return false;
            }
            foreach (AttributeDefinition attributeDefinition in tableDescription.AttributeDefinitions)
            {
                switch (attributeDefinition.AttributeName)
                {
                    case "Company":
                    case "Email":
                    case "First":
                    case "Last":
                        if (!attributeDefinition.AttributeType.Equals("S"))
                        {
                            // マッチする属性があるが、型が違う
                            Console.WriteLine("{0} attribute is wrong type in attribute definition.", attributeDefinition.AttributeName);
                            return false;
                        }
                        break;
                    case "Age":
                        if (!attributeDefinition.AttributeType.Equals("N"))
                        {
                            Console.WriteLine("Age attribute is wrong type in attribute definition.");
                            // マッチする属性があるが、型が違う
                            return false;
                        }
                        break;
                }
            }
            // ここまでくれば、属性は正しい。ここでキースキーマをチェックする
            if (tableDescription.KeySchema.Count != 2)
            {
                Console.WriteLine("Wrong number of elements in the key schema.");
                return false;
            }
            foreach (KeySchemaElement keySchemaElement in tableDescription.KeySchema)
            {
                switch (keySchemaElement.AttributeName)
                {
                    case "Company":
                        if (!keySchemaElement.KeyType.Equals("HASH"))
                        {
                            // マッチする属性があるが、型が違う
                            Console.WriteLine("Company attribute is wrong type in key schema.");
                            return false;
                        }
                        break;
                    case "Email":
                        if (!keySchemaElement.KeyType.Equals("RANGE"))
                        {
                            // マッチする属性があるが、型が違う.
                            Console.WriteLine("Email attribute is wrong type in key schema.");
                            return false;
                        }
                        break;
                    default:
                        Console.WriteLine("Unexpected attribute ({0}) in the key schema.", keySchemaElement.AttributeName);
                        return false;
                }
                    
            }
            Console.WriteLine("Table schema is as expected.");
            // チェック完了
            return true;
            
        }

        #endregion

    }

    #region Non-Student Code (Account class)
    /// <summary>
    /// テーブル内のアイテムを表すためのクラスを使用
    /// </summary>
    public class Account
    {
        public string Email { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Age { get; set; }
        public string Company { get; set; }

        public Account()
        {
            Age = String.Empty;
            Last = String.Empty;
            First = String.Empty;
            Email = String.Empty;
            Company = String.Empty;
        }
    }
    #endregion


}