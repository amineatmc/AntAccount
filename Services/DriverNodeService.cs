using AntalyaTaksiAccount.Utils;
using System.Net.Http.Json;

namespace AntalyaTaksiAccount.Services
{
    public class DriverNodeService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DriverNodeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://taxi7x24winfunctionappwin.azurewebsites.net/api");
            _configuration = configuration;
        }

        public async Task<bool> SendDriver(int driverId, int stationId,int allUserId)
        {
            AddJwtToken();

            DriverNode driverNode = new DriverNode
            {
                driverId = driverId,
                stationId = stationId,
                allUserId= allUserId

            };
            var sendDriverResult = await _httpClient.PostAsJsonAsync<DriverNode>("/SendDriver?code=hqCMNbhwWLqFsJafskQ2LOQfqi4kWhcQshq0_LKEuVJTAzFu2ePoVQ==", driverNode);
            string message = await sendDriverResult.Content.ReadAsStringAsync();
            return sendDriverResult.IsSuccessStatusCode;
        }

        public async Task<bool> SendPassenger(int passengerId,int allUserId)
        {
            AddJwtToken();

            UserNode userNode = new UserNode
            {
                passengerId= passengerId,
                allUserId=allUserId
            };
            var sendDriverResult = await _httpClient.PostAsJsonAsync<UserNode>("/SendPassenger?code=pEZpwil0NRiKULC0Y_D4Vf__D2OyQ3gahHj2Ik1mo4rAAzFuo3MmFA==", userNode);
            string message = await sendDriverResult.Content.ReadAsStringAsync();
            return sendDriverResult.IsSuccessStatusCode;
        }

        private void AddJwtToken()
        {
            JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
            string token = jwtTokenGenerator.Generate(0, "apiuser", "apiuser@apimail.com", "",0);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        }

        public async Task<bool> SendStation(int stationID, string latitude, string longtitude,int allUserId)
        {
            AddJwtToken(); 

            var latitudeAsDouble = Convert.ToDouble(latitude);
            var longtitudeAsDouble = Convert.ToDouble(longtitude);

           
            StationNode stationNode = new StationNode
            {
                latitude = latitudeAsDouble,
                longitude = longtitudeAsDouble,
                stationId = stationID,
                allUserId = allUserId

            };
            var sendDriverResult = await _httpClient.PostAsJsonAsync($"/SendStation?code=w3-4TUW1_d3AWTo3X9il4VNCHzXlc6G8RE_Y4lom9D6BAzFuCD2tYA==",stationNode);
            string message = await sendDriverResult.Content.ReadAsStringAsync();
            return sendDriverResult.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStation(int stationID)
        {
            AddJwtToken();
           
            var deleteDriverResult = await _httpClient.DeleteAsync("/DeleteStation?code=jFL8N9IzIUh4Oc1UgyltXj1NMsOF0t5PG24qG9fv9Ip_AzFujwnDgw==&stationID=" + stationID);

            string message = await deleteDriverResult.Content.ReadAsStringAsync();

            return deleteDriverResult.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDriver(int driverID)
        {
            AddJwtToken();
            var deleteDriverResult = await _httpClient.DeleteAsync("/DeleteDriver?code=8qZ_7X7GnU3wV5hRFMkTUFpXKp6QwYa1-xdyBB4em5XlAzFuSEWSsA==&driverID=" + driverID);

            string message = await deleteDriverResult.Content.ReadAsStringAsync();

            return deleteDriverResult.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePassenger(int userId)
        {
            AddJwtToken();
            var deletePassengerResult = await _httpClient.DeleteAsync("/DeletePassenger?code=CzfWF8VP4ksCcfMi_LMcObb9aM4OPbtTATTo1ZoZXQzVAzFuNL4rLA==&userID=" + userId);

            string message = await deletePassengerResult.Content.ReadAsStringAsync();

            return deletePassengerResult.IsSuccessStatusCode;
        }
    }

    public class DriverNode
    {
        public int driverId { get; set; }
        public int stationId { get; set; }
        public int allUserId { get; set; }
    }


    public class StationNode
    {
        public int stationId { get; set; }
        public int allUserId { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
    public class UserNode
    {
        public int passengerId { get; set; }
        public int allUserId { get; set; }
    }

}
