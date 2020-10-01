using System.Collections.Generic;
using UnityEngine;

public class BaseDB<T> : ScriptableObject where T : DBComponent
{
    [SerializeField, HideInInspector] protected List<T> list;

    [SerializeField] protected T current;

    public int CurInd { get; protected set; } = 0;
    public string nameFind;
    public int idFind;

    public T GetName()
    {
        int findInd = CurInd;

        T temp;
        for (int i = 0; i < list.Count; i++)
        {
            temp = list[i];
            if (temp.name == nameFind)
            {
                findInd = i;
                current = temp;
            }
        }
        CurInd = findInd;
        return current;
    }

    public T GetName(string name)
    {
        T finded = list[0]; ;
        T temp;
        for (int i = 0; i < list.Count; i++)
        {
            temp = list[i];
            if (temp.name == name)
            {
                finded = temp;
            }
        }
        return finded;
    }

    public T GetNext()
    {
        if (CurInd < list.Count - 1)
            CurInd++;
        current = this[CurInd];
        return current;
    }

    public T GetPrev()
    {
        if (CurInd > 0)
            CurInd--;
        current = this[CurInd];
        return current;
    }

    public T GetId()
    {
        if (idFind > 0 && idFind < list.Count)
        {
            CurInd = idFind;
            current = this[CurInd];
            return current;
        }
        return null;
    }

    public T GetId(int id)
    {
        if (id > 0 && id < list.Count)
        {
            return this[id];
        }
        return null;
    }

    public T GetRandom()
    {
        int random = Random.Range(0, list.Count);
        return list[random];
    }

    public virtual T NewComponent()
    {
        return null;
    }

    public void ClearDataBase()
    {
        list.Clear();
        list.Add(NewComponent());
        current = list[0];
        CurInd = 0;
    }

    public void AddElement()
    {
        if (list == null)
            list = new List<T>();

        current = NewComponent();
        list.Insert(list.Count, current);
        CurInd = list.Count - 1;
    }

    public void RemoveElement()
    {
        if (CurInd > 0)
        {
            current = list[--CurInd];
            list.RemoveAt((int)(++this.CurInd));
        }
        else
        {
            if (CurInd == 0)
            {
                //current = list[currentIndex];
                list.RemoveAt((int)(++this.CurInd));
            }
            else
            {
                list.Clear();
                current = null;
            }
        }
    }

    public T this[int index]
    {
        get
        {
            if (list != null && index >= 0 && index < list.Count)
                return list[index];
            return null;
        }
        set
        {
            if (list == null)
                list = new List<T>();
            if (index >= 0 && index < list.Count && value != null)
                list[index] = value;
            else Debug.LogError("Выход за границы массива, либо переданное значение value = null!");
        }
    }
}

public abstract class DBComponent
{
    public string name;
}