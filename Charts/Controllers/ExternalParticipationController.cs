using System.Linq;
using System.Web;
using Charts.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Charts.Controllers
{
    public class ExternalParticipationController : ControllerBase
    {

        public static ScoreToAdd score = new (){GurvanScore = 0, NathanScore = 0};
        private static HashSet<string> _phoneNumberHashSet= new HashSet<string>();

        [HttpPost]
        [ActionName("/Reset")]
        public static void Reset()
        {
            _phoneNumberHashSet = new HashSet<string>();
        }

        [HttpPost]
        [ActionName("vote")]
        public static Task Vote([FromBody] HttpContext context)
        {
            var bodyStream = new StreamReader(context.Request.Body);
            var bodyText = bodyStream.ReadToEndAsync().GetAwaiter().GetResult();
            var form = JsonConvert.DeserializeObject<VoteForm>(bodyText);
            var name = form.Entry[0].Changes[0].Value.Messages[0].Text.Body;
            if (name is "1" or "2")
            {
                if (_phoneNumberHashSet.Contains(form.Entry[0].Changes[0].Value.Metadata.PhoneNumberId))
                    return Task.CompletedTask;

                _phoneNumberHashSet.Add(form.Entry[0].Changes[0].Value.Metadata.PhoneNumberId);
                switch (name)
                {
                    case "1":
                        score.NathanScore++;
                        break;

                    case "2":
                        score.GurvanScore++;
                        break;
                }
                return Task.CompletedTask;
            }
            return Task.CompletedTask;

        }

        [HttpGet]
        [ActionName("vote")]
        public static Task ValidateToken([FromBody] HttpContext context)
        {
            var query = context.Request.Query;

            if (!query.ContainsKey("hub.challenge") && query.ContainsKey("hub.verify_token"))
                return Task.CompletedTask;

            var form = new TokenRequest()
            {
                Challenge = query["hub.challenge"],
                Token = query["hub.verify_token"]
            };
            if (form?.Token == "AAAAA")
            {
                context.Response.WriteAsync(form.Challenge);
            }
            return Task.CompletedTask;
        }



    }

}
