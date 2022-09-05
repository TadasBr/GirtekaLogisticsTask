using ElectricityDataAPI.Data;
using ElectricityDataAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ElectricityDataAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricityController : ControllerBase
    {
        private readonly ElectricityDbContext _context;
        private readonly ILogger<ElectricityController> _logger;
        public ElectricityController(ElectricityDbContext context, ILogger<ElectricityController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<ElectricityReport>> Get(int count, int pageNumber)
        {
            _logger.LogInformation("Getting Electricity reports... ");

            IQueryable<ElectricityReport> reports = _context.ElectricityReports.Include(x => x.RealEstate);

            return reports.Skip(pageNumber * count).Take(count);
        }

        //// Found these dynamic links so i just played around a little
        //[Route($"{nameof(GetFiltered)}")]
        //[HttpGet]

        //public async Task<IEnumerable<ElectricityReport>> GetFiltered(int count, int pageNumber, string filter)
        //{
        //    _logger.LogInformation("Getting filtered Electricity reports... ");

        //    IQueryable<ElectricityReport> reports = _context.ElectricityReports.Include(x => x.RealEstate);

        //    return reports.Where(filter).Skip(pageNumber * count).Take(count);
        //}

        //[Route($"{nameof(GetOrdered)}")]
        //[HttpGet]
        //public async Task<IEnumerable<ElectricityReport>> GetOrdered(int count, int pageNumber, string orderFilter)
        //{
        //    _logger.LogInformation("Getting ordered Electricity reports... ");

        //    IQueryable<ElectricityReport> reports = _context.ElectricityReports.Include(x => x.RealEstate);

        //    return reports.OrderBy(orderFilter).Skip(pageNumber * count).Take(count);
        //}

        //[Route($"{nameof(GetFilteredAndOrdered)}")]
        //[HttpGet]
        //public async Task<IEnumerable<ElectricityReport>> GetFilteredAndOrdered(int count, int pageNumber, string filter, string orderFilter)
        //{
        //    _logger.LogInformation("Getting filtered and ordered Electricity reports... ");

        //    IQueryable<ElectricityReport> reports = _context.ElectricityReports.Include(x => x.RealEstate);

        //    return reports.Where(filter).OrderBy(orderFilter).Skip(pageNumber * count).Take(count);
        //}
    }
}
