using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Services
{
    public interface ISkillService
    {
        Skill Find(int? id);
        List<Skill> FindAll(int page, int maxByPage, string searchField);
        List<Skill> FindMany(int[] ids);
        bool NextExist(int page, int maxByPage, string searchField);
        void Remove(int id);
        void Save(Skill skill);
        List<Skill> Search(string searchField);
        void Update(Skill skill);
    }
}