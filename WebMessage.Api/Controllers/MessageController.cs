
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebMessage.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly ILogger<MessageController> _logger;
    private readonly NpgsqlDataSource _dataSource;
    private readonly IHubContext<MessageHub> _hubContext;
    private static bool isTableCreated = false;

    public MessageController(ILogger<MessageController> logger, NpgsqlDataSource dataSource, IHubContext<MessageHub> hubContext)
    {
        _logger = logger;
        _dataSource = dataSource;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> AddMessage(MessageDto message)
    {
        await CreateTable();
        using var con = _dataSource.OpenConnection();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO messages (text, date, number) VALUES (@text, @date, @number)";
        cmd.Parameters.AddWithValue("text", message.Text);
        cmd.Parameters.AddWithValue("date", DateTime.Now);
        cmd.Parameters.AddWithValue("number", message.Number);
        cmd.ExecuteNonQuery();

        await _hubContext.Clients.All.SendAsync("message", new Message { Text = message.Text, Date = DateTime.Now, Number = message.Number});
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<Message>>> GetMessages()
    {
        await CreateTable();
        var result = new List<Message>();

        using var con = _dataSource.OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT text, date, number FROM messages ORDER BY date LIMIT 10", con);        
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string text = reader.GetString(0);
                DateTime date = reader.GetDateTime(1);
                long number = reader.GetInt64(2);

                result.Add(new Message() { Text = text, Date = date, Number = number});
            }
        }
        
        return result;
    }

    private async Task CreateTable()
    {
        if (isTableCreated)
            return;

        using var con = _dataSource.OpenConnection();
        using var command = con.CreateCommand();
        command.CommandText = """
        DO
        $$
        BEGIN
        IF NOT EXISTS(SELECT 1 FROM information_schema.tables WHERE table_name = 'messages') THEN
        CREATE TABLE messages(
            id SERIAL PRIMARY KEY,
            text VARCHAR(128),
            date TIMESTAMP,
            number NUMERIC
        );
        END IF;
        END
        $$;
        """;

        await command.ExecuteNonQueryAsync();
        isTableCreated = true;
    }
}
