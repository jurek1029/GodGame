using UnityEngine;
using Assets;
using System.Collections.Generic;
using System;
using System.Linq;

public enum technology
{
    primal,antient,medieval,victirina,modern,future
}

[RequireComponent(typeof(V))]
public class Meneger : MonoBehaviour
{

    private static Meneger s_instance = null;

    public static Meneger instance
    {
        get
        {
            if (s_instance == null)
                s_instance = FindObjectOfType(typeof(Meneger)) as Meneger;

            if (s_instance == null)
            {
                GameObject obj = new GameObject("Menager");
                s_instance = obj.AddComponent(typeof(Meneger)) as Meneger;
            }

            return s_instance;
        }
    }

    public Environment e;
    public People p;
    public int population = 0; 
    public technology tech = technology.primal;
    technology previousTech;
    public SortedList<int,Village> vList; // posortowane po ilosci chalupek 
    public SortedList<technology,Person> pList;
    public int indexOfFirstPersonCurrentTech = 0;
    public Terrain terrain;
    public int WaterTexIndex = 3;
    public float[,,] map;
    public int coastLastIndex = 0;
    public List<Vector2> land;
    public List<KeyValuePair<int, House>> HousesWith0People;

    private float timeDelayBD = 0; //Birth Death
    private float timeDelayUH = 0; // Upgrade House
    private float timeDealyUV = 0; // Upgrade villlage
    private float timeDealyHDamge = 0;

    void Start()
    {
        vList = new SortedList<int,Village>(new DKeyComparer<int>());
        pList = new SortedList<technology,Person>(new DKeyComparer<technology>());
        land = new List<Vector2>();
        HousesWith0People = new List<KeyValuePair<int, House>>();

        if (p == null) Debug.Log("people = null"); // taaaaaaaaak to mam sens
        previousTech = tech;
        updateTech();
       
        map = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
        detectCoast();

        population = V.instance.startingPopulation;
        e.update(population);

        // findLand();

    }
    void OnDrawGizmos()
    {
       /* foreach(Vector3 v in coast)
        {
            Gizmos.DrawCube(new Vector3(v.x,10,v.y), Vector3.one);
        }*/
    }
    void Update()
    {    
        
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
                    {
                        Debug.Log(isOnWater(hit.point.x, hit.point.z, 10));
                        //_x = Mathf.RoundToInt(hit.point.z / terrain.terrainData.size.z * terrain.terrainData.alphamapWidth);
                        //_y = Mathf.RoundToInt(hit.point.x / terrain.terrainData.size.x * terrain.terrainData.alphamapHeight);
                    }
                }
                /*
                if (x != _x || y != _y)
                {
                    x = _x;y = _y;
                    Debug.Log("nowy");
                    for (int i = 0; i < terrain.terrainData.alphamapLayers; i++)
                        Debug.Log(i+") "+ map[x, y, i]);
                }
                */
    }

    void FixedUpdate()
    {
        timeDelayBD += Time.fixedDeltaTime;
        if (timeDelayBD >= V.instance.timeDelayOnBDRatio) // death birth ratio menagment
        {
            timeDelayBD = 0;
            population -= V.instance.deaths;
            e.decresePopulationGlobaly(population);
            population += V.instance.births;
        }
        if (tech != previousTech) updateTech();
        e.update(population);

        timeDelayUH += Time.fixedDeltaTime;
        if (timeDelayUH >= V.instance.timeDelayOnHouseUpgrades) // house upgrades menagment
        {
            timeDelayUH = 0;
            e.upgradeRNGHouse(V.getNormalDistributino(vList.Values.Count)); 
        }

        timeDealyUV += Time.fixedDeltaTime;
        if (timeDealyUV >= V.instance.timeDelayOnVillageUpgrade)
        {
            timeDealyUV = 0;
            e.upgradeRNGVillage();
        }
        timeDealyHDamge += Time.fixedDeltaTime;
        if (timeDealyHDamge >= V.instance.timeDelayOnEmptyHouseDamage)
        {
            timeDealyHDamge = 0;
            for (int i = 0; i < HousesWith0People.Count; i++)
            {
                
                //jeżeli doszła mu osoba to go usuń z listy
                  if (HousesWith0People[i].Value.people.Count > 0) HousesWith0People.ToList().RemoveAt(i--);
                // zliczaj striki
                if (++HousesWith0People[i].Value.Strikes >= V.instance.ticksBeforeHouseDeath)
                    HousesWith0People[i].Value.parent.houses.Remove(HousesWith0People[i].Key);
            }
        }
    }

    public Person AddPerson()
    {
        if (previousTech > tech) indexOfFirstPersonCurrentTech++;
        if (indexOfFirstPersonCurrentTech == 0) previousTech = tech;
        Person p = new Person(V.ID++);
        pList.Add(tech,p);
        return p;
    }

    public void RemovePersonByIndex(int index)
    {
        pList.RemoveAt(index);
    }

    public void RemovePersonByID(int ID)
    {
        foreach (KeyValuePair<technology, Person> p in pList)
        {
            if (p.Value.ID == ID)
            {
                pList.Values.Remove(p.Value);
                return;
            }
        }
    }

    public void updateTech()
    {
        switch (tech)
        {
            case technology.primal:
                e = new PrimalCiti(ref vList, population);
                break;
            case technology.antient:
                e = new AntientCiti(ref vList, population);
                break;
            case technology.medieval:
                e = new MedievalCiti(ref vList, population);
                break;
            case technology.victirina:
                e = new VictorianCiti(ref vList, population);
                break;
            case technology.modern:
                e = new ModernCiti(ref vList, population);
                break;
            case technology.future:
                e = new FutureCiti(ref vList, population);
                break;
        }
    }
    //----------------------------------------------------------------Terrain help function----------------------------------------------------------------
    public bool isOnWater(float x, float z)
    {
        int _x, _y;
        _x = Mathf.RoundToInt(z / terrain.terrainData.size.z * terrain.terrainData.alphamapWidth);
        _y = Mathf.RoundToInt(x / terrain.terrainData.size.x * terrain.terrainData.alphamapHeight);
        if (map[_x, _y, WaterTexIndex] > 0) return true;
        else return false;
    }

    public bool isOnWater(float x, float z, float r)
    {
        int _x, _y;
        _x = Mathf.RoundToInt(z / terrain.terrainData.size.z * terrain.terrainData.alphamapWidth);
        _y = Mathf.RoundToInt(x / terrain.terrainData.size.x * terrain.terrainData.alphamapHeight);
        for (int i = 0; i < r * 2; i++)
        {
            for (int j = 0; j < r * 2; j++)
            {
                if (i * i + j * j <= r * r)
                {
                    if (map[_x - Mathf.RoundToInt(r) + j, _y - Mathf.RoundToInt(r) + i, WaterTexIndex] > 0) return true;
                }
            }
        }
        return false;
    }

    public bool isOnWater(float x, float z, float w,float h)
    {
        int _x, _y;
        _x = Mathf.RoundToInt(z / terrain.terrainData.size.z * terrain.terrainData.alphamapWidth);
        _y = Mathf.RoundToInt(x / terrain.terrainData.size.x * terrain.terrainData.alphamapHeight);
        for (int i = _x; i < _x + w; i++)
        {
            for (int j = _y; j < _y + h; j++)
            {
                if (map[j, i, WaterTexIndex] > 0) return true;
            }
        }
        return false;
    }

    void detectCoast()//posortowane po y/x
    {
        float Gx, Gy, v;
        float x, z;
        for(int i = 1; i < (terrain.terrainData.heightmapHeight - 2); i++)
        {
            for(int j = 1; j < (terrain.terrainData.heightmapWidth - 2); j++)
            {
                Gx = -map[i - 1, j - 1, WaterTexIndex] - 2 * map[i, j - 1, WaterTexIndex] - map[i + 1, j - 1, WaterTexIndex]
                    + map[i - 1, j + 1, WaterTexIndex] + 2 * map[i, j + 1, WaterTexIndex] + map[i + 1, j + 1, WaterTexIndex];

                Gy = -map[i - 1, j - 1, WaterTexIndex] - 2 * map[i - 1, j, WaterTexIndex] - map[i - 1, j + 1, WaterTexIndex]
                    + map[i + 1, j - 1, WaterTexIndex] + 2 * map[i + 1, j, WaterTexIndex] + map[i + 1, j + 1, WaterTexIndex];
                v = Gx * Gx + Gy * Gy;
                if(v > 0 && map[i,j,WaterTexIndex] < .2f)
                {
                    x =  (float)j / terrain.terrainData.alphamapWidth * terrain.terrainData.size.x;
                    z = (float)i / terrain.terrainData.alphamapHeight * terrain.terrainData.size.z;
                    land.Insert(coastLastIndex++, new Vector2(x, z));    
                }
                else if(map[i,j,WaterTexIndex] == 0)
                {
                    x = (float)j / terrain.terrainData.alphamapWidth * terrain.terrainData.size.x;
                    z = (float)i / terrain.terrainData.alphamapHeight * terrain.terrainData.size.z;
                    land.Add( new Vector2(x, z));
                }
            }
        }
    
    }

  /*  class mComper : IComparer<Vector3>
    {
        public int Compare(Vector3 x, Vector3 y)
        {
            if (x.x == y.x)
                if (x.y < y.y) return -1;
                else return 1;
            else if (x.x < y.x) return -1;
            else return 1;
        }
    }*/
    void findLand()//posortowane po y/x
    {
        float x, z;
        for (int i = 0; i < terrain.terrainData.alphamapHeight; i++)
            for (int j = 0; j < terrain.terrainData.alphamapWidth; j++)
                if (map[i, j, WaterTexIndex] == 0)
                {
                    x = (float)j / terrain.terrainData.alphamapWidth * terrain.terrainData.size.x;
                    z = (float)i / terrain.terrainData.alphamapHeight * terrain.terrainData.size.z;
                    land.Add(new Vector2(x, z));
                }
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
}
