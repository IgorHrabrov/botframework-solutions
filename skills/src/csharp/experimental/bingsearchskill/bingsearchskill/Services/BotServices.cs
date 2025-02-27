﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Solutions;
using Microsoft.Bot.Builder.Solutions.TaskExtensions;

namespace BingSearchSkill.Services
{
    public class BotServices
    {
        public BotServices()
        {
        }

        public BotServices(BotSettings settings, IBotTelemetryClient client)
        {
            foreach (var pair in settings.CognitiveModels)
            {
                var set = new CognitiveModelSet();
                var language = pair.Key;
                var config = pair.Value;

                var telemetryClient = client;

                var luisOptions = new LuisPredictionOptions()
                {
                    TelemetryClient = telemetryClient,
                    LogPersonalInformation = true,
                };

                if (config.DispatchModel != null)
                {
                    var dispatchApp = new LuisApplication(config.DispatchModel.AppId, config.DispatchModel.SubscriptionKey, config.DispatchModel.GetEndpoint());
                    set.DispatchService = new LuisRecognizer(dispatchApp, luisOptions);
                }

                if (config.LanguageModels != null)
                {
                    foreach (var model in config.LanguageModels)
                    {
                        var luisApp = new LuisApplication(model.AppId, model.SubscriptionKey, model.GetEndpoint());
                        set.LuisServices.Add(model.Id, new LuisRecognizer(luisApp, luisOptions));
                    }
                }

                if (config.Knowledgebases != null)
                {
                    foreach (var kb in config.Knowledgebases)
                    {
                        var qnaEndpoint = new QnAMakerEndpoint()
                        {
                            KnowledgeBaseId = kb.KbId,
                            EndpointKey = kb.EndpointKey,
                            Host = kb.Hostname,
                        };
                        var qnaMaker = new QnAMaker(qnaEndpoint);
                        set.QnAServices.Add(kb.Id, qnaMaker);
                    }
                }

                CognitiveModelSets.Add(language, set);
            }
        }

        public Dictionary<string, CognitiveModelSet> CognitiveModelSets { get; set; } = new Dictionary<string, CognitiveModelSet>();
    }
}