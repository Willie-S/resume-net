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

        /// <summary>
        /// Execute DB transactions with error handling and transaction-rollback upon failed executions
        /// </summary>
        /// <param name="action">Any DB-related functions that returns an IActionResult object</param>
        /// <returns>The IActionResult of the provided action</returns>
        public async Task<IActionResult> ExecuteInTransactionAsync(Func<Task<IActionResult>> action)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Execute the enclosed action
                var result = await action();

                // Commit any DB transactions
                await transaction.CommitAsync();

                // Return the result of the executed action upon successful DB transactions
                return result;
            }
            catch (Exception ex)
            {
                // Rollback all DB transactions
                await transaction.RollbackAsync();

                // Return the error
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
