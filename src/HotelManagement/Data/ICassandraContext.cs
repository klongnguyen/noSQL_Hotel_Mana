using Cassandra;
using ISession = Cassandra.ISession;

namespace HotelManagement.Data;

public interface ICassandraContext : IDisposable
{
    ISession Session { get; }
    ICluster Cluster { get; }
    string Keyspace { get; }
    bool IsConnected { get; }
    string? ClusterReleaseVersion { get; }
}
