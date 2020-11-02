using UnityEngine;

/// <summary>
/// Displays a configurable health bar for any object with a Damageable as a parent
/// </summary>
public class ClassicBar : Bar {
    public MaterialPropertyBlock matBlock;
    public MeshRenderer meshRenderer;
    public Camera mainCamera;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        matBlock = new MaterialPropertyBlock();
    }
    private void Start() {
        // Cache since Camera.main is super slow
        mainCamera = Camera.main;       
    }
    
    public override void UpdateBar() 
    {
        if (meshRenderer.enabled)
        {
            AlignCamera();
            UpdateParams();
        }       
    }
    public override void Enable()
    {
        meshRenderer.enabled = true;
        base.Enable();
    }
    public override void Disable()
    {
        meshRenderer.enabled = false;
        base.Disable();
    }

    private void UpdateParams() 
    {
        meshRenderer.GetPropertyBlock(matBlock);
        matBlock.SetFloat("_Fill", Value / MaxValue);
        meshRenderer.SetPropertyBlock(matBlock);
    }
    private void AlignCamera()
    {
        if (mainCamera != null) {
            var camXform = mainCamera.transform;
            var forward = transform.position - camXform.position;
            forward.Normalize();
            var up = Vector3.Cross(forward, camXform.right);
            transform.rotation = Quaternion.LookRotation(forward, up);
        }
    }   
}