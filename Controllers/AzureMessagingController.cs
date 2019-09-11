using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;

using Microsoft.Azure.EventHubs;

namespace aspnetcore.Controllers
{
    public class AzureMessagingController : Controller
    {
        private static EventHubClient eventHubClient = null;
        private const string eventHubConnectionString = "Endpoint=sb://saleseventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=t+Txas1x112LvFnFmmLYTGPWJ+LjhpU7aq1O2G7PZJ0=";
        private const string eventHubName = "weekendpromotion";

        public IActionResult Index()
        {
            //load the empty view
            return View("EventHub");
        }

        [HttpPost]
        public void GenerateEvents()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubConnectionString)
            {   
                EntityPath = eventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            SendMessagesToEventHub(100).GetAwaiter().GetResult();

            eventHubClient.CloseAsync().GetAwaiter();

          //  ViewBag["Message"] = "Events Generated successfully";
        }

        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            for (var i =0; i<numMessagesToSend; i++)
            {
                try
                {
                    var message = $"MessaGe {i}";
                    Console.WriteLine($"Sending message: {message}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {ex.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }
    }
}