
namespace Miilya2023.Services.Abstract
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    public interface IFamilyService
    {
        public Task<List<FamilyDocument>> GetAll();

        public string GetFamilyTreeSerialized(string familyId);
    }
}
