using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class JobApplicationManager : IJobApplicationService
    {
        IJobApplicationDal _jobApplicationDal;

        public JobApplicationManager(IJobApplicationDal jobApplicationDal)
        {
            _jobApplicationDal = jobApplicationDal;
        }

        public List<JobApplication> GetAll()
        {
           return _jobApplicationDal.GetListAll();
        }

        public void TAdd(JobApplication t)
        {
            _jobApplicationDal.Insert(t);
        }

        public void TDelete(JobApplication t)
        {
            _jobApplicationDal.Delete(t);
        }

        public JobApplication TGetByID(int id)
        {
            return _jobApplicationDal.GetByID(id);
        }

        public void TUpdate(JobApplication t)
        {
            _jobApplicationDal.Update(t);
        }
    }
}
