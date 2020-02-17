#define PRESENTATIONMODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresentationManager : MonoBehaviour
{
    [SerializeField]
    private Image slide = null;

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
                _currentImageIndex = Mathf.Clamp(value, 0, sprites.Length-1);
                slide.sprite = (Sprite)sprites[_currentImageIndex];
            }
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
            currentImageIndex = Mathf.Clamp(currentImageIndex + 1, 0, sprites.Length-1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentImageIndex = Mathf.Clamp(currentImageIndex - 1, 0, sprites.Length-1);
        }
    }
#endif
}
