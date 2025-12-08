using Api.Authorization;
using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/vectors")]
[Authorize]
public class VectorsController : ControllerBase
{
    private readonly IVetorRepository _vetorRepository;

    public VectorsController(IVetorRepository vetorRepository)
    {
        _vetorRepository = vetorRepository;
    }

    /// <summary>
    /// Lista todos os vetores disponíveis
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de vetores</returns>
    [HttpGet]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetVectors(CancellationToken cancellationToken = default)
    {
        var vectors = await _vetorRepository.GetAllAsync(cancellationToken);
        
        var result = vectors.Select(v => new 
        { 
            id = v.Id, 
            name = v.Name, 
            email = v.Email, 
            active = v.Active 
        });

        return Ok(result);
    }
}