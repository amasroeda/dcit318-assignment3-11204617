using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventorySystem
{
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll() => new(_log);

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                using var writer = new StreamWriter(_filePath);
                writer.Write(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"File not found: {_filePath}");
                    return;
                }
                using var reader = new StreamReader(_filePath);
                var json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                _log.Clear();
                if (items != null) _log.AddRange(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading file: {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Today));
            _logger.Add(new InventoryItem(2, "Smartphone", 25, DateTime.Today));
            _logger.Add(new InventoryItem(3, "Desk Chair", 15, DateTime.Today));
            _logger.Add(new InventoryItem(4, "Printer", 5, DateTime.Today));
            _logger.Add(new InventoryItem(5, "Monitor", 8, DateTime.Today));
        }

        public void SaveData() => _logger.SaveToFile();

        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:d}");
            }
        }

        public static void Main()
        {
            string filePath = "inventory.json";
            var app = new InventoryApp(filePath);

            app.SeedSampleData();
            app.SaveData();

            Console.WriteLine("\n--- New Session ---");
            var newApp = new InventoryApp(filePath);
            newApp.LoadData();
            newApp.PrintAllItems();
        }
    }
}
