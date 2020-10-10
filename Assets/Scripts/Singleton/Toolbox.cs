using SavingSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Toolbox : Singleton<Toolbox>
{
    // Used to track any global components added at runtime.
    private Dictionary<string, Component> m_Components = new Dictionary<string, Component>();


    // Prevent constructor use.
    protected Toolbox() { }


    private void Awake()
    {
        //Debug.Log("Awake");
        worldSavingSystem.Init();
        // Поместите сюда код инициализации.
    }


    //Определите здесь все необходимые глобальные компоненты.Это жестко запрограммированные компоненты
    //это всегда будет добавляться.В отличие от дополнительных компонентов, добавляемых во время выполнения.
    private WorldSavingSystem worldSavingSystem = new WorldSavingSystem();

    public WorldSavingSystem GetWorldSaving()
    {
        return worldSavingSystem;
    }

    // Приведенные ниже методы позволяют нам добавлять глобальные компоненты во время выполнения.
    // ЗАДАЧА: преобразование идентификаторов строк в типы компонентов.
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