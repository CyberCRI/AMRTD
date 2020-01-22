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
    private GameObject arrowSystem = null;

    private ExternalOnPressButton _target = null;

    private bool _isClicksBlocked = false;
    private Vector3 _baseFocusMaskScale, _baseHoleScale;
    [SerializeField]
    private Advisor _advisor;

    private Camera _camera;

    public enum Quadrant
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }

    [SerializeField]
    private RectTransform topRight = null;

    // test code
#if DEVMODE
    public ExternalOnPressButton testClickable;
    public GameObject testObject;
    public string testObjectName = "WeakEnemyTextured1(Clone)";
    public Vector3 testPosition;
    public string testAdvisorTextKey = null;
    public GameObject tutorial;
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

    public void focusOn(
        ExternalOnPressButton target
        , Callback callback = null
        , string advisorTextKey = null
        , bool scaleToComponent = false
        , bool showButton = false
        )
    {
        // Debug.Log(this.GetType() + " focusOn0 ExternalOnPressButton " + target.name);
        focusOn(target, Vector3.zero, callback, advisorTextKey, scaleToComponent, showButton);
    }

    public void focusOn(
        ExternalOnPressButton target
        , Vector3 manualScale
        , Callback callback = null
        , string advisorTextKey = null
        , bool scaleToComponent = false
        , bool showButton = false
        )
    {
        if (null != target)
        {
            // Debug.Log(this.GetType() + " focusOn ExternalOnPressButton " + target.name);
            float scaleFactor = computeScaleFactor(scaleToComponent, target.transform.localScale, manualScale);

            //focusOn(getScreenPosition(_camera, target.transform.position), callback, scaleFactor, false, advisorTextKey);
            focusOn(
                getScreenPosition(target.transform.position)
                , callback
                , scaleFactor
                , advisorTextKey
                , showButton
                );
            _target = target;
        }
        else
        {
            // Debug.Log(this.GetType() + " focusOn null");
        }
    }

    public void focusOn(
        GameObject go
        , Callback callback = null
        , string advisorTextKey = null
        , bool scaleToComponent = false
        , bool showButton = false
        )
    {
        // Debug.Log(this.GetType() + " focusOn(GameObject go, Callback callback,...)");
        focusOn(
            go
            , Vector3.zero
            , callback
            , advisorTextKey
            , scaleToComponent
            , showButton
            );
    }

    public void focusOn(
        GameObject go
        , Vector3 manualScale
        , Callback callback = null
        , string advisorTextKey = null
        , bool scaleToComponent = false
        , bool showButton = false
        )
    {
        // Debug.Log(this.GetType() + " focusOn(GameObject go, Vector3 manualScale,...) - will compute GO position");
        // Debug.Log(this.GetType() + " GO " + go.name + " position " + go.transform.position);
        // Debug.Log(this.GetType() + " GO " + go.name + " localPosition " + go.transform.localPosition);

        if (null != go)
        {

            float scaleFactor = computeScaleFactor(scaleToComponent, go.transform.localScale, manualScale);

            bool isInterfaceObject = (this.gameObject.layer == go.layer);
            // Debug.Log(this.GetType() + " isInterfaceObject=" + isInterfaceObject + " because layer=" + go.layer);

            if (!isInterfaceObject)
            {
#if DEVMODE
                Debug.Log(this.GetType() + " !isInterfaceObject");
#endif
                focusOn(
                    getScreenPosition(go.transform.position)
                    , callback
                    , scaleFactor
                    , advisorTextKey
                    , showButton
                    );
            }
            else
            {
#if DEVMODE
                Debug.Log(this.GetType() + " isInterfaceObject");
#endif
                focusSystem.transform.SetParent(go.transform);
                focusSystem.anchoredPosition = Vector2.zero;
                focusSystem.transform.SetParent(this.transform);

                complete(callback, advisorTextKey, showButton);
            }
        }
        else
        {
            Debug.LogWarning(this.GetType() + " focusOn: game object is null");
        }
    }

    private Vector2 getScreenPosition(Vector3 gameObjectPosition)
    {
#if DEVMODE
        Debug.Log("getScreenPosition of " + gameObjectPosition);
#endif
        Vector2 screenPosition = Vector2.zero;

        if (null != gameObjectPosition)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(gameObjectPosition);
            //Vector2 screenPosition = new Vector2(screenPoint.x / _camera.pixelWidth - 0.5f, screenPoint.y / _camera.pixelHeight - 0.5f);
            screenPosition = new Vector2(screenPoint.x, screenPoint.y);
        }
        else
        {
            Debug.LogWarning("getScreenPosition: null gameObjectPosition");
        }

