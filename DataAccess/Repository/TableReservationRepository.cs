using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataAccess.Repository.IRepository.Blog;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository;

public class TableReservationRepository : Repository<TableReservation>, ITableReservationRepository
{
    private readonly ApplicationDbContext _db;

    public TableReservationRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(TableReservation obj)
    {
        
    }
}
