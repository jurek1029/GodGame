using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class AntientCiti : Environment
    {
        public AntientCiti(ref SortedList<int,Village> _vList, int _curentPopulation) : base(ref _vList, _curentPopulation) {}

        public override void firstUpdate() // dodać rynek w środku , rng świątynia 
        {

        }

        protected override Vector3 getRandomHouseLocation(Village v, House h) // promieniście za tym co już powstało 
        {
            throw new NotImplementedException();
        }

        protected override void getRandomVillageLocation(ref Village v)
        {
            int index;
            float x, z;
            // rand from coast or land
            if (UnityEngine.Random.value < V.instance.probOfSpawnNearWater)
            {
                index = Mathf.RoundToInt(UnityEngine.Random.value * (Meneger.instance.coastLastIndex - 1));
                x = Meneger.instance.land[index].x;
                z = Meneger.instance.land[index].y;
                V.removeFildsInRadius(ref Meneger.instance.land, x, z, V.instance.startRadius, index, ref v.freeSpace);

            }
            else
            {
                index = Mathf.RoundToInt(Meneger.instance.coastLastIndex + UnityEngine.Random.value * (Meneger.instance.land.Count - Meneger.instance.coastLastIndex - 1));
                x = Meneger.instance.land[index].x;
                z = Meneger.instance.land[index].y;
                V.removeFildsInRadius(ref Meneger.instance.land, x, z, V.instance.startRadius, index, ref v.freeSpace);
            }

            v.pos = new Vector3(x, 0, z);
        }
    }
}
