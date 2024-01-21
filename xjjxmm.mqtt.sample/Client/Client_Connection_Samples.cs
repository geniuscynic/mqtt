// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using mqtt.client.test;
using mqtt.server.Options;
using xjjxmm.mqtt.Client;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.sample.Client;

public static class Client_Connection_Samples
{
    public static async Task Clean_Disconnect()
    {
        var mqttClient = new MqttClient();
        
        var mqttClientOptions = new ConnectOption("127.0.0.1", 1883, "testClientId");
        await mqttClient.Connect(mqttClientOptions);
    }

    public static async Task Connect_Client()
    {
        var mqttClient = new MqttClient();
        /*mqttClient.ConnAckAction = option =>
        {
            option.ToString().Dump();
        };*/
        
       

        
        var mqttClientOptions = new ConnectOption("127.0.0.1", 1883, "testClientId")
        {
            CleanSession = false
        };
        await mqttClient.Connect(mqttClientOptions);
    }
    
}