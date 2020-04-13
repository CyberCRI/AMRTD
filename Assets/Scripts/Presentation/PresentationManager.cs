#define PRESENTATIONMODE
//#pragma warning disable 0219

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PresentationManager : MonoBehaviour
{
    private static PresentationManager instance = null;

    [Header("Slide")]
    [SerializeField]
    private Image slideImage = null;
    [SerializeField]
    private RectTransform slideTransform = null;
    private Vector3 slideStartLocalPosition = Vector3.zero;
    [SerializeField]
    private BoxCollider2D slideCollider = null;
    [SerializeField]
    private Rigidbody2D slideRigidbody = null;

    [Header("Cube Collider")]
    [SerializeField]
    private RectTransform cubeTransform = null;
    [SerializeField]
    private RectTransform cubeStartTransform = null;
    [SerializeField]
    private RectTransform cubeEndTransform = null;
    [SerializeField]
    private BoxCollider2D cubeCollider = null;
    [SerializeField]
    private Rigidbody2D cubeRigidbody = null;

#if PRESENTATIONMODE
    private const string folderPath = "Presentation/slides/";
    private Object[] sprites = null;
    private int _currentImageIndex = 0;
    private int currentImageIndex
    {
        get
        {
            return _currentImageIndex;
        }
        set
        {
            if (null != sprites)
            {
                if (value >= sprites.Length)
                {
                    _currentImageIndex = sprites.Length;
                    slideImage.enabled = false;
                }
                else
                {
                    _currentImageIndex = Mathf.Clamp(value, 0, sprites.Length - 1);
                    slideImage.sprite = (Sprite)sprites[_currentImageIndex];
                    slideImage.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            slideStartLocalPosition = slideTransform.localPosition;
            slideCollider.size = new Vector2(slideTransform.rect.width, slideTransform.rect.height);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sprites = Resources.LoadAll(folderPath, typeof(Sprite));
        currentImageIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentImageIndex = currentImageIndex + 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentImageIndex = currentImageIndex - 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentImageIndex = Mathf.Clamp(currentImageIndex, 0, sprites.Length - 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            slideImage.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveCube();
            slideRigidbody.simulated = true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            slideRigidbody.simulated = false;
            slideTransform.localPosition = slideStartLocalPosition;
            slideTransform.rotation = Quaternion.identity;
        }
    }

    private void moveCube()
    {
        float t = Random.Range(0f, 1f);
        cubeTransform.localPosition = t * cubeStartTransform.localPosition + (1 - t) * cubeEndTransform.localPosition;
    }

#endif
}
