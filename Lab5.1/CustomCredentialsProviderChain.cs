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
using System.Security.Authentication;
using Amazon.Runtime;

namespace AwsLabs
{
    /// <summary>
    ///     カスタマイズ認証情報プロバイダーチェーンを作成するクラス
    /// </summary>
    public class CustomCredentialsProviderChain
    {
        private delegate AWSCredentials CredentialsGenerator();

        private readonly Dictionary<string, CredentialsGenerator> _credentialsGenerators =
            new Dictionary<string, CredentialsGenerator>();

        /// <summary>
        ///     認証情報プロバイダーチェーンを使用するクラスのインスタンスを作成
        ///     InstanceProfileAWSCredentials
        ///     EnvironmentAWSCredentials
        ///     SystemEnvironmentAWSCredentials
        /// </summary>
        public CustomCredentialsProviderChain()
        {
            AddProvider<InstanceProfileAWSCredentials>();
            AddProvider<EnvironmentAWSCredentials>();
            AddProvider<SystemEnvironmentAWSCredentials>();
        }

        /// <summary>
        ///     このクラスに定義された認証情報プロバイダーチェーン内の有効な認証情報の最初のセットを入手する
        /// </summary>
        /// <returns>AWS認証情報</returns>
        public AWSCredentials GetCredentials()
        {
            var exceptions = new List<Exception>();

            foreach (var generator in _credentialsGenerators)
            {
                AWSCredentials credentials = null;
                try
                {
                    credentials = generator.Value();
                    _Default.LogMessageToPage("({0}) {1}", generator.Key, "Credentials found.");
                    return credentials;
                }
                catch (Exception ex)
                {
                    _Default.LogMessageToPage("({0}) {1}", generator.Key, ex.ToString());
                }
            }
            _Default.LogMessageToPage("No credentials found.");
            throw new AuthenticationException("No credentials found.");
        }

        /// <summary>
        ///     認証情報プロバイダーチェーンの最後に新しいクラスを追加
        /// </summary>
        /// <typeparam name="T">
        ///     追加するクラスの型。 AWSCredentials classから得られたクラスで、新しいオペレータの使用をサポートしなければならない
        /// </typeparam>
        public void AddProvider<T>() where T : AWSCredentials, new()
        {
            string providerName = typeof (T).Name;

            if (!_credentialsGenerators.ContainsKey(providerName))
            {
                _credentialsGenerators.Add(providerName, () => new T());
            }
            else
            {
                throw new Exception(String.Format("A provider of type {0} is already registered.", providerName));
            }
        }

        /// <summary>
        ///     認証情報プロバイダーチェーンからクラスを削除
        /// </summary>
        /// <typeparam name="T">削除するクラスのタイプ</typeparam>
        public void RemoveProvider<T>() where T : AWSCredentials, new()
        {
            string providerName = typeof (T).Name;

            if (_credentialsGenerators.ContainsKey(providerName))
            {
                _credentialsGenerators.Remove(providerName);
            }
        }

        /// <summary>
        ///     認証情報プロバイダーチェーンからすべてのエントリを削除
        /// </summary>
        public void Clear()
        {
            _credentialsGenerators.Clear();
        }
    }
}
