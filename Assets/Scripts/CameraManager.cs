using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{

    public enum Mode { Player, Cursor };

    public Mode face; // вектор смещения, относительно "лица" персонажа или положения курсора
    public float smooth = 2.5f; // сглаживание при следовании за персонажем
    public float offset; // значение смещения (отключить = 0)
    public Bounds bounds; // спрайт, в рамках которого будет перемещаться камера
    public bool useBounds = true; // использовать или нет, границы для камеры

    #region Init Zoom
    [Header("Настройки зума:")]
    [Range(0, 999)]
    public int minZoom = 1;
    [Range(1, 64)]
    public int normalZoom = 5;
    [Range(0, 128)]
    public int maxZoom = 10;
    [Range(1, 4)]
    public int stepZoom = 4;
    [SerializeField]
    private int _zoom = 5;
    //private float ratio;
    #endregion

    private Transform target;
    private Vector3 min, max, direction;
    private static CameraManager _inst;
    private Camera cam;
    
    private void Awake()
    {
        _inst = this;
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        CalculateBounds();
        FindTarget();
    }
    private void Start()
    {
        _zoom = normalZoom;
        SetZoom();
        CalculateBounds();
    }
    private void Update()
    {
        UpdateZoom();
    }
    private void LateUpdate()
    {
        if (target)
        {
            Follow();
        }
    }

    #region Public
    public static void SetTarget(Transform target)
    {
        _inst.target = target;
    }

    // переключатель, для использования из другого класса
    public static void UseCameraBounds(bool value)
    {
        _inst.UseCameraBounds_inst(value);
    }

    public static void FindTarget()
    {
        _inst.FindPlayer_inst();
    }

    // если в процессе игры, было изменено разрешение экрана
    // или параметр "Orthographic Size", то следует сделать вызов данной функции повторно
    public static void CalculateBounds()
    {
        _inst?.CalculateBounds_inst();
    }
    #endregion

    #region Private
    //zoom
    private void UpdateZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        { // forward
            _zoom += stepZoom;
            if (_zoom > maxZoom)
            {
                _zoom = maxZoom;
            }
            SetZoom();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        { // backwards
            _zoom -= stepZoom;
            if (_zoom < minZoom)
            {
                _zoom = minZoom;
            }
            SetZoom();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(2))
        {
            _zoom = normalZoom;
            SetZoom();
        }
        if ((Input.GetKey(KeyCode.LeftShift)) && (Input.GetMouseButtonDown(2)))
        {
            _zoom = maxZoom;
            SetZoom();
        }
        if ((Input.GetKey(KeyCode.LeftControl)) && (Input.GetKey(KeyCode.LeftShift)) && (Input.GetMouseButtonDown(2)))
        {
            _zoom = minZoom;
            SetZoom();
        }
    }
    private void SetZoom()
    {
        cam.orthographicSize = _zoom;
        CalculateBounds();
    }
    //public
    private void CalculateBounds_inst()
    {
        if (this.bounds == null) return;
        Bounds bounds = Camera2DBounds();
        min = bounds.max + this.bounds.min;
        max = bounds.min + this.bounds.max;
    }
    private void FindPlayer_inst()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (target)
        {
            if (face == Mode.Player) direction = target.right; else direction = (Mouse() - target.position).normalized;
            Vector3 position = target.position + direction * offset;
            position.z = transform.position.z;
            transform.position = MoveInside(position, new Vector3(min.x, min.y, position.z), new Vector3(max.x, max.y, position.z));
        }
    }
    private void UseCameraBounds_inst(bool value)
    {
        useBounds = value;
    }

    private void Follow()
    {
        if (face == Mode.Player) direction = target.right; else direction = (Mouse() - target.position).normalized;
        Vector3 position = target.position + direction * offset;
        position.z = transform.position.z;
        position = MoveInside(position, new Vector3(min.x, min.y, position.z), new Vector3(max.x, max.y, position.z));
        transform.position = Vector3.Lerp(transform.position, position, smooth * Time.deltaTime);
    }

    private Bounds Camera2DBounds()
    {
        float height = cam.orthographicSize * 2;
        return new Bounds(Vector3.zero, new Vector3(height * cam.aspect, height, 0));
    }

    private Vector3 MoveInside(Vector3 current, Vector3 pMin, Vector3 pMax)
    {
        if (!useBounds || bounds == null) return current;
        current = Vector3.Max(current, pMin);
        current = Vector3.Min(current, pMax);
        return current;
    }

    private Vector3 Mouse()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = -transform.position.z;
        return cam.ScreenToWorldPoint(mouse);
    }
    #endregion
}