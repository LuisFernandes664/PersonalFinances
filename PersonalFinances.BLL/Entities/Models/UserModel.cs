using BCrypt.Net;
using PersonalFinances.BLL.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models
{
    public class UserModel : BaseEntity
    {
        #region Propertys

        /// <summary>
        /// Nome de utilizador (único). Usado para login.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Senha do utilizador, armazenada de forma segura (por exemplo, com hash e salt).
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Endereço de email do utilizador.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Contacto telefonico do utilizador.
        /// </summary>
        public string PhoneNumber { get; set; }


        #endregion

        /// <summary>
        /// Construtor que mapeia os dados de um DataRow para as propriedades de uma sessão de Pomodoro.
        /// Utiliza o método IfExists para evitar problemas com valores nulos ou ausentes.
        /// </summary>
        /// <param name="row">Linha de dados que contém as informações da sessão de Pomodoro.</param>
        public UserModel(DataRow row) : base(row)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));

            Username = row.Table.Columns.Contains("user_name") ? row.Field<string>("user_name") ?? string.Empty : string.Empty;
            Password = row.Table.Columns.Contains("password") ? row.Field<string>("password") ?? string.Empty : string.Empty;
            Email = row.Table.Columns.Contains("email") ? row.Field<string>("email") ?? string.Empty : string.Empty;
            PhoneNumber = row.Table.Columns.Contains("phone_number") ? row.Field<string>("phone_number") ?? string.Empty : string.Empty;
        }

        public UserModel(string username, string password, string email, string phoneNumber)
        {
            Username = !string.IsNullOrEmpty(username) ? username : throw new ArgumentException("O nome de utilizador não pode ser nulo ou vazio.");
            Password = !string.IsNullOrEmpty(password) ? BCrypt.Net.BCrypt.HashPassword(password) : throw new ArgumentException("A senha não pode ser nula ou vazia.");
            Email = !string.IsNullOrEmpty(email) ? email : throw new ArgumentException("O e-mail não pode ser nulo ou vazio.");
            PhoneNumber = phoneNumber; // Pode ser opcional, dependendo dos requisitos
        }

        public UserModel(object row) : base(row)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));

            if (row is Dictionary<string, object> rowDict)
            {
                Username = rowDict.ContainsKey("user_name") && rowDict["user_name"] is string ? (string)rowDict["user_name"] : string.Empty;
                Password = rowDict.ContainsKey("password") && rowDict["password"] is string ? (string)rowDict["password"] : string.Empty;
                Email = rowDict.ContainsKey("email") && rowDict["email"] is string ? (string)rowDict["email"] : string.Empty;
                PhoneNumber = rowDict.ContainsKey("phone_number") && rowDict["phone_number"] is string ? (string)rowDict["phone_number"] : string.Empty;
            }
            else
            {
                throw new InvalidCastException("O objeto passado não é um Dictionary<string, object>.");
            }
        }


        public UserModel(CreateUserViewModel model)
        {
            Username = model.Username;
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            Email = model.Email;
            PhoneNumber = model.PhoneNumber;
        }

        public void UpdatePassword(string newPassword)
        {
            Password = !string.IsNullOrEmpty(newPassword) ? BCrypt.Net.BCrypt.HashPassword(newPassword) : throw new ArgumentException("A senha não pode ser nula ou vazia.");
        }

    }

}