#if DEVMODE
        Debug.Log("FocusMaskManager getScreenPosition(" + gameObjectPosition + ")\nresult=" + screenPosition);
#endif
        return screenPosition;
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

    public void focusOn(
        Vector2 position
        , Callback callback = null
        , float scaleFactor = 1f
        , string advisorTextKey = null
        , bool showButton = false
        )
    {
        // Debug.Log(this.GetType() + " final stack focusOn(" + position + ")");
        _target = null;
        focusSystem.position = new Vector3(position.x, position.y, focusSystem.position.z);
        
        
        complete(callback, advisorTextKey, showButton);
    }

    private void complete(Callback callback, string advisorTextKey, bool showButton)
    {        
        reset(true);

        Vector3 screenPos = _camera.WorldToScreenPoint(focusSystem.position);
        Quadrant quadrant = getQuadrant(new Vector2(screenPos.x, screenPos.y));

        rotateArrowAt(quadrant);
        _callback = callback;
        if (!string.IsNullOrEmpty(advisorTextKey))
        {
            _advisor.setSpeechBubble(quadrant, advisorTextKey, showButton);
            _advisor.gameObject.SetActive(true);
        }
        else
        {
            _advisor.gameObject.SetActive(false);
        }
        show(true);
    }

    public void blockClicks(bool block)
    {
        _isClicksBlocked = block;
        // Debug.Log("NOT IMPLEMENTED - blockClicks");
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

        GameManager.instance.setPause(keepDisplayed);
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
        /*
        if (Input.GetKeyDown(KeyCode.Home))
        {
            testClickable = GameObject.Find("CraftButton").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            testClickable = GameObject.Find("CraftButton").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            testClickable = GameObject.Find("AvailableDisplayedDTER").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            focusOn(testPosition);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            focusOn(GameObject.Find("CraftButton"), null, null, true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            focusOn(Character.get().gameObject, null, null, false);
        }
        */
        if (Input.GetKeyDown(KeyCode.Home))
        {
            //GameObject testObject = GameObject.Find("TestRock12");
            focusOn(testObject, null, testAdvisorTextKey, true, true);
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            testObject = GameObject.Find(testObjectName);
            focusOn(testObject, null, testAdvisorTextKey, true, true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.gameObject.AddComponent<TestTutorial>();
        }
    }
#endif

    public Quadrant getQuadrant(Vector2 screenPos)
    {

        // WorldToScreenPoint should be in [0,pixelWidth]x[0,pixelHeight] but isn't unfortunately! 
        //bool top = screenPos.y > _camera.pixelHeight / 2;
        //bool left = screenPos.x < _camera.pixelWidth / 2;

        Vector3 maxScreenPos = _camera.WorldToScreenPoint(topRight.transform.position);
        bool top = screenPos.y > maxScreenPos.y / 2;
        bool left = screenPos.x < maxScreenPos.x / 2;

#if DEVMODE
         Debug.Log("FocusMaskManager getQuadrant(" + screenPos + ") "
         + "\nscreenpos=" + screenPos + ", left=" + left + ", top=" + top
         + "\nscreenPos.x=" + screenPos.x + ", screenPos.y=" + screenPos.y
         + "\n_camera.pixelWidth=" + _camera.pixelWidth + ", _camera.pixelHeight=" + _camera.pixelHeight
         + "\ntopRight.transform.position=" + topRight.transform.position
         + "\nmaxScreenPos.x=" + maxScreenPos.x + ", maxScreenPos.y=" + maxScreenPos.y
         );
#endif

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

    private void rotateArrowAt(Quadrant quadrant)
    {
        float rotZ = 0f;

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
        // Debug.Log("rotateArrowAt(" + position + ") = " + rotZ);
        arrowSystem.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    public void stopFocusOn()
    {
        // Debug.Log("stopFocusOn");
        reset(false);
    }

    public void linkCommon(Camera camera)
    {
        _camera = camera;
    }
}
