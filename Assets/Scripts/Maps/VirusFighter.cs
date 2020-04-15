//#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WhiteBloodCellMovement))]
public class VirusFighter : MonoBehaviour
{
    [SerializeField]
    private float hitsMaxCount = 10f;
    public float _hitsLeft = 0f;
    private float hitsLeft {
        get
        {
            return _hitsLeft;
        }
        set
        {
            _hitsLeft = value;
            updateHealthIndicators();
        }
    }
    [SerializeField]
    private string[] targetTags = null;
    [SerializeField]
    private WhiteBloodCellMovement m_wbcm = null;
    [SerializeField]
    private Renderer _renderer = null;
    private MaterialPropertyBlock _propBlock = null;
    [SerializeField]
    private Color colorHealthy = Color.white;
    [SerializeField]
    private Color colorWounded = Color.grey;
    [SerializeField]
    private Image healthBar = null;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_propBlock);
        hitsLeft = hitsMaxCount;
    }

    private void updateHealthIndicators()
    {
        healthBar.fillAmount = hitsLeft/hitsMaxCount;
        _propBlock.SetColor("_Color", Color.Lerp(colorHealthy, colorWounded, healthBar.fillAmount));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    void OnTriggerEnter(Collider collider)
    {
        bool collides = false;
        for (int i = 0; i < targetTags.Length; i++)
        {
            if (collider.tag == targetTags[i])
            {
                hitsLeft--;
                Virus virus = collider.gameObject.GetComponent<Virus>();

                if (0 == hitsLeft)
                {
                    // leave map + destroy virus
                    m_wbcm.absorb(virus);
                }
                else
                {
                    // destroy virus manually
                    virus.getAbsorbed(this.transform);
                }
            }
        }
    }
}