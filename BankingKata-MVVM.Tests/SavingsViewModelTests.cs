using BankingKata_MVVM.Services;
using BankingKata_MVVM.ViewModels;
using BankingKata_MVVM.Repositories;
using Xunit;

namespace BankingKata_MVVM.Tests;

public class SavingsViewModelTests
{
    private readonly AccountsViewModel _viewModel;
    private readonly IAccountService _accountService;

    public SavingsViewModelTests()
    {
        var bankRepo = new BankAccountRepository();
        var transactionRepo = new TransactionRepository();
        var savingsRepo = new SavingsAccountRepository();
        
        _accountService = new AccountService(bankRepo, transactionRepo, savingsRepo);
        _viewModel = new AccountsViewModel(_accountService);
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

        _viewModel.AddSavingsAccountCommand.Execute(model);

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

        _viewModel.AddSavingsAccountCommand.Execute(model);

        Assert.Throws<InvalidOperationException>(() => _viewModel.AddSavingsAccountCommand.Execute(model));
    }

    [Fact]
    public void DepositSavings_ValidAmount_IncreasesBalance()
    {
        _viewModel.AddSavingsAccountCommand.Execute(new CreateSavingsAccountViewModel { AccountNumber = "SAV001", DepositCeiling = 10000, InitialBalance = 100 });

        _viewModel.DepositSavingsCommand.Execute(new DepositCommandParameter { AccountNumber = "SAV001", Amount = 50 });

        var account = _viewModel.SavingsAccounts.First(a => a.AccountNumber == "SAV001");
        Assert.Equal(150, account.Balance);
    }

    [Fact]
    public void DepositSavings_ExceedsCeiling_ThrowsException()
    {
        _viewModel.AddSavingsAccountCommand.Execute(new CreateSavingsAccountViewModel { AccountNumber = "SAV001", DepositCeiling = 100, InitialBalance = 50 });

        Assert.Throws<InvalidOperationException>(() => 
            _viewModel.DepositSavingsCommand.Execute(new DepositCommandParameter { AccountNumber = "SAV001", Amount = 60 }));
    }

    [Fact]
    public void WithdrawSavings_ValidAmount_DecreasesBalance()
    {
        _viewModel.AddSavingsAccountCommand.Execute(new CreateSavingsAccountViewModel { AccountNumber = "SAV001", DepositCeiling = 10000, InitialBalance = 100 });

        _viewModel.WithdrawSavingsCommand.Execute(new WithdrawCommandParameter { AccountNumber = "SAV001", Amount = 30 });

        var account = _viewModel.SavingsAccounts.First(a => a.AccountNumber == "SAV001");
        Assert.Equal(70, account.Balance);
    }

    [Fact]
    public void WithdrawSavings_InsufficientFunds_ThrowsException()
    {
        _viewModel.AddSavingsAccountCommand.Execute(new CreateSavingsAccountViewModel { AccountNumber = "SAV001", DepositCeiling = 10000, InitialBalance = 50 });

        Assert.Throws<InvalidOperationException>(() => 
            _viewModel.WithdrawSavingsCommand.Execute(new WithdrawCommandParameter { AccountNumber = "SAV001", Amount = 100 }));
    }

    [Fact]
    public void DepositSavings_NonExistingAccount_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => 
            _viewModel.DepositSavingsCommand.Execute(new DepositCommandParameter { AccountNumber = "NONEXISTENT", Amount = 50 }));
    }

    [Fact]
    public void WithdrawSavings_NonExistingAccount_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => 
            _viewModel.WithdrawSavingsCommand.Execute(new WithdrawCommandParameter { AccountNumber = "NONEXISTENT", Amount = 50 }));
    }
}