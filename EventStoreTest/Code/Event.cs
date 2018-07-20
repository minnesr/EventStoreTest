using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace EventStoreTest.Code
{
    public class Event
    {
        public string Type { get; }
        public byte[] Payload { get; }

        public Event(string type, byte[] payload = null)
        {
            if (payload == null)
                payload = Encoding.UTF8.GetBytes("{}");

            Type = type;
            Payload = payload;
        }

        public Event(string type, object payload)
        {
            Type = type;

            if (payload == null)
            {
                Payload = Encoding.UTF8.GetBytes("{}");
                return;
            }

            var payloadAsString = JsonConvert.SerializeObject(payload);
            Payload = Encoding.UTF8.GetBytes(payloadAsString);
        }

        public T GetPayloadAs<T>()
        {
            if (Payload == null)
                return default(T);

            var payloadAsString = Encoding.UTF8.GetString(Payload);
            return (T)JsonConvert.DeserializeObject(payloadAsString, typeof(T));
        }
    }
}