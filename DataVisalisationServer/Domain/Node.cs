namespace FakeNewsBackend.DataVisualisation;

public class Node
{
    public int id;
    public string label;
    public int totalArticles;
    public int amountOfConnections;

    public void Increment()
    {
        amountOfConnections++;
    }
}