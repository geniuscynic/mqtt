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
using xjjxmm.mqtt.Server;

namespace xjjxmm.mqtt.sample.Client;

public static class Server_Samples
{
    public static async Task ServerStart()
    {
        var server = new MqttServer();
        await server.Start();
    }
}