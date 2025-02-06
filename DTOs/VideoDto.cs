using DesafioBackEndRDManipulacao.Models;

namespace DesafioBackEndRDManipulacao.DTOs
{

        public class VideoDto
        {
            public int Id { get; set; }
            public string VideoId { get; set; }
            public string Titulo { get; set; }
            public string Descricao { get; set; } //Talvez você não queira retornar a descrição completa em uma listagem
            public string Duracao { get; set; }
            public string Autor { get; set; }
            public DateTime DataPublicacao { get; set; }

            // Construtor que recebe um Video e copia os dados.
            public VideoDto(Video video)
            {
                Id = video.Id;
                VideoId = video.VideoId;
                Titulo = video.Titulo;
                Descricao = video.Descricao;
                Duracao = video.Duracao;
                Autor = video.Autor;
                DataPublicacao = video.DataPublicacao;
            }
    }
}
