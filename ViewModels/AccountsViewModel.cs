using System.Collections.ObjectModel;

namespace BankingKata_MVVM.ViewModels;

public class AccountsViewModel
{
    private readonly Repositories.IBankAccountRepository _bankAccountRepo;
    private readonly Repositories.ITransactionRepository _transactionRepo;
    private readonly Repositories.ISavingsAccountRepository _savingsRepo;

    public AccountsViewModel(
        Repositories.IBankAccountRepository bankAccountRepo,
        Repositories.ITransactionRepository transactionRepo,
        Repositories.ISavingsAccountRepository savingsRepo)
    {
        _bankAccountRepo = bankAccountRepo;
        _transactionRepo = transactionRepo;
        _savingsRepo = savingsRepo;
    }

    public ObservableCollection<AccountViewModel> Accounts { get; } = new();
    public ObservableCollection<SavingsAccountViewModel> SavingsAccounts { get; } = new();

    public void AddAccount(CreateAccountViewModel model)
    {
        if (_bankAccountRepo.Exists(model.AccountNumber))
            throw new InvalidOperationException($"Account {model.AccountNumber} already exists");

        var account = new Models.BankAccount(model.AccountNumber, model.InitialBalance, model.OverdraftLimit);
        _bankAccountRepo.Save(account);

        Accounts.Add(new AccountViewModel
        {
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            OverdraftLimit = account.OverdraftLimit
        });
    }

    public void Deposit(string accountNumber, decimal amount)
    {
        var account = _bankAccountRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Account {accountNumber} not found");

        account.Deposit(amount);
        _bankAccountRepo.Update(account);

        RecordTransaction(accountNumber, amount, Models.TransactionType.Deposit, account.Balance);

        var vm = Accounts.First(a => a.AccountNumber == accountNumber);
        vm.Balance = account.Balance;
    }

    public void Withdraw(string accountNumber, decimal amount)
    {
        var account = _bankAccountRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Account {accountNumber} not found");

        account.Withdraw(amount);
        _bankAccountRepo.Update(account);

        RecordTransaction(accountNumber, amount, Models.TransactionType.Withdrawal, account.Balance);

        var vm = Accounts.First(a => a.AccountNumber == accountNumber);
        vm.Balance = account.Balance;
    }

    public void SetOverdraft(string accountNumber, decimal limit)
    {
        var account = _bankAccountRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Account {accountNumber} not found");

        account.SetOverdraftLimit(limit);
        _bankAccountRepo.Update(account);

        var vm = Accounts.First(a => a.AccountNumber == accountNumber);
        vm.OverdraftLimit = account.OverdraftLimit;
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

    public void AddSavingsAccount(CreateSavingsAccountViewModel model)
    {
        if (_savingsRepo.Exists(model.AccountNumber))
            throw new InvalidOperationException($"Savings account {model.AccountNumber} already exists");

        var account = new Models.SavingsAccount(model.AccountNumber, model.DepositCeiling, model.InitialBalance);
        _savingsRepo.Save(account);

        SavingsAccounts.Add(new SavingsAccountViewModel
        {
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            DepositCeiling = account.DepositCeiling
        });
    }

    public void DepositSavings(string accountNumber, decimal amount)
    {
        var account = _savingsRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Savings account {accountNumber} not found");

        account.Deposit(amount);
        _savingsRepo.Update(account);

        var vm = SavingsAccounts.First(a => a.AccountNumber == accountNumber);
        vm.Balance = account.Balance;
    }

    public void WithdrawSavings(string accountNumber, decimal amount)
    {
        var account = _savingsRepo.GetByAccountNumber(accountNumber)
            ?? throw new InvalidOperationException($"Savings account {accountNumber} not found");

        account.Withdraw(amount);
        _savingsRepo.Update(account);

        var vm = SavingsAccounts.First(a => a.AccountNumber == accountNumber);
        vm.Balance = account.Balance;
    }

    private void RecordTransaction(string accountNumber, decimal amount, Models.TransactionType type, decimal balanceAfter)
    {
        var transaction = new Models.Transaction(accountNumber, amount, type, balanceAfter);
        _transactionRepo.Save(transaction);
    }
}