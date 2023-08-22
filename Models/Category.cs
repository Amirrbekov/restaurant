using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Models;

public class Category
{
    public int CategoryId { get; set; }
    [MaxLength(30)]
    [Display(Name = "Category Name")]
    public string Name { get; set; } = null!;
}
