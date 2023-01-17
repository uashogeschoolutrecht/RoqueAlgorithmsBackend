using FakeNewsBackend.Common.Types;

namespace FakeNewsBackend.Domain;

public class Similarity
{
    public int OriginalWebsiteId { get; set; }
    public int FoundWebsiteId { get; set; }
    public string UrlToOriginalArticle { get; set; }
    public string UrlToFoundArticle { get; set; }
    public float SimilarityScore { get; set; }
    public Language OriginalLanguage { get; set; }
    public Language FoundLanguage { get; set; }
    public DateTime OriginalPostDate { get; set; }
    public DateTime FoundPostDate { get; set; }


    public void swap()
    {
        (OriginalWebsiteId, FoundWebsiteId) = (FoundWebsiteId, OriginalWebsiteId);
        (UrlToOriginalArticle, UrlToFoundArticle) = (UrlToFoundArticle, UrlToOriginalArticle);
        (OriginalLanguage, FoundLanguage) = (FoundLanguage, OriginalLanguage);
        (OriginalPostDate, FoundPostDate)= (FoundPostDate, OriginalPostDate);
    }

    public string ToString()
    {
        return $"OriginalWebsite: {OriginalWebsiteId} \n" +
               $"FoundWebsite: {FoundWebsiteId} \n" +
               $"UrlToOriginalArticle: {UrlToOriginalArticle} \n" +
               $"UrlToFoundArticle: {UrlToFoundArticle} \n" +
               $"SimilarityScore: {SimilarityScore} \n" +
               $"OriginalLanguage: {OriginalLanguage} \n" +
               $"FoundLanguage: {FoundLanguage} \n" +
               $"OriginalPostDate: {OriginalPostDate} \n" +
               $"FoundPostDate: {FoundPostDate} \n";
    }
}