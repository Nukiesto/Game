using SimpleLocalizator;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ItemData;

[CreateAssetMenu(menuName = "Block/BlocksData", fileName = "BlockData")]
public class BlockData : ScriptableObject
{
    [Header("Основные параметры")]
    [Tooltip("Твёрдый?")]public bool isSolid = true;
    [Tooltip("Создавать предмет при уничтожении?")]public bool toCreateItem = true;
    [Tooltip("Ломаемый?")]public bool isBreackable = true;
    [Tooltip("Размещаем на фоне?")]public bool toPlaceBack = true;
    [Tooltip("Функциональный?")]public bool isInteractable = false;
    [Tooltip("Должен иметь опору?")]public bool mustHaveDownerBlock = false;
    [Tooltip("Показывать в панели песочницы")]public bool showInSandboxPanel = true;
    [Tooltip("Препятствие света?")]public bool isLightObstacle = true;
    [Tooltip("Источник света?")]public bool isLightSource = false;
    [Tooltip("Включать свет в старте?")]public bool isLightOnStart = true;
    
    public string nameBlock;
    [HideInInspector] public int Id { get; private set; }
    [Header("")]
    [Range(0, 1000)]
    public int hp;
    
    [Header("Компоненты")]
    public BaseBlockScript script;
    public BaseBlockMemory memory;

    [Header("Основной тайл")]
    public Tile tile;
    public Tile[] tileVariables;
    [Header("Световой тайл")]
    public Tile tileLightSource;
    public Tile[] tileLightSourceVariables;
    [Header("Тайл с анимацией")]
    public AnimatedTile tileAnimation;
    public AnimatedTile[] tileAnimationVariables;
    
    [Header("Предмет")]
    public TranslateString nameTranslations;
    public TranslateString descriptionTranslations;
    [HideInInspector] public Data Item { get; private set; }

    private void OnEnable()
    { 
        InitItem();
    }
    public void OnValidate()
    {
        InitItem();
    }
    public void InitItem()
    {
        Item = new Data()
        {
            showInSandboxPanel = showInSandboxPanel,
            type = ItemType.Block,
            sprite = tile?.sprite,
            description = descriptionTranslations,
            Name = nameBlock,
            name = nameTranslations,
            block = this,
            maxCount = 64
        };
    }
}