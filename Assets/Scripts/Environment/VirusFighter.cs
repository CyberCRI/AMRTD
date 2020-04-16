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
    private WhiteBloodCellMovement m_wbcm = null;
    [SerializeField]
    private Renderer _sphericalRenderer = null;
    [SerializeField]
    private Renderer _silhouetteRenderer = null;
    private MaterialPropertyBlock _propBlock = null;
    [SerializeField]
    private Color colorHealthy = Color.white;
    [SerializeField]
    private Color colorWounded = Color.grey;
    [SerializeField]
    private Image healthBar = null;
    private Color _color;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        hitsLeft = hitsMaxCount;
    }

    private void updateHealthIndicators()
    {
        healthBar.fillAmount = hitsLeft/hitsMaxCount;
        _color = Color.Lerp(colorWounded, colorHealthy, healthBar.fillAmount);
        
        _sphericalRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", _color);
        _sphericalRenderer.SetPropertyBlock(_propBlock);
        
        _silhouetteRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", _color);
        _silhouetteRenderer.SetPropertyBlock(_propBlock);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (hitsLeft > 0)
        {
            bool collides = false;
            
            if (collider.tag == Virus.virusTag)
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
            else if (collider.tag == Enemy.enemyTag)
            {
                hitsLeft = 0;
                m_wbcm.absorb(collider.gameObject.GetComponent<EnemyMovement>());
            }
        }
    }
}