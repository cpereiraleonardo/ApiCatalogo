using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCatalogo.Models;

[Table("Categorias")]
public class Categoria
{
    /// <summary>
    /// Boa pratica toda vez que a classe tiver um propriedade do tipo coleção ser instanciado no constructor a coleção.
    /// </summary>
    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }

    [Key]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage ="Campo nome é obrigatório!")]
    [StringLength(80, ErrorMessage = "O tamanho máximo permitido é de {1} caracteres!")]
    public string? Nome { get; set; }

    [Required(ErrorMessage ="Campo ImageUrl é obrigatório!")]
    [StringLength(300, ErrorMessage = "O tamanho máximo permitido é de {1} caracteres!")]
    public string? ImageUrl { get; set; }

    public ICollection<Produto>? Produtos{ get; set; }
}
