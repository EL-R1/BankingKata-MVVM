using Microsoft.AspNetCore.Mvc;
using BankingKata_MVVM.ViewModels;

namespace BankingKata_MVVM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountsViewModel _viewModel;

    public AccountsController()
    {
        _viewModel = new AccountsViewModel();
    }

    [HttpGet]
    public ActionResult<IEnumerable<AccountViewModel>> GetAll()
    {
        return Ok(_viewModel.Accounts);
    }

    [HttpGet("{accountNumber}")]
    public ActionResult<AccountViewModel> Get(string accountNumber)
    {
        var account = _viewModel.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        if (account is null)
            return NotFound(new { message = $"Account {accountNumber} not found" });
        return Ok(account);
    }

    [HttpPost]
    public ActionResult<AccountViewModel> Create([FromBody] CreateAccountViewModel model)
    {
        try
        {
            _viewModel.AddAccount(model);
            var account = _viewModel.Accounts.Last();
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
    public ActionResult<AccountViewModel> Deposit(string accountNumber, [FromBody] TransactionViewModel model)
    {
        try
        {
            _viewModel.Deposit(accountNumber, model.Amount);
            var account = _viewModel.Accounts.First(a => a.AccountNumber == accountNumber);
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
    public ActionResult<AccountViewModel> Withdraw(string accountNumber, [FromBody] TransactionViewModel model)
    {
        try
        {
            _viewModel.Withdraw(accountNumber, model.Amount);
            var account = _viewModel.Accounts.First(a => a.AccountNumber == accountNumber);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{accountNumber}/overdraft")]
    public ActionResult<AccountViewModel> SetOverdraft(string accountNumber, [FromBody] OverdraftViewModel model)
    {
        try
        {
            _viewModel.SetOverdraft(accountNumber, model.OverdraftLimit);
            var account = _viewModel.Accounts.First(a => a.AccountNumber == accountNumber);
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

    [HttpGet("{accountNumber}/statement")]
    public ActionResult<StatementViewModel> GetStatement(string accountNumber, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        try
        {
            var statement = _viewModel.GetStatement(accountNumber, fromDate, toDate);
            return Ok(statement);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}