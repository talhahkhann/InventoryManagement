using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Helpers
{
    public static class ControllerHelper
    {
        // Gets controller name without "Controller" suffix
        public static string GetControllerName<T>() where T : Controller
        {
            return typeof(T).Name.Replace("Controller", "");
        }
    }
}
