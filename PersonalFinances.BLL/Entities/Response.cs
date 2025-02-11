
using Microsoft.AspNetCore.Mvc.ModelBinding;

    namespace PersonalFinances.BLL.Entities
    {
        /// <summary>
        /// Representa uma resposta genérica que pode ser usada para retornar resultados de operações.
        /// </summary>
        /// <typeparam name="T">O tipo de dados contido na resposta.</typeparam>
        public class APIResponse<T>
        {
            /// <summary>
            /// Indica se a operação foi bem-sucedida.
            /// </summary>
            public bool Success { get; }

            /// <summary>
            /// Os dados retornados pela operação.
            /// </summary>
            public T Data { get; }

            /// <summary>
            /// Uma mensagem descritiva sobre o resultado da operação.
            /// </summary>
            public string Message { get; }

            /// <summary>
            /// Inicializa uma nova instância da classe <see cref="APIResponse{T}"/>.
            /// </summary>
            /// <param name="success">Indica se a operação foi bem-sucedida.</param>
            /// <param name="data">Os dados retornados pela operação.</param>
            /// <param name="message">Uma mensagem descritiva sobre o resultado da operação.</param>
            public APIResponse(bool success, T data, string message = null)
            {
                Success = success;
                Data = data;
                Message = message ?? string.Empty;
            }

            /// <summary>
            /// Cria uma resposta de sucesso.
            /// </summary>
            /// <param name="data">Os dados retornados pela operação.</param>
            /// <param name="message">Uma mensagem descritiva sobre o resultado da operação.</param>
            /// <returns>Uma resposta de sucesso.</returns>
            public static APIResponse<T> SuccessResponse(T data, string message = null)
            {
                return new APIResponse<T>(true, data, message);
            }

            /// <summary>
            /// Cria uma resposta de falha.
            /// </summary>
            /// <param name="message">Uma mensagem descritiva sobre o erro ocorrido.</param>
            /// <returns>Uma resposta de falha.</returns>
            public static APIResponse<T> FailResponse(string message)
            {
                return new APIResponse<T>(false, default, message);
            }

            /// <summary>
            /// Cria uma resposta de falha com dados opcionais.
            /// </summary>
            /// <param name="data">Os dados retornados pela operação.</param>
            /// <param name="message">Uma mensagem descritiva sobre o erro ocorrido.</param>
            /// <returns>Uma resposta de falha com dados.</returns>
            public static APIResponse<T> FailResponse(T data, string message)
            {
                return new APIResponse<T>(false, data, message);
            }

            public static APIResponse<T> FailResponse(ModelStateDictionary modelState)
            {
                // Extrai todas as mensagens de erro do ModelState
                var errors = modelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrEmpty(m))
                    .ToList();

                // Junta as mensagens numa única string
                var errorMessage = string.Join("; ", errors);

                return new APIResponse<T>(false, default, errorMessage);
            }
        }
    }
