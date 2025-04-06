using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            var tags = await _tagService.GetUserTagsAsync(userId);
            return Ok(APIResponse<IEnumerable<TagModel>>.SuccessResponse(tags, "Tags obtidas com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTag(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tag = await _tagService.GetTagByIdAsync(id);

            if (tag == null)
                return NotFound(APIResponse<object>.FailResponse("Tag não encontrada."));

            if (tag.UserId != userId)
                return Forbid();

            return Ok(APIResponse<TagModel>.SuccessResponse(tag, "Tag obtida com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagModel tag)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            tag.UserId = userId;
            var createdTag = await _tagService.CreateTagAsync(tag);

            return Ok(APIResponse<TagModel>.SuccessResponse(createdTag, "Tag criada com sucesso."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(string id, [FromBody] TagModel tag)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingTag = await _tagService.GetTagByIdAsync(id);

            if (existingTag == null)
                return NotFound(APIResponse<object>.FailResponse("Tag não encontrada."));

            if (existingTag.UserId != userId)
                return Forbid();

            tag.StampEntity = id;
            tag.UserId = userId;

            await _tagService.UpdateTagAsync(tag);
            return Ok(APIResponse<TagModel>.SuccessResponse(tag, "Tag atualizada com sucesso."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingTag = await _tagService.GetTagByIdAsync(id);

            if (existingTag == null)
                return NotFound(APIResponse<object>.FailResponse("Tag não encontrada."));

            if (existingTag.UserId != userId)
                return Forbid();

            await _tagService.DeleteTagAsync(id);
            return Ok(APIResponse<object>.SuccessResponse(null, "Tag removida com sucesso."));
        }

        [HttpPost("transaction/{transactionId}/tag/{tagId}")]
        public async Task<IActionResult> AddTagToTransaction(string transactionId, string tagId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tag = await _tagService.GetTagByIdAsync(tagId);

            if (tag == null)
                return NotFound(APIResponse<object>.FailResponse("Tag não encontrada."));

            if (tag.UserId != userId)
                return Forbid();

            await _tagService.AddTagToTransactionAsync(transactionId, tagId);
            return Ok(APIResponse<object>.SuccessResponse(null, "Tag adicionada à transação com sucesso."));
        }

        [HttpDelete("transaction/{transactionId}/tag/{tagId}")]
        public async Task<IActionResult> RemoveTagFromTransaction(string transactionId, string tagId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tag = await _tagService.GetTagByIdAsync(tagId);

            if (tag == null)
                return NotFound(APIResponse<object>.FailResponse("Tag não encontrada."));

            if (tag.UserId != userId)
                return Forbid();

            await _tagService.RemoveTagFromTransactionAsync(transactionId, tagId);
            return Ok(APIResponse<object>.SuccessResponse(null, "Tag removida da transação com sucesso."));
        }

        [HttpGet("transaction/{tagId}")]
        public async Task<IActionResult> GetTransactionsByTag(string tagId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tag = await _tagService.GetTagByIdAsync(tagId);

            if (tag == null)
                return NotFound(APIResponse<object>.FailResponse("Tag não encontrada."));

            if (tag.UserId != userId)
                return Forbid();

            var transactions = await _tagService.GetTransactionsByTagAsync(tagId);
            return Ok(APIResponse<IEnumerable<TransactionModel>>.SuccessResponse(transactions, "Transações obtidas com sucesso."));
        }
    }
}
