using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool doMouseMovement = true;
    public float panSpeed = 30f;
    public float panBorderThickness = 10f;
    public float scrollSpeed = 10f;
    public float minY = 10f;
    public float maxY = 100f;

    private Vector3 defaultPosition;

    void Start()
    {
        defaultPosition = transform.position;
    }

    void Update()
    {
        if (GameManager.GameIsOver)
        {
            this.enabled = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            doMouseMovement = !doMouseMovement;

        if (Input.GetKeyDown(KeyCode.Tab))
            ResetCameraPosition();

        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKey("w") || (doMouseMovement && Input.mousePosition.y >= Screen.height - panBorderThickness))
        {
            transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("s") || (doMouseMovement && Input.mousePosition.y <= panBorderThickness))
        {
            transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("a") || (doMouseMovement && Input.mousePosition.x <= panBorderThickness))
        {
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("d") || (doMouseMovement && Input.mousePosition.x >= Screen.width - panBorderThickness))
        {
            transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }
    }

    private void HandleMouseInput()
    {
        if (!doMouseMovement)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scroll * 500 * scrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    private void ResetCameraPosition()
    {
        transform.position = defaultPosition;
    }
}
