using JWTTokenBaseAuthentication.DTO;
using JWTTokenBaseAuthentication.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTTokenBaseAuthentication.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class ProductController : ControllerBase
    //{
    //    private readonly IProductInterface _productInterface;
    //    public ProductController(IProductInterface productInterface)
    //    {
    //        _productInterface = productInterface;
    //    }
    //}

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductInterface productInterface) : ControllerBase
    {
        [Authorize(Policy ="AdminOnly")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var products = await productInterface.GetAll();
            return Ok(products);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<IActionResult> Create(ProductCreateDto productCreateDto)
        {
            var result = await productInterface.Create(productCreateDto);
            return Ok(result);
        }
        [Authorize(Roles = "Employee")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var result= await productInterface.GetById(Id);
            return Ok(result);
        }

        [Authorize(Roles = "User")]
        [HttpPut("Update")]
        public async  Task<IActionResult> Update(ProductUpdateDto productUpdateDto)
        {
           var result= await productInterface.Update(productUpdateDto);
            return Ok(result);
        }

        [Authorize(Roles = "Employee")]
        [HttpDelete("Delete")]
        public async Task<IActionResult>Delete(int id)
        {
            var result= await productInterface.Delete(id);
            return Ok(result);
        }
    }
}
