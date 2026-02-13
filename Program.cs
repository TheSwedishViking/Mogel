using TempData_grupparbete.Services;

namespace TempData_grupparbete
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Writer.Delete("baddata.txt");
            ReadFile.ReadAll();
            await UI.StartMenu();

        }
    }
}
