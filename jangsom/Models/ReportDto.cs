namespace jangsom.Models;

public class CreateReportDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageBeforeUrl { get; set; }
    public Guid LocationId { get; set; }
    public Guid? AssetId { get; set; }
}

public class ReportResponseDto
{
    public Guid ReportId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageBeforeUrl { get; set; }
    public Guid LocationId { get; set; }
    public Guid? AssetId { get; set; }
    public Guid ReporterId { get; set; }
    public Guid? TechnicianId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string LocationName { get; set; }
}