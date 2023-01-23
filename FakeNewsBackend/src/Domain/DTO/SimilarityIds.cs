using FakeNewsBackend.Common.Types;

namespace FakeNewsBackend.Domain.DTO;

public class SimilarityIds
{
    public int originalId;
    public int foundId;
    public string originalUrl;
    public string foundUrl;
    public DateTime originalDate;
    public DateTime foundDate;


}