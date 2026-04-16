using BankingKata_MVVM.Services;
using BankingKata_MVVM.ViewModels;
using BankingKata_MVVM.Repositories;
using Xunit;

namespace BankingKata_MVVM.Tests;

public class AccountsViewModelTests
{
    private readonly AccountsViewModel _viewModel;
    private readonly IAccountService _accountService;

    public AccountsViewModelTests()
    {
        var bankRepo = new BankAccountRepository();
        var transactionRepo = new TransactionRepository();
        var savingsRepo = new SavingsAccountRepository();
        
        _accountService = new AccountService(bankRepo, transactionRepo, savingsRepo);
        _viewModel = new AccountsViewModel(_accountService);
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

        _viewModel.AddAccountCommand.Execute(model);

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

        _viewModel.AddAccountCommand.Execute(model);

        Assert.Throws<InvalidOperationException>(() => _viewModel.AddAccountCommand.Execute(model));
    }

    [Fact]
    public void Deposit_ValidAmount_IncreasesBalance()
    {
        _viewModel.AddAccountCommand.Execute(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100 });

        _viewModel.DepositCommand.Execute(new DepositCommandParameter { AccountNumber = "ACC001", Amount = 50 });

        var account = _viewModel.Accounts.First(a => a.AccountNumber == "ACC001");
        Assert.Equal(150, account.Balance);
    }

    [Fact]
    public void Deposit_NonExistingAccount_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => 
            _viewModel.DepositCommand.Execute(new DepositCommandParameter { AccountNumber = "NONEXISTENT", Amount = 50 }));
    }

    [Fact]
    public void Withdraw_ValidAmount_DecreasesBalance()
    {
        _viewModel.AddAccountCommand.Execute(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100 });

        _viewModel.WithdrawCommand.Execute(new WithdrawCommandParameter { AccountNumber = "ACC001", Amount = 30 });

        var account = _viewModel.Accounts.First(a => a.AccountNumber == "ACC001");
        Assert.Equal(70, account.Balance);
    }

    [Fact]
    public void Withdraw_InsufficientFunds_ThrowsException()
    {
        _viewModel.AddAccountCommand.Execute(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100, OverdraftLimit = 0 });

        Assert.Throws<InvalidOperationException>(() => 
            _viewModel.WithdrawCommand.Execute(new WithdrawCommandParameter { AccountNumber = "ACC001", Amount = 150 }));
    }

    [Fact]
    public void SetOverdraft_ValidLimit_UpdatesOverdraft()
    {
        _viewModel.AddAccountCommand.Execute(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100, OverdraftLimit = 0 });

        _viewModel.SetOverdraftCommand.Execute(new SetOverdraftCommandParameter { AccountNumber = "ACC001", Limit = 200 });

        var account = _viewModel.Accounts.First(a => a.AccountNumber == "ACC001");
        Assert.Equal(200, account.OverdraftLimit);
    }

    [Fact]
    public void GetStatement_ExistingAccount_ReturnsStatement()
    {
        _viewModel.AddAccountCommand.Execute(new CreateAccountViewModel { AccountNumber = "ACC001", InitialBalance = 100 });
        _viewModel.DepositCommand.Execute(new DepositCommandParameter { AccountNumber = "ACC001", Amount = 50 });

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