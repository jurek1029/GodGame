using UnityEngine;
using System.Collections;
using System.Linq;
using Assets;
using System.Collections.Generic;

public abstract class Environment
{
    public SortedList<int,Village> vList;
    public int curentPopulation;

    public Environment(ref SortedList<int,Village> _vList, int _curentPopulation)
    {
        vList = _vList;
        curentPopulation = _curentPopulation;
        firstUpdate();
    }
    abstract public void firstUpdate();
    abstract protected void getRandomVillageLocation(ref Village v);
    abstract protected Vector3 getRandomHouseLocation(Village v, House h);
    virtual public void update(int _currentPopulation)
    {
        Vector2 index;
        while (curentPopulation < _currentPopulation)
        {
            Person p = Meneger.instance.AddPerson();
            curentPopulation++;
            index = isPlaceInAnyVillage();
            if (index.x == -1) // nie ma wioski z wolnym miejscem
            {

                Village v = new Village();
                getRandomVillageLocation(ref v);
                v.set(V.instance.startRadius, V.instance.startHousesInV);

                House h = new House(ref v);
                Vector3 hPos = getRandomHouseLocation(v, h);
                if (hPos.x != -1 && hPos.y != -1 && hPos.z != -1)
                {
                    h.set(hPos, v.pos);
                    v.AddHouse(h);
                    v.houses.Last().Value.AssingPerson(p);
                    vList.Add(1, v);
                }
                else vList.Add(0, v);

            }
            else if (index.y == -1) // nie ma domu z wolnym miejscem
            {
                Village v = vList.Values[(int)index.x];
                House h = new House(ref v);
                Vector3 hPos = getRandomHouseLocation(vList.Values[(int)index.x], h);
                if (hPos.x != -1 && hPos.y != -1 && hPos.z != -1)
                {
                    h.set(hPos, vList.Values[(int)index.x].pos);

                    vList.Values[(int)index.x].AddHouse(h);
                    vList.Values[(int)index.x].houses.Last().Value.AssingPerson(p);
                    KeyValuePair<int, Village> temp = new KeyValuePair<int, Village>(vList.Keys[(int)index.x] + 1, vList.Values[(int)index.x]);
                    vList.RemoveAt((int)index.x);
                    vList.Add(temp.Key, temp.Value);
                }
            }
            else // jest wolne miejsce
            {
                vList.Values[(int)index.x].houses.Values[(int)index.y].AssingPerson(p);
            }

        }
    }
    
    virtual public void decresePopulationGlobaly(int _currentPopulation)
    {
        while (curentPopulation >= _currentPopulation)
        {
            // lusuje i usuwa osobe z listy wiosek
            int indexP;
            if (Meneger.instance.indexOfFirstPersonCurrentTech > 0)
            {
                indexP = Mathf.RoundToInt(UnityEngine.Random.value * (Meneger.instance.indexOfFirstPersonCurrentTech - 1));
                Meneger.instance.indexOfFirstPersonCurrentTech--;
            }
            else indexP = Mathf.RoundToInt(UnityEngine.Random.value * (Meneger.instance.pList.Count - 1));
            int IDp = Meneger.instance.pList.Values[indexP].ID;
            int x = 0, y = 0;
            foreach (KeyValuePair<int, Village> v in vList)
            {
                foreach (KeyValuePair<float, House> h in v.Value.houses)
                {
                    foreach (Person p in h.Value.people)
                    {
                        if (p.ID == IDp)
                        {
                            h.Value.people.Remove(p);
                            goto found;
                        }
                    }
                    y++;
                }
                x++;
            }
        found:
            // usuwa osobe z gownej listy                
            Meneger.instance.RemovePersonByIndex(indexP);
            curentPopulation--;
        }
    }
    virtual public void decresePopulationInRadius(int _currentPopulation, int radius, int x, int z)
    {

    }
    virtual public void upgradeRNGHouse(int indexOfVillage)
    {
        int indexH = V.getNormalDistributino(vList.Values[indexOfVillage].houses.Count);

        vList.Values[indexOfVillage].houses.Values[indexH].maxPeople++;
        if (vList.Values[indexOfVillage].houses.Values[indexH].maxPeople == V.instance.peopleInBigH)
            vList.Values[indexOfVillage].houses.Values[indexH].increaseSize();
        else if (vList.Values[indexOfVillage].houses.Values[indexH].maxPeople == V.instance.peopleInMidH)
            vList.Values[indexOfVillage].houses.Values[indexH].increaseSize();
    }
    virtual public void upgradeRNGVillage()
    {
        int indexV = V.getNormalDistributino(vList.Values.Count);
        vList.Values[indexV].maxHouses++;
    }
    virtual protected Vector2 isPlaceInAnyVillage()
    {
        Vector2 index = new Vector2();
        index.x = 0;
        foreach (KeyValuePair<int, Village> v in vList)
        {
            index.y = v.Value.checkForPlacesInHouses();
            if (index.y != -1) return index;
            else if (v.Value.maxHouses > v.Value.houses.Count) return index;
            index.x++;
        }
        return new Vector2(-1, -1);
    }

}
