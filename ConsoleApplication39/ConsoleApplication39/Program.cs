using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.Collections.Concurrent;

namespace ConsoleApplication39
{

    interface ISerializer
         {
             string Serializer<T>(T obj);
             T Deserializer<T>(string src);
         }


    class Json : ISerializer
    {
        public string Serializer<T>(T obj)
        {
         
          return JsonConvert.SerializeObject(obj);
        }

        public T Deserializer<T>(string src)
        {
           
           return JsonConvert.DeserializeObject<T>(src);

        }
   }


    class Xml : ISerializer
    {

        private readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            OmitXmlDeclaration = true,
            Encoding = new UTF8Encoding(false),
            Indent = true,
        };
        private readonly ConcurrentDictionary<Type, System.Xml.Serialization.XmlSerializer> serializers =
            new ConcurrentDictionary<Type, System.Xml.Serialization.XmlSerializer>();
        private readonly XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
        public Xml()
        {
            xmlSerializerNamespaces.Add("", "");
        }
        public System.Xml.Serialization.XmlSerializer GetSerializer<T>()
        {
            return serializers.GetOrAdd(typeof(T),
                type => new System.Xml.Serialization.XmlSerializer(type, new XmlAttributeOverrides()));
        }

        public T Deserializer<T>(string src)
        {
            var xml = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var byteArray = Encoding.UTF8.GetBytes(src);

            using (var memoryStream = new MemoryStream(byteArray))
            {
                return (T)xml.Deserialize(memoryStream);
            }
        }

        public string Serializer<T>(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                GetSerializer<T>().Serialize(XmlWriter.Create(memoryStream, xmlWriterSettings),
                    obj, xmlSerializerNamespaces);

                var bytes = memoryStream.ToArray();
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }

public class JsonXmlStart
    {
      
        public JsonXmlStart(String serialType, String serialObject)
        {
            var newInput = new Input();
            var newOutput = new Output();
           
            JsonXml(serialType, serialObject, newInput, newOutput);
            Console.ReadKey();
        }

        private void JsonXml(String Type, String serialObject,Input newInput,Output newOutput)
        {
            if (Type == "json")
            {
                ISerializer serializer = new Json();
                newInput = serializer.Deserializer<Input>(serialObject);
                newOutput = CreateOutput(newInput);
                Console.WriteLine(serializer.Serializer(newOutput).Replace(Environment.NewLine, "").Replace(" ", ""));

            }
            else if (Type == "xml")
            {
                ISerializer serializer = new Xml();
                newInput = serializer.Deserializer<Input>(serialObject);
                newOutput = CreateOutput(newInput);
                Console.WriteLine(serializer.Serializer(newOutput).Replace(Environment.NewLine, "").Replace(" ", ""));
            }
        }

        static public Output CreateOutput(Input input)
        {
            var output = new Output();
            output.SumResult = input.Sums.Sum() * input.K;
            output.MulResult = input.Muls.Aggregate((p, x) => p *= x);

            var tmp = input.Sums.ToList();

            for (int i = 0; i < input.Muls.Length; i++)
            {
                tmp.Add(Convert.ToDecimal(input.Muls[i]));
            }

            tmp.Sort();
            output.SortedInputs = tmp.ToArray();

            return output;
        }

    }


    public class Output
    {
        public decimal SumResult { get; set; }
        public int MulResult { get; set; }
        public decimal[] SortedInputs { get; set; }
    }
    public class Input
    {
        public int K { get; set; }
        public decimal[] Sums { get; set; }
        public int[] Muls { get; set; }
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            var serialType = Console.ReadLine().ToLower();
            var serialObject = Console.ReadLine();
            var Start = new JsonXmlStart(serialType, serialObject);        
        }
    }


   
}
