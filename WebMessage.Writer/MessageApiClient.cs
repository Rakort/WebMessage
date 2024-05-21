using WebMessage.Writer.Controllers;

public class MessageApiClient(HttpClient httpClient)
{
    public async Task SendMessageAsync(
        MessageDto message,
        CancellationToken cancellationToken = default)
    {

        await httpClient.PostAsJsonAsync(
                "/message", message, cancellationToken);
    }
}