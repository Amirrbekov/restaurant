using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class Company
{
    public int CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? PhoneNumber { get; set; }
}