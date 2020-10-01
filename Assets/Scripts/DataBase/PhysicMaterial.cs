using UnityEngine;

[CreateAssetMenu(menuName = "PhysicMaterial")]
public class PhysicMaterial : ScriptableObject
{
    [Header("Плотность материала кг/м3")]
    public float density;
}