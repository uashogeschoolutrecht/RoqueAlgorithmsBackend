using System.ComponentModel.DataAnnotations;

namespace FakeNewsBackend.Domain;

public class WebsiteProgress
{
    [Key]
    public int WebsiteId { get; set; }
    public string CurrentlyWorkingOn { get; set; }
    public int NumberOfLinkInSiteMap { get; set; }
    public bool IsDone { get; set; }
    
}