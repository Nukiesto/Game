using UnityEngine;
using UnityEditor;


public abstract class DBBaseInspector: Editor
{
    public void GUI<T,U>(T db) where T : BaseDB<U> where U : DBComponent
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("RemoveAll!"))
        {
            db.ClearDataBase();
        }
        if (GUILayout.Button("Remove"))
        {
            db.RemoveElement();
        }
        if (GUILayout.Button("Add"))
        {
            db.AddElement();
        }
        if (GUILayout.Button("FindName"))
        {
            db.GetName();
        }
        if (GUILayout.Button("FindId"))
        {
            db.GetId();
        }
        if (GUILayout.Button("<="))
        {
            db.GetPrev();
        }
        if (GUILayout.Button("=>"))
        {
            db.GetNext();
        }

        GUILayout.EndHorizontal();
        /////////////////////////////////////////////////////////////
        GUILayout.BeginHorizontal();

        GUILayout.TextArea("Current Index: " + db.CurInd.ToString());

        GUILayout.EndHorizontal();
    }
}
