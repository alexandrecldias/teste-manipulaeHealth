using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesafioBackEndRDManipulacao.Data;
using DesafioBackEndRDManipulacao.Models;

public class VideoService
{
    private readonly YouTubeDbContext _context;

    public VideoService(YouTubeDbContext context)
    {
        _context = context;
    }

    public async Task<List<Video>> GetAllVideosAsync(string titulo = null, string duracao = null, string autor = null, DateTime? dataPublicacao = null, string q = null)
    {
        IQueryable<Video> query = _context.Videos.Where(v => !v.Excluido);

        if (!string.IsNullOrEmpty(titulo))
        {
            query = query.Where(v => v.Titulo.Contains(titulo));
        }
        if (!string.IsNullOrEmpty(duracao))
        {
            query = query.Where(v => v.Duracao == duracao);
        }
        if (!string.IsNullOrEmpty(autor))
        {
            query = query.Where(v => v.Autor.Contains(autor));
        }
        if (dataPublicacao.HasValue)
        {
            query = query.Where(v => v.DataPublicacao >= dataPublicacao.Value);
        }
        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(v => v.Titulo.Contains(q) || v.Descricao.Contains(q) || v.Autor.Contains(q));
        }

        return await query.ToListAsync();
    }

    public async Task<Video> GetVideoByIdAsync(int id)
    {
        return await _context.Videos.FirstOrDefaultAsync(v => v.Id == id && !v.Excluido);
    }

    public async Task<Video> AddVideoAsync(Video video)
    {
        _context.Videos.Add(video);
        await _context.SaveChangesAsync();
        return video;
    }

    public async Task UpdateVideoAsync(int id, Video video)
    {
        if (id != video.Id)
        {
            throw new ArgumentException("IDs não correspondem.");
        }
        var existingVideo = await _context.Videos.FindAsync(id);

        if (existingVideo == null || existingVideo.Excluido)
        {
            throw new KeyNotFoundException("Vídeo não encontrado ou excluído."); //Melhor do que retornar null
        }
        // Copiar as propriedades *exceto* Excluido.  Nunca deve ser sobrescrito por uma atualização.
        existingVideo.Titulo = video.Titulo;
        existingVideo.Descricao = video.Descricao;
        existingVideo.Duracao = video.Duracao;
        existingVideo.Autor = video.Autor;
        existingVideo.DataPublicacao = video.DataPublicacao;
        existingVideo.VideoId = video.VideoId;

        _context.Entry(existingVideo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) //Importante para lidar com concorrência
        {
            if (!VideoExists(id))
            {
                throw new KeyNotFoundException("Vídeo não encontrado.");
            }
            else
            {
                throw; //Relança a exceção
            }
        }
    }

    public async Task DeleteVideoAsync(int id)
    {
        var video = await _context.Videos.FindAsync(id);
        if (video == null || video.Excluido)
        {
            return; // Já excluído (ou não existe)
        }
        video.Excluido = true; //Exclusão lógica
        _context.Entry(video).State = EntityState.Modified; //Precisa marcar como modificado.
        await _context.SaveChangesAsync();
    }
    private bool VideoExists(int id)
    {
        return _context.Videos.Any(e => e.Id == id);
    }
}
