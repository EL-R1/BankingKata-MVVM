using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BankingKata_MVVM.ViewModels;

public class AccountViewModel : INotifyPropertyChanged
{
    private string _accountNumber = string.Empty;
    private decimal _balance;
    private decimal _overdraftLimit;

    [Required(ErrorMessage = "Account number is required")]
    [StringLength(20, MinimumLength = 1)]
    public string AccountNumber
    {
        get => _accountNumber;
        set { _accountNumber = value; OnPropertyChanged(); }
    }

    public decimal Balance
    {
        get => _balance;
        set { _balance = value; OnPropertyChanged(); }
    }

    public decimal OverdraftLimit
    {
        get => _overdraftLimit;
        set { _overdraftLimit = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class CreateAccountViewModel
{
    [Required(ErrorMessage = "Account number is required")]
    [StringLength(20, MinimumLength = 1)]
    public string AccountNumber { get; set; } = string.Empty;

    public decimal InitialBalance { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Overdraft limit cannot be negative")]
    public decimal OverdraftLimit { get; set; }
}

public class TransactionViewModel
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
    public decimal Amount { get; set; }
}

public class OverdraftViewModel
{
    [Range(0, double.MaxValue, ErrorMessage = "Overdraft limit cannot be negative")]
    public decimal OverdraftLimit { get; set; }
}

public class StatementViewModel : INotifyPropertyChanged
{
    private string _accountNumber = string.Empty;
    private string _accountType = string.Empty;
    private decimal _balance;
    private DateTime _statementDate;

    public string AccountNumber
    {
        get => _accountNumber;
        set { _accountNumber = value; OnPropertyChanged(); }
    }

    public string AccountType
    {
        get => _accountType;
        set { _accountType = value; OnPropertyChanged(); }
    }

    public decimal Balance
    {
        get => _balance;
        set { _balance = value; OnPropertyChanged(); }
    }

    public DateTime StatementDate
    {
        get => _statementDate;
        set { _statementDate = value; OnPropertyChanged(); }
    }

    public List<OperationViewModel> Transactions { get; set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class OperationViewModel : INotifyPropertyChanged
{
    private Guid _id;
    private string _accountNumber = string.Empty;
    private decimal _amount;
    private string _type = string.Empty;
    private DateTime _date;
    private decimal _balanceAfterTransaction;

    public Guid Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }

    public string AccountNumber
    {
        get => _accountNumber;
        set { _accountNumber = value; OnPropertyChanged(); }
    }

    public decimal Amount
    {
        get => _amount;
        set { _amount = value; OnPropertyChanged(); }
    }

    public string Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(); }
    }

    public DateTime Date
    {
        get => _date;
        set { _date = value; OnPropertyChanged(); }
    }

    public decimal BalanceAfterTransaction
    {
        get => _balanceAfterTransaction;
        set { _balanceAfterTransaction = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SavingsAccountViewModel : INotifyPropertyChanged
{
    private string _accountNumber = string.Empty;
    private decimal _balance;
    private decimal _depositCeiling;

    public string AccountNumber
    {
        get => _accountNumber;
        set { _accountNumber = value; OnPropertyChanged(); }
    }

    public decimal Balance
    {
        get => _balance;
        set { _balance = value; OnPropertyChanged(); }
    }

    public decimal DepositCeiling
    {
        get => _depositCeiling;
        set { _depositCeiling = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class CreateSavingsAccountViewModel
{
    [Required(ErrorMessage = "Account number is required")]
    [StringLength(20, MinimumLength = 1)]
    public string AccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Deposit ceiling is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Deposit ceiling must be positive")]
    public decimal DepositCeiling { get; set; }

    public decimal InitialBalance { get; set; }
}