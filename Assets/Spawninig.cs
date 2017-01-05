using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public bool free = true;
    public int chance = 0; 
    public Color color = Color.white;
    public float x, y, z;
    public int indexX, indexZ;
    public int up = 4, down = 4, left = 4, right = 4;
    public Node(float _x, float _y, float _z , int _indexX, int _indexZ)
    {
        x = _x;
        y = _y;
        z = _z;
        indexX = _indexX;
        indexZ = _indexZ;
    }
}

public class Type
{
    public int width, height;
    public int spawnW, spawnH;
    public Type(int _width, int _height, int _spawnW,int _spawnH)
    {
        width = _width;
        height = _height;
        spawnH = _spawnH;
        spawnW = _spawnW;
    }

}


public class Spawninig : MonoBehaviour {

    public int population = 0;
    public int width = 50;
    public int heigth = 50;
    public int spawnSizeLimit = 4;
    int count = 0;
    Node[,] tab;
    List<Node> chance1 = new List<Node>();
    List<Node> chance2 = new List<Node>();
    List<Node> chance3 = new List<Node>();
    List<Node> chance4 = new List<Node>();
    void Start()
    {
        tab = new Node[heigth, width];
        for (int i = 0; i < heigth; i++)
            for (int j = 0; j < width; j++)
                tab[i, j] = new Node(-width / 2 + j, 0, -heigth / 2 + i, i, j);
        spawnAt(heigth / 2 , width / 2,new Type(1,1,0,0));
        spawnAt(heigth / 2+1, width / 2+1, new Type(1, 1, 0, 0));
        Debug.Log(tab[heigth / 2 + 1, width / 2].down + " " + tab[heigth / 2 + 1, width / 2].up + " " + tab[heigth / 2 + 1, width / 2].left + " " + tab[heigth / 2 + 1, width / 2].right);
        spawnAt(heigth / 2 + 1, width / 2);
    }
    void spawnAt(int x, int z)
    {
        Color c = new Color(Random.value, Random.value, Random.value);
        Type t = new Type(1,1,0,0);
        t.spawnH = Mathf.RoundToInt(tab[x, z].down * Random.value); 
        t.height = t.spawnH + Mathf.RoundToInt(tab[x, z].up * Random.value)+1;
        t.spawnW = Mathf.RoundToInt(tab[x, z].left * Random.value);
        t.width = t.spawnW + Mathf.RoundToInt(tab[x, z].right * Random.value)+1;
        Debug.Log(t.spawnH+" "+t.height+" "+t.spawnW+" "+t.width+":"+x+" "+z);
        for (int i = 0; i < t.height; i++)
            for (int j = 0; j < t.width; j++)
            {
                try
                {
                    chance1.Remove(tab[x - t.spawnH + i, z - t.spawnW + j]);
                    chance2.Remove(tab[x - t.spawnH + i, z - t.spawnW + j]);
                    chance3.Remove(tab[x - t.spawnH + i, z - t.spawnW + j]);
                    chance4.Remove(tab[x - t.spawnH + i, z - t.spawnW + j]);
                }
                catch(System.Exception e)
                {

                }
                tab[x - t.spawnH + i, z - t.spawnW + j].free = false;
                tab[x - t.spawnH + i, z - t.spawnW + j].color = c;              
            }
        updateTab(x , z , t);
    }

    void spawnAt(int x, int z, Type t)
    {
        Color c = new Color(Random.value, Random.value, Random.value);
        for (int i = 0; i < t.height; i++)
            for (int j = 0; j < t.width; j++)
            { 
                chance1.Remove(tab[x - t.spawnH + i, z - t.spawnW + j]);
                chance2.Remove(tab[x - t.spawnH + i, z - t.spawnW + j]);
                chance3.Remove(tab[x - t.spawnH + i, z - t.spawnW + j]);
                chance4.Remove(tab[x - t.spawnH + i, z - t.spawnW + j]);
                tab[x - t.spawnH + i, z - t.spawnW + j].free = false;
                tab[x - t.spawnH + i, z - t.spawnW + j].color = c;
            }

        updateTab(x , z , t);
    }

