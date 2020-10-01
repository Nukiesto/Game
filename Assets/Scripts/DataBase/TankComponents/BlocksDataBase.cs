//using UnityEngine;

//[CreateAssetMenu(menuName = "Blocks/BlocksDataBase", fileName = "BlocksDataBase")]
//public class BlocksDataBase : BaseDB<BlockData>
//{
//    public override BlockData NewComponent()
//    {
//        return new BlockData();
//    }
//}

//[System.Serializable]
//public class BlockData : DBComponent
//{
//    [Header("Основные параметры")]
//    public Sprite sprite;
//    public BaseBlockScript script;

//    public bool HasScript() => script != null;
//}