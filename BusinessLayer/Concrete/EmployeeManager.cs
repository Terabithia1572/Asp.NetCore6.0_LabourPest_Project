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
    public class EmployeeManager : IEmployeeService
    {
        IEmployeeDal _employeeDal;

        public EmployeeManager(IEmployeeDal employeeDal)
        {
            _employeeDal = employeeDal;
        }

        public List<Employee> GetAll()
        {
            return _employeeDal.GetListAll();
        }

        public void TAdd(Employee t)
        {
            _employeeDal.Insert(t);
        }

        public void TDelete(Employee t)
        {
            _employeeDal.Delete(t);
        }

        public Employee TGetByID(int id)
        {
            return _employeeDal.GetByID(id);
        }

        public void TUpdate(Employee t)
        {
            _employeeDal.Update(t);
        }
    }
}
