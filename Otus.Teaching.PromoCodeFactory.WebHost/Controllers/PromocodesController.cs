using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        public readonly IRepository<PromoCode> promocodesRepository;
        private readonly IRepository<Preference> preferenceRepository;
        private readonly IRepository<Customer> customerRepository;
        public PromocodesController(IRepository<PromoCode> promocodesRepository, IRepository<Preference> preferenceRepository, IRepository<Customer> customerRepository)
        {
            this.promocodesRepository = promocodesRepository;
            this.preferenceRepository = preferenceRepository;
            this.customerRepository = customerRepository;
        }
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            //TODO: Получить все промокоды 

            var promocodes = await promocodesRepository.GetAllAsync();

            var promocodeModelList = promocodes.Select(p =>
                new PromoCodeShortResponse()
                {
                    Id = p.Id,
                    Code = p.Code,
                    ServiceInfo = p.ServiceInfo,
                    BeginDate = p.BeginDate.ToString("yyyy.MM.dd"),
                    EndDate = p.EndDate.ToString("yyyy.MM.dd"),
                    PartnerName = p.PartnerName
                }).ToList();

            return promocodeModelList;
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var preference = await preferenceRepository
                .FirstOrDefaultAsync(p => p.Name == request.Preference);

            if (preference == null)
                return BadRequest();

            var customer = await customerRepository
                .FirstOrDefaultAsync(c => c.Preferences
                .Any(cp => cp.PreferenceId == preference.Id));

            if (customer == null)
                return NotFound($"Нет пользователей с предпочтением \"{request.Preference}\"");
            //TODO: Создать промокод и выдать его клиентам с указанным предпочтением
            PromoCode promoCode = new PromoCode
            {
                ServiceInfo = request.ServiceInfo,
                PartnerName = request.PartnerName,
                Code = request.PromoCode,
                Customer = customer,
                Preference = preference,
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14)
            };

            await promocodesRepository.AddAsync(promoCode);
            return Ok();
        }
    }
}