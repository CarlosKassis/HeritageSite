namespace HeritageSite.Controllers.PrivateHistory
{
    using CsvHelper;
    using CsvHelper.Configuration.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using HeritageSite.Constants;
    using HeritageSite.Services.Abstract;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/PrivateHistory/[Controller]")]
    public class FamilyController : ControllerBase
    {
        private readonly IFamilyService _familyService;

        public class BalkanFamilyTreePerson
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("mid")]
            public string MotherId { get; set; }

            [JsonProperty("fid")]
            public string FatherId { get; set; }

            [JsonProperty("pids")]
            public List<string> PartnerIds { get; set; }

            [JsonProperty("name")]
            public string FullName { get; set; }

            [JsonProperty("gender")]
            public string Gender { get; set; }
        }

        public class FamilyEchoCsvEntry
        {
            [Name("ID")]
            public string Id { get; set; }

            [Name("Mother ID")]
            public string MotherId { get; set; }

            [Name("Father ID")]
            public string FatherId { get; set; }

            [Name("Partner ID")]
            public string PartnerId { get; set; }

            // TODO: handle 2 ex-partners
            [Name("Ex-partner IDs")]
            public string ExPartnerId { get; set; }

            [Name("Surname at birth")]
            public string SurnameAtBirth { get; set; }

            [Name("Given names")]
            public string Name { get; set; }

            [Name("Gender")]
            public string Gender { get; set; }

            [Name("Birth year")]
            public int? BirthYear { get; set; }


            public BalkanFamilyTreePerson ToBalkanFamilyTreePerson()
            {
                return new BalkanFamilyTreePerson
                {
                    Id = Id,
                    MotherId = MotherId,
                    FatherId = FatherId,
                    PartnerIds = new List<string> { PartnerId, ExPartnerId }.Where(id => !string.IsNullOrEmpty(id)).ToList(),
                    FullName = Name + " " + SurnameAtBirth,
                    Gender = Gender.ToLower()
                };
            }
        }


        public FamilyController(IFamilyService familyService)
        {
            _familyService = familyService;
        }

        [HttpGet]
        [Route("All")]
        public async Task<string> GetAll()
        {
            var results = await _familyService.GetAll();
            return JsonConvert.SerializeObject(results);
        }

        [HttpGet]
        [Route("Tree/{familyId}")]
        public string GetFamilyTree(string familyId)
        {
            ValidateFamilyName(familyId);

            return _familyService.GetFamilyTreeSerialized(familyId);
        }

        private static void ValidateFamilyName(string familyId)
        {
            if (string.IsNullOrEmpty(familyId))
            {
                throw new ArgumentException("Empty family");
            }

            if (familyId.Contains('.') || familyId.Contains('/') || familyId.Contains('\\'))
            {
                throw new ArgumentException("Invalid family");
            }
        }
    }
}
