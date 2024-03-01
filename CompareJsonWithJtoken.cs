using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main()
    {
        // Example JSON strings
        string json1 = "{\"id\": 1, \"name\": \"John\", \"details\": {\"age\": 25, \"city\": \"New York\"}, \"tags\": [\"tag1\", \"tag2\"]}";
        string json2 = "{\"id\": 2, \"name\": \"Jane\", \"details\": {\"age\": 25, \"city\": \"Los Angeles\"}, \"tags\": [\"tag1\", \"tag3\"]}";

        // Deserialize JSON strings into JToken
        JToken token1 = JToken.Parse(json1);
        JToken token2 = JToken.Parse(json2);

        // Compare and generate HTML table
        string htmlTable = GenerateHtmlTable(token1, token2);

        Console.WriteLine(htmlTable);

        Console.ReadLine();
    }

    static string GenerateHtmlTable(JToken token1, JToken token2)
    {
        StringBuilder html = new StringBuilder();

        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Property</th><th>Value in JSON 1</th><th>Value in JSON 2</th></tr>");

        CompareTokens(token1, token2, "", html);

        html.AppendLine("</table>");

        return html.ToString();
    }

    static void CompareTokens(JToken token1, JToken token2, string path, StringBuilder html)
    {
        if (JToken.DeepEquals(token1, token2))
        {
            return;
        }

        switch (token1.Type)
        {
            case JTokenType.Object:
                var obj1 = (JObject)token1;
                var obj2 = (JObject)token2;

                foreach (var property in obj1.Properties())
                {
                    var propertyPath = path + "." + property.Name;
                    if (obj2.TryGetValue(property.Name, out var value2))
                    {
                        CompareTokens(property.Value, value2, propertyPath, html);
                    }
                    else
                    {
                        AppendTableRow(propertyPath, property.Value?.ToString() ?? "null", "null", html);
                    }
                }

                foreach (var property in obj2.Properties())
                {
                    if (!obj1.ContainsKey(property.Name))
                    {
                        AppendTableRow(path + "." + property.Name, "null", property.Value?.ToString() ?? "null", html);
                    }
                }

                break;

            case JTokenType.Array:
                var array1 = (JArray)token1;
                var array2 = (JArray)token2;

                for (int i = 0; i < Math.Max(array1.Count, array2.Count); i++)
                {
                    var elementPath = path + $"[{i}]";
                    if (i < array1.Count && i < array2.Count)
                    {
                        CompareTokens(array1[i], array2[i], elementPath, html);
                    }
                    else if (i < array1.Count)
                    {
                        AppendTableRow(elementPath, array1[i]?.ToString() ?? "null", "null", html);
                    }
                    else
                    {
                        AppendTableRow(elementPath, "null", array2[i]?.ToString() ?? "null", html);
                    }
                }

                break;

            default:
                AppendTableRow(path, token1?.ToString() ?? "null", token2?.ToString() ?? "null", html);
                break;
        }
    }

    static void AppendTableRow(string property, string value1, string value2, StringBuilder html)
    {
        html.AppendLine($"<tr><td>{property}</td><td>{value1}</td><td>{value2}</td></tr>");
    }
}
