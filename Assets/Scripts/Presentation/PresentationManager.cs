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
    private const string fileNamePattern = "Presentation/slides/slide-{0:00}";
    private const int fileCount = 8;
    private int _currentImageIndex = 0;
    private int currentImageIndex
    {
        get
        {
            return _currentImageIndex;
        }
        set
        {
            _currentImageIndex = value;
            string imageName = string.Format(fileNamePattern, _currentImageIndex);
            Debug.Log("loading " + imageName);
            slide.sprite = (Sprite)Resources.Load<Sprite>(imageName);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= fileCount; i++)
        {
            Debug.Log(string.Format(fileNamePattern, i));
        }
        currentImageIndex = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentImageIndex = Mathf.Clamp(currentImageIndex + 1, 1, fileCount);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentImageIndex = Mathf.Clamp(currentImageIndex - 1, 1, fileCount);
        }
    }
#endif
}
