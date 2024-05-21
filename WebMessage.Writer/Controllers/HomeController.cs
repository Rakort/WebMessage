using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using WebMessage.Writer.Models;

namespace WebMessage.Writer.Controllers;

public partial class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MessageApiClient _messageApiClient;

    public HomeController(ILogger<HomeController> logger, MessageApiClient messageApiClient)
    {
        _logger = logger;
        _messageApiClient = messageApiClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public async Task<IActionResult> SendRequest(string requestData)
    {
        await _messageApiClient.SendMessageAsync(new MessageDto { Text = requestData, Number = 1 });
        
        return RedirectToAction("Index");
    }
}
