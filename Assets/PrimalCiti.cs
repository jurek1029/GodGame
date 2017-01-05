using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class PrimalCiti : Environment
    {
        public PrimalCiti(ref SortedList<int,Village> _vList, int _curentPopulation) : base(ref _vList, _curentPopulation) {}

        public override void firstUpdate() { }

        protected override void getRandomVillageLocation(ref Village v)
        {
            int index;
            float x, z;
            // rand from coast or land
             if (UnityEngine.Random.value < V.instance.probOfSpawnNearWater)
            {
                index = Mathf.RoundToInt(UnityEngine.Random.value * (Meneger.instance.coastLastIndex-1));
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

            v.pos = new Vector3(x,0,z);
        }

        protected override Vector3 getRandomHouseLocation(Village v,House h)
        {
            Vector3 _out;
            int index = Mathf.RoundToInt(UnityEngine.Random.value * v.freeSpace.Count()); // po kułku
            int i = doALoop(index, v.freeSpace.Count, v);
            if (i == -1) i = doALoop(0, index, v);
            if (i == -1) return new Vector3(-1,-1,-1);
            _out = new Vector3(v.freeSpace[i].x, 0, v.freeSpace[i].y);
            bool t = V.removeFildsInBox(ref v.freeSpace, h.BBsize, i);
            return _out;
        }  

        int doALoop(int sindex, int eIndex, Village v) // find first index in loop 
        {
            float d;
            for(int i = sindex; i < eIndex; i++)
            {
                d = (v.freeSpace[i].x - v.pos.x) * (v.freeSpace[i].x - v.pos.x) + (v.freeSpace[i].y - v.pos.z) * (v.freeSpace[i].y - v.pos.z);
                if (d > V.instance.primalR1 * V.instance.primalR1 && d < V.instance.primalR2 * V.instance.primalR2)
                    return i;
            }
            return -1;
        }

        bool isIntersectingWithAnyVillage(float x, float y, float r)
        {
            foreach(KeyValuePair<int, Village> v in vList)
            {
                if ((v.Value.pos.x - x) * (v.Value.pos.x - x) + (v.Value.pos.y - y) * (v.Value.pos.y - y) < (r + v.Value.radius) * (r + v.Value.radius)) return true;
            }
            return false;
        }
    }
}
