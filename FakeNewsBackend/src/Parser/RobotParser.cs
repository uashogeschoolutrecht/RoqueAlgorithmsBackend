using FakeNewsBackend.Common.debug;
using FakeNewsBackend.Common.Types;

namespace FakeNewsBackend.Parser;

public class RobotParser
{
    private string[] RobotRuleLines;
    private RobotFields.Fields fields;

    /// <summary>
    /// Adds document.
    /// </summary>
    /// <param name="RobotFile">Robot file to add.</param>
    public void AddFile(string RobotFile)
    {
        if(RobotFile == "")
            return;
        RobotRuleLines = RobotFile.Split("\n");
        fields = new RobotFields.Fields();
        fields.fields = new List<RobotFields.FieldsForUserAgents>();
        fields.sitemaps = new List<string>();
    }
    
    /// <summary>
    /// Parses the Robots.txt file given in <see cref="AddFile"/>.
    /// </summary>
    public void ParseRobot()
    {
        var strct = new RobotFields.FieldsForUserAgents();
        foreach (var line in RobotRuleLines)
        {
            if(line.StartsWith("#") || line.StartsWith(" ") || line.StartsWith("\n") || string.IsNullOrWhiteSpace(line))
                continue;
            var splittedLine = line.Split(": ");
            var key = splittedLine[0];
            var value = splittedLine.Length > 1 ? 
                splittedLine[1].Replace("\r", "").Replace("\n", "") 
                : "";
            switch (key)
            {
                case "User-agent":
                case "User-Agent":
                    strct = new RobotFields.FieldsForUserAgents();
                    strct.AllowedRules = new List<string>();
                    strct.DisAllowedRules = new List<string>();
                    strct.UserAgent = value;
                    fields.fields.Add(strct);
                    break;
                case "Disallow":
                    strct.DisAllowedRules.Add(value);
                    break;
                case "Allow":
                    strct.AllowedRules.Add(value);
                    break;
                case "Sitemap":
                    fields.sitemaps.Add(value);
                    break;
                default:
                    break;
            }
        }
        fields.fields = fields.fields.Where(field => field.UserAgent == "*")
            .ToList();
    }

    /// <summary>
    /// Collects all links which are disallowed and returns them.
    /// </summary>
    /// <returns><see cref="List{T}"/> containing all the disallowed links.</returns>
    public List<string> GetDisAllowedRules()
    {
        return fields.fields.Aggregate(new List<string>(), (list, field) => list.Concat(field.DisAllowedRules)
                .ToList())
            .ToList();
    }

    /// <summary>
    /// Collects all links which are allowed and returns them.
    /// </summary>
    /// <returns><see cref="List{T}"/> containing all the allowed links.</returns>
    public List<string> GetAllowedRules()
    {
        return fields.fields.Aggregate(new List<string>(), (list, field) => list.Concat(field.AllowedRules)
                .ToList())
            .ToList();
    }
}