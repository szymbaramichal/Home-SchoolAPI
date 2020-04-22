using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Models;

namespace HomeSchoolAPI.Helpers
{
    public interface ISubjectHelper
    {
        Task<SubjectReturn> AddSubjectToClass(string teacherId, Class classToEdit, string subjectName);
        Task<List<Subject>> ReturnAllSubjects(List<Class> userClases);
    }
}