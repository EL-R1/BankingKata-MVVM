using Microsoft.AspNetCore.Mvc;
using BankingKata_MVVM.Services;
using BankingKata_MVVM.ViewModels;

namespace BankingKata_MVVM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SavingsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public SavingsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SavingsAccountViewModel>> GetAll()
    {
        return Ok(_accountService.GetAllSavingsAccounts());
    }

    [HttpGet("{accountNumber}")]
    public ActionResult<SavingsAccountViewModel> Get(string accountNumber)
    {
        var account = _accountService.GetSavingsAccount(accountNumber);
        if (account is null)
            return NotFound(new { message = $"Savings account {accountNumber} not found" });
        return Ok(account);
    }

    [HttpPost]
    public ActionResult<SavingsAccountViewModel> Create([FromBody] CreateSavingsAccountViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _accountService.CreateSavingsAccount(model);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _accountService.DepositSavings(accountNumber, model.Amount);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _accountService.WithdrawSavings(accountNumber, model.Amount);
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
}
