namespace HotelManagement.Data;

public class CassandraSettings
{
    public string SecureBundlePath { get; set; } = "secure-connect-hotel-management-demo.zip";
    public string ApplicationToken { get; set; } = string.Empty;
    public string Keyspace { get; set; } = "hotel_ks";
}
