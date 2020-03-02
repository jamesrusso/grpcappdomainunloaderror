using Common;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Cloud.PubSub.V1.SubscriberClient;

namespace Plugin
{
    public class Plugin : MarshalByRefObject, IPlugin
    {
        private SubscriberClient client;
        private Task clientTask;

        public async Task<Task> StartAsync()
        {
            var builder = new SubscriberServiceApiClientBuilder();
            var subscriber = builder.Build();
            var projectId = "second-impact-574";
            var subscriptionId = "test-subscription";
            var topicId = "sync_client_commands";

            TopicName topicName = new TopicName(projectId, topicId);
            SubscriptionName subscriptionName = new SubscriptionName(projectId, subscriptionId);
            try
            {
                Subscription subscription = subscriber.CreateSubscription(
                    subscriptionName, topicName, pushConfig: null,
                    ackDeadlineSeconds: 60);
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                Console.Out.WriteLine("subscription already exists...");
            }

            client = await SubscriberClient.CreateAsync(subscriptionName);
            return client.StartAsync(async (message, token) =>
               {
                   await Task.Delay(5 * 1000);
                   return Reply.Ack;
               });
        }

        public void Start()
        {
            Console.Out.WriteLine("Plugin: Start() called.");
            var startTask = StartAsync();
            Console.Out.WriteLine("Plugin: Waiting on StartAsync to complete.");
            startTask.Wait();
            Console.Out.WriteLine("Plugin: Start Task has completed.");
            clientTask = startTask.Result;
        }

        public void Stop()
        {
            Console.Out.WriteLine("Plugin: Stop() called.");
            var stopTask = StopAsync();
            Console.Out.WriteLine("Plugin: Waiting on StopAsync to complete.");
            stopTask.Wait();
            Console.Out.WriteLine("Plugin: StopTask has completed.");

        }

        private async Task StopAsync()
        {
            Console.Out.WriteLine("Plugin: calling client.StopAsync().");
            await client.StopAsync(CancellationToken.None);
            Console.Out.WriteLine("Plugin: awaiting clientTask.");
            await clientTask;
            Console.Out.WriteLine("Plugin: clientTask has completed.");

        }
    }
}
