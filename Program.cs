using TempData_grupparbete.Services;

namespace TempData_grupparbete
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ReadFile.ReadAll();
            await UI.StartMenu();
        }
    }
}
