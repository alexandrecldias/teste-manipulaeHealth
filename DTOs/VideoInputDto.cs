namespace DesafioBackEndRDManipulacao.DTOs
{
    public class VideoInputDto
    {
        // Não inclui o Id, pois ele é gerado pelo banco de dados.
        public string VideoId { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Duracao { get; set; }
        public string Autor { get; set; }
        public DateTime DataPublicacao { get; set; }
        //Sem o campo Excluido!  O usuário da API não deve controlar isso.
    }
}
