using System.Net;
using System.Text;
using FakeNewsBackend.Domain.DTO;
using NLog;

namespace FakeNewsBackend.Controller;

public class HttpController
{
    private readonly HttpClient _client;
    private readonly SemaphoreSlim semaphore;
    private long circuitStatus;
    private const long CLOSED = 0;
    private const long TRIPPED = 1;
    public string UNAVAILABLE = "Unavailable";
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public const int MaxConcurrentRequestsBackup = 15;
    public static string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
        "Chrome/74.0.3729.169 Safari/537.36";

    public HttpController(HttpClient client) : this(client, MaxConcurrentRequestsBackup) {}
    public HttpController(HttpClient client, int maxConcurrentRequests)
    {
        _client = client;
        _client.DefaultRequestHeaders.Add("User-Agent",UserAgent);
        semaphore = new SemaphoreSlim(maxConcurrentRequests);
        circuitStatus = CLOSED;
        
        var backup = new HttpResponseMessage();
        backup.StatusCode = HttpStatusCode.BadRequest;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// Makes a GET request to given Url.
    /// </summary>
    /// <param name="link">Url to make the request to.</param>
    /// <param name="headers">(Optional) A IDictionary containing the headers to use.</param>
    /// <returns>A <see cref="Task"/> containing a <see cref="HttpDTO"/>.</returns>
    public virtual async Task<HttpDTO> MakeGetRequest(string link, IDictionary<string, string> headers = default)
    {
        try
        {
            await semaphore.WaitAsync();
            using var request = new HttpRequestMessage(HttpMethod.Get, link);
            
            if (headers != default)
                foreach (var headersKey in headers.Keys)
                    request.Headers.Add(headersKey, headers[headersKey]);
            var response = await _client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
                return new HttpDTO
                {
                    content = await response.Content.ReadAsStringAsync(),
                    Response = response
                };
            return new HttpDTO
            {
                content = UNAVAILABLE,
                Response = response
            };
            
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, $"[GET] Request failed: {link}");
            return new HttpDTO
            {
                content = UNAVAILABLE,
                Response = new HttpResponseMessage(HttpStatusCode.Locked)
            };
        }
        catch (Exception ex) when (ex is OperationCanceledException || 
                                   ex is TaskCanceledException)
        {
            _logger.Error(ex, "[GET] Request failed: {Link}", link);
            Console.WriteLine(ex);
            Console.WriteLine("Timed out: " + link);
            TripCircuit(reason: "Timed out");
            return new HttpDTO
            {
                content = UNAVAILABLE,
                Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            };
        }
        finally
        {
            semaphore.Release();
        }
    }
    /// <summary>
    /// Makes POST request to the given url.
    /// </summary>
    /// <param name="link">Url to make the request to.</param>
    /// <param name="body">The parametes for the request.</param>
    /// <returns>A <see cref="Task"/> containing a <see cref="HttpDTO"/>.</returns>
    
    public virtual async Task<HttpDTO> MakePostRequest(string link, IDictionary<string, string> body)
    {
        try
        {
            await semaphore.WaitAsync();

            if (IsTripped())
                return new HttpDTO
                {
                    content = UNAVAILABLE,
                    Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                };

            HttpContent content = new FormUrlEncodedContent(body);
            var response = await _client.PostAsync(link, content);

            if (response.StatusCode == HttpStatusCode.OK)
                return new HttpDTO
                {
                    content = await response.Content.ReadAsStringAsync(),
                    Response = response
                };

            TripCircuit(reason: $"Status not OK. Status={response.StatusCode} at={link}");
            return new HttpDTO
            {
                content = UNAVAILABLE,
                Response = response
            };

        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, $"[POST] Request failed: {link}");
            return new HttpDTO
            {
                content = UNAVAILABLE,
                Response = new HttpResponseMessage(HttpStatusCode.Locked)
            };
        }
        catch (Exception ex) when (ex is OperationCanceledException || 
                                   ex is TaskCanceledException)
        {
            _logger.Error(ex, $"[POST] Request failed: {link}");
            Console.WriteLine("Timed out");
            TripCircuit(reason: "Timed out");
            return new HttpDTO
            {
                content = UNAVAILABLE,
                Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            };
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    /// <summary>
    /// Closes Circuit so that no more requests could be made.
    /// </summary>
    public void CloseCircuit()
    {
        if (Interlocked.CompareExchange(ref circuitStatus, CLOSED, TRIPPED) == TRIPPED)
        {
            _logger.Fatal("Http circuit closed");
            Console.WriteLine("Closed circuit");
        }
    }
    /// <summary>
    /// Trips Circuit so that no more requests could be made for a while. 
    /// </summary>
    /// <param name="reason">The reason the ciruit is tripped.</param>
    private void TripCircuit(string reason)
    {
        if (Interlocked.CompareExchange(ref circuitStatus, TRIPPED, CLOSED) == CLOSED)
        {
            _logger.Warn("Http circuit tripped, Reason: {Reason}", reason);
            Console.WriteLine($"Tripping circuit because: {reason}");
        }
    }
    /// <summary>
    /// Checks if the Circuit is tripped.
    /// </summary>
    /// <returns>Whether the Circuit is tripped.</returns>
    private bool IsTripped()
    {
        return Interlocked.Read(ref circuitStatus) == TRIPPED;
    }
}