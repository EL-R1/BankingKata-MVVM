namespace BankingKata_MVVM.Repositories;

using System.Collections.Concurrent;
using BankingKata_MVVM.Models;

public interface IBankAccountRepository
{
    bool Exists(string accountNumber);
    BankAccount? GetByAccountNumber(string accountNumber);
    IEnumerable<BankAccount> GetAll();
    void Save(BankAccount account);
    void Update(BankAccount account);
    bool Delete(string accountNumber);
}

public class BankAccountRepository : IBankAccountRepository
{
    private readonly ConcurrentDictionary<string, BankAccount> _accounts = new();

    public bool Exists(string accountNumber) => _accounts.ContainsKey(accountNumber);

    public BankAccount? GetByAccountNumber(string accountNumber)
    {
        _accounts.TryGetValue(accountNumber, out var account);
        return account;
    }

    public IEnumerable<BankAccount> GetAll() => _accounts.Values;

    public void Save(BankAccount account)
    {
        _accounts[account.AccountNumber] = account;
    }

    public void Update(BankAccount account)
    {
        _accounts[account.AccountNumber] = account;
    }

    public bool Delete(string accountNumber)
    {
        return _accounts.TryRemove(accountNumber, out _);
    }
}

public interface ITransactionRepository
{
    void Save(Transaction transaction);
    IEnumerable<Transaction> GetByAccountNumberInRange(string accountNumber, DateTime fromDate, DateTime toDate);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly ConcurrentBag<Transaction> _transactions = new();
    private readonly object _lock = new();

    public void Save(Transaction transaction)
    {
        _transactions.Add(transaction);
    }

    public IEnumerable<Transaction> GetByAccountNumberInRange(string accountNumber, DateTime fromDate, DateTime toDate)
    {
        return _transactions
            .Where(t => t.AccountNumber == accountNumber && t.Date >= fromDate && t.Date <= toDate)
            .OrderByDescending(t => t.Date)
            .ToList();
    }
}

public interface ISavingsAccountRepository
{
    bool Exists(string accountNumber);
    SavingsAccount? GetByAccountNumber(string accountNumber);
    IEnumerable<SavingsAccount> GetAll();
    void Save(SavingsAccount account);
    void Update(SavingsAccount account);
    bool Delete(string accountNumber);
}

public class SavingsAccountRepository : ISavingsAccountRepository
{
    private readonly ConcurrentDictionary<string, SavingsAccount> _accounts = new();

    public bool Exists(string accountNumber) => _accounts.ContainsKey(accountNumber);

    public SavingsAccount? GetByAccountNumber(string accountNumber)
    {
        _accounts.TryGetValue(accountNumber, out var account);
        return account;
    }

    public IEnumerable<SavingsAccount> GetAll() => _accounts.Values;

    public void Save(SavingsAccount account)
    {
        _accounts[account.AccountNumber] = account;
    }

    public void Update(SavingsAccount account)
    {
        _accounts[account.AccountNumber] = account;
    }

    public bool Delete(string accountNumber)
    {
        return _accounts.TryRemove(accountNumber, out _);
    }
}