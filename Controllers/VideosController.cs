
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DesafioBackEndRDManipulacao.DTOs; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesafioBackEndRDManipulacao.Models;
using DesafioBackEndRDManipulacao.Services;

[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
{
    private readonly YouTubeService _youTubeService;
    private readonly VideoService _videoService;

    public VideosController(YouTubeService youTubeService, VideoService videoService)
    {
        _youTubeService = youTubeService;
        _videoService = videoService;
    }

    [HttpGet("populate")]
    public async Task<IActionResult> PopulateDatabase()
    {
        try
        {
            var videos = await _youTubeService.SearchVideosAsync();
            foreach (var video in videos)
            {
                if (!_videoService.GetAllVideosAsync().Result.Any(v => v.VideoId == video.VideoId))
                {
                    await _videoService.AddVideoAsync(video);
                }
            }
            return Ok(videos.Select(v => new VideoDto(v)));  
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, $"Bad Gateway: {ex.Message}"); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoDto>>> GetVideos(  
        [FromQuery] string titulo = null,
        [FromQuery] string duracao = null,
        [FromQuery] string autor = null,
        [FromQuery] DateTime? dataPublicacao = null,
        [FromQuery] string q = null)
    {
        try
        {
            var videos = await _videoService.GetAllVideosAsync(titulo, duracao, autor, dataPublicacao, q);
            return Ok(videos.Select(v => new VideoDto(v)));
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VideoDto>> GetVideo(int id) 
    {
        try
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            return Ok(new VideoDto(video));
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }

    }

    [HttpPost]
    public async Task<ActionResult<VideoDto>> PostVideo(VideoInputDto videoInputDto) 
    {
        try
        {
            
            var video = new Video
            {
                VideoId = videoInputDto.VideoId,
                Titulo = videoInputDto.Titulo,
                Descricao = videoInputDto.Descricao,
                Duracao = videoInputDto.Duracao,
                Autor = videoInputDto.Autor,
                DataPublicacao = videoInputDto.DataPublicacao,
                
            };

            await _videoService.AddVideoAsync(video);
            
            return CreatedAtAction(nameof(GetVideo), new { id = video.Id }, new VideoDto(video));
        }
        catch (DbUpdateException ex) 
        {
            
            if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE constraint failed"))
            {
                return Conflict("Um vídeo com este ID já existe.");  
            }
            return StatusCode(500, "Erro ao salvar no banco de dados: " + ex.Message);

        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutVideo(int id, VideoInputDto videoInputDto)  
    {
        try
        {
            
            var video = new Video
            {
                Id = id, 
                VideoId = videoInputDto.VideoId,
                Titulo = videoInputDto.Titulo,
                Descricao = videoInputDto.Descricao,
                Duracao = videoInputDto.Duracao,
                Autor = videoInputDto.Autor,
                DataPublicacao = videoInputDto.DataPublicacao,

            };

            await _videoService.UpdateVideoAsync(id, video);
            return NoContent(); 
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message); 
        }
        catch (DbUpdateConcurrencyException) 
        {
            return StatusCode(500, "Erro de concorrência ao atualizar o vídeo.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVideo(int id)
    {
        try
        {
            await _videoService.DeleteVideoAsync(id);
            return NoContent(); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }
    }
}