using Microsoft.AspNetCore.Mvc;
using WealthAPI.Data;

namespace WealthAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class WealthController : Controller
    {
        private WealthContext _wealthContext;
        
        public WealthController(WealthContext wealthContext)
        {
            _wealthContext = wealthContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WealthObject>> GetWealthObject(int id)
        {
            var wealthObj = await _wealthContext.WealthObjects.FindAsync(id);

            if (wealthObj == null)
            {
                return NotFound();
            }

            return wealthObj;
        }

        [HttpPost]
        public async Task<ActionResult> CreateWealthObject(WealthObject wealthObject)
        {
            _wealthContext.WealthObjects.Add(wealthObject);
            await _wealthContext.SaveChangesAsync();

            return CreatedAtAction("GetWealthObject", new {id = wealthObject.Id}, wealthObject);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWealthObject(int id, WealthObject wealthObject)
        {
            if (id != wealthObject.Id)
            {
                return BadRequest();
            }

            _wealthContext.Entry(wealthObject).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            try
            {
                await _wealthContext.SaveChangesAsync();
            }
            catch
            {
                return NoContent();
            }

            var updatedWealthObj = _wealthContext.WealthObjects.Find(id);
            return Ok(updatedWealthObj);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteWealthObject(int id)
        {
            var wealthObj = _wealthContext.WealthObjects.Find(id);

            if (wealthObj == null)
            {
                return NotFound();
            }

            _wealthContext.WealthObjects.Remove(wealthObj);
            await _wealthContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("/value/{type}")]
        public ActionResult<decimal> GetTotalValueByType(WealthTypeEnum type, [FromQuery] int? year, [FromQuery] int? month)
        {
            var wealthObjectsByType = _wealthContext.WealthObjects.Where(obj => obj.Type == type).ToList();

            if (year.HasValue)
            {
                var yearVal = year.Value;
                wealthObjectsByType = wealthObjectsByType.Where(asset => asset.Year == yearVal).ToList();
            }

            if (month.HasValue)
            {
                var monthVal = month.Value;
                wealthObjectsByType = wealthObjectsByType.Where(asset => asset.Month == monthVal).ToList();
            }

            return Ok(wealthObjectsByType.Sum(obj  => obj.Value));
        }

        [HttpGet("/total")]
        public ActionResult<decimal> GetTotalWealth([FromQuery] int? year, [FromQuery] int? month)
        {
            var assets = _wealthContext.WealthObjects.Where(obj => obj.Type == WealthTypeEnum.Asset).ToList();
            var liabilities = _wealthContext.WealthObjects.Where(obj => obj.Type == WealthTypeEnum.Liability).ToList();

            if (year.HasValue)
            {
                var yearVal = year.Value;
                assets = assets.Where(asset => asset.Year == yearVal).ToList();
                liabilities = liabilities.Where(liability => liability.Year == yearVal).ToList();
            }

            if (month.HasValue)
            {
                var monthVal = month.Value;
                assets = assets.Where(asset => asset.Month == monthVal).ToList();
                liabilities = liabilities.Where(liability => liability.Month == monthVal).ToList();
            }

            var assetsVal = assets.Sum(obj => obj.Value);
            var liabilitiesVal = liabilities.Sum(obj => obj.Value);
            var totalWealth = assetsVal - liabilitiesVal;

            return Ok(totalWealth);
        }
    }
}
