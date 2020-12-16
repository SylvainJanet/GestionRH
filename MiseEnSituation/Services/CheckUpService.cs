using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Controllers
{
    internal class CheckUpService : ICheckUpService
    {
        private readonly ICheckUpRepository _checkUpRepository;

        public CheckUpService(ICheckUpRepository checkUpRepository)
        {
            this._checkUpRepository = checkUpRepository;
        }

        public CheckUp Find(int? id)
        {
            return _checkUpRepository.Find(id);
        }

        public List<CheckUp> FindAll(int page, int maxByPage)
        {
            int start = (page - 1) * maxByPage;
            return _checkUpRepository.FindAll(start, maxByPage);
        }

        public bool NextExist(int page, int maxByPage)
        {
            return (page * maxByPage) < _checkUpRepository.Count();
        }

        public void Remove(int id)
        {
            _checkUpRepository.Remove(id);
        }

        public void Save(CheckUp checkUp)
        {
            _checkUpRepository.Save(checkUp);
        }

        public void Update(CheckUp checkUp)
        {
            _checkUpRepository.Update(checkUp);

        }
    }
}