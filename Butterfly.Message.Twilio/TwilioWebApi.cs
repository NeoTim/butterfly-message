﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NLog;

using Butterfly.Util;
using Butterfly.Web.WebApi;

namespace Butterfly.Message.Twilio {

    public static class TwilioWebApi {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void Setup(IWebApi webApi, Func<string, string, string, string[], Task<string>> handler) {
            webApi.OnPost(
                "/api/twilio/webhook",
                async (req, res) => {
                    logger.Debug($"/api/twilio/webhook");
                    Dictionary<string, string> evt = await req.ParseAsUrlEncodedAsync();
                    logger.Debug($"/api/twilio/webhook,evt=" + JsonUtil.Serialize(evt));
                    string fromPhone = evt.GetAs("From", (string)null);
                    string toPhone = evt.GetAs("To", (string)null);
                    string body = evt.GetAs("Body", (string)null);
                    int numMedia = evt.GetAs("NumMedia", 0);
                    string[] mediaUrls = Enumerable.Range(1, numMedia).Select(x => evt.GetAs($"MediaUrl{x}", (string)null)).ToArray();
                    string responseText = await handler(fromPhone, toPhone, body, mediaUrls);
                    //logger.Debug($"/api/twilio/webhook,responseText={responseText}");
                    if (!string.IsNullOrEmpty(responseText)) {
                        await res.WriteAsTextAsync(responseText);
                    }
                }
            );
        }

    }

}

