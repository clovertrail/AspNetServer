// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(ChatRoom.Startup))]

namespace ChatRoom
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionCount = 15;
            var count = Environment.GetEnvironmentVariable("ConnectionCount");
            if (!String.IsNullOrEmpty(count))
            {
                if (int.TryParse(count, out int c))
                {
                    connectionCount = c;
                }
            }
            // Any connection or hub wire up and configuration should go here
            app.MapAzureSignalR(this.GetType().FullName, options=> {
                options.ConnectionCount = connectionCount;
            });
            //GlobalHost.TraceManager.Switch.Level = System.Diagnostics.SourceLevels.All;
        }
    }
}
