using login_api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace login_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _DBContext;
        private readonly JwtSettings jwtSettings;
        public EmployeeController(AppDbContext dbContext, IOptions<JwtSettings> options)
        {
            this._DBContext = dbContext;
            this.jwtSettings = options.Value;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var employee = this._DBContext.Employees.ToList();
            return Ok(employee);
        }

        [HttpGet("GetById")]
        public IActionResult EmployeeDetails(int id)
        {
            var data = this._DBContext.Employees.FirstOrDefault(x => x.id == id);
            if (data != null)
            {                
                return Ok(data);
            }
            return NotFound(new { Message = "User Does Not Exist" });
        }

        [AllowAnonymous]    
        [HttpPost("Login")]
        public IActionResult Login(Authenticate auth)
        {
            if (auth == null)
            return BadRequest();
            var result = this._DBContext.Employees.FirstOrDefault(res => res.email == auth.Email && res.password == auth.Password);
            if (result != null)
            {
                result.Token = CreateJwt(result);
                return Ok(new JwtToken(){
                    AccessToken =result.Token,
                });
            }            
            return NotFound(new { Message = "Enter Correct Email and Password" });
        }

        [AllowAnonymous] 
        [HttpPost("Register")]
        public IActionResult Register([FromBody] Employee user)
        {            
            if (user != null)
            {              
                this._DBContext.Employees.Add(user);
                this._DBContext.SaveChanges();
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [HttpPut("Update")]
        public IActionResult UpdateDetails([FromBody] Employee user)
        {
            var emp = this._DBContext.Employees.Find(user.id);
            if (emp != null)
            {
                emp.firstName = user.firstName;
                emp.lastName = user.lastName;
                emp.contact = user.contact;
                emp.location = user.location;
                emp.email = user.email;
                emp.password = user.password;
                emp.confirmPassword = user.confirmPassword;
                _DBContext.Employees.Update(emp);
                _DBContext.SaveChanges();
                return Ok(new { Message = "Employee detail has been updated" });
            }
            return NotFound(new { Message = "something is wrong with details" });
        }

        [HttpDelete("RemoveData/{id}")]
        public IActionResult RemoveData(int id)
        {
            var data = this._DBContext.Employees.FirstOrDefault(x => x.id == id);
            if (data != null)
            {
                this._DBContext.Employees.Remove(data);
                this._DBContext.SaveChanges();
                return Ok(true);
            }
            return Ok(false);
        }

        private string CreateJwt(Employee empInfo){
             var jwtTokenHandler = new JwtSecurityTokenHandler();
             var key = Encoding.UTF8.GetBytes(this.jwtSettings.SignKey);
             
             var identity = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Role, $"{empInfo.email}"),
                new Claim(ClaimTypes.Name, $"{empInfo.password}"),
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(1200),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }   
    }
}