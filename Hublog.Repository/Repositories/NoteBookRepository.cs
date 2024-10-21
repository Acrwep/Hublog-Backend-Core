using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class NoteBookRepository : INoteBookRepository
    {
        private readonly Dapperr _dapper;
        public NoteBookRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }
        public async Task<int> CreateNote(Notebook notebook)
        {
            string query = @"INSERT INTO Notebook (NoteTitle, Notes, UserId) VALUES (@NoteTitle, @Notes, @UserId)";
            return await _dapper.ExecuteAsync(query, new {notebook.NoteTitle, notebook.Notes, notebook.UserId});
        }

        public async Task<int> UpdateNote(Notebook notebook)    
        {
            string query = "UPDATE Notebook SET NoteTitle = @NoteTitle, Notes = @Notes WHERE NoteId = @NoteId AND UserId = @UserId";
            return await _dapper.ExecuteAsync(query, new { notebook.NoteTitle, notebook.Notes, notebook.UserId, notebook.NoteId });
        }

        public async Task<int> DeleteNote(int noteId)   
        {
            string query = "DELETE FROM Notebook WHERE NoteId = @NoteId";
            return await _dapper.ExecuteAsync(query, new { NoteId = noteId });
        }

        public async Task<List<Notebook>> GetNotebookById(int organizationId, int userId)
        {
            string query = @"
        SELECT NoteId, NoteTitle, Notes, UserId 
        FROM Notebook 
        WHERE UserId = @UserId AND UserId IN 
        (SELECT Id FROM Users WHERE OrganizationId = @OrganizationId AND Id = @UserId)";

            var parameters = new
            {
                OrganizationId = organizationId,
                UserId = userId,
            };

            return await _dapper.GetAllAsyncs<Notebook>(query, parameters);
        }

    }
}
