using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EventStore.ClientAPI;
using EventStoreTest.Code;
using Newtonsoft.Json;

namespace EventStoreTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            // connection string for event store
            var connectionString = "ConnectTo = tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";

            // Create the connection
            var connection = EventStoreConnection.Create(connectionString);
            // open connection and wait for it to connect
            connection.ConnectAsync().Wait();

            // create a test object o add to the stream
            dynamic exp = new ExpandoObject();
            exp.Text = "this is the text";
            exp.Description = "this is the description";

            // create a test event
            var evt = new Event("Test", exp);
            var streamEvent = new EventData(Guid.NewGuid(), evt.Type, true, evt.Payload, new byte[] { });

            // send the test event to event store
            connection.AppendToStreamAsync("TestStream", ExpectedVersion.Any, streamEvent).Wait();

            var currentSlice = connection.ReadStreamEventsForwardAsync("TestStream", 0, 100, true).Result;

            foreach (var resolvedEvent in currentSlice.Events.ToList())
            {
                var thisEvt = resolvedEvent.Event;
                var payloadAsString = Encoding.UTF8.GetString(thisEvt.Data);
                dynamic payload = (ExpandoObject)JsonConvert.DeserializeObject(payloadAsString, typeof(ExpandoObject));
            }

                return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}