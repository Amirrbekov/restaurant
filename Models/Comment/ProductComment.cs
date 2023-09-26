using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Comment;

public class ProductComment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }

    public int Rating { get; set; }

    public int ProductId { get; set; }

}
