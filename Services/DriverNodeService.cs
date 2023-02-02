namespace AntalyaTaksiAccount.Services
{
    public class DriverNodeService
    {
        private readonly HttpClient _httpClient;

        public DriverNodeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://70bb-89-43-78-197.eu.ngrok.io");

        }

        public async Task<bool> SendDriver(int driverId, int stationId)
        {
            DriverNode driverNode = new DriverNode
            {
                driverId = driverId,
                stationId = stationId
            };
            var sendDriverResult = await _httpClient.PostAsJsonAsync<DriverNode>("/drivers",driverNode);
            return sendDriverResult.IsSuccessStatusCode;
        }

    }

    public class DriverNode
    {
        public int driverId { get; set; }
        public int stationId { get; set; }
    }

}
