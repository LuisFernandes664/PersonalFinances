using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities.ViewModel;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models;
using PersonalFinances.BLL.Interfaces.User;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthsController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var user = new UserModel(model);
                await _userService.RegisterUser(user);

                // Mapeia para o UserResponseViewModel antes de enviar a resposta
                var response = new UserResponseViewModel(user);

                var res = APIResponse<object>.SuccessResponse(response);
                return Ok(res);
            }
            catch (Exception ex)
            {
                var response = APIResponse<object>.FailResponse(ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Realiza o login de um utilizador.
        /// </summary>
        /// <param name="model">Modelo contendo as credenciais do utilizador.</param>
        /// <returns>Token de autenticação ou erro em caso de falha.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> SignIn([FromBody] SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.AuthenticateUser(model.Email, model.Password);
                if (user == null)
                {
                    return Unauthorized("Credenciais inválidas.");
                }

                var token = _userService.GenerateJwtToken(user);

                var response = new
                {
                    Token = token,
                    User = new UserResponseViewModel(user)
                };

                return Ok(APIResponse<object>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.FailResponse(ex.Message));
            }
        }

    }
}
