using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float panSpeed = 0f;
    [SerializeField]
    private float panBorderThickness = 0f;
    [SerializeField]
    private bool doMovement = true;
    [SerializeField]
    private float scrollSpeed = 0f;
    [SerializeField]
    private float minY = 0f;
    [SerializeField]
    private float maxY = 0f;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            doMovement = !doMovement;
        }

        if (doMovement)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                this.transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= panBorderThickness)
            {
                this.transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                this.transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= panBorderThickness)
            {
                this.transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Vector3 pos = this.transform.position;
            pos.y -= scroll * scrollSpeed * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            this.transform.position = pos;
        }
    }
}
