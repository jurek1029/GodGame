using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
   public class Village
    {
        public Vector3 pos;
        public float radius;
        public SortedList<float,House> houses;
        public int maxHouses;
        public List<Vector2> freeSpace;
        public GameObject village;

        public Village()
        {
            village = new GameObject("Village");
            houses = new SortedList<float, House>(new DKeyComparer<float>());
            freeSpace = new List<Vector2>();
        }

        public Village(float _x, float _z, float _radius)
        {
            village = new GameObject("Village");
            pos.x = _x;
            pos.z = _z;
            radius = _radius;
            houses = new SortedList<float, House>(new DKeyComparer<float>());
            freeSpace = new List<Vector2>();
        }

        public Village(Vector3 _pos, float _radius, int _maxHouses)
        {
            village = new GameObject("Village");
            pos = _pos;
            radius = _radius;
            maxHouses = _maxHouses;
            houses = new SortedList<float, House>(new DKeyComparer<float>());
            freeSpace = new List<Vector2>();
        }

        public void set(float _radius, int _maxHouses)
        {
            radius = _radius;
            maxHouses = _maxHouses;
        }

        public void AddHouse(House _house)
        {
            houses.Add(_house.RforCenter,_house);
        }

        public int checkForPlacesInHouses()
        {
            int i = 0;            
            foreach(KeyValuePair<float, House> h in houses)
            {
                if (h.Value.checkForFreePlace()) return i;
                i++;
            }            
            return -1;
        }

    }
}
