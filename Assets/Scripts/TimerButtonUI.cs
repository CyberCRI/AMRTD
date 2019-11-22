using UnityEngine;
using UnityEngine.UI;

public class TimerButtonUI : MonoBehaviour
{
    void Start()
    {
        Debug.Log("add listener");
		this.gameObject.GetComponent<Button>().onClick.AddListener(click);
    }

    public void click()
    {
        Debug.Log("click");
        WaveSpawner.instance.setCountdownToZero();
    }
}
