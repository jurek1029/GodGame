using UnityEngine;
using System.Collections.Generic;
using Assets;
using System;

public class V : MonoBehaviour
{
    public int startingPopulation = 10;

    public float startRadius = 10;
    public float primalR1 = 6; // poczatek paska w którym sie spawnuje domki
    public float primalR2 = 9; // koniec paska w którm spamuje sie domki
    public int startHousesInV = 4;
    public int startPeopleInH = 3;
    public int peopleInMidH = 6;
    public int peopleInBigH = 9;
    public float probOfSpawnNearWater = 0.8f;
    public int births = 2;
    public int deaths = 1;
    public int ticksBeforeHouseDeath = 4;
    public float timeDelayOnBDRatio = 2;
    public float timeDelayOnHouseUpgrades = 10;
    public float timeDelayOnVillageUpgrade = 20;
    public float timeDelayOnEmptyHouseDamage = 10;

    public GameObject[] cubePrefab; // oder same as in enume type

    public static int ID = 0;
    private static V s_instance = null;

    public static V instance
    {
        get
        {
            if (s_instance == null)
                s_instance = FindObjectOfType(typeof(V)) as V;

            if (s_instance == null)
            {
                GameObject obj = new GameObject("Values");
                s_instance = obj.AddComponent(typeof(V)) as V;
            }

            return s_instance;
        }
    }

    void OnApplicationQuit()
    {
        s_instance = null;
    }

    public static int getNormalDistributino(int count)
    {
        System.Random rng = new System.Random();
        double u1 = rng.NextDouble();
        double u2 = rng.NextDouble();
        int index = Mathf.RoundToInt((float)(Math.Abs(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2)) / 3 * (count - 1)));
        if (index > count - 1) index = count - 1;
        if (index < 0) index = 0;
        return index;
    }

    public static void removeFildsInRadius(ref List<Vector2> l, float x, float y, float r, int index)
    {
        int i = index;

        while (l[i].x - x < r)
        {
            if ((l[i].x - x) * (l[i].x - x) + (l[i].y - y) * (l[i].y - y) < r * r) l.RemoveAt(i);
            else i++;
        }
        i = index;
        while (x - l[i].x < r)
        {
            if ((l[i].x - x) * (l[i].x - x) + (l[i].y - y) * (l[i].y - y) < r * r) l.RemoveAt(i);
            else i--;
        }
    }

    public static void removeFildsInRadius(ref List<Vector2> l, float x, float y, float r, int index, ref List<Vector2> _out)
    {
        int i = index;

        while (i < l.Count && l[i].y - y < r)
        {
            if ((l[i].x - x) * (l[i].x - x) + (l[i].y - y) * (l[i].y - y) < r * r) { _out.Add(l[i]); l.RemoveAt(i); }
            else i++;
        }
        i = index;
        while (i > -1 && y - l[i].y < r)
        {
            if ((l[i].x - x) * (l[i].x - x) + (l[i].y - y) * (l[i].y - y) < r * r) { _out.Add(l[i]); l.RemoveAt(i); }
            else i--;
        }
    }

    public static bool removeFildsInBox(ref List<Vector2> l, Vector2 size, int index)
    {
        if (index >= l.Count || index < 0)
            return false;

        for (int i = index + 1; i < l.Count && Mathf.Abs(l[i].y - l[index].y) < size.y; i++)
        {
            if (Mathf.Abs(l[i].x - l[index].x) < size.x)
            {
                l.RemoveAt(i--);
                if (l.Count == 0)
                    return false;
            }
        }
        for (int i = index - 1; i > 0 && Mathf.Abs(l[index].y - l[i].y) < size.y; i--)
        {
            if (Mathf.Abs(l[i].x - l[index].x) < size.x)
            {
                l.RemoveAt(i); index--;
                if (l.Count == 0)
                    return false;
            }
        }
        l.RemoveAt(index);
        return true;
    }


}
