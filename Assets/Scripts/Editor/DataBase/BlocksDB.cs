using UnityEditor;


[CustomEditor(typeof(BlockData))]
public class BodyDB : Editor
{
    private BlockData db;

    private void Awake()
    {
        db = (BlockData)target;
    }

    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();
    //}
}

