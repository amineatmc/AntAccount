using AntalyaTaksiAccount.Utils;
using global::AntalyaTaksiAccount.Utils;
using System.Net.Http.Json;

namespace AntalyaTaksiAccount.Services
{
    
    namespace AntalyaTaksiAccount.Services
    {
        public class DriverNodeServiceOld
        {
            private readonly HttpClient _httpClient;
            private readonly IConfiguration _configuration;

            public DriverNodeServiceOld(HttpClient httpClient, IConfiguration configuration)
            {
                _httpClient = httpClient;
                _httpClient.BaseAddress = new Uri("https://antalyataksinode.azurewebsites.net");
                _configuration = configuration;
            }

            public async Task<bool> SendDriver(int driverId, int stationId, int allUserId)
            {
                AddJwtToken();

                DriverNode driverNode = new DriverNode
                {
                    driverId = driverId,
                    stationId = stationId,
                    allUserId = allUserId

                };
                var sendDriverResult = await _httpClient.PostAsJsonAsync<DriverNode>("/drivers", driverNode);
                string message = await sendDriverResult.Content.ReadAsStringAsync();
                return sendDriverResult.IsSuccessStatusCode;
            }

            public async Task<bool> SendPassenger(int passengerId, int allUserId)
            {
                AddJwtToken();

                UserNode userNode = new UserNode
                {
                    userId = passengerId,
                    allUserId = allUserId
                };
                var sendDriverResult = await _httpClient.PostAsJsonAsync<UserNode>("/users", userNode);
                string message = await sendDriverResult.Content.ReadAsStringAsync();
                return sendDriverResult.IsSuccessStatusCode;
            }

            private void AddJwtToken()
            {
                JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
                string token = jwtTokenGenerator.Generate(0, "apiuser", "apiuser@apimail.com", "", 0);
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            }

            public async Task<bool> SendStation(int stationID, string latitude, string longtitude, int allUserId)
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

            public async Task<bool> DeletePassenger(int userId)
            {
                AddJwtToken();
                var deletePassengerResult = await _httpClient.DeleteAsync("/users/" + userId);

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
            public int userId { get; set; }
            public int allUserId { get; set; }
        }

    }

}
