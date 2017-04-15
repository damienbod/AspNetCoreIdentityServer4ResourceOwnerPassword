using AspNetCoreResourceServer.Model;
using AspNetCoreResourceServer.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreResourceServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DataEventRecordsController : Controller
    {
        private readonly IDataEventRecordRepository _dataEventRecordRepository;
        private readonly ILogger _logger;


        public DataEventRecordsController(IDataEventRecordRepository dataEventRecordRepository, ILoggerFactory loggerFactory)
        {
            _dataEventRecordRepository = dataEventRecordRepository;
            _logger = loggerFactory.CreateLogger("DataEventRecordsController");
        }

        [Authorize("dataEventRecordsUser")]
        [HttpGet]
        public IActionResult Get()
        {
            var userName = HttpContext.User.FindFirst("username")?.Value;
            _logger.LogInformation("User requested all data {UserName}", userName);
            return Ok(_dataEventRecordRepository.GetAll());
        }

        [Authorize("dataEventRecordsAdmin")]
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            return Ok(_dataEventRecordRepository.Get(id));
        }

        [Authorize("dataEventRecordsAdmin")]
        [HttpPost]
        public void Post([FromBody]DataEventRecord value)
        {
            _dataEventRecordRepository.Post(value);
        }

        [Authorize("dataEventRecordsAdmin")]
        [HttpPut("{id}")]
        public void Put(long id, [FromBody]DataEventRecord value)
        {
            _dataEventRecordRepository.Put(id, value);
        }

        [Authorize("dataEventRecordsAdmin")]
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            _dataEventRecordRepository.Delete(id);
        }
    }
}
