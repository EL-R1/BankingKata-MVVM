using Microsoft.AspNetCore.Mvc;
using BankingKata_MVVM.ViewModels;

namespace BankingKata_MVVM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SavingsController : ControllerBase
{
    private readonly ViewModels.AccountsViewModel _viewModel;

    public SavingsController(ViewModels.AccountsViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SavingsAccountViewModel>> GetAll()
    {
        return Ok(_viewModel.SavingsAccounts);
    }

    [HttpGet("{accountNumber}")]
    public ActionResult<SavingsAccountViewModel> Get(string accountNumber)
    {
        var account = _viewModel.SavingsAccounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        if (account is null)
            return NotFound(new { message = $"Savings account {accountNumber} not found" });
        return Ok(account);
    }

    [HttpPost]
    public ActionResult<SavingsAccountViewModel> Create([FromBody] CreateSavingsAccountViewModel model)
    {
        try
        {
            _viewModel.AddSavingsAccount(model);
            var account = _viewModel.SavingsAccounts.Last();
            return CreatedAtAction(nameof(Get), new { accountNumber = account.AccountNumber }, account);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{accountNumber}/deposit")]
    public ActionResult<SavingsAccountViewModel> Deposit(string accountNumber, [FromBody] TransactionViewModel model)
    {
        try
        {
            _viewModel.DepositSavings(accountNumber, model.Amount);
            var account = _viewModel.SavingsAccounts.First(a => a.AccountNumber == accountNumber);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{accountNumber}/withdraw")]
    public ActionResult<SavingsAccountViewModel> Withdraw(string accountNumber, [FromBody] TransactionViewModel model)
    {
        try
        {
            _viewModel.WithdrawSavings(accountNumber, model.Amount);
            var account = _viewModel.SavingsAccounts.First(a => a.AccountNumber == accountNumber);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}