namespace ResuMeAPI.Utilities
{
    public static class ExceptionHelper
    {
        public static string AggregateExceptionMessages(Exception ex)
        {
            var messages = new List<string>();

            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }

            return string.Join(" -> ", messages);
        }
    }
}
