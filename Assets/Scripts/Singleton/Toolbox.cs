using System;
using System.Collections.Generic;
using UnityEngine;

namespace Singleton
{
    public enum ComponentType
    {
    }
    public class Toolbox : MonoGlobalSingleton<Toolbox>
    {
        // Used to track any global components added at runtime.
        private Dictionary<ComponentType, UnityEngine.Component> m_Components = new Dictionary<ComponentType, UnityEngine.Component>();
        
        // Prevent constructor use.
        protected Toolbox() { }
        
        private void Awake()
        {
            AddCompon(ref mGameSceneManager);
            AddCompon(ref mFpscounter).enabled = false;
            AddCompon(ref mGameManager);
            AddCompon(ref mWorldSaver);
            AddCompon(ref mEntityManager);
            AddCompon(ref mItemManager);
        }

        private T AddCompon<T>(ref T m) where T : UnityEngine.Component
        {
            m = gameObject.AddComponent<T>();
            return m;
        }

        public FpsCounter mFpscounter;
        public GameManager mGameManager;
        public GameSceneManager mGameSceneManager;
        public WorldSaver mWorldSaver;
        public ItemManager mItemManager;
        public EntityManager mEntityManager;
        
        public Component AddGlobalComponent(ComponentType componentTypeId, Type component)
        {
            if (m_Components.ContainsKey(componentTypeId))
            {
                Debug.LogWarning("[Toolbox] Global component ID \""
                    + componentTypeId + "\" already exist! Returning that.");
                return GetGlobalComponent(componentTypeId);
            }

            var newComponent = gameObject.AddComponent(component);
            m_Components.Add(componentTypeId, newComponent);
            return newComponent;
        }
        public void RemoveGlobalComponent(ComponentType componentTypeId)
        {
            if (m_Components.TryGetValue(componentTypeId, out var component))
            {
                Destroy(component);
                m_Components.Remove(componentTypeId);
            }
            else
            {
                Debug.LogWarning("[Toolbox] Trying to remove nonexistent component ID \""
                    + componentTypeId + "\"! Typo?");
            }
        }
        public Component GetGlobalComponent(ComponentType componentTypeId)
        {
            if (m_Components.TryGetValue(componentTypeId, out var component))
            {
                return component;
            }

            Debug.LogWarning("[Toolbox] Global component ID \""
                + componentTypeId + "\" doesn't exist! Typo?");
            return null;
        }
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