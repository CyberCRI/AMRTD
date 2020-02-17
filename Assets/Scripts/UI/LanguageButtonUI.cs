using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButtonUI : MonoBehaviour
{
    [SerializeField]
    private LanguageButtonsManagerUI manager = null;
    [SerializeField]
    private Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1.2f);

    public LocalizationManager.LANGUAGES target;
    private Vector3 initialScale;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        initialScale = this.transform.localScale;
    }

    public void onClick()
    {
        manager.selectButton(this, target);
    }

    public void setSelected(bool selected)
    {
        this.transform.localScale = selected ? selectedScale : initialScale;
    }
}
