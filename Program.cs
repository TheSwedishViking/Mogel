using TempData_grupparbete.Services;

namespace TempData_grupparbete
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReadFile.ReadAll();
            UI.StartMenu();
        }
    }
}
