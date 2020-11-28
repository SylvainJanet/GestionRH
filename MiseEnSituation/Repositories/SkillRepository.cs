using MiseEnSituation.Exceptions;
using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class SkillRepository : GenericRepository<Skill>, ISkillRepository
    {
        public SkillRepository(MyDbContext db) : base(db)
        {
        }

        public List<Skill> FindMany(int[] ids)
        {
            List<Skill> lst = new List<Skill>();
            foreach (var skillId in ids)
            {
                lst.Add(FindById(skillId));
            }
            return lst;
        }

        public List<Skill> FindManyTracked(int[] ids)
        {
            List<Skill> lst = new List<Skill>();
            foreach (var skillId in ids)
            {
                lst.Add(FindByIdTracked(skillId));
            }
            return lst;
        }

        public List<Skill> SearchByDescription(string searchField)
        {
            return (from s in Collection()
                    where s.Description.ToLower().Contains(searchField)
                    select s
                    ).ToList();
        }

        public List<Skill> SearchByDescriptionTracked(string searchField)
        {
            return (from s in CollectionTracked()
                    where s.Description.ToLower().Contains(searchField)
                    select s
                    ).ToList();
        }
    }
}