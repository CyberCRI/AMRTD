#define DEVMODE
using UnityEngine;
using UnityEngine.UI;

public class FocusMaskManager : MonoBehaviour
{
    public static FocusMaskManager instance = null;

    public const string waveTimerTextGOName = "WaveTimerText";
    public const string resistanceBarGOName = "ResistanceBar";
    public const string lifeBarGOName = "LifeBar";

    public delegate void FocusEvent();
    //public static event FocusEvent onFocusOn;

    [SerializeField]
    private RectTransform focusSystem = null;
    [SerializeField]
    private GameObject focusMask = null;
    [SerializeField]
    private GameObject hole = null;
    [SerializeField]
    private GameObject arrowGO = null;

    private ExternalOnPressButton _target = null;

    private bool _isClicksBlocked = false;
    private Vector3 _baseFocusMaskScale, _baseHoleScale;
    [SerializeField]
    private Advisor _advisor;
    [SerializeField]
    private Camera _interfaceCamera;
    [SerializeField]
    private Camera _worldCamera;

    // test code
#if DEVMODE
    public ExternalOnPressButton testClickable;
    public GameObject testObject;
    public Vector3 testPosition;
#endif

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            _baseFocusMaskScale = focusMask.transform.localScale;
            _baseHoleScale = hole.transform.localScale;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        reset(false);
    }

    public delegate void Callback();
    private Callback _callback;

    public void focusOn(ExternalOnPressButton target, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        // Debug.Log(this.GetType() + " focusOn0 ExternalOnPressButton " + target.name);
        focusOn(target, Vector3.zero, callback, advisorTextKey, scaleToComponent);
    }

    public void focusOn(ExternalOnPressButton target, Vector3 manualScale, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        if (null != target)
        {
            // Debug.Log(this.GetType() + " focusOn ExternalOnPressButton " + target.name);
            float scaleFactor = computeScaleFactor(scaleToComponent, target.transform.localScale, manualScale);

            //focusOn(getScreenPosition(_interfaceCamera, target.transform.position), callback, scaleFactor, false, advisorTextKey);
            focusOn(getScreenPosition(_interfaceCamera, target.transform.position), callback, scaleFactor, false, advisorTextKey);
            _target = target;
        }
        else
        {
            // Debug.Log(this.GetType() + " focusOn null");
        }
    }

    public void focusOn(GameObject go, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        // Debug.Log(this.GetType() + " focusOn(GameObject go, Callback callback,...)");
        focusOn(go, Vector3.zero, callback, advisorTextKey, scaleToComponent);
    }

    public void focusOn(GameObject go, Vector3 manualScale, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        Debug.Log(this.GetType() + " focusOn(GameObject go, Vector3 manualScale,...) - will compute GO position");

        if (null != go)
        {

            float scaleFactor = computeScaleFactor(scaleToComponent, go.transform.localScale, manualScale);

            bool isInterfaceObject = (this.gameObject.layer == go.layer);
            // Debug.Log(this.GetType() + " isInterfaceObject=" + isInterfaceObject + " because layer=" + go.layer);
            Camera camera = _interfaceCamera;
            if (!isInterfaceObject)
            {
                Debug.Log(this.GetType() + " !isInterfaceObject");
                _worldCamera = null == _worldCamera ? GameObject.FindWithTag("MainCamera").GetComponentInChildren<Camera>() : _worldCamera;
                camera = _worldCamera;
                focusOn(getScreenPosition(camera, go.transform.position), callback, scaleFactor, !isInterfaceObject, advisorTextKey, true);
            }
            else
            {
                Debug.Log(this.GetType() + " isInterfaceObject");
                Debug.Log(this.GetType() + " GO " + go.name + " position " + go.transform.position);
                Debug.Log(this.GetType() + " GO " + go.name + " localPosition " + go.transform.localPosition);
                //focusOn(getScreenPosition(camera, go.transform.position), callback, scaleFactor, !isInterfaceObject, advisorTextKey, true);
                //this.gameObject.SetActive(true);
                focusSystem.transform.SetParent(go.transform);
                focusSystem.anchoredPosition = Vector2.zero;
                reset(true);
                rotateArrowAt(go.transform.position);
                _callback = callback;
                // activate and position the advisor
                if (!string.IsNullOrEmpty(advisorTextKey))
                {
                    //_advisor.setUpAdvisor(position, advisorTextKey, scaleFactor, showButton);
                    _advisor.gameObject.SetActive(true);
                }
                else
                {
                    _advisor.gameObject.SetActive(false);
                }
                show(true);
            }
        }
        else
        {
            Debug.LogWarning(this.GetType() + " focusOn: game object is null");
        }
    }

    float computeScaleFactor(bool scaleToComponent, Vector3 localScale, Vector3 manualScale)
    {
        Vector3 scale = Vector3.zero;
        float result = 1f;

        if (Vector3.zero != manualScale)
        {
            scale = manualScale;
        }
        else if (scaleToComponent)
        {
            scale = localScale;
        }

        if (Vector3.zero != scale)
        {
            float max = scale.x > scale.y ? scale.x : scale.y;
            result = max / _baseHoleScale.x;
        }

        result = result < 1f ? 1f : result;

        return result;
    }

    private static Vector2 getScreenPosition(Camera camera, Vector3 gameObjectPosition)
    {
        Debug.Log("getScreenPosition of " + gameObjectPosition);
        if (null != camera && null != gameObjectPosition)
        {
            // Debug.Log("FocusMaskManager getScreenPosition(" + camera.name + "," + gameObjectPosition + ")");
            Vector3 screenPoint = camera.WorldToScreenPoint(gameObjectPosition);
            //Vector2 screenPosition = new Vector2(screenPoint.x / camera.pixelWidth - 0.5f, screenPoint.y / camera.pixelHeight - 0.5f);
            Vector2 screenPosition = new Vector2(gameObjectPosition.x, gameObjectPosition.y);
            // Debug.Log("FocusMaskManager getScreenPosition(" + camera.name + "," + gameObjectPosition + ") result=" + screenPosition);
            return screenPosition;
        }
        else
        {
            Debug.LogWarning("getScreenPosition: null parameter");
            return Vector2.zero;
        }
    }

    // TODO add bool argument to force updated positioning of focus mask and arrow to prevent misplacement bugs
    // cf issue #345
    // position.x and position.y are in [0,1]
    public void focusOn(Vector2 position, Callback callback = null, float scaleFactor = 1f, bool local = true, string advisorTextKey = null, bool showButton = false)
    {
        Debug.Log(this.GetType() + " focusOn(" + position + ")");
        if (null != position
        /*
            && position.x >= -1
            && position.x <= 1
            && position.y >= -1
            && position.y <= 1
        */
            )
        {
            Debug.Log("focusOn position ok with position = " + position);

            reset(true);

            _target = null;

            Debug.Log("BEFORE focusSystem.anchoredPosition = " + focusSystem.anchoredPosition);
            Debug.Log("AFTER  focusSystem.anchoredPosition = " + position);
            //focusSystem.anchoredPosition = position;
            focusSystem.position = new Vector3(position.x, position.y, focusSystem.position.z);

/*
            if (1f != scaleFactor)
            {
                // Debug.Log(this.GetType() + " will scale focusMask=" + focusMask.transform.localScale + " and hole=" + hole.transform.localScale + " with factor=" + scaleFactor);
                focusMask.transform.localScale = scaleFactor * _baseFocusMaskScale;
                hole.transform.localScale = scaleFactor * _baseHoleScale;
                // Debug.Log(this.GetType() + " now focusMask=" + focusMask.transform.localScale + " and hole=" + hole.transform.localScale);
            }
            else
            {
                // Debug.Log(this.GetType() + " will scale back focusMask=" + focusMask.transform.localScale + " and hole=" + hole.transform.localScale);
                focusMask.transform.localScale = _baseFocusMaskScale;
                hole.transform.localScale = _baseHoleScale;
                // Debug.Log(this.GetType() + " now focusMask=" + focusMask.transform.localScale + " and hole=" + hole.transform.localScale);
            }
*/

            rotateArrowAt(position);

            _callback = callback;

            // activate and position the advisor
            if (!string.IsNullOrEmpty(advisorTextKey))
            {
                _advisor.setUpAdvisor(position, advisorTextKey, scaleFactor, showButton);
                _advisor.gameObject.SetActive(true);
            }
            else
            {
                _advisor.gameObject.SetActive(false);
            }

            show(true);
            //onFocusOn();
        }
        else
        {
            Debug.Log("Incorrect position");
        }

    }

    public void blockClicks(bool block)
    {
        _isClicksBlocked = block;
        Debug.Log("NOT IMPLEMENTED - blockClicks");
    }

    private void show(bool show)
    {
        focusSystem.gameObject.SetActive(show);
        _advisor.gameObject.SetActive(show);
    }

    public void reset(bool keepDisplayed)
    {
        // Debug.Log(this.GetType() + " reinitialize");
        this.gameObject.SetActive(true);
        show(keepDisplayed);
        //clickBlocker.SetActive(keepDisplayed);

        focusMask.transform.localScale = _baseFocusMaskScale;
        hole.transform.localScale = _baseHoleScale;
        _callback = null;
    }

    public void click()
    {
        if (!_isClicksBlocked)
        {
            if (_target)
            {
                _target.OnPress(true);
            }
            if (null != _callback)
            {
                _callback();
            }
        }
    }

    // test code
#if DEVMODE
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            testClickable = GameObject.Find("CraftButton").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        /*
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            testClickable = GameObject.Find("CraftButton").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            testClickable = GameObject.Find("AvailableDisplayedDTER").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            focusOn(testPosition);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            focusOn(GameObject.Find("CraftButton"), null, null, true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            focusOn(Character.get().gameObject, null, null, false);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameObject testObject = GameObject.Find("TestRock12");
            focusOn(testObject, null, null, false);
        }
        */
    }
#endif

    public enum Quadrant
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }

    public Quadrant getQuadrant(Vector3 pos)
    {
        Vector3 screenPos = _interfaceCamera.WorldToScreenPoint(pos);
        bool top = screenPos.y > _interfaceCamera.pixelHeight / 2;
        bool left = screenPos.x < _interfaceCamera.pixelWidth / 2;

        if (top)
        {
            if (left)
                return Quadrant.TOP_LEFT;
            else
                return Quadrant.TOP_RIGHT;
        }
        else
        {
            if (left)
                return Quadrant.BOTTOM_LEFT;
            else
                return Quadrant.BOTTOM_RIGHT;
        }
    }

    private void rotateArrowAt(Vector3 position)
    {
        float rotZ = 0f;

        Quadrant quadrant = getQuadrant(position);
        float quarterTurns = 0f;
        switch (quadrant)
        {
            case Quadrant.BOTTOM_RIGHT:
                quarterTurns = 0;
                break;
            case Quadrant.TOP_RIGHT:
                quarterTurns = 1f;
                break;
            case Quadrant.TOP_LEFT:
                quarterTurns = 2f;
                break;
            case Quadrant.BOTTOM_LEFT:
                quarterTurns = 3f;
                break;
            default:
                break;
        }
        rotZ = 45f + quarterTurns * 90f;
        Debug.Log("rotateArrowAt(" + position + ") = " + rotZ);
        arrowGO.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    public void stopFocusOn()
    {
        Debug.Log("stopFocusOn");
        reset(false);
    }
}
