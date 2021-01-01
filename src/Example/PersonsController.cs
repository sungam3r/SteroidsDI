using Microsoft.AspNetCore.Mvc;

namespace Example
{
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IEntryPoint _entry;

        public PersonsController(IEntryPoint entry)
        {
            _entry = entry;
        }

        [HttpGet]
        public object Get()
        {
            return new { Count = _entry.DoSomethingImportant() };
        }
    }
}
