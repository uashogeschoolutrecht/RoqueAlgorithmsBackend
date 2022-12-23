namespace FakeNewsBackend.Common.Types;

public class RobotFields
{
    public struct Fields
    {
        public List<FieldsForUserAgents> fields;  
        public List<string> sitemaps;
    }
    public struct FieldsForUserAgents
    {
        public string UserAgent;
        public List<string> DisAllowedRules;
        public List<string> AllowedRules;
    }
}