using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataAccess.Repository.IRepository.Comment;
using Models;
using Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Comment;

public class ContactRepository : Repository<Contact>, IContactRepository
{
	private readonly ApplicationDbContext _db;

	public ContactRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

	public Contact FirstOrDefaultAsync()
	{
		return _dbSet.FirstOrDefault();
	}

	public List<Contact> ToList()
	{
		return _dbSet.ToList();
	}
}
