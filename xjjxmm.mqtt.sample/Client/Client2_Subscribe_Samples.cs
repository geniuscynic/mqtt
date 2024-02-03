// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Client;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.sample.Client;

public static class Client2_Subscribe_Samples
{
    public static async Task SubscribeQos0()
    {
        var mqttClient = new MqttClient2();
        
        var mqttClientOptions = new ConnectOption("127.0.0.1", 1883, "testClientId")
        {
            CleanSession = true
        };
        
        mqttClient.ReceiveMessage = option =>
        {
            option.Message.Dump();
            return Task.CompletedTask;
        };
        
        await mqttClient.Connect(mqttClientOptions);
        
        await mqttClient.Subscribe(new SubscribeOption("testTopic"));
    }
    
    public static async Task SubscribeQos1()
    {
        var mqttClient = new MqttClient2();
        
        var mqttClientOptions = new ConnectOption("127.0.0.1", 1883, "testClientId")
        {
            CleanSession = true
        };

        mqttClient.ReceiveMessage = option =>
        {
            option.Message.Dump();
            return Task.CompletedTask;
        };
        
        await mqttClient.Connect(mqttClientOptions);
        
        await mqttClient.Subscribe(new SubscribeOption("testTopic")
        {
            QoS = Qos.AtLeastOnce
        });
    }

    public static async Task SubscribeQos2()
    {
        var mqttClient = new MqttClient2();
        
        var mqttClientOptions = new ConnectOption("127.0.0.1", 1883, "testClientId")
        {
            CleanSession = true
        };

        mqttClient.ReceiveMessage = option =>
        {
            option.Message.Dump();
            return Task.CompletedTask;
        };
        
        await mqttClient.Connect(mqttClientOptions);
        
        var rep = await mqttClient.Subscribe(new SubscribeOption("testTopic")
        {
            QoS = Qos.ExactlyOnce
        });
        
        //rep.ReasonCodes.Dump();
    }
}