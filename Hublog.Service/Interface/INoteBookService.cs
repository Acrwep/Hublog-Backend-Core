using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface INoteBookService
    {
        Task<int> CreateNote(Notebook notebook);

        Task<int> UpdateNote(Notebook notebook);

        Task<int> DeleteNote(int noteId);   
    }
}
