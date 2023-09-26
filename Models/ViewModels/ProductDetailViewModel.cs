using Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels;

public class ProductDetailViewModel
{
    public ShoppingCart ShoppingCart { get; set; }
    public ProductComment ProductComment { get; set; }
    public List<ProductComment> ProductCommentList { get; set; }
}
