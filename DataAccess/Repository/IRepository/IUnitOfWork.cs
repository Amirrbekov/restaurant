using DataAccess.Repository.IRepository.Blog;
using DataAccess.Repository.IRepository.Comment;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    ICompanyRepository Company { get; }
    IShoppingCartRepository ShoppingCart { get; }
    IApplicationUserRepository ApplicationUser { get; }
    IOrderHeaderRepository OrderHeader { get; }
    IOrderDetailRepository OrderDetail { get; }


    //Blog
    IPosttRepository Post { get; }

	//Comment
	IContactRepository Contact { get; }
	ICommentRepository Comment { get; }
	IProductCommentRepository ProductComment { get; }

	//Reservation
	IRestaurantGroupRepository RestaurantGroup { get; }
    ITableReservationRepository TableReservation { get; }
    ITableRepository Table { get; }

    // Image
    IProductImageRepository ProductImage { get; }

    void Save();
}
