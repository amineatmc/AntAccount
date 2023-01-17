﻿using AntalyaTaksiAccount.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ATAccountContext _aTAccountContext;
        public DepartmentController(ATAccountContext aTAccountContext)
        {
            _aTAccountContext= aTAccountContext;
        } 

        [HttpGet("Get")]
        public async Task<List<Department>> Get()
        {
            var departments =await _aTAccountContext.Departments.Where(c => c.Activity == 1).ToListAsync();
            return departments;
        }


        [HttpGet("Get/{id}")]
        public async Task<Department> Get(int id)
        {
            var department = await _aTAccountContext.Departments.Where(c => c.DepartmentID == id && c.Activity == 1).FirstOrDefaultAsync();
            return department;
        }


        [HttpPost("Post")]
        public async Task<ActionResult> Post(Department department)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model bilgileri doğru değil.");
                }
                Department department1 = new Department();
                department1.DepartmentName = department.DepartmentName;
                _aTAccountContext.Departments.Add(department1);
                _aTAccountContext.SaveChanges();
                return Ok("Kayıt Eklendi.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT api/<DepartmentController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}