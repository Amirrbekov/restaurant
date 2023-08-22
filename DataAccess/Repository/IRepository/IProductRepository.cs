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

public interface IProductRepository : IRepository<Product>
{
	void Update(Product obj);
    public ShopViewModel GetAllPostsPage(int pageNumber, string category, string search);
    IQueryable<Product> Include(params Expression<Func<Product, object>>[] includeProperties);

}
