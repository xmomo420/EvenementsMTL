using Microsoft.AspNetCore.Mvc;

namespace TestReactOther.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Session : ControllerBase
{
    [HttpGet]
    public Int32? Get()
    {
        return  HttpContext.Session.GetInt32("idUtilisateur");
    }
}