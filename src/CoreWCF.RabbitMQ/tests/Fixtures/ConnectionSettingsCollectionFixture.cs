// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CoreWCF.RabbitMQ.Tests;

public class ConnectionSettingsFixture
{
    private const string RabbitMqKey = "RABBITMQ";
    private const string StandardHostVariable = "STANDARD_HOST";
    private const string StandardHostPortVariable = "STANDARD_HOST_PORT";
    private const string StandardHostUserNameVariable = "STANDARD_HOST_USERNAME";
    private const string StandardHostPasswordVariable = "STANDARD_HOST_PASSWORD";
    private const string SecureUriVariable = "SECURE_URI";
    private const string SecureUriUserNameVariable = "SECURE_URI_USERNAME";
    private const string SecureUriPasswordVariable = "SECURE_URI_PASSWORD";

    public string StandardHost {get; }
    public int StandardHostPort {get; }
    public string UserNameForStandardHost { get; }
    public string PasswordForStandardHost { get; }
    public Uri SecureUri {get; }
    public string UserNameForSecureUri {get; }
    public string PasswordForSecureUri {get; }

    public ConnectionSettingsFixture()
    {
        var json = File.ReadAllText("appsettings.test.json");
        var appSettingsDictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

        var settingsDict = appSettingsDictionary[RabbitMqKey];
        StandardHost = Environment.GetEnvironmentVariable(StandardHostVariable) ?? settingsDict[StandardHostVariable].ToString();
        StandardHostPort = (int)(Environment.GetEnvironmentVariable(StandardHostPortVariable) ?? settingsDict[StandardHostPortVariable]);
        UserNameForStandardHost = Environment.GetEnvironmentVariable(StandardHostUserNameVariable) ?? settingsDict[StandardHostUserNameVariable].ToString();
        PasswordForStandardHost = Environment.GetEnvironmentVariable(StandardHostPasswordVariable) ?? settingsDict[StandardHostPasswordVariable].ToString();

        var secureUriString = Environment.GetEnvironmentVariable(SecureUriVariable) ?? settingsDict[SecureUriVariable].ToString();
        SecureUri = new Uri(secureUriString);
        UserNameForSecureUri = Environment.GetEnvironmentVariable(SecureUriUserNameVariable) ?? settingsDict[SecureUriUserNameVariable].ToString();
        PasswordForSecureUri = Environment.GetEnvironmentVariable(SecureUriPasswordVariable) ?? settingsDict[SecureUriPasswordVariable].ToString();
    }
}
