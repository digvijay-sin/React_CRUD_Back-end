using CrudApp.Context;
using CrudApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace CrudApp.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeeAPIController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetEmployees()
        {
            if(_context.Employees == null)
            {
                return NotFound();
            }
            var employees = await _context.Employees.OrderBy(e => e.DisplayOrder).ToListAsync();
            return Ok(employees);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetEmployee(int? id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest();
            }

            var employee = await _context.Employees.FindAsync(id);
            return Ok(employee);
        }

        [HttpPut]
        [Route("[action]/{id}")]
        public async Task<IActionResult> UpdateEmployee(int? id, Employee updateEmployee)
        {
            if (id != updateEmployee.Id )
            {
                return BadRequest();
            }
            _context.Entry(updateEmployee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(updateEmployee);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddEmployee(Employee newEmp)
        {
            if (newEmp == null)
            {
                return BadRequest();
            }

            int Max = _context.Employees.Any() ? _context.Employees.Max(i => i.DisplayOrder) : -1;
            newEmp.DisplayOrder = Max +  1;
            _context.Add(newEmp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new {id = newEmp.Id }, newEmp);
        }

        [HttpDelete]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || id  == 0)
            {
                return BadRequest();
            }

            var emp = _context.Employees.Find(id);
            if(emp == null)
            {
                return NotFound();
            }
             _context.Remove(emp);
            await _context.SaveChangesAsync();
            await RenumberDisplayOrder();
            return NoContent();
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateDisplayOrder( [FromBody] List<Employee> updatedList)
        {
            //var employees = await _context.Employees.ToListAsync();

            for(int i = 0; i < updatedList.Count; i++)
            {
                var item  = await _context.Employees.FindAsync(updatedList[i].Id);

                if(item == null)
                {
                    return NotFound(item);
                }

                item.DisplayOrder = i;

                _context.Entry(item).State = EntityState.Modified;                
            }

            await _context.SaveChangesAsync();
            
            return Ok();
        }

        [HttpOptions]
        private async Task RenumberDisplayOrder()
        {

            var items = await _context.Employees.OrderBy(i => i.DisplayOrder).ToListAsync();

            for (int i = -1; i < items.Count; i++)
            {
                items[i].DisplayOrder = i + 1;
            }

            await _context.SaveChangesAsync();
        }
    }
}
