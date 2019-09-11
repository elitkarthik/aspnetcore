using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace aspnetcore.Hubs
{
    public class AzureEventHubSynchronizer : Hub
    {
        public static StringBuilder dataFromEventHub = new StringBuilder();

        private const string eventHubConnectionString = "Endpoint=sb://saleseventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=t+Txas1x112LvFnFmmLYTGPWJ+LjhpU7aq1O2G7PZJ0=";
        private const string eventHubName = "weekendpromotion";

        private const string StorageContainerName = "eventhubcontainer";
        private const string StorageAccountName = "storagenodatalake";
        private const string StorageAccountKey = "QMOcE2wq4FRqlUrKHKecn3CJ6am3oqu+8p3EPSDDmQ59uqciZR/pu4CFDRIGC6EcGizV+QjnBv81v4L09W49gA==";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);
        public static bool WAIT_TILL_MESSAGES_ARE_DOWNLOADED = false;

        //This method will be called from UI (button click) through Chat.js (signalr), which will connect and retrieve Azure EventHub messages
        public async Task RetrieveMessagesFromAzure()
        {
            try
            {
                Console.WriteLine("Registering EventProcessor...");

                var eventProcessorHost = new EventProcessorHost(
                    eventHubName,
                    PartitionReceiver.DefaultConsumerGroupName,
                    eventHubConnectionString,
                    StorageConnectionString,
                    StorageContainerName);

                // Registers the Event Processor Host and starts receiving messages
                await eventProcessorHost.RegisterEventProcessorAsync<MyEventProcessor>();
                               
                //To make the current process to wait till the Async process is done downloading all messages from Azure Event Hub
                while (!WAIT_TILL_MESSAGES_ARE_DOWNLOADED)
                {
                    if (WAIT_TILL_MESSAGES_ARE_DOWNLOADED)
                        break;
                }

                //All data should have been populated to the List by now.  So Publish it to all subscribers.
                await Clients.All.SendAsync("ReceiveMessage", dataFromEventHub.ToString());
                
                // Disposes of the Event Processor Host
                await eventProcessorHost.UnregisterEventProcessorAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    } 

    public class MyEventProcessor : IEventProcessor
    {

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                                
                //add the list of messages received from Event Hub to the string builder object
                AzureEventHubSynchronizer.dataFromEventHub.AppendLine($"<li>Message received. Partition: '{context.PartitionId}', Data: '{data}'</li>");                
                Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");
            }

            AzureEventHubSynchronizer.WAIT_TILL_MESSAGES_ARE_DOWNLOADED = true;

            return context.CheckpointAsync();
        }
    }
}
