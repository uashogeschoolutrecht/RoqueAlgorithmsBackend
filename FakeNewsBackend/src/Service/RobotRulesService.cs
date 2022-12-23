using FakeNewsBackend.Context;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Service.Interface;

namespace FakeNewsBackend.Service;

public class RobotRulesService : IRobotRulesService
{
    public RobotRulesService() { }

    public RobotRules GetRulesOfWebsite(Website webSite)
    {
        using (var context = new RobotRulesContext())
        {
            return context.robotRules.ToList()
                .FirstOrDefault(rule => rule.WebsiteId == webSite.Id);
        }
    }

    public void Add(RobotRules rules)
    {
        using (var context = new RobotRulesContext())
        {
            context.robotRules.Add(rules);
            context.SaveChanges();

        }
    }

    public void Update(RobotRules rules)
    {
        using (var context = new RobotRulesContext())
        {
            context.robotRules.Update(rules);
            context.SaveChanges();
        }
    }

    public void Delete(RobotRules rules)
    {
        using (var context = new RobotRulesContext())
        {
            context.robotRules.Remove(rules);
            context.SaveChanges();
        }
    }
    
    public bool ExistsWithWebsiteId(int id)
    {
        using (var context = new RobotRulesContext())
        {
            return context.robotRules.Any(rule => rule.WebsiteId == id);
        }
    }
}