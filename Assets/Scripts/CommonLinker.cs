using UnityEngine;
using UnityEngine.UI;

public class CommonLinker : MonoBehaviour
{
    [SerializeField]
    private Camera camera = null;


    // Start is called before the first frame update
    void Start()
    {
        GameUI.instance.linkCommon(
            camera
            );
        FocusMaskManager.instance.linkCommon(
            camera
            );
    }
}