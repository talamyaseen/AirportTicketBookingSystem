using System.IO;
using System.Text.Json;

namespace AirportTicketBookingSystem.Storage
{
    public class JsonStorage<T> : IStorage<T>
    {
        private readonly string _fileName;
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        private readonly T _defaultValue;

        public JsonStorage(string fileName, T defaultValue)
        {
            _fileName = fileName;
            _defaultValue = defaultValue;
        }

        public T Load()
        {
            if (!File.Exists(_fileName)) return _defaultValue;
            var json = File.ReadAllText(_fileName);
            var data = JsonSerializer.Deserialize<T>(json);
            return data is null ? _defaultValue : data;
        }

        public void Save(T data)
        {
            var json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(_fileName, json);
        }
    }
}
