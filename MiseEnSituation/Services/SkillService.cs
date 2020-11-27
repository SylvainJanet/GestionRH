using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Services
{
    public class SkillService : GenericService<Skill>, ISkillService
    {
        private ISkillRepository _SkillRepository;

        public SkillService(ISkillRepository skillRepository) : base(skillRepository)
        {
            _SkillRepository = (ISkillRepository)_repository;
        }
       
        public List<Skill> FindAll(int page, int maxByPage, string searchField)
        {
            int start = (page - 1) * maxByPage;
            searchField = searchField.Trim().ToLower();
            return _repository.GetAll(start, maxByPage, null,
                                        s => s.Description.Contains(searchField));
        }

        public List<Skill> FindAllTracked(int page, int maxByPage, string searchField)
        {
            int start = (page - 1) * maxByPage;
            searchField = searchField.Trim().ToLower();
            return _repository.GetAllTracked(start, maxByPage, null,
                                        s => s.Description.Contains(searchField));
        }

        public List<Skill> FindMany(int[] ids)
        {
            return _SkillRepository.FindMany(ids);
        }
        
        public List<Skill> FindManyTracked(int[] ids)
        {
            return _SkillRepository.FindManyTracked(ids);
        }

        public bool NextExist(int page, int maxByPage, string searchField)
        {
            searchField = searchField.Trim().ToLower();
            return (page * maxByPage) < _repository.Count(s => s.Description.Contains(searchField));
        }

        public List<Skill> Search(string searchField)
        {
            searchField = searchField.Trim().ToLower();
            return _SkillRepository.SearchByDescription(searchField);
        }

        public List<Skill> SearchTracked(string searchField)
        {
            searchField = searchField.Trim().ToLower();
            return _SkillRepository.SearchByDescriptionTracked(searchField);
        }
    }
}