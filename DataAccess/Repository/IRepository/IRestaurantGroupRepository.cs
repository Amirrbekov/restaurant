using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository;

public interface IRestaurantGroupRepository : IRepository<RestaurantsGroup>
{
    void Update(RestaurantsGroup restaurantsGroup);
}
