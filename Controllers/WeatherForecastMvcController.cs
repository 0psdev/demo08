using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace Demo08.Controllers
{
    public class WeatherForecastMvcController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public WeatherForecastMvcController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5001/WeatherForecast");

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DateOnlyJsonConverter() }
                };
                var forecasts = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStream, options);
                return View("~/Views/WeatherForecast/Index.cshtml", forecasts);
            }
            else
            {
                return View("~/Views/WeatherForecast/Index.cshtml", new List<WeatherForecast>());
            }
        }
    }

    public class DateOnlyJsonConverter : System.Text.Json.Serialization.JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.ParseExact(reader.GetString()!, Format);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}

