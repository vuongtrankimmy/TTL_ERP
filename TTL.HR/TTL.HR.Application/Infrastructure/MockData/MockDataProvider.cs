using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Concurrent;

namespace TTL.HR.Application.Infrastructure.MockData;

/// <summary>
/// Provider để đọc và quản lý Mock Data từ file JSON
/// </summary>
public class MockDataProvider
{
    private readonly IWebHostEnvironment _environment;
    private readonly ConcurrentDictionary<string, List<JToken>> _mockData = new();
    private readonly ConcurrentDictionary<string, object> _locks = new();
    private bool _isLoaded = false;

    public MockDataProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Load mock data từ file JSON
    /// </summary>
    public async Task<bool> LoadMockDataAsync()
    {
        if (_isLoaded) return true;

        var mockDataDir = Path.Combine(_environment.WebRootPath, "MockData");

        // Use Task.CompletedTask to satisfy async signature without overhead
        await Task.CompletedTask;

        if (!Directory.Exists(mockDataDir))
        {
            Console.WriteLine($"❌ [MOCK] Thư mục Mock Data không tồn tại: {mockDataDir}");
            Console.WriteLine($"   💡 Gợi ý: Hãy đảm bảo thư mục wwwroot/MockData tồn tại trong project Web.");
            return false;
        }

        var files = Directory.GetFiles(mockDataDir, "*.json");
        if (files.Length == 0)
        {
            Console.WriteLine($"⚠️  [MOCK] Thư mục {mockDataDir} trống. Không tìm thấy file .json nào.");
            return false;
        }

        _isLoaded = true;
        Console.WriteLine($"✅ [MOCK] Đã cấu hình Mock Data Mode ({files.Length} files found). Dữ liệu sẽ được load lazy khi cần.");
        return true;
    }

    /// <summary>
    /// Lấy dữ liệu từ collection
    /// </summary>
    public List<T> GetCollection<T>(string collectionName)
    {
        if (!_isLoaded)
        {
            throw new InvalidOperationException("Mock data chưa được cấu hình. Gọi LoadMockDataAsync() trước.");
        }

        if (!_mockData.ContainsKey(collectionName))
        {
            var collectionLock = _locks.GetOrAdd(collectionName, _ => new object());
            lock (collectionLock)
            {
                if (!_mockData.ContainsKey(collectionName))
                {
                    var mockDataPath = Path.Combine(_environment.WebRootPath, "MockData", $"{collectionName}.json");
                    if (!File.Exists(mockDataPath))
                    {
                        Console.WriteLine($"⚠️  File {collectionName}.json không tồn tại trong mock data");
                        _mockData[collectionName] = new List<JToken>();
                    }
                    else
                    {
                        try
                        {
                            var jsonContent = File.ReadAllText(mockDataPath);
                            var array = JArray.Parse(jsonContent);
                            var list = new List<JToken>();
                            foreach (var item in array)
                            {
                                list.Add(item.DeepClone());
                            }
                            _mockData[collectionName] = list;
                            Console.WriteLine($"✅ Lazy load {collectionName}.json thành công ({list.Count} items).");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi load {collectionName}.json: {ex.Message}");
                            _mockData[collectionName] = new List<JToken>();
                        }
                    }
                }
            }
        }

        var items = _mockData[collectionName];
        var result = new List<T>();

        foreach (var item in items)
        {
            try
            {
                var obj = item.ToObject<T>();
                if (obj != null)
                {
                    result.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Lỗi deserialize item trong {collectionName}: {ex.Message}");
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy item theo ID
    /// </summary>
    public T? GetById<T>(string collectionName, string id) where T : class
    {
        var items = GetCollection<T>(collectionName);
        
        foreach (var item in items)
        {
            string? itemId = null;
            
            if (item is JObject jObj)
            {
                // Try various ID fields for JObject
                if (jObj.TryGetValue("_id", out var idToken) || 
                    jObj.TryGetValue("id", out idToken) || 
                    jObj.TryGetValue("Id", out idToken))
                {
                    if (idToken.Type == JTokenType.Object && ((JObject)idToken).TryGetValue("$oid", out var oid))
                    {
                        itemId = oid.ToString();
                    }
                    else
                    {
                        itemId = idToken.ToString();
                    }
                }
            }
            else
            {
                // Use reflection for typed objects
                var idProperty = typeof(T).GetProperty("Id") ?? 
                                 typeof(T).GetProperty("id") ?? 
                                 typeof(T).GetProperty("_id");
                                 
                if (idProperty != null)
                {
                    itemId = idProperty.GetValue(item)?.ToString();
                }
            }

            if (itemId == id) return item;
        }

        return null;
    }

    /// <summary>
    /// Lấy item đầu tiên của collection (dùng cho các endpoint như /me)
    /// </summary>
    public T? GetFirst<T>(string collectionName) where T : class
    {
        var items = GetCollection<T>(collectionName);
        return items.FirstOrDefault();
    }

    /// <summary>
    /// Kiểm tra xem mock data đã được load chưa
    /// </summary>
    public bool IsLoaded => _isLoaded;
}
