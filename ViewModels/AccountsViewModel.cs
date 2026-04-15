using System.Collections.ObjectModel;
using System.Windows.Input;
using BankingKata_MVVM.Commands;
using BankingKata_MVVM.Services;
using BankingKata_MVVM.ViewModels;

namespace BankingKata_MVVM.ViewModels;

public class AccountsViewModel
{
    private readonly IAccountService _accountService;

    public AccountsViewModel(IAccountService accountService)
    {
        _accountService = accountService;

        AddAccountCommand = new RelayCommand<CreateAccountViewModel>(AddAccount);
        DepositCommand = new RelayCommand<DepositCommandParameter>(Deposit);
        WithdrawCommand = new RelayCommand<WithdrawCommandParameter>(Withdraw);
        SetOverdraftCommand = new RelayCommand<SetOverdraftCommandParameter>(SetOverdraft);
        AddSavingsAccountCommand = new RelayCommand<CreateSavingsAccountViewModel>(AddSavingsAccount);
        DepositSavingsCommand = new RelayCommand<DepositCommandParameter>(DepositSavings);
        WithdrawSavingsCommand = new RelayCommand<WithdrawCommandParameter>(WithdrawSavings);
    }

    public ObservableCollection<AccountViewModel> Accounts { get; } = new();
    public ObservableCollection<SavingsAccountViewModel> SavingsAccounts { get; } = new();

    public ICommand AddAccountCommand { get; }
    public ICommand DepositCommand { get; }
    public ICommand WithdrawCommand { get; }
    public ICommand SetOverdraftCommand { get; }
    public ICommand AddSavingsAccountCommand { get; }
    public ICommand DepositSavingsCommand { get; }
    public ICommand WithdrawSavingsCommand { get; }

    private void AddAccount(CreateAccountViewModel? model)
    {
        if (model is null) return;

        var account = _accountService.CreateAccount(model);
        Accounts.Add(account);
    }

    private void Deposit(DepositCommandParameter? param)
    {
        if (param is null) return;

        var account = _accountService.Deposit(param.AccountNumber, param.Amount);
        var existing = Accounts.FirstOrDefault(a => a.AccountNumber == param.AccountNumber);
        if (existing is not null)
        {
            var index = Accounts.IndexOf(existing);
            Accounts[index] = account;
        }
    }

    private void Withdraw(WithdrawCommandParameter? param)
    {
        if (param is null) return;

        var account = _accountService.Withdraw(param.AccountNumber, param.Amount);
        var existing = Accounts.FirstOrDefault(a => a.AccountNumber == param.AccountNumber);
        if (existing is not null)
        {
            var index = Accounts.IndexOf(existing);
            Accounts[index] = account;
        }
    }

    private void SetOverdraft(SetOverdraftCommandParameter? param)
    {
        if (param is null) return;

        var account = _accountService.SetOverdraft(param.AccountNumber, param.Limit);
        var existing = Accounts.FirstOrDefault(a => a.AccountNumber == param.AccountNumber);
        if (existing is not null)
        {
            var index = Accounts.IndexOf(existing);
            Accounts[index] = account;
        }
    }

    private void AddSavingsAccount(CreateSavingsAccountViewModel? model)
    {
        if (model is null) return;

        var account = _accountService.CreateSavingsAccount(model);
        SavingsAccounts.Add(account);
    }

    private void DepositSavings(DepositCommandParameter? param)
    {
        if (param is null) return;

        var account = _accountService.DepositSavings(param.AccountNumber, param.Amount);
        var existing = SavingsAccounts.FirstOrDefault(a => a.AccountNumber == param.AccountNumber);
        if (existing is not null)
        {
            var index = SavingsAccounts.IndexOf(existing);
            SavingsAccounts[index] = account;
        }
    }

    private void WithdrawSavings(WithdrawCommandParameter? param)
    {
        if (param is null) return;

        var account = _accountService.WithdrawSavings(param.AccountNumber, param.Amount);
        var existing = SavingsAccounts.FirstOrDefault(a => a.AccountNumber == param.AccountNumber);
        if (existing is not null)
        {
            var index = SavingsAccounts.IndexOf(existing);
            SavingsAccounts[index] = account;
        }
    }

    public StatementViewModel GetStatement(string accountNumber, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return _accountService.GetStatement(accountNumber, fromDate, toDate);
    }

    public void LoadAccounts()
    {
        Accounts.Clear();
        foreach (var account in _accountService.GetAllAccounts())
        {
            Accounts.Add(account);
        }
    }
}

public class DepositCommandParameter
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class WithdrawCommandParameter
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class SetOverdraftCommandParameter
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Limit { get; set; }
}
