using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Repositories
{
    public interface ISkillRepository
    {
        void Add(Skill s);
        long Count(string searchField);
        Skill Find(int? id);
        List<Skill> FindAll(int start, int maxByPage, string searchField);
        void Remove(int id);
        void Save(Skill skill);
        List<Skill> SearchByDescription(string searchField);
        void Update(Skill skill);
        List<Skill> FindMany(int[] ids);
    }
}