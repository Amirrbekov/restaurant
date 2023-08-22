using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository;

public interface IProductImageRepository : IRepository<ProductImage>
{
	void Update(ProductImage obj);

}
