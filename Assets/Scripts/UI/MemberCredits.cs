using UnityEngine;
using UnityEngine.UI;

public class MemberCredits : MonoBehaviour
{
    [SerializeField]
    private string code = null;
    [SerializeField]
    private CreditLinkOpener pictureCLO = null;
    [SerializeField]
    private LocalizedText memberNameLT = null;
    [SerializeField]
    private CreditLinkOpener memberNameCLO = null;
    [SerializeField]
    private LocalizedText memberRoleLT = null;
    [SerializeField]
    private CreditLinkOpener memberRoleCLO = null;

    private const string membersStemCode = "CREDITS.MEMBERS.";
    private const string nameSuffix = ".NAME";
    private const string roleSuffix = ".ROLE";
    private const string urlSuffix  = ".URL";

    private string getCode(string suffix)
    {
        return membersStemCode + code + suffix;
    }
    
    private string getMemberNameCode()
    {
        return getCode(nameSuffix);
    }
    private string getMemberRoleCode()
    {
        return getCode(roleSuffix);
    }
    private string getMemberURLCode()
    {
        return getCode(urlSuffix);
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(code))
        {
            memberNameLT.setKey(getMemberNameCode());
            memberRoleLT.setKey(getMemberRoleCode());

            string urlCode = getMemberURLCode();
            string localized = LocalizationManager.instance.getLocalizedValue(urlCode);
            pictureCLO.setURLCode(urlCode, localized);
            memberNameCLO.setURLCode(urlCode, localized);
            memberRoleCLO.setURLCode(urlCode, localized);
        }
    }
}