using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MainClass
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task Main()
    {
        try
        {
            string s = await client.GetStringAsync("https://coderbyte.com/api/challenges/json/json-cleaning");
            Console.WriteLine("Raw JSON Data:");
            Console.WriteLine(s);

            JObject jsonObject = JObject.Parse(s);
            List<string> cleanedKeys = new List<string>();

            CleanJson(jsonObject, cleanedKeys);

            string cleanedJsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

            Console.WriteLine("\nCleaned JSON:");
            Console.WriteLine(cleanedJsonString);
            Console.WriteLine("List of cleaned keys:");
            cleanedKeys.ForEach(Console.WriteLine);

            string outputPath = "./cleaned_json_output.json";
            await File.WriteAllTextAsync(outputPath, cleanedJsonString);
            Console.WriteLine($"Cleaned JSON has been saved to: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred:");
            Console.WriteLine(ex.Message);
        }

        Console.Write("Press any key to exit...");
        Console.ReadKey();
    }

    private static void CleanJson(JToken token, List<string> cleanedKeys)
    {
        if (token.Type == JTokenType.Object)
        {
            JObject obj = (JObject)token;
            string[] unwantedValues = { "N/A", "-", "" };
            var properties = obj.Properties().ToList(); // Use Properties() and convert to List
            foreach (JProperty property in properties)
            {
                var propertyValue = Convert.ToString(property.Value); // Safely convert to string
                if (unwantedValues.Contains(propertyValue))
                {
                    cleanedKeys.Add(property.Path);
                    property.Remove();
                }
                else
                {
                    CleanJson(property.Value, cleanedKeys);
                }
            }
        }
        else if (token.Type == JTokenType.Array)
        {
            JArray array = (JArray)token;
            string[] unwantedValues = { "N/A", "-", "" };
            for (int i = array.Count - 1; i >= 0; i--)
            {
                var itemValue = Convert.ToString(array[i]); // Safely convert to string
                if (unwantedValues.Contains(itemValue))
                {
                    cleanedKeys.Add($"{array.Path}[{i}]");
                    array.RemoveAt(i);
                }
                else
                {
                    CleanJson(array[i], cleanedKeys);
                }
            }
        }
    }
}