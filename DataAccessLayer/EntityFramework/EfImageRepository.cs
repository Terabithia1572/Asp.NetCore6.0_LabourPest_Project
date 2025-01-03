using DataAccessLayer.Abstract;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
	public class EfImageRepository : GenericRepository<Image>, IImageDal
	{
		public void Delete(EntityLayer.Concrete.Image t)
		{
			throw new NotImplementedException();
		}

		public List<EntityLayer.Concrete.Image> GetListAll(Expression<Func<EntityLayer.Concrete.Image, bool>> filter)
		{
			throw new NotImplementedException();
		}

		public void Insert(EntityLayer.Concrete.Image t)
		{
			throw new NotImplementedException();
		}

		public void Update(EntityLayer.Concrete.Image t)
		{
			throw new NotImplementedException();
		}

		EntityLayer.Concrete.Image IGenericDal<EntityLayer.Concrete.Image>.GetByID(int id)
		{
			throw new NotImplementedException();
		}

		List<EntityLayer.Concrete.Image> IGenericDal<EntityLayer.Concrete.Image>.GetListAll()
		{
			throw new NotImplementedException();
		}
	}
}
