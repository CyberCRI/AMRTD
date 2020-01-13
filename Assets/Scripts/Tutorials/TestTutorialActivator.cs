using UnityEngine;
using UnityEngine.UI;

public class TestTutorialActivator : MonoBehaviour
{
    public GameObject tutorial;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            tutorial.SetActive(true);
        }
    }
}