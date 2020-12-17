using GenericRepositoryAndService.Service;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Services
{
    public class SkillService : GenericService<Skill>, ISkillService
    {
        public SkillService(ISkillRepository skillRepository) : base(skillRepository)
        {
        }

        public override Expression<Func<IQueryable<Skill>, IOrderedQueryable<Skill>>> OrderExpression()
        {
            return req => req.OrderBy(s => s.Description);
        }

        public override Expression<Func<Skill, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return s => s.Description.Trim().ToLower().Contains(searchField);
        }
    }
}