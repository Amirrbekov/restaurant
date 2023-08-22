using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels;

public class ShopViewModel
{
    public int PageNumber { get; set; }
    public int PageCount { get; set; }
    public bool NextPage { get; set; }
    public string Search { get; set; }
    public string Category { get; set; }
    public IEnumerable<Product> Products { get; set; }
	public IEnumerable<int> Pages { get; set; }
}
