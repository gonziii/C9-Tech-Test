using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TechTestBackend.Services;

namespace TechTestBackend.Controllers;

[ApiController]
[Route("api/spotify")]
public class SpotifyController : ControllerBase
{
    private readonly ISpotifyService _spotifyService;
    private readonly IUnitOfWork _unitOfWork;

    public SpotifyController(ISpotifyService spotifyService, IUnitOfWork unitOfWork)
    {
        _spotifyService = spotifyService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [Route("searchTracks")]
    public async Task<IActionResult> SearchTracks(string name)
    {
        try
        {
            var tracks = await _spotifyService.GetTracks(name);
            return Ok(tracks);
        }
        catch (Exception exception)
        {
            return BadRequest(exception);
        }
    }

    [HttpPost]
    [Route("like")]
    public async Task<IActionResult> Like(string id)
    {
        if (!IsCorrectSpotifyIdLength(id))
            return BadRequest("Id length is not accepted");
        
        try
        {
            var track = await _spotifyService.GetTrack(id);
            if (track == null)
            {
                throw new Exception("No track is found");
            }
            _unitOfWork.Spotify.AddSong(track);
            _unitOfWork.Complete();
            return Ok();
        }
        catch (Exception exception)
        {
            // Log error
            return BadRequest(exception);
        }
    }

    [HttpPost]
    [Route("removeLike")]
    public async Task<IActionResult> RemoveLike(string id)
    {
        if (!IsCorrectSpotifyIdLength(id))
            return BadRequest("Id length is not accepted");
        try
        {
            var track = await _spotifyService.GetTrack(id);
            if (track == null)
            {
                throw new Exception("No track is found");
            }
            _unitOfWork.Spotify.RemoveSong(track);
            _unitOfWork.Complete();
            return Ok();
        }
        catch (Exception exception)
        {
            // we should probably log this, yes do it
            return BadRequest(exception);
        }
    }

    [HttpGet]
    [Route("listLiked")]
    public IActionResult ListLiked()
    {
        try
        {
            var songs = _unitOfWork.Spotify.GetSongs();
            return Ok(songs);
        }
        catch (Exception exception)
        {
            return BadRequest(exception);
        }
        
    }

    private bool IsCorrectSpotifyIdLength(string id)
    {
        return id.Length == 22;
    }
}