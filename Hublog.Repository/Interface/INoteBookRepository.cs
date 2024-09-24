using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface INoteBookRepository
    {
        Task<int> CreateNote(Notebook notebook);

        Task<int> UpdateNote(Notebook notebook);

        Task<int> DeleteNote(int noteId);

        Task<Notebook> GetNotebookById(int organizationId, int userId, int noteId); 
    }
}
