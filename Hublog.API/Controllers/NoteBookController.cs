using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteBookController : ControllerBase
    {
        private readonly INoteBookService _noteBookService;
        public NoteBookController(INoteBookService noteBookService)
        {
            _noteBookService = noteBookService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotebook([FromBody] Notebook notebook)
        {
            var result = await _noteBookService.CreateNote(notebook);
            if (result > 0)
            {
                return Ok(new { message = "Notebook created successfully." });
            }
            return BadRequest(new { message = "Failed to create notebook." });
        }

        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNotebook(int noteId, [FromBody] Notebook notebook)
        {
            notebook.NoteId = noteId; 
            var result = await _noteBookService.UpdateNote(notebook);

            if (result > 0)
            {
                return Ok(new { message = "Notebook updated successfully." });
            }
            return BadRequest(new { message = "Failed to update notebook." });
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNotebook(int noteId)
        {
            var result = await _noteBookService.DeleteNote(noteId);
            if (result > 0)
            {
                return Ok(new { message = "Notebook deleted successfully." });
            }
            return NotFound(new { message = "Notebook not found." });
        }

    }
}
