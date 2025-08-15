// Question 1 - Finance Management System
using System;
using System.Collections.Generic;

namespace FinanceManagement
{
    public readonly record struct Transaction(
        int Id,
        DateTime Date,
        decimal Amount,
        string Category
    );

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public sealed class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine(
                $"[Bank Transfer] Processing {transaction.Amount:C} for '{transaction.Category}' dated {transaction.Date:d}.");
        }
    }

    public sealed class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine(
                $"[Mobile Money] Processing {transaction.Amount:C} for '{transaction.Category}' dated {transaction.Date:d}.");
        }
    }

    public sealed class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine(
                $"[Crypto Wallet] Processing {transaction.Amount:C} for '{transaction.Category}' dated {transaction.Date:d}.");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required", nameof(accountNumber));
            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");

            AccountNumber = accountNumber;
            Balance = initialBalance;
        }
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied: -{transaction.Amount:C}. New balance: {Balance:C}.");
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"SavingsAccount updated. Deducted {transaction.Amount:C}. Balance: {Balance:C}.");
        }
    }

    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            Console.WriteLine("=== Finance Management System (Question 1) ===\n");

            var account = new SavingsAccount(accountNumber: "SA-001", initialBalance: 1000m);
            Console.WriteLine($"Created SavingsAccount {account.AccountNumber} with balance {account.Balance:C}.\n");

            var t1 = new Transaction(1, DateTime.Today, 150.75m, "Groceries");
            var t2 = new Transaction(2, DateTime.Today, 230.00m, "Utilities");
            var t3 = new Transaction(3, DateTime.Today, 900.00m, "Entertainment");

            ITransactionProcessor p1 = new MobileMoneyProcessor();   // -> Transaction 1
            ITransactionProcessor p2 = new BankTransferProcessor();  // -> Transaction 2
            ITransactionProcessor p3 = new CryptoWalletProcessor();  // -> Transaction 3

            p1.Process(t1);
            p2.Process(t2);
            p3.Process(t3);
            Console.WriteLine();

            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3); 
            Console.WriteLine();


            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("--- Transaction Summary ---");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($"#{tx.Id}: {tx.Category} on {tx.Date:d} -> {tx.Amount:C}");
            }

            Console.WriteLine($"\nFinal Balance for {account.AccountNumber}: {account.Balance:C}");
        }

        public static void Main()
        {
            new FinanceApp().Run();
        }
    }
}
