using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class AuditLogFilterViewModel
    {
        [Display(Name = "Entity Type")]
        public string? EntityType { get; set; }

        [Display(Name = "User")]
        public string? PerformedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "From")]
        public DateTime? From { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "To")]
        public DateTime? To { get; set; }

        public int Page     { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class AuditLogRowViewModel
    {
        public int      Id          { get; set; }
        public string   PerformedBy { get; set; } = string.Empty;
        public string   Role        { get; set; } = string.Empty;
        public string   EntityType  { get; set; } = string.Empty;
        public string?  EntityId    { get; set; }
        public string   Action      { get; set; } = string.Empty;
        public string?  Details     { get; set; }
        public DateTime Timestamp   { get; set; }
    }

    public class AuditLogPageViewModel
    {
        public AuditLogFilterViewModel   Filter     { get; set; } = new();
        public List<AuditLogRowViewModel> Items     { get; set; } = new();
        public int                        TotalCount { get; set; }
        public int                        TotalPages => (int)Math.Ceiling((double)TotalCount / Filter.PageSize);

        // Dropdown options
        public List<string> EntityTypes { get; set; } = new();
        public List<string> Users       { get; set; } = new();
    }
}
