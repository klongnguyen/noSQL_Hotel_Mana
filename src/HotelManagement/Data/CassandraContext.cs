using Cassandra;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ISession = Cassandra.ISession;

namespace HotelManagement.Data;

public class CassandraContext : ICassandraContext
{
    private readonly ICluster _cluster = null!;
    private readonly ISession _session = null!;
    private readonly ILogger<CassandraContext> _logger;
    private readonly string _keyspace;
    private bool _disposed;
    private string? _releaseVersion;

    public ISession Session => _session;
    public ICluster Cluster => _cluster;
    public string Keyspace => _keyspace;
    public bool IsConnected => _session != null && !_session.IsDisposed;
    public string? ClusterReleaseVersion => _releaseVersion;

    public CassandraContext(IOptions<CassandraSettings> settingsOptions, ILogger<CassandraContext> logger)
    {
        _logger = logger;
        var settings = settingsOptions.Value;

        var bundlePath = ResolveBundlePath(settings.SecureBundlePath);
        var token = !string.IsNullOrWhiteSpace(settings.ApplicationToken) 
            ? settings.ApplicationToken 
            : Environment.GetEnvironmentVariable("ASTRA_DB_APPLICATION_TOKEN") ?? string.Empty;

        _keyspace = !string.IsNullOrWhiteSpace(settings.Keyspace) 
            ? settings.Keyspace 
            : Environment.GetEnvironmentVariable("ASTRA_DB_KEYSPACE") ?? "hotel_ks";

        if (string.IsNullOrWhiteSpace(bundlePath) || !File.Exists(bundlePath))
        {
            throw new FileNotFoundException(
                $"Không tìm thấy file Astra DB Secure Connect Bundle! Đã tìm tại: {bundlePath}. Hãy kiểm tra cấu hình.");
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "Chưa cấu hình Astra DB Application Token! Hãy thêm vào appsettings.Development.json hoặc biến môi trường.");
        }

        _logger.LogInformation("Đang khởi tạo kết nối Cassandra Astra DB... Bundle: {BundlePath}, Keyspace: {Keyspace}", bundlePath, _keyspace);

        int maxRetries = 3;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                _cluster = Cassandra.Cluster.Builder()
                    .WithCloudSecureConnectionBundle(bundlePath)
                    .WithCredentials("token", token)
                    .WithSocketOptions(new SocketOptions()
                        .SetConnectTimeoutMillis(30000)
                        .SetReadTimeoutMillis(30000))
                    .Build();

                _session = _cluster.Connect(_keyspace);

                // Kiểm tra version Cassandra
                try
                {
                    var row = _session.Execute("SELECT release_version FROM system.local").FirstOrDefault();
                    _releaseVersion = row?.GetValue<string>("release_version") ?? "Unknown";
                    _logger.LogInformation("Kết nối Astra DB thành công! Cassandra Version: {ReleaseVersion}", _releaseVersion);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Kết nối thành công nhưng không lấy được release_version từ system.local");
                }

                break;
            }
            catch (Exception ex)
            {
                if (attempt < maxRetries)
                {
                    _logger.LogWarning(ex, "Thử kết nối lại Astra DB (Lần {Attempt}/{MaxRetries}) sau lỗi tạm thời...", attempt, maxRetries);
                    Thread.Sleep(2000);
                }
                else
                {
                    _logger.LogError(ex, "Lỗi nghiêm trọng khi kết nối tới DataStax Astra DB sau {MaxRetries} lần thử.", maxRetries);
                    throw;
                }
            }
        }
    }

    private static string ResolveBundlePath(string configuredPath)
    {
        if (File.Exists(configuredPath))
        {
            return Path.GetFullPath(configuredPath);
        }

        var candidates = new List<string>
        {
            Path.Combine(Directory.GetCurrentDirectory(), configuredPath),
            Path.Combine(AppContext.BaseDirectory, configuredPath),
            Path.Combine(Directory.GetCurrentDirectory(), "..", configuredPath),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", configuredPath),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", configuredPath)
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                return Path.GetFullPath(candidate);
            }
        }

        // Tìm kiếm file secure-connect-*.zip ở thư mục hiện tại và các thư mục cha
        var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (currentDir != null)
        {
            var zips = currentDir.GetFiles("secure-connect-*.zip");
            if (zips.Length > 0)
            {
                return zips[0].FullName;
            }
            currentDir = currentDir.Parent;
        }

        return configuredPath;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            _session?.Dispose();
            _cluster?.Dispose();
            _logger.LogInformation("Đã đóng kết nối Cassandra Session và Cluster.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đóng kết nối Cassandra.");
        }

        GC.SuppressFinalize(this);
    }
}
