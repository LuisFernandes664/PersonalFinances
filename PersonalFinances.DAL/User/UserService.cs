
using Microsoft.IdentityModel.Tokens;
using PersonalFinances.BLL.Entities.Models;
using PersonalFinances.BLL.Entities.Models.Notification;
using PersonalFinances.BLL.Entities.ViewModel;
using PersonalFinances.BLL.Interfaces.Notification;
using PersonalFinances.BLL.Interfaces.Notification.Senders;
using PersonalFinances.BLL.Interfaces.Repository;
using PersonalFinances.BLL.Interfaces.User;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public UserService(IEmailSender emailSender, ISmsSender smsSender, IUserRepository userRepository, IPasswordResetRepository passwordResetRepository)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _userRepository = userRepository;
            _passwordResetRepository = passwordResetRepository;
        }

        public async Task<UserModel> AuthenticateUser(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return user;
        }

        public string GenerateJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(CommonStrings.SecretKey);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.StampEntity),
                new(ClaimTypes.NameIdentifier, user.StampEntity),
                new(ClaimTypes.Email, user.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<UserModel> GetUserByStampEntity(string stampEntity)
        {
            var user = await _userRepository.GetUserByStampEntity(stampEntity);
            if (user == null)
            {
                throw new Exception("Utilizador não encontrado.");
            }

            return user;
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("Utilizador não encontrado.");
            }

            return user;
        }


        public async Task RegisterUser(UserModel user)
        {
            if (string.IsNullOrEmpty(user.Username))
            {   
                throw new Exception("Nome de Utilizador é obrigatório.");
            }
            var existUser = await _userRepository.GetUserByEmail(user.Email);
            if (existUser == null)
            {
                await _userRepository.SaveUser(user);
            } else
            {
                throw new Exception("O email já se encontra registado!");
            }
        }

        public async Task<UserModel> UpdateUser(string stampEntity, UpdateUserViewModel model)
        {
            var user = await _userRepository.GetUserByStampEntity(stampEntity);
            if (user == null)
            {
                throw new Exception("Utilizador não encontrado.");
            }

            user.Username = model.Name ?? user.Username;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUser(user);

            return user;
        }

        public async Task ResetPassword(string userIdentifier, string resetType)
        {
            var user = await _userRepository.GetUserByStampEntity(userIdentifier);
            if (user == null)
            {
                throw new Exception("Utilizador não encontrado.");
            }

            // Gera o token e expiração
            string resetToken = Guid.NewGuid().ToString();
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(30);

            var resetRequest = new PasswordResetRequestModel
            (
                user.StampEntity,
                resetToken,
                expiresAt,
                false
            );

            // Apaga resets anteriores e salva o novo pedido
            await _passwordResetRepository.DeleteAsync(userIdentifier);
            await _passwordResetRepository.SaveAsync(resetRequest);

            // Criar a notificação
            IAsyncNotification notification = CreatePasswordResetNotification(user, resetType, resetToken);

            // Enviar notificação
            await notification.SendNotificationAsync();
        }

        /// <summary>
        /// Cria a notificação para o reset de senha com base no tipo de reset (email ou sms).
        /// </summary>
        private IAsyncNotification CreatePasswordResetNotification(UserModel user, string resetType, string resetToken)
        {
            if (resetType == "email")
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    throw new Exception("O utilizador não possui um e-mail registado.");
                }

                string resetLink = GenerateResetLink(user.StampEntity, resetToken);
                return EmailNotificationModel.CreatePasswordResetNotification(user.Email, resetLink, _emailSender);
            }
            else if (resetType == "sms")
            {
                if (string.IsNullOrEmpty(user.PhoneNumber))
                {
                    throw new Exception("O utilizador não possui um número de telefone registado.");
                }

                string resetCode = GenerateResetCode();
                return SMSNotificationModel.CreatePasswordResetNotification(user.PhoneNumber, resetCode, _smsSender);
            }
            else
            {
                throw new Exception("Tipo de notificação inválido.");
            }
        }

        /// <summary>
        /// Gera um link de reset de senha com o token.
        /// </summary>
        private string GenerateResetLink(string userID, string token) => $"https://example.com/reset/{userID}/{token}";

        /// <summary>
        /// Gera um código numérico de 6 dígitos para reset via SMS.
        /// </summary>
        private string GenerateResetCode() => new Random().Next(100000, 999999).ToString();

        public async Task ConfirmResetPassword(string token, string newPassword)
        {
            // Buscar o pedido de reset pelo token
            var resetRequest = await _passwordResetRepository.GetByToken(token);

            if (resetRequest == null || resetRequest.Used || resetRequest.ExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Token inválido ou expirado.");
            }

            var user = await _userRepository.GetUserByStampEntity(resetRequest.StampEntity);

            if (user == null)
            {
                throw new Exception("Utilizador não encontrado.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateUser(user);

            // Marcar o token como usado
            resetRequest.Used = true;
            await _passwordResetRepository.UpdateAsync(resetRequest);
        }


    }
}
