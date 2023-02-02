using AntalyaTaksiAccount.Utils;

namespace AntalyaTaksiAccount.Services
{
    public class DriverNodeService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DriverNodeService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://5995-89-43-78-197.eu.ngrok.io");
            _configuration = configuration;
        }

        public async Task<bool> SendDriver(int driverId, int stationId)
        {
            AddJwtToken();

            DriverNode driverNode = new DriverNode
            {
                driverId = driverId,
                stationId = stationId
            };
            var sendDriverResult = await _httpClient.PostAsJsonAsync<DriverNode>("/drivers",driverNode);
            string message=await sendDriverResult.Content.ReadAsStringAsync();
            return sendDriverResult.IsSuccessStatusCode;
        }

        private void AddJwtToken()
        {
            JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
            string token=jwtTokenGenerator.Generate(0, "apiuser", "apiuser@apimail.com", 1);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        }

    }

    public class DriverNode
    {
        public int driverId { get; set; }
        public int stationId { get; set; }
    }

}
