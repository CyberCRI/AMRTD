using UnityEngine;
using UnityEngine.UI;

public class CommonLinker : MonoBehaviour
{
    [SerializeField]
    private Camera _camera = null;


    // Start is called before the first frame update
    void Start()
    {
        GameUI.instance.linkCommon(
            _camera
            );
        FocusMaskManager.instance.linkCommon(
            _camera
            );
    }
}