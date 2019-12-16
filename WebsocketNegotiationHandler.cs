using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ChatRoom
{
    public class WebsocketNegotiationHandler : OwinMiddleware
    {
        public WebsocketNegotiationHandler(OwinMiddleware next) : base(next)
        {
        }
        public override async Task Invoke(IOwinContext owinContext)
        {
            if (owinContext == null)
            {
                throw new ArgumentNullException(nameof(owinContext));
            }
            var context = new HostContext(owinContext.Environment);
            if (IsNegotiationRequest(context.Request))
            {
                var owinResponse = owinContext.Response;
                var owinResponseStream = owinResponse.Body;
                var responseBuffer = new MemoryStream();
                owinResponse.Body = responseBuffer;
                await Next.Invoke(owinContext);
                if (context.Response.StatusCode == 200)
                {
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(responseBuffer.ToArray()));
                    response["TryWebSockets"] = true;
                    var customResponseBody = new StringContent(JsonConvert.SerializeObject(response));
                    var customResponseStream = await customResponseBody.ReadAsStreamAsync();
                    await customResponseStream.CopyToAsync(owinResponseStream);
                    owinResponse.ContentType = "application/json";
                    owinResponse.ContentLength = customResponseStream.Length;
                    owinResponse.StatusCode = 200;
                    owinResponse.Body = owinResponseStream;
                }
            }
            else
            {
                await Next.Invoke(owinContext);
            }
        }

        private static bool IsNegotiationRequest(IRequest request)
        {
            return request.LocalPath.EndsWith("/negotiate", StringComparison.OrdinalIgnoreCase);
        }
    }
}