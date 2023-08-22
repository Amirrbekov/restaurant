using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Models;

public class Product
{
	public int ProductId { get; set; }
	public string Name { get; set; } = null!;
	public string? Description { get; set; }
	public string SKU { get; set; }

	[Display(Name = "Category")]
	public int CategoryId { get; set; }
	[ForeignKey("CategoryId")]
	[ValidateNever]
	public virtual Category Category { get; set; } = null!;
	[Range(1, int.MaxValue, ErrorMessage = "Price should be greater than ${1}")]
	public double Price { get; set; }

	[ValidateNever]
    public List<ProductImage> ProductImages { get; set; }
}
