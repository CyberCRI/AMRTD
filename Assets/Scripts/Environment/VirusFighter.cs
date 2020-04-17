//#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WhiteBloodCellMovement))]
public class VirusFighter : MonoBehaviour
{
    [SerializeField]
    private float hitsMaxCount = 10f;
    public float _hitsLeft = 0f;
    
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
}