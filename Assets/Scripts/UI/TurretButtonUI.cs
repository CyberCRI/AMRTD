//#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.UI;

public class TurretButtonUI : MonoBehaviour
{
    [SerializeField]
    private Text description = null;


    // Start is called before the first frame update
    void Start()
    {
#if VERBOSEDEBUG
        string[] split = this.gameObject.name.Split('_');
        description.text = split[split.Length-1];
        
        description.transform.parent.gameObject.SetActive(split.Length > 1);
#else
        description.transform.parent.gameObject.SetActive(false);
#endif
    }
}
