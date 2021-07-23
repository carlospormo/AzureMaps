using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blazor.Extensions.AzureMaps
{
    public class MapMouseEvent
    {
        [JsonPropertyName("position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<double>? Position { get; set; }
    }
}