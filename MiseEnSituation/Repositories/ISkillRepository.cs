using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Repositories
{
    public interface ISkillRepository : IGenericRepository<Skill>
    {
        List<Skill> FindMany(int[] ids);
        List<Skill> FindManyTracked(int[] ids);
        List<Skill> SearchByDescription(string searchField);
        List<Skill> SearchByDescriptionTracked(string searchField);
    }
}