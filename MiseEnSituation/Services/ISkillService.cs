using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Services
{
    public interface ISkillService : IGenericService<Skill>
    {
        List<Skill> FindAll(int page, int maxByPage, string searchField);
        List<Skill> FindAllTracked(int page, int maxByPage, string searchField);

        List<Skill> FindMany(int[] ids);
        List<Skill> FindManyTracked(int[] ids);

        bool NextExist(int page, int maxByPage, string searchField);
        
        List<Skill> Search(string searchField);
        List<Skill> SearchTracked(string searchField);
    }
}