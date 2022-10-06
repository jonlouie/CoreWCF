// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Net;
using RabbitMQ.Client;

namespace CoreWCF.Channels
{
    public class RabbitMqConnectionSettings
    {
        public const string TempQueuePrefix = "corewcf-temp-";
        private const string AMQPScheme = "amqp";
        private const string SecureAMQPScheme = "amqps";
        private const string DefaultVirtualHost = "/";

        public string Host { get; set; }
        public int Port { get; set; }
        public string Exchange { get; set; } = string.Empty;
        public string QueueName { get; set; }
        public string VirtualHost { get; set; } = DefaultVirtualHost;
        public virtual string RoutingKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public SslOption SslOption { get; set; }
        public bool AutomaticRecoveryEnabled => true;

        public static RabbitMqConnectionSettings FromUri(
            Uri uri,
            ICredentials credentials = null,
            SslOption sslOption = null,
            string virtualHost = DefaultVirtualHost)
        {
            sslOption = ConfigureSslOption(sslOption, uri);
            var queueName = GetQueueNameFromUri(uri);
            var exchange = GetExchangeFromUri(uri);
            var routingKey = GetRoutingKeyFromUri(uri, queueName);
            var userName = SetUserName(uri, credentials);
            var password = SetPassword(uri, credentials, sslOption);


            return new RabbitMqConnectionSettings
            {
                Host = uri.Host,
                Port = uri.Port,
                Exchange = exchange,
                RoutingKey = routingKey,
                QueueName = queueName,
                VirtualHost = virtualHost,
                UserName = userName,
                Password = password,
                SslOption = sslOption
            };
        }

        public ConnectionFactory GetConnectionFactory()
        {
            return new ConnectionFactory
            {
                HostName = Host,
                Port = Port,
                VirtualHost = VirtualHost,
                UserName = UserName,
                Password = Password,
                Ssl = SslOption,
                AutomaticRecoveryEnabled = AutomaticRecoveryEnabled
            };
        }

        private static string GetQueueNameFromUri(Uri uri)
        {
            if (uri.Segments.Length < 2)
            {
                throw new ArgumentException($"Expected a non-empty segment in the uri {uri}");
            }

            var lastSegment = uri.Segments.LastOrDefault();
            if (lastSegment.EndsWith("/"))
            {
                // Exchange found but no queueName, so create unique queueName
                return $"{TempQueuePrefix}{new Guid()}";
            }

            return lastSegment.Replace("/", string.Empty);
        }

        private static string GetExchangeFromUri(Uri uri)
        {
            if (uri.Segments.Length < 2)
            {
                throw new ArgumentException($"Expected a non-empty segment in the uri {uri}");
            }

            var secondSegment = uri.Segments[1];
            if (secondSegment.EndsWith("/"))
            {
                // Exchange name was found
                return secondSegment.Replace("/", string.Empty);
            }

            // Exchange name was not found
            return string.Empty;
        }

        private static string GetRoutingKeyFromUri(Uri uri, string queueName)
        {
            if (string.IsNullOrEmpty(uri.Fragment))
            {
                return queueName;
            }

            return uri.Fragment.Replace("#", string.Empty);
        }

        private static string SetUserName(Uri uri, ICredentials credentials)
        {
            var userNameFromCredentials = credentials?.GetCredential(uri, string.Empty).UserName;
            if (!string.IsNullOrEmpty(userNameFromCredentials))
            {
                return userNameFromCredentials;
            }

            return GetUserNameFromUri(uri);
        }

        private static string GetUserNameFromUri(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.UserInfo))
            {
                return ConnectionFactory.DefaultUser;
            }

            return uri.UserInfo.Split(':').First();
        }

        private static string SetPassword(Uri uri, ICredentials credentials, SslOption sslOption)
        {
            if (!string.IsNullOrEmpty(sslOption.CertPath) || sslOption.Certs?.Count > 0)
            {
                return string.Empty;
            }

            var passwordFromCredentials = credentials?.GetCredential(uri, string.Empty).Password;
            if (!string.IsNullOrEmpty(passwordFromCredentials))
            {
                return passwordFromCredentials;
            }

            return GetPasswordFromUri(uri);
        }

        private static string GetPasswordFromUri(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.UserInfo) || uri.UserInfo.Split(':').Length < 2)
            {
                return ConnectionFactory.DefaultPass;
            }

            return uri.UserInfo.Split(':').Last();
        }

        private static SslOption ConfigureSslOption(SslOption sslOption, Uri uri)
        {
            if (sslOption != null)
            {
                return sslOption;
            }

            if (uri.Scheme.Contains(SecureAMQPScheme))
            {
                return new SslOption { ServerName = uri.Host, Enabled = true };
            }

            return new SslOption();
        }
    }
}
