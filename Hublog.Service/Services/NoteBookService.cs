using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class NoteBookService : INoteBookService
    {
        private readonly INoteBookRepository _notebookRepository;
        public NoteBookService(INoteBookRepository notebookRepository)
        {
            _notebookRepository = notebookRepository;
        }
        public async Task<int> CreateNote(Notebook notebook)
        {
            return await _notebookRepository.CreateNote(notebook);
        }

        public async Task<int> UpdateNote(Notebook notebook)
        {
            return await _notebookRepository.UpdateNote(notebook);  
        }

        public async Task<int> DeleteNote(int noteId)
        {
            return await _notebookRepository.DeleteNote(noteId);
        }
    }
}
