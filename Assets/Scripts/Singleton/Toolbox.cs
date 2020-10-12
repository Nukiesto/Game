using System;
using System.Collections.Generic;
using UnityEngine;

public class Toolbox : MonoGlobalSingleton<Toolbox>
{
    // Used to track any global components added at runtime.
    private Dictionary<string, Component> m_Components = new Dictionary<string, Component>();


    // Prevent constructor use.
    protected Toolbox() { }


    private void Awake()
    {
        // Put initialization code here.
    }


    // Define all required global components here. These are hard-codded components
    // that will always be added. Unlike the optional components added at runtime.

    //private PlayerData m_PlayerData = new PlayerData();

    //public PlayerData GetPlayerData()
    //{
    //    return m_PlayerData;
    //}

    // The methods below allow us to add global components at runtime.
    // TODO: Convert from string IDs to component types.
    public Component AddGlobalComponent(string componentID, Type component)
    {
        if (m_Components.ContainsKey(componentID))
        {
            Debug.LogWarning("[Toolbox] Global component ID \""
                + componentID + "\" already exist! Returning that.");
            return GetGlobalComponent(componentID);
        }

        var newComponent = gameObject.AddComponent(component);
        m_Components.Add(componentID, newComponent);
        return newComponent;
    }


    public void RemoveGlobalComponent(string componentID)
    {
        Component component;

        if (m_Components.TryGetValue(componentID, out component))
        {
            Destroy(component);
            m_Components.Remove(componentID);
        }
        else
        {
            Debug.LogWarning("[Toolbox] Trying to remove nonexistent component ID \""
                + componentID + "\"! Typo?");
        }
    }


    public Component GetGlobalComponent(string componentID)
    {
        Component component;

        if (m_Components.TryGetValue(componentID, out component))
        {
            return component;
        }

        Debug.LogWarning("[Toolbox] Global component ID \""
    + componentID + "\" doesn't exist! Typo?");
        return null;
    }
}
/*
Все необходимые компоненты должны быть определены в самом классе toolbox. Так, например, допустим, у нас есть очень важный игрок-Монобехавиор. Мы должны создать экземпляр этого в наборе инструментов и добавить публичную функцию, чтобы позволить нам получить его.

private PlayerData m_PlayerData = new PlayerData();
 
public PlayerData GetPlayerData()
{
    return m_PlayerData;
}

Теперь для извлечения этого глобального компонента мы просто используем Toolbox.Экземпляр, за которым следует функция, определенная в наборе инструментов.

var playerData = Toolbox.Instance.GetPlayerData();
Debug.Log(playerData.CurrentLevel);

Вы также можете добавлять, получать или удалять глобальные компоненты во время выполнения. Однако эти методы используют строки для идентификаторов, которые подвержены ошибкам, поэтому используйте с катионом!

// Добавьте и получите глобальный компонент.
var playerData = Toolbox.Instance.AddGlobalComponent("PlayerData", PlayerData);
Debug.Log(playerData.CurrentLevel);

/ / получение глобального компонента.
var playerData = Toolbox.Instance.GetGlobalComponent("PlayerData");
Debug.Log(playerData.CurrentLevel);


/ / удалить существующий глобальный компонент.
Toolbox.Instance.RemoveGlobalComponent("PlayerData");
*/