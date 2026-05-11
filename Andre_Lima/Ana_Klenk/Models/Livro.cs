namespace BibliotecaApi.Models 
{
    public class Livro
    {
        public int Id { get; set; }
        
        public string Nome { get; set; } = string.Empty;
        
        
        public string Autor { get; set; } = string.Empty;
        
        public DateTime CriadoEm { get; set; }
        
        public bool Disponivel { get; set; } = true;
    }
}