namespace FakeNewsBackend.DataVisualisation;

public class Node
{
    public int id;
    public string label;
    public int totalArticles;
    public int totalArticlesScraped;
    public int amountOfConnections;
    public int totalSimilarities;

    public void Increment()
    {
        amountOfConnections++;
    }
    public void AddSimilarities(int count)
    {
        totalSimilarities += count;
    }
}