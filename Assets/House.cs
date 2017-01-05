using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public enum Type
    {
        small,mid,big
    }

    public class House
    {
        public Vector3 pos;
        public Type type;
        public List<Person> people;
        public float RforCenter;
        public GameObject mesh;
        public Vector3 LookAtPoint;
        public int maxPeople;
        public Vector2 BBcenter, BBsize;
        public int Strikes = 0;
        public Village parent;

        public House(ref Village _parent)
        {
            parent = _parent; 
            mesh = GameObject.Instantiate(V.instance.cubePrefab[0]);
            mesh.transform.parent = parent.village.transform;
            BBsize = new Vector2(mesh.GetComponent<BoxCollider>().size.x, mesh.GetComponent<BoxCollider>().size.z);
            type = Type.small;
            maxPeople = V.instance.startPeopleInH;
            people = new List<Person>();
        }

        public House(GameObject parent,GameObject _mesh, Vector2 _BBsize, Type _type, Vector3 _LookAtPoint, int _maxPeople)
        {
            mesh = GameObject.Instantiate(_mesh);
            mesh.transform.parent = parent.transform;
            BBsize = _BBsize;
            type = _type;
            LookAtPoint = _LookAtPoint;
            maxPeople = _maxPeople;
            people = new List<Person>();
        }

        public House(GameObject parent, Vector3 _LookAtPoint)
        {
            mesh = GameObject.Instantiate(V.instance.cubePrefab[0]);
            mesh.transform.parent = parent.transform;
            BBsize = new Vector2(mesh.GetComponent<BoxCollider>().size.x, mesh.GetComponent<BoxCollider>().size.z);
            type = Type.small;
            LookAtPoint = _LookAtPoint;
            maxPeople = V.instance.startPeopleInH;
            people = new List<Person>();
        }

        public void set(Vector3 _pos, Vector3 _LookAtPoint)
        {
            pos = _pos;
            pos.y = Meneger.instance.terrain.SampleHeight(_pos) + mesh.GetComponent<BoxCollider>().size.y/2;
            mesh.transform.position = pos;
            LookAtPoint = _LookAtPoint;
            LookAt();
        }
        /* public House(Vector3 _pos, Type _type, Vector3 _LookAtPoint, int _maxPeople)
         {
             pos = _pos;
             type = _type;
             maxPeople = _maxPeople;
             LookAtPoint = _LookAtPoint;
             people = new List<Person>();
             LookAt();
         }*/

        public void LookAt()
        {
            Vector3 temp = LookAtPoint;
            temp.y = pos.y;
            mesh.transform.LookAt(temp);
        }

        public void LookAt(Vector3 _LookAtPoint)
        {
            _LookAtPoint.y = pos.y;
            mesh.transform.LookAt(_LookAtPoint);
        }

        public void getBoundingBox(out Vector2 center, out Vector2 size)
        {
            center = BBcenter;
            size = BBsize;
        }

        public void increaseSize()
        {
            switch (type)
            {
                case Type.small:
                    type = Type.mid;
                    mesh.GetComponent<MeshFilter>().sharedMesh = V.instance.cubePrefab[1].GetComponent<MeshFilter>().sharedMesh;
                    mesh.transform.localScale = V.instance.cubePrefab[1].transform.localScale;
                    BBsize = new Vector2(mesh.GetComponent<BoxCollider>().size.x, mesh.GetComponent<BoxCollider>().size.z);
                    break;
                case Type.mid:
                    type = Type.big;
                    mesh.GetComponent<MeshFilter>().sharedMesh = V.instance.cubePrefab[2].GetComponent<MeshFilter>().sharedMesh;
                    mesh.transform.localScale = V.instance.cubePrefab[2].transform.localScale;
                    BBsize = new Vector2(mesh.GetComponent<BoxCollider>().size.x, mesh.GetComponent<BoxCollider>().size.z);
                    break;
                case Type.big:
                    Debug.Log("nie da sie już bardziej powiększyć tego domu");
                    break;
            }
        }

        public void decereaseSize()
        {
            switch (type)
            {
                case Type.small:
                    Debug.Log("nie da sie już bardziej zminejszyć tego domu");
                    break;
                case Type.mid:
                    type = Type.small;
                    mesh.GetComponent<MeshFilter>().sharedMesh = V.instance.cubePrefab[0].GetComponent<MeshFilter>().sharedMesh;
                    mesh.transform.localScale = V.instance.cubePrefab[0].transform.localScale;
                    BBsize = new Vector2(mesh.GetComponent<BoxCollider>().size.x, mesh.GetComponent<BoxCollider>().size.z);
                    break;
                case Type.big:
                    type = Type.mid;
                    mesh.GetComponent<MeshFilter>().sharedMesh = V.instance.cubePrefab[1].GetComponent<MeshFilter>().sharedMesh;
                    mesh.transform.localScale = V.instance.cubePrefab[1].transform.localScale;
                    BBsize = new Vector2(mesh.GetComponent<BoxCollider>().size.x, mesh.GetComponent<BoxCollider>().size.z);
                    break;
            }
        }

        public void AssingPerson(Person _p)
        {
            people.Add(_p);
        }

        public bool checkForFreePlace()
        {
            if (people.Count < maxPeople)
                return true;
            else
                return false;
        }
    }
}
