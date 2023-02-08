using AntalyaTaksiAccount.Utils;

namespace AntalyaTaksiAccount.Services
{
    public class DriverNodeService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DriverNodeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://3f74-89-43-78-197.eu.ngrok.io");
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
            var sendDriverResult = await _httpClient.PostAsJsonAsync<DriverNode>("/drivers", driverNode);
            string message = await sendDriverResult.Content.ReadAsStringAsync();
            return sendDriverResult.IsSuccessStatusCode;
        }

        public async Task<bool> SendPassenger(int passengerId)
        {
            AddJwtToken();

            UsersNode driverNode = new DriverNode
            {
                driverId = driverId,
                stationId = stationId
            };
            var sendDriverResult = await _httpClient.PostAsJsonAsync<DriverNode>("/drivers", driverNode);
            string message = await sendDriverResult.Content.ReadAsStringAsync();
            return sendDriverResult.IsSuccessStatusCode;
        }

        private void AddJwtToken()
        {
            JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
            string token = jwtTokenGenerator.Generate(0, "apiuser", "apiuser@apimail.com", "");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        }

        public async Task<bool> SendStation(int stationID, string latitude, string longtitude)
        {
            double latitudeAsDouble = Convert.ToDouble(latitude);
            double longtitudeAsDouble = Convert.ToDouble(longtitude);

            AddJwtToken();
            StationNode stationNode = new StationNode
            {
                latitude = latitudeAsDouble,
                longitude = longtitudeAsDouble,
                stationId = stationID

            };
            var sendDriverResult = await _httpClient.PostAsJsonAsync("/stations", stationNode);
            string message = await sendDriverResult.Content.ReadAsStringAsync();
            return sendDriverResult.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStation(int stationID)
        {
            AddJwtToken();
            var deleteDriverResult = await _httpClient.DeleteAsync("/stations/" + stationID);

            string message = await deleteDriverResult.Content.ReadAsStringAsync();

            return deleteDriverResult.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDriver(int driverID)
        {
            AddJwtToken();
            var deleteDriverResult = await _httpClient.DeleteAsync("/drivers/" + driverID);

            string message = await deleteDriverResult.Content.ReadAsStringAsync();

            return deleteDriverResult.IsSuccessStatusCode;
        }

    }

    public class DriverNode
    {
        public int driverId { get; set; }
        public int stationId { get; set; }
    }


    public class StationNode
    {
        public int stationId { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
    public class UserNode
    {
        public int userId { get; set; }
    }

}
