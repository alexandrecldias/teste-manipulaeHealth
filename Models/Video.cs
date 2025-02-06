namespace DesafioBackEndRDManipulacao.Models
{
    public class Video : ExclusaoLogica  //Herda ou implementa a interface.
    {
        public int Id { get; set; }
        public string VideoId { get; set; } // ID do vídeo no YouTube
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Duracao { get; set; } // Formato ISO 8601 (ex: PT5M30S)
        public string Autor { get; set; } // Nome do canal
        public DateTime DataPublicacao { get; set; }
        //Outros campos relevantes.
        public bool Excluido { get; set; } = false; //Campo para "exclusão" logica.

    }
}
