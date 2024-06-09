using Microsoft.AspNetCore.Mvc;

namespace ResuMeAPI.Interfaces
{
    public interface IDbTransactionService
    {
        Task<IActionResult> ExecuteInTransactionAsync(Func<Task<IActionResult>> action);
    }
}
