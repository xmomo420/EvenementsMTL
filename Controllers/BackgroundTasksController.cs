using Microsoft.AspNetCore.Mvc;
using TestReactOther.BackGroundScheduler;

namespace TestReactOther.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackgroundTasksController : ControllerBase
{
    private readonly Worker _worker;

    public BackgroundTasksController(Worker worker)
    {
        _worker = worker;
    }

    [HttpPost]
    public async Task<IActionResult> PlanifierTaches()
    {
        try
        {
            _worker.PlanifierToutesLesTaches();
            return await Task.FromResult<IActionResult>(Ok());
        }
        catch (Exception e)
        {
            return await Task.FromResult<IActionResult>(Problem(e.Message));
        }
    }
    
    // TODO : Ajouter toutes les tâches pour les tester une par une
}