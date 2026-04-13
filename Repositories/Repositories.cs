namespace BankingKata_MVVM.Repositories;

using BankingKata_MVVM.Models;

public interface IBankAccountRepository
{
    bool Exists(string accountNumber);
    BankAccount? GetByAccountNumber(string accountNumber);
    IEnumerable<BankAccount> GetAll();
    void Save(BankAccount account);
    void Update(BankAccount account);
}

public class BankAccountRepository : IBankAccountRepository
{
    private readonly Dictionary<string, BankAccount> _accounts = new();

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
}

public interface ITransactionRepository
{
    void Save(Transaction transaction);
    IEnumerable<Transaction> GetByAccountNumberInRange(string accountNumber, DateTime fromDate, DateTime toDate);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly List<Transaction> _transactions = new();

    public void Save(Transaction transaction)
    {
        _transactions.Add(transaction);
    }

    public IEnumerable<Transaction> GetByAccountNumberInRange(string accountNumber, DateTime fromDate, DateTime toDate)
    {
        return _transactions
            .Where(t => t.AccountNumber == accountNumber && t.Date >= fromDate && t.Date <= toDate)
            .OrderByDescending(t => t.Date);
    }
}

public interface ISavingsAccountRepository
{
    bool Exists(string accountNumber);
    SavingsAccount? GetByAccountNumber(string accountNumber);
    IEnumerable<SavingsAccount> GetAll();
    void Save(SavingsAccount account);
    void Update(SavingsAccount account);
}

public class SavingsAccountRepository : ISavingsAccountRepository
{
    private readonly Dictionary<string, SavingsAccount> _accounts = new();

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
}