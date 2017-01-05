using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class MedievalCiti : Environment
    {
        public MedievalCiti(ref SortedList<int, Village> _vList, int _curentPopulation) : base(ref _vList, _curentPopulation)
        {
        }

        public override void decresePopulationInRadius(int _currentPopulation, int radius, int x, int z)
        {
        }

        public override void firstUpdate()
        {
        }

        public override void upgradeRNGHouse(int indexOfVillage)
        {
        }

        public override void upgradeRNGVillage()
        {
        }

        protected override Vector3 getRandomHouseLocation(Village v, House h)
        {
            throw new NotImplementedException();
        }

        protected override void getRandomVillageLocation(ref Village v)
        {
            throw new NotImplementedException();
        }

        protected override Vector2 isPlaceInAnyVillage()
        {
            throw new NotImplementedException();
        }
    }
}
