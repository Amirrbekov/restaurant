using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Comment;

public class Contact
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Name { get; set; }
    [Required, StringLength(15), EmailAddress]
    public string Email { get; set; }
    [Required, StringLength(500)]
    public string Message { get; set; }
    [ScaffoldColumn(false)]
    public DateTime CreateDate { get; set; }
}
