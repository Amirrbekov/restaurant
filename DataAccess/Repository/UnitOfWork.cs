using DataAccess.Data;
using DataAccess.Repository.Blog;
using DataAccess.Repository.IRepository;
using DataAccess.Repository.IRepository.Blog;

namespace DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public ICompanyRepository Company { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }
    public IOrderHeaderRepository OrderHeader { get; private set; }
    public IOrderDetailRepository OrderDetail { get; private set; }

    //Blog
    public IPosttRepository Post { get; private set; }

    //Comment

    //Reservation
    public IRestaurantGroupRepository RestaurantGroup { get; private set; }
    public ITableReservationRepository TableReservation { get; private set; }
    public ITableRepository Table { get; private set; }

    //Image
    public IProductImageRepository ProductImage { get; private set; }

    public UnitOfWork(ApplicationDbContext db)
    {
		_db = db;
		Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        Company = new CompanyRepository(_db);
        ShoppingCart = new ShoppingCartRepository(_db);
        ApplicationUser = new ApplicationUserRepository(_db);
        OrderHeader = new OrderHeaderRepository(_db);
        OrderDetail = new OrderDetailRepository(_db);

        //blog
        Post = new PosttRepository(_db);

        //comment

        //reservation
        RestaurantGroup = new RestaurantGroupRepository(_db);
        TableReservation = new TableReservationRepository(_db);
        Table = new TableRepository(_db);

        //image
        ProductImage = new ProductImageRepository(_db);
    }

    
    public void Save()
    {
        _db.SaveChanges();
    }
}
