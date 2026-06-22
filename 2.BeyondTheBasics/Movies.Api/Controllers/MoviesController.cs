using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieRepository;
    
    public MoviesController(IMovieService movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request,
        CancellationToken token)
    {
        var movie = request.MapToMovie();

        await _movieRepository.CreateAsync(movie, token);
        
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        //return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug,
        CancellationToken token)
    {
        var movie = Guid.TryParse(idOrSlug, out var id) 
            ? await _movieRepository.GetByIdAsync(id, token) 
            : await _movieRepository.GetBySlugAsync(idOrSlug, token);
        
        if  (movie == null)
            return NotFound();

        var response = movie.MapToResponse();
        
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var movies = await _movieRepository.GetAllAsync(token);
        
        var moviesResponse = movies.MapToResponse();
        
        return Ok(moviesResponse);
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken token)
    {
        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieRepository.UpdateAsync(movie, token);
        
        if (updatedMovie is null) return NotFound();

        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id,
        CancellationToken token)
    {
        var deleted = await _movieRepository.DeleteByIdAsync(id, token);
        
        if (!deleted) return NotFound();

        return Ok();
    }
    
}   