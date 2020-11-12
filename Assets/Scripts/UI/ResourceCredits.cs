using UnityEngine;
using UnityEngine.UI;

public class ResourceCredits : MonoBehaviour
{
    [SerializeField]
    private string code = null;
    [SerializeField]
    private CreditLinkOpener pictureCLO = null;
    [SerializeField]
    private LocalizedText resourceNameLT = null;
    [SerializeField]
    private CreditLinkOpener resourceURLCLO = null;
    [SerializeField]
    private LocalizedText authorNameLT = null;
    [SerializeField]
    private CreditLinkOpener authorURLCLO = null;

    private const string resourceStemCode = "CREDITS.RESOURCES.";
    private const string resourceName = ".RESOURCE_NAME";
    private const string resourceURL  = ".RESOURCE_URL";
    private const string authorName   = ".AUTHOR_NAME";
    private const string authorURL    = ".AUTHOR_URL";

    private string getCode(string suffix)
    {
        return resourceStemCode + code + suffix;
    }
    
    private string getResourceNameCode()
    {
        return getCode(resourceName);
    }
    private string getResourceURLCode()
    {
        return getCode(resourceURL);
    }
    private string getAuthorNameCode()
    {
        return getCode(authorName);
    }
    private string getAuthorURLCode()
    {
        return getCode(authorURL);
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(code))
        {
            resourceNameLT.setKey(getResourceNameCode());
            string resourceCode = getResourceURLCode();
            string localizedResourceURL = LocalizationManager.instance.GetLocalizedValue(resourceCode);
            pictureCLO.setCode(resourceCode);
            pictureCLO.setURL(localizedResourceURL);
            resourceURLCLO.setCode(resourceCode);
            resourceURLCLO.setURL(localizedResourceURL);

            if (null != authorNameLT)
            {
                authorNameLT.setKey(getAuthorNameCode());
                string authorCode = getAuthorURLCode();
                authorURLCLO.setCode(authorCode);
                authorURLCLO.setURL(LocalizationManager.instance.GetLocalizedValue(authorCode));
            }
        }
    }
}