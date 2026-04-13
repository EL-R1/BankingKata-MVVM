using BankingKata_MVVM.ViewModels;
using BankingKata_MVVM.Repositories;
using Xunit;

namespace BankingKata_MVVM.Tests;

public class AccountsViewModelTests
{
    private readonly AccountsViewModel _viewModel;

    public AccountsViewModelTests()
    {
        _viewModel = new AccountsViewModel(
            new BankAccountRepository(),
            new TransactionRepository(),
            new SavingsAccountRepository());
    }

    [Fact]
    public void Accounts_IsEmpty_Initially()
    {
        Assert.Empty(_viewModel.Accounts);
    }

    [Fact]
    public void AddAccount_ValidAccount_AddsToAccounts()
    {
        var model = new CreateAccountViewModel
        {
            AccountNumber = "ACC001",
            InitialBalance = 1000,
            OverdraftLimit = 500
        };

        _viewModel.AddAccount(model);

        Assert.Single(_viewModel.Accounts);
        var account = _viewModel.Accounts[0];
        Assert.Equal("ACC001", account.AccountNumber);
        Assert.Equal(1000, account.Balance);
        Assert.Equal(500, account.OverdraftLimit);
    }

    [Fact]
    public void AddAccount_DuplicateAccount_ThrowsException()
    {
        var model = new CreateAccountViewModel
        {
            AccountNumber = "ACC001",
            InitialBalance = 1000
        };

        _viewModel.AddAccount(model);

        Assert.Throws<InvalidOperationException>(() => _viewModel.AddAccount(model));
    }

    [Fact]
    public void Deposit_ValidAmount_IncreasesBalance()
    {
        _viewModel.AddAccount(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100 });

        _viewModel.Deposit("ACC001", 50);

        var account = _viewModel.Accounts.First(a => a.AccountNumber == "ACC001");
        Assert.Equal(150, account.Balance);
    }

    [Fact]
    public void Deposit_NonExistingAccount_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => _viewModel.Deposit("NONEXISTENT", 50));
    }

    [Fact]
    public void Withdraw_ValidAmount_DecreasesBalance()
    {
        _viewModel.AddAccount(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100 });

        _viewModel.Withdraw("ACC001", 30);

        var account = _viewModel.Accounts.First(a => a.AccountNumber == "ACC001");
        Assert.Equal(70, account.Balance);
    }

    [Fact]
    public void Withdraw_InsufficientFunds_ThrowsException()
    {
        _viewModel.AddAccount(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100, OverdraftLimit = 0 });

        Assert.Throws<InvalidOperationException>(() => _viewModel.Withdraw("ACC001", 150));
    }

    [Fact]
    public void SetOverdraft_ValidLimit_UpdatesOverdraft()
    {
        _viewModel.AddAccount(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100, OverdraftLimit = 0 });

        _viewModel.SetOverdraft("ACC001", 200);

        var account = _viewModel.Accounts.First(a => a.AccountNumber == "ACC001");
        Assert.Equal(200, account.OverdraftLimit);
    }

    [Fact]
    public void GetStatement_ExistingAccount_ReturnsStatement()
    {
        _viewModel.AddAccount(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100 });
        _viewModel.Deposit("ACC001", 50);

        var statement = _viewModel.GetStatement("ACC001");

        Assert.Equal("ACC001", statement.AccountNumber);
        Assert.Equal(150, statement.Balance);
        Assert.Single(statement.Transactions);
    }

    [Fact]
    public void GetStatement_NonExistingAccount_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => _viewModel.GetStatement("NONEXISTENT"));
    }
}