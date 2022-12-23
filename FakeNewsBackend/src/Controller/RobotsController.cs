using FakeNewsBackend.Domain;
using FakeNewsBackend.Parser;
using FakeNewsBackend.Service.Interface;
using NLog;

namespace FakeNewsBackend.Controller;

public class RobotsController
{
    private readonly HttpController _httpController;
    private readonly IRobotRulesService _robotRulesService;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public RobotsController(HttpController httpController, IRobotRulesService robotRulesService)
    {
        _httpController = httpController;
        _robotRulesService = robotRulesService;
    }

    /// <summary>
    /// Gets the Rules of the given website
    /// </summary>
    /// <param name="website">The <see cref="Website"/> to get the rules from.</param>
    /// <returns>A <see cref="Task"/> containing a <see cref="RobotRules"/> object.</returns>
    public async Task<RobotRules> GetRobotsRules(Website website)
    {
        try
        {
            if (_robotRulesService.ExistsWithWebsiteId(website.Id))
                return _robotRulesService.GetRulesOfWebsite(website);
            
            string roborstr;
            if(website.Url.EndsWith("/"))
                roborstr = website.Url + "robots.txt";
            else
                roborstr = website.Url + "/robots.txt";
            
            var result = await _httpController.MakeGetRequest(roborstr);
            if (!result.Response.IsSuccessStatusCode || 
                result.content.Length <= 0)
                return null;

            var rules = new RobotRules();

            var parser = new RobotParser();
            parser.AddFile(result.content);
            parser.ParseRobot();

            parser.GetAllowedRules()
                .ForEach(rule => rules.AddAllowedLink(rule));
            parser.GetDisAllowedRules()
                .ForEach(rule => rules.AddDisAllowedLink(rule));
            rules.WebsiteId = website.Id;

            _robotRulesService.Add(rules);
            
            return rules;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Could not parse robotrules.txt of: {Website}", website.Name);
        }
        return null;
    }
}