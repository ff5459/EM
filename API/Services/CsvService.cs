using System.ComponentModel;
using System.Reflection;

namespace API.Services
{
    public static class CsvService
    {
        public static async Task WriteToStreamAsync<T>(Stream stream, IEnumerable<T> items)
        {
            await using var writer = new StreamWriter(stream, leaveOpen: true);

            var properties = typeof(T).GetProperties();

            await writer.WriteLineAsync(string.Join(',', properties.Select(p => p.Name)));

            foreach (var item in items)
            {
                string line = string.Join(',', properties.Select(p => p.GetValue(item)));
                await writer.WriteLineAsync(line);
            }
        }

        public static async Task<List<T>> ParseAsync<T>(Stream file, char separator) where T : new()
        {
            using var reader = new StreamReader(file);

            string? headers = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(headers))
                throw new InvalidDataException("No headers");

            string[] headerNames = headers.Split(separator);
            if (headerNames.Length == 0)
                throw new InvalidDataException("No headers");

            var properties = typeof(T).GetProperties();

            var nameToIndexMap = headerNames.Select((key, value) => (key, value))
                .ToDictionary(pair => pair.key, pair => pair.value);

            var propertyMap = new PropertyInfo[properties.Length];
            foreach (var property in properties)
            {
                if (nameToIndexMap.TryGetValue(property.Name, out int index))
                    propertyMap[index] = property;
            }

            var result = new List<T>();

            while (!reader.EndOfStream)
            {
                string? line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] values = line.Split(separator);
                if (values.Length != headerNames.Length)
                    throw new InvalidDataException($"Data has {values.Length} values, but header expects {headerNames.Length} columns");

                var item = new T();

                for (int i = 0; i < headerNames.Length; i++)
                {
                    var converter = TypeDescriptor.GetConverter(propertyMap[i].PropertyType);
                    try
                    {
                        object? convertedValue = converter.ConvertFromString(values[i]);
                        propertyMap[i].SetValue(item, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException($"Failed to convert '{values[i]}' to type '{propertyMap[i].PropertyType}'", ex);
                    }
                }

                result.Add(item);
            }

            return result;
        }
    }
}