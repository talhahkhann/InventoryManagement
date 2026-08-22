namespace InventoryManagement.ViewModels
{
    public class StockAlertViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int Threshold { get; set; }
        public string Severity { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    public class AlertNotificationViewModel
    {
        public int UnresolvedCount { get; set; }
        public List<StockAlertViewModel> RecentAlerts { get; set; } = new();
    }
}