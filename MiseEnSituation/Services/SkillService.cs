using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Services
{
    public class SkillService : ISkillService
    {
        private ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public Skill Find(int? id)
        {
            return _skillRepository.Find(id);
        }

        public List<Skill> FindAll(int page, int maxByPage, string searchField)
        {
            int start = (page - 1) * maxByPage;
            return _skillRepository.FindAll(start, maxByPage, searchField);
        }

        public bool NextExist(int page, int maxByPage, string searchField)
        {
            return (page * maxByPage) < _skillRepository.Count(searchField);
        }

        public void Remove(int id)
        {
            _skillRepository.Remove(id);
        }

        public void Save(Skill skill)
        {
            _skillRepository.Save(skill);
        }

        public List<Skill> Search(string searchField)
        {
            searchField = searchField.Trim().ToLower();
            return _skillRepository.SearchByDescription(searchField);
        }

        public void Update(Skill skill)
        {
            _skillRepository.Update(skill);
        }

        public List<Skill> FindMany(int[] ids)
        {
            return _skillRepository.FindMany(ids);
        }
    }
}