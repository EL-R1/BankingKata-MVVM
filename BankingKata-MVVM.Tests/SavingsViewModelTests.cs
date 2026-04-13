using BankingKata_MVVM.ViewModels;
using BankingKata_MVVM.Repositories;
using Xunit;

namespace BankingKata_MVVM.Tests;

public class SavingsViewModelTests
{
    private readonly AccountsViewModel _viewModel;

    public SavingsViewModelTests()
    {
        _viewModel = new AccountsViewModel(
            new BankAccountRepository(),
            new TransactionRepository(),
            new SavingsAccountRepository());
    }

    [Fact]
    public void SavingsAccounts_IsEmpty_Initially()
    {
        Assert.Empty(_viewModel.SavingsAccounts);
    }

    [Fact]
    public void AddSavingsAccount_ValidAccount_AddsToList()
    {
        var model = new CreateSavingsAccountViewModel
        {
            AccountNumber = "SAV001",
            DepositCeiling = 10000,
            InitialBalance = 500
        };

        _viewModel.AddSavingsAccount(model);

        Assert.Single(_viewModel.SavingsAccounts);
        var account = _viewModel.SavingsAccounts[0];
        Assert.Equal("SAV001", account.AccountNumber);
        Assert.Equal(500, account.Balance);
        Assert.Equal(10000, account.DepositCeiling);
    }

    [Fact]
    public void AddSavingsAccount_DuplicateAccount_ThrowsException()
    {
        var model = new CreateSavingsAccountViewModel
        {
            AccountNumber = "SAV001",
            DepositCeiling = 10000
        };

        _viewModel.AddSavingsAccount(model);

        Assert.Throws<InvalidOperationException>(() => _viewModel.AddSavingsAccount(model));
    }

    [Fact]
    public void DepositSavings_ValidAmount_IncreasesBalance()
    {
        _viewModel.AddSavingsAccount(new CreateSavingsAccountViewModel { AccountNumber = "SAV001", DepositCeiling = 10000, InitialBalance = 100 });

        _viewModel.DepositSavings("SAV001", 50);

        var account = _viewModel.SavingsAccounts.First(a => a.AccountNumber == "SAV001");
        Assert.Equal(150, account.Balance);
    }

    [Fact]
    public void DepositSavings_ExceedsCeiling_ThrowsException()
    {
        _viewModel.AddSavingsAccount(new CreateSavingsAccountViewModel { AccountNumber = "SAV001", DepositCeiling = 100, InitialBalance = 50 });

        Assert.Throws<InvalidOperationException>(() => _viewModel.DepositSavings("SAV001", 60));
    }

    [Fact]
    public void WithdrawSavings_ValidAmount_DecreasesBalance()
    {
        _viewModel.AddSavingsAccount(new CreateSavingsAccountViewModel { AccountNumber = "SAV001", DepositCeiling = 10000, InitialBalance = 100 });

        _viewModel.WithdrawSavings("SAV001", 30);

        var account = _viewModel.SavingsAccounts.First(a => a.AccountNumber == "SAV001");
        Assert.Equal(70, account.Balance);
    }

    [Fact]
    public void WithdrawSavings_InsufficientFunds_ThrowsException()
    {
        _viewModel.AddSavingsAccount(new CreateSavingsAccountViewModel { AccountNumber = "SAV001", DepositCeiling = 10000, InitialBalance = 50 });

        Assert.Throws<InvalidOperationException>(() => _viewModel.WithdrawSavings("SAV001", 100));
    }

    [Fact]
    public void DepositSavings_NonExistingAccount_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => _viewModel.DepositSavings("NONEXISTENT", 50));
    }

    [Fact]
    public void WithdrawSavings_NonExistingAccount_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => _viewModel.WithdrawSavings("NONEXISTENT", 50));
    }
}