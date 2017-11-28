using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleApplication39
{

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
                newInput = JsonConvert.DeserializeObject<Input>(serialObject);
                newOutput = CreateOutput(newInput);

                var outputString = JsonConvert.SerializeObject(newOutput);
                Console.WriteLine(outputString.Replace(Environment.NewLine, "").Replace(" ", ""));
            }
            else if (Type == "xml")
            {
                XmlSerializer formatter = new XmlSerializer(typeof(Input));

                using (var reader = new StringReader(serialObject))
                {
                    newInput = (Input)formatter.Deserialize(reader);
                }

                newOutput = CreateOutput(newInput);

                formatter = new XmlSerializer(typeof(Output));
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, newOutput);

                    var outputString = Encoding.UTF8.GetString(stream.ToArray());
                    outputString = outputString.Remove(0, outputString.IndexOf('>') + 1);
                    outputString = outputString.Remove(outputString.IndexOf(' '), outputString.IndexOf('>', outputString.IndexOf(' ')) - outputString.IndexOf(' '))
                    .Replace(Environment.NewLine, string.Empty)
                    .Replace(" ", string.Empty);

                    Console.WriteLine(outputString);
                }
            }
        }

        static private Output CreateOutput(Input input)
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
