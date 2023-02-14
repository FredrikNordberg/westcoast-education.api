using Microsoft.AspNetCore.Mvc;

namespace westcoast_education.api.Controllers
{
    public interface IBaseApiController
    {
        Task<ActionResult> Add(string name);
        Task<ActionResult> Delete(int id);
        Task<ActionResult> GetById(int id);
        Task<ActionResult> ListAll();
        Task<ActionResult> List(string name);
        Task<ActionResult> Update(int id, string name);
    }
}