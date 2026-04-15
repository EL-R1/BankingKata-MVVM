using Microsoft.AspNetCore.Mvc;
using BankingKata_MVVM.Services;
using BankingKata_MVVM.ViewModels;

namespace BankingKata_MVVM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AccountViewModel>> GetAll()
    {
        return Ok(_accountService.GetAllAccounts());
    }

    [HttpGet("{accountNumber}")]
    public ActionResult<AccountViewModel> Get(string accountNumber)
    {
        var account = _accountService.GetAccount(accountNumber);
        if (account is null)
            return NotFound(new { message = $"Account {accountNumber} not found" });
        return Ok(account);
    }

    [HttpPost]
    public ActionResult<AccountViewModel> Create([FromBody] CreateAccountViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _accountService.CreateAccount(model);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _accountService.Deposit(accountNumber, model.Amount);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _accountService.Withdraw(accountNumber, model.Amount);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _accountService.SetOverdraft(accountNumber, model.OverdraftLimit);
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
            var statement = _accountService.GetStatement(accountNumber, fromDate, toDate);
            return Ok(statement);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
