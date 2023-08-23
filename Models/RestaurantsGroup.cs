using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class RestaurantsGroup
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "RestarauntGroupName")]
    [MaxLength(50)]
    [Required]
    public string Name { get; set; }
}