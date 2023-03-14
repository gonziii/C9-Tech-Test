using Domain;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SpotifyDataRepository;
using SpotifyDataRepository.Service;
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
    public IActionResult SearchTracks(string name)
    {
        try
        {
            var tracks = _spotifyService.GetTracks(name);
            return Ok(tracks);
        }
        catch (Exception exception)
        {
            return NotFound(exception);
        }
    }

    [HttpPost]
    [Route("like")]
    public async Task<IActionResult> Like(string id)
    {
        try
        {
            var track = await _spotifyService.GetTrack(id);
            if (track == null || id.Length == 22)
            {
                throw new ArgumentNullException();
            }
            _unitOfWork.Spotify.AddSong(new SpotifySong
            {
                Id = track.Id,
                Name = track.Name
            });
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
        try
        {
            var track = await _spotifyService.GetTrack(id);
            if (track == null || id.Length == 22)
            {
                throw new ArgumentNullException();
            }
            _unitOfWork.Spotify.RemoveSong(new SpotifySong
            {
                Id = track.Id,
                Name = track.Name
            });
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
        var songList = _unitOfWork.Spotify.GetAll();
        object storage = HttpContext.RequestServices.GetService(typeof(SongStorageContext));

        int songsnumber = ((SongStorageContext)storage).Songs.Count();
        List<SpotifySong> songs = new List<SpotifySong>(); //((SongStorageContext)storage).Songs.ToList();

        if (songsnumber > 0)
        {
            for (int i = 0; i <= songsnumber - 1; i++)
            {
                string songid = ((SongStorageContext)HttpContext.RequestServices.GetService(typeof(SongStorageContext))).Songs.ToList()[i].Id;

                var track = _spotifyService.GetTrack(songid);
                if (track.Id == null)
                {
                    // TODO: remove song from database, but not sure how
                }
                else
                {
                    // not working for some reason so we have to do the check manually for now
                    // if(SongExists(track.Id) == false)

                    int numerofsong = songs.Count();
                    for (int num = 0; num <= numerofsong; num++)
                    {
                        try
                        {
                            if (songs[num].Id == songid)
                            {
                                break;
                            }
                            else if (num == numerofsong - 1)
                            {

                                for (int namenum = 0; namenum < numerofsong; namenum++)
                                {
                                    if (songs[namenum].Name == track.Name)
                                    {
                                        break; // we dont want to add the same song twice
                                        //does this break work?
                                    }
                                    else if (namenum == numerofsong - 1)
                                    {
                                        songs.Add(((SongStorageContext)storage).Songs.ToList()[i]);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // something went wrong, but it's not important
                            songs.Add(((SongStorageContext)storage).Songs.ToList()[i]);
                        }
                    }
                }
            }
        }

        //save the changes, just in case
        ((SongStorageContext)storage).SaveChanges();

        return Ok(songs);
    }

    private bool SongExists(string id)
    {
        return ((SongStorageContext)HttpContext.RequestServices.GetService(typeof(SongStorageContext))).Songs.First(e => e.Id == id) != null;
    }

    private static bool SpotifyId(object id)
    {
        return id.ToString().Length == 22;
    }
}