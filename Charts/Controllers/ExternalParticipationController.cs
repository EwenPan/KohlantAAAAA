﻿using Charts.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Charts.Controllers
{
    public class ExternalParticipationController : ControllerBase
    {

        public static Score score = new() { GurvanScore = 0, NathanScore = 0 };
        private static Dictionary<string, HashSet<string>> _registrationByFunction = new() { { "1", new HashSet<string>() { "stan", "ewen" } }, { "vote", new HashSet<string>() } };
        public static string TextToAdd;
        [HttpPost]
        [ActionName("/resetVote")]
        public static void ResetVote()
        {
            _registrationByFunction["vote"] = new HashSet<string>();
            score = new Score();
        }

        [HttpPost]
        [ActionName("resetContest")]
        public static void ResetContest()
        {
            var votes = _registrationByFunction["vote"];
            _registrationByFunction = new Dictionary<string, HashSet<string>>();
            _registrationByFunction["vote"] = votes;
        }

        [HttpPost]
        [ActionName("vote")]
        public static Task Vote([FromBody] HttpContext context)
        {
            var bodyStream = new StreamReader(context.Request.Body);
            var bodyText = bodyStream.ReadToEndAsync().GetAwaiter().GetResult();
            TextToAdd += bodyText;
            var form = JsonConvert.DeserializeObject<VoteForm>(bodyText);
            var body = form.Entry[0].Changes[0].Value.Messages[0].Text.Body;
            var num = form.Entry[0].Changes[0].Value.Messages[0].From;
            var stringArray = body.Split(" ");
            if (stringArray.Length != 2)
                return Task.CompletedTask;
            var (function, argument) = (stringArray[0].ToLower(), stringArray[1].ToLower());
            switch (function)
            {
                case "vote":
                    AddVote(argument, num);
                    break;
                case "concours":
                    AddContestParticipation(function, num);
                    break;
            }
            return Task.CompletedTask;
        }

        private static void AddContestParticipation(string function, string num)
        {
            if (_registrationByFunction.ContainsKey(function))
            {
                _registrationByFunction[function].Add(num);
            }
        }

        public static void AddVote(string vote, string num)
        {
            if (_registrationByFunction[vote].Contains(num))
                return;
            if (vote is not ("1" or "2"))
                return;

            switch (vote)
            {
                case "1":
                    score.NathanScore++;
                    break;
                case "2":
                    score.GurvanScore++;
                    break;
            }
        }

        [HttpGet]
        [ActionName("getWinner")]
        public static Task GetWinner([FromBody] HttpContext context)
        {
            var query = context.Request.Query;

            if (!query.ContainsKey("contest"))
                return Task.CompletedTask;
            if (!_registrationByFunction.ContainsKey(query["contest"]))
                return Task.CompletedTask;

            var players = _registrationByFunction[query["contest"]];
            Random random = new Random();
            string winner = players.ElementAt(random.Next(players.Count));
            context.Response.WriteAsync(winner);
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
