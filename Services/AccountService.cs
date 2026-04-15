using BankingKata_MVVM.Models;
using BankingKata_MVVM.Repositories;
using BankingKata_MVVM.ViewModels;

namespace BankingKata_MVVM.Services;

public interface IAccountService
{
    AccountViewModel CreateAccount(CreateAccountViewModel model);
    AccountViewModel? GetAccount(string accountNumber);
    IEnumerable<AccountViewModel> GetAllAccounts();
    AccountViewModel Deposit(string accountNumber, decimal amount);
    AccountViewModel Withdraw(string accountNumber, decimal amount);
    AccountViewModel SetOverdraft(string accountNumber, decimal limit);
    StatementViewModel GetStatement(string accountNumber, DateTime? fromDate = null, DateTime? toDate = null);
    SavingsAccountViewModel CreateSavingsAccount(CreateSavingsAccountViewModel model);
    SavingsAccountViewModel? GetSavingsAccount(string accountNumber);
    IEnumerable<SavingsAccountViewModel> GetAllSavingsAccounts();
    SavingsAccountViewModel DepositSavings(string accountNumber, decimal amount);
    SavingsAccountViewModel WithdrawSavings(string accountNumber, decimal amount);
}

public class AccountService : IAccountService
{
    private readonly IBankAccountRepository _bankAccountRepo;
    private readonly ITransactionRepository _transactionRepo;
    private readonly ISavingsAccountRepository _savingsRepo;

    public AccountService(
        IBankAccountRepository bankAccountRepo,
        ITransactionRepository transactionRepo,
        ISavingsAccountRepository savingsRepo)
    {
        _bankAccountRepo = bankAccountRepo;
        _transactionRepo = transactionRepo;
        _savingsRepo = savingsRepo;
    }

    public AccountViewModel CreateAccount(CreateAccountViewModel model)
    {
        if (_bankAccountRepo.Exists(model.AccountNumber))
            throw new InvalidOperationException($"Account {model.AccountNumber} already exists");

        var account = new BankAccount(model.AccountNumber, model.InitialBalance, model.OverdraftLimit);
        _bankAccountRepo.Save(account);

        return MapToViewModel(account);
    }

    public AccountViewModel? GetAccount(string accountNumber)
    {
        var account = _bankAccountRepo.GetByAccountNumber(accountNumber);
        return account is null ? null : MapToViewModel(account);
    }

    public IEnumerable<AccountViewModel> GetAllAccounts()
    {
        return _bankAccountRepo.GetAll().Select(MapToViewModel);
    }

    public AccountViewModel Deposit(string accountNumber, decimal amount)
    {
        var account = _bankAccountRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Account {accountNumber} not found");

        account.Deposit(amount);
        _bankAccountRepo.Update(account);

        RecordTransaction(accountNumber, amount, TransactionType.Deposit, account.Balance);

        return MapToViewModel(account);
    }

    public AccountViewModel Withdraw(string accountNumber, decimal amount)
    {
        var account = _bankAccountRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Account {accountNumber} not found");

        account.Withdraw(amount);
        _bankAccountRepo.Update(account);

        RecordTransaction(accountNumber, amount, TransactionType.Withdrawal, account.Balance);

        return MapToViewModel(account);
    }

    public AccountViewModel SetOverdraft(string accountNumber, decimal limit)
    {
        var account = _bankAccountRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Account {accountNumber} not found");

        account.SetOverdraftLimit(limit);
        _bankAccountRepo.Update(account);

        return MapToViewModel(account);
    }

    public StatementViewModel GetStatement(string accountNumber, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var account = _bankAccountRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Account {accountNumber} not found");

        var to = toDate ?? DateTime.UtcNow;
        var from = fromDate ?? to.AddMonths(-1);

        var transactions = _transactionRepo.GetByAccountNumberInRange(accountNumber, from, to);

        return new StatementViewModel
        {
            AccountNumber = account.AccountNumber,
            AccountType = "Compte Courant",
            Balance = account.Balance,
            StatementDate = to,
            Transactions = transactions.Select(t => new OperationViewModel
            {
                Id = t.Id,
                AccountNumber = t.AccountNumber,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Date = t.Date,
                BalanceAfterTransaction = t.BalanceAfterTransaction
            }).ToList()
        };
    }

    public SavingsAccountViewModel CreateSavingsAccount(CreateSavingsAccountViewModel model)
    {
        if (_savingsRepo.Exists(model.AccountNumber))
            throw new InvalidOperationException($"Savings account {model.AccountNumber} already exists");

        var account = new SavingsAccount(model.AccountNumber, model.DepositCeiling, model.InitialBalance);
        _savingsRepo.Save(account);

        return MapToSavingsViewModel(account);
    }

    public SavingsAccountViewModel? GetSavingsAccount(string accountNumber)
    {
        var account = _savingsRepo.GetByAccountNumber(accountNumber);
        return account is null ? null : MapToSavingsViewModel(account);
    }

    public IEnumerable<SavingsAccountViewModel> GetAllSavingsAccounts()
    {
        return _savingsRepo.GetAll().Select(MapToSavingsViewModel);
    }

    public SavingsAccountViewModel DepositSavings(string accountNumber, decimal amount)
    {
        var account = _savingsRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Savings account {accountNumber} not found");

        account.Deposit(amount);
        _savingsRepo.Update(account);

        return MapToSavingsViewModel(account);
    }

    public SavingsAccountViewModel WithdrawSavings(string accountNumber, decimal amount)
    {
        var account = _savingsRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Savings account {accountNumber} not found");

        account.Withdraw(amount);
        _savingsRepo.Update(account);

        return MapToSavingsViewModel(account);
    }

    private void RecordTransaction(string accountNumber, decimal amount, TransactionType type, decimal balanceAfter)
    {
        var transaction = new Transaction(accountNumber, amount, type, balanceAfter);
        _transactionRepo.Save(transaction);
    }

    private static AccountViewModel MapToViewModel(BankAccount account)
    {
        return new AccountViewModel
        {
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            OverdraftLimit = account.OverdraftLimit
        };
    }

    private static SavingsAccountViewModel MapToSavingsViewModel(SavingsAccount account)
    {
        return new SavingsAccountViewModel
        {
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            DepositCeiling = account.DepositCeiling
        };
    }
}
