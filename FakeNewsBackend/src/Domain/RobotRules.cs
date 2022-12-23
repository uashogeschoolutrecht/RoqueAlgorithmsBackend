using System.ComponentModel.DataAnnotations;

namespace FakeNewsBackend.Domain;

public class RobotRules
{
    [Key] 
    public int WebsiteId { get; set; }
    public List<string> DisallowedLinks;
    public List<string> AllowedLinks;

    public RobotRules()
    {
        DisallowedLinks = new List<string>();
        AllowedLinks = new List<string>();
    }

    public void AddWebsiteId(int id)
    {
        WebsiteId = id;
    }
    public void AddDisAllowedLink(string link)
    {
        if(link.Equals("")) 
            return;
        DisallowedLinks.Add(link);
    }
    public void AddAllowedLink(string link)
    {
        if(link.Equals("")) 
            return;
        AllowedLinks.Add(link);
    }

    public bool IsLinkAllowed(string link)
    {
        // todo: the more specific rule overrules...
        var isAllowed = true;
        string? decidedLink = "";
        foreach (var disallowedLink in DisallowedLinks)
        {
            if (!link.Contains(disallowedLink))
                continue;
            isAllowed = false;
            decidedLink = disallowedLink;
            break;
        }
        if (isAllowed == true) return true;
        foreach (var allowedLink in AllowedLinks)
        {
            if (!link.Contains(allowedLink))
                continue;
            if(decidedLink.Length > allowedLink.Length)
                break;
            if (decidedLink.Length <= allowedLink.Length)
                return true;
        }
        return false;
    }

    public override string ToString()
    {
        var allowed = AllowedLinks.Count >= 1
            ? AllowedLinks.Aggregate((str, rule) => str += "\t" + rule + "\n")
            : "No specific rule";
        var disallowed = DisallowedLinks.Count >= 1
            ? DisallowedLinks.Aggregate((str, rule) => str += "\t" + rule + "\n")
            : "No specific rule";
        return $"Website id: {WebsiteId}\n" +
               $"Allowed Links: {allowed}\n" + 
               $"Disallowed Links: {disallowed}";
    }
}