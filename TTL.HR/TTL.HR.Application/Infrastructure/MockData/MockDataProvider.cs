using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace TTL.HR.Application.Infrastructure.MockData;

/// <summary>
/// Provider để đọc và quản lý Mock Data từ file JSON.
/// Phiên bản này an toàn cho cả Server và WebAssembly (WASM).
/// </summary>
public class MockDataProvider
{
    private readonly ConcurrentDictionary<string, List<JToken>> _mockData = new();
    private bool _isLoaded = false;

    public MockDataProvider()
    {
    }

    /// <summary>
    /// Thêm dữ liệu collection từ chuỗi JSON
    /// </summary>
    public void AddCollection(string collectionName, string jsonContent)
    {
        try
        {
            var array = JArray.Parse(jsonContent);
            var list = new List<JToken>();
            foreach (var item in array)
            {
                list.Add(item.DeepClone());
            }
            _mockData[collectionName] = list;
            _isLoaded = true;
            Console.WriteLine($"✅ [MOCK] Đã nạp {collectionName}.json ({list.Count} items).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [MOCK] Lỗi khi nạp {collectionName}.json: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy toàn bộ dữ liệu từ một collection
    /// </summary>
    public List<T> GetCollection<T>(string collectionName)
    {
        if (!_mockData.ContainsKey(collectionName))
        {
            return new List<T>();
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
                Console.WriteLine($"⚠️  [MOCK] Lỗi deserialize item trong {collectionName}: {ex.Message}");
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy một item theo ID từ một collection
    /// </summary>
    public T? GetById<T>(string collectionName, string id) where T : class
    {
        var items = GetCollection<T>(collectionName);
        
        foreach (var item in items)
        {
            string? itemId = null;
            
            if (item is JObject jObj)
            {
                // Thử các trường ID phổ biến
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
                // Dùng reflection cho object có kiểu
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
    /// Đánh dấu đã hoàn thành việc nạp dữ liệu
    /// </summary>
    public void SetLoaded(bool loaded) => _isLoaded = loaded;

    /// <summary>
    /// Kiểm tra xem mock data đã được nạp chưa
    /// </summary>
    public bool IsLoaded => _isLoaded;
}
