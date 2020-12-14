using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Public;
using Service.Repository.Interfaces.Public;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 注册
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService registerService;

        /// <summary>
        /// 
        /// </summary>
        public RegisterController(IRegisterService _registerService)
        {
            registerService = _registerService;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> Post([FromBody] Register register)
        {
            return await registerService.RegisterAsync(register);
        }
    }
}