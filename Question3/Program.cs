using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseInventory
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"[Electronic] ID: {Id}, {Name} (Brand: {Brand}, Warranty: {WarrantyMonths} mo) – Qty: {Quantity}";
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() => $"[Grocery] ID: {Id}, {Name} (Expires: {ExpiryDate:yyyy-MM-dd}) – Qty: {Quantity}";
    }

    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Cannot remove – item with ID {id} not found.");
        }

        public List<T> GetAllItems() => _items.Values.ToList();

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id); 
            item.Quantity = newQuantity;
        }
    }

    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(201, "Smartphone", 15, "TechNova", 24));
            _electronics.AddItem(new ElectronicItem(202, "Laptop", 8, "AeroBook", 12));
            _electronics.AddItem(new ElectronicItem(203, "Bluetooth Speaker", 25, "SoundMax", 18));

            _groceries.AddItem(new GroceryItem(101, "Rice (5kg)", 40, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(102, "Milk (1L)", 60, DateTime.Today.AddDays(14)));
            _groceries.AddItem(new GroceryItem(103, "Eggs (Tray)", 30, DateTime.Today.AddDays(10)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id); 
                if (quantity < 0)
                    throw new InvalidQuantityException("Increase amount cannot be negative.");
                repo.UpdateQuantity(id, checked(item.Quantity + quantity));
                Console.WriteLine($"Stock increased for ID {id}. New Qty: {repo.GetItemById(id).Quantity}");
            }
            catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException || ex is OverflowException)
            {
                Console.WriteLine($"[IncreaseStock Error] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"[Remove Error] {ex.Message}");
            }
        }

        public static void Main()
        {
            Console.WriteLine("=== Warehouse Inventory Management (Question 3) ===\n");
            var manager = new WareHouseManager();

            manager.SeedData();

            Console.WriteLine("-- Groceries --");
            manager.PrintAllItems(manager._groceries);
            Console.WriteLine();

            Console.WriteLine("-- Electronics --");
            manager.PrintAllItems(manager._electronics);
            Console.WriteLine();

            Console.WriteLine("-- Exception Scenarios --");

            try
            {
                manager._groceries.AddItem(new GroceryItem(101, "Rice (10kg)", 20, DateTime.Today.AddMonths(10)));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[Duplicate Add Error] {ex.Message}");
            }

            manager.RemoveItemById(manager._electronics, 999);

            try
            {
                manager._groceries.UpdateQuantity(102, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"[Update Error] {ex.Message}");
            }

            manager.IncreaseStock(manager._groceries, 103, 12);
            manager.IncreaseStock(manager._groceries, 404, 5);     
            manager.IncreaseStock(manager._electronics, 202, -3);  
        }
    }
}
