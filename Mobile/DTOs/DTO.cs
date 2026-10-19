using System.Text.Json.Serialization;

namespace Mobile.DTOs
{
    public class ResponseApi<T>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("msg")]
        public string msg { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class PageData<T>
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("rows")]
        public IEnumerable<T> Rows { get; set; }
    }

}
