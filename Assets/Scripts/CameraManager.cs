using UnityEngine;
using UnityEngine.U2D;

public class CameraManager : MonoBehaviour
{
    #region Init
    Vector3 VectorNone = new Vector3();
    public float defaultSpeed = 15.0f;
    public float mainSpeed = 15.0f; //обычная скорость
    public float shiftAdd = 20.0f; //умножается на то, как долго держится сдвиг. В основном работает
    public float maxShift = 1000.0f; //Максимальная скорость при удержании gshift
    private float totalRun = 1.0f;

    public PixelPerfectCamera pixelCamera;
    #endregion
    #region Init Zoom
    [Range(0, 999)]
    public int minZoom = 4;
    [Range(16, 64)]
    public int normalZoom = 16;
    [Range(0, 128)]
    public int maxZoom = 64;
    [Range(1, 16)]
    public int stepZoom = 4;

    private int _zoom = 16;
    private float ratio;
    #endregion
    private void Start()
    {
        SetZoom();
    }
    void Update()
    {
        #region Zoom
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        { // forward
            _zoom += stepZoom;
            if (_zoom > maxZoom)
            {
                _zoom = maxZoom;
            }
            SetZoom();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        { // backwards
            _zoom -= stepZoom;
            if (_zoom < minZoom)
            {
                _zoom = minZoom;
            }
            SetZoom();
        }

        if (((Input.GetKey(KeyCode.LeftControl)) && (Input.GetKey(KeyCode.Equals))) || (Input.GetMouseButtonDown(2)))
        {
            _zoom = normalZoom;
            SetZoom();
        }
        if ((Input.GetKey(KeyCode.LeftShift)) && (Input.GetMouseButtonDown(2)))
        {
            _zoom = maxZoom;
            SetZoom();
        }
        if ((Input.GetKey(KeyCode.LeftControl)) && (Input.GetMouseButtonDown(2))) {
            _zoom = minZoom;
            SetZoom();
        }
        #endregion
        #region Move
        Vector3 p = GetBaseInput();
        if (p != VectorNone)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p *= mainSpeed;
            }
        }
        else
        {
            //Debug.Log("Test");
        }
        p *= Time.deltaTime;

        transform.Translate(p); 
        #endregion
    }
    private void SetZoom()
    {     
        pixelCamera.assetsPPU = _zoom;
        //изменение скорости камеры
        if (_zoom < normalZoom)
        {
            ratio = normalZoom / _zoom / 2;
            mainSpeed = defaultSpeed * ratio;
        }
        else
        {
            ratio = _zoom / normalZoom;
            mainSpeed = defaultSpeed / ratio;
        }      
    }
    private Vector3 GetBaseInput()
    { 
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}
