using ApiCatalogo.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiCatalogo.Models;

[Table("Produtos")]
public class Produto : IValidatableObject
{
    [Key]
    public int ProdutoId { get; set; }

    [Required(ErrorMessage ="O nome é obrigatório!")]
    [StringLength(80, ErrorMessage = "O tamanho máximo permitido é de {1} caracteres!")]
    //[PrimeiraLetraMaiuscula]
    public string? Nome { get; set; }

    [Required(ErrorMessage ="A descrição é Obrigatória!")]
    [StringLength(300, ErrorMessage ="O tamanho máximo permitido é de {1} caracteres!")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage ="O campo preço é obrigatório!")]
    [Column(TypeName = "decimal(8,2)")]
    [Range(1, 100000, ErrorMessage ="Só é permitidos informar valores ente {1} à {2}!")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage ="O campo ImageUrl é obrigatório!")]
    [StringLength(300, ErrorMessage = "O tamanho máximo permitido é de {1} caracteres!")]
    public string? ImagemUrl { get; set;}

    public float Estoque { get; set;}

    public DateTime DataCadastro { get; set;}

    public int CategoriaId { get; set; }

    [JsonIgnore]
    public Categoria? Categoria { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(this.Nome))
        {
            var primeiraLetra = this.Nome[0].ToString();

            if (primeiraLetra != primeiraLetra.ToUpper())
            {
                yield return new ValidationResult("A primeira letra do nome do produto deve ser maiúscula!", new[] {nameof(this.Nome)});
            }       
        }

        if (this.Estoque <= 0)
        {
            yield return new ValidationResult("O estoque informado deve ser maior que zero 0", new[] {nameof(this.Estoque)});
        }
    }
}