    void checkSpawnSize(int x , int z)
    {
        bool canL = true, canR = true, canU = true, canD = true;
        int l = 0, d = 0, u = 0, r = 0;
        Vector4 dim = new Vector4();
        while(canL ||canR || canU || canD)
        {
            dim = new Vector4();
            if (canL)
            {
                dim = new Vector4(x - d, x + u, z - l - 1, z);
                canL = checkDim(dim);
                if (canL) l++;
                if (l + r >= spawnSizeLimit ) break;
            }
            if (canR)
            {
                dim = new Vector4(x - d, x + u, z + r, z + 1);
                canR = checkDim(dim);
                if (canR) r++;
                if (l + r >= spawnSizeLimit ) break;
            }
            if (canD)
            {
                dim = new Vector4(x - d - 1, x, z - l, z + r);
                canD = checkDim(dim);
                if (canD) d++;
                if ( d + u >= spawnSizeLimit) break;
            }
            if (canU)
            {
                dim = new Vector4(x + u, x + 1, z - l, z + r);
                canU = checkDim(dim);
                if (canU) u++;
                if ( u + d >= spawnSizeLimit) break;
            }
        }
        tab[x, z].up = u;
        tab[x, z].down = d;
        tab[x, z].left = l;
        tab[x, z].right = r;
    }

    bool checkDim(Vector4 dim)
    {
        for (int i = (int)dim.x; i <= dim.y; i++)
            if (!tab[i,(int) dim.z].free) return false;
        for (int i = (int)dim.z; i <= dim.w; i++)
            if (!tab[(int)dim.x, i].free) return false;
        return true;
    }

    void updateTab(int x , int z, Type t)
    {
        int i = -t.spawnH;
        do
        {
            if (tab[x + i, z - t.spawnW - 1].free)
            {
                tab[x + i, z - t.spawnW - 1].chance++;
                checkSpawnSize(x + i, z - t.spawnW - 1);
                upgradeNode(tab[x + i, z - t.spawnW - 1]);
            }
            if (tab[x + i, z - t.spawnW + t.width].free)
            {
                tab[x + i, z - t.spawnW + t.width].chance++;
                checkSpawnSize(x + i, z - t.spawnW + t.width);
                upgradeNode(tab[x + i, z - t.spawnW + t.width]);
            }
            i++;
        } while (i < t.height - t.spawnH );

        i = -t.spawnW;
        do
        {
            if (tab[x - t.spawnH - 1, z + i].free)
            {
                tab[x - t.spawnH - 1, z + i].chance++;
                checkSpawnSize(x - t.spawnH - 1, z + i);
                upgradeNode(tab[x - t.spawnH - 1, z + i]);
            }
            if (tab[x - t.spawnH + t.height, z + i].free)
            {
                tab[x - t.spawnH + t.height, z + i].chance++;
                checkSpawnSize(x - t.spawnH + t.height, z + i);
                upgradeNode(tab[x - t.spawnH + t.height, z + i]);
            } 
            i++;
        } while (i < t.width - t.spawnW );

    } 

    void upgradeNode(Node n)
    {
        switch(n.chance)
        {
            case 1:
                chance1.Add(n);
                break;
            case 2:
                chance1.Remove(n);
                chance2.Add(n);
                break;
            case 3:
                chance2.Remove(n);
                chance3.Add(n);
                break;
            case 4:
                chance3.Remove(n);
                chance4.Add(n);
                break;
        }
    }

	void Update ()
    {
	    while(count < population)
        {
            drawRandRectangle(choosePlace());
            count++;
        }
	}

    void OnDrawGizmos()
    {
        if (tab != null)
            foreach (Node n in tab)
            {
                Gizmos.color = n.color;
                Gizmos.DrawCube(new Vector3(n.x, 0, n.z), Vector3.one*.93f);
            }
    }

    Vector3 choosePlace()
    {
        Vector3 _out = new Vector3();
        Node _n = new Node(0,0,0,0,0);
        int range = 1;
        if (chance4.Count != 0) range  = 10;
        else if (chance3.Count != 0) range = 6;
        else if (chance2.Count != 0) range += 3;
        Rand:
        float v = Random.value * range;
        if (v > 6)
        {
            _n = chance4[Mathf.RoundToInt(Random.value * (chance4.Count - 1))];
            _out.x = _n.indexX;
            _out.z = _n.indexZ;
        }
        else if (v > 3)
        {
            if (chance3.Count != 0)
            {
                _n = chance3[Mathf.RoundToInt(Random.value * (chance3.Count - 1))];
                _out.x = _n.indexX;
                _out.z = _n.indexZ;
            }
            else goto Rand;
        }
        else if (v > 1)
        {
            if (chance2.Count != 0)
            {
                _n = chance2[Mathf.RoundToInt(Random.value * (chance2.Count - 1))];
                _out.x = _n.indexX;
                _out.z = _n.indexZ;
            }
            else goto Rand;
        }
        else
        {
            if (chance1.Count != 0)
            {
                _n = chance1[Mathf.RoundToInt(Random.value * (chance1.Count - 1))];
                _out.x = _n.indexX;
                _out.z = _n.indexZ;
            }
        }
        
        return _out;
    }

    void drawRandRectangle(Vector3 pos)
    {
        spawnAt((int)pos.x, (int)pos.z);
    }
}
