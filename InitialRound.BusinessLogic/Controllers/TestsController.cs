///
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace InitialRound.BusinessLogic.Controllers
{
    public static class TestsController
    {
        public class Test
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public string Input { get; set; }
            public string ExpectedOutput { get; set; }
        }

        public static Test[] FromJson(string json)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(Test[]));
                return (Test[])serializer.ReadObject(stream);
            }
        }

        public static string ToJson(Test[] tests)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(Test[]));
                serializer.WriteObject(stream, tests);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
