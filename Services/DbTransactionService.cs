using Microsoft.AspNetCore.Mvc;
using ResuMeAPI.Data;
using ResuMeAPI.Interfaces;
using ResuMeAPI.Utilities;

namespace ResuMeAPI.Helpers
{
    public class DbTransactionService : IDbTransactionService
    {
        private readonly ResuMeApiDbContext _context;

        public DbTransactionService(ResuMeApiDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ExecuteInTransactionAsync(Func<Task<IActionResult>> action)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = await action();

                await transaction.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Details = ExceptionHelper.AggregateExceptionMessages(ex),
#if DEBUG
                    StackTrace = ex.StackTrace
#else
                    StackTrace = null
#endif
                };

                return new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
