﻿using APIS.Services;
using APIS.WebScrapperLogic.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebScrapperController : ControllerBase
    {
        [HttpGet]
        public ActionResult<WebScrappedData> Webscrape(string gln, string path, string gtin, string internalCode, string description)
        {
            try
            {
                WebScrappedData result = WebScrapperService.WebScrappe(gln, path, gtin, internalCode, description);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
                //return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}