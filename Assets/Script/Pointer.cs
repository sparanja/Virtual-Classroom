using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pointer : MonoBehaviour
{
    public float m_Distance = 10.0f;
    public LineRenderer m_LineRender = null;
    public LayerMask m_EverythingMask = 0;
    public LayerMask m_InteractableMask = 0;
    public UnityAction<Vector3, GameObject> OnPointerUpdate = null;

    private Transform m_CurrentOrigin = null;
    private GameObject m_CurrentObject = null;

    // Start is called before the first frame update
    void Start()
    {
        SetLineColor();
    }

    private void Awake()
    {
        PlayerEvent.OnControllerSource += UpdateOrigin;
        PlayerEvent.OnTouchpadDown += ProcessTouchpadDown;
    }

    private void OnDestroy()
    {
        PlayerEvent.OnControllerSource -= UpdateOrigin;
        PlayerEvent.OnTouchpadDown -= ProcessTouchpadDown;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hitPoint = UpdateLine();

        m_CurrentObject = UpdatePointerStatus();

        if (OnPointerUpdate != null)
            OnPointerUpdate(hitPoint, m_CurrentObject);
    }

    private Vector3 UpdateLine()
    {
        // Create ray
        RaycastHit hit = CreateRaycast(m_EverythingMask);
        // Default end
        Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
        //Check hit
        if (hit.collider != null)
            endPosition = hit.point;
        // Set position
        m_LineRender.SetPosition(0, m_CurrentOrigin.position);
        m_LineRender.SetPosition(1, endPosition);

        return endPosition;
    }

    private void UpdateOrigin(OVRInput.Controller controller, GameObject controllerObject)
    {
        Debug.Log("Pointer UpdateOriginvvvvvvvv");
        // Set origin of pointer
        m_CurrentOrigin = controllerObject.transform;

        Debug.Log("controller==OVRInput.Controller.Touchpad?"+(controller == OVRInput.Controller.Touchpad).ToString());
        // Is the laser visible?
        if (controller==OVRInput.Controller.Touchpad)
        {
            m_LineRender.enabled = false;
        }
        else
        {
            m_LineRender.enabled = true;
        }
        Debug.Log("Pointer UpdateOrigin^^^^^^^^");
    }

    private GameObject UpdatePointerStatus()
    {
        // Create ray
        RaycastHit hit = CreateRaycast(m_InteractableMask);

        // Check hit
        if (hit.collider)
            return hit.collider.gameObject;

        // Return
        return null;
    }

    private RaycastHit CreateRaycast(int layer)
    {
        RaycastHit hit;
        Ray ray = new Ray(m_CurrentOrigin.position,m_CurrentOrigin.forward);
        Physics.Raycast(ray, out hit, m_Distance, layer);
        return hit;
    }

    private void SetLineColor()
    {
        if (!m_LineRender)
            return;

        Color endColor = Color.cyan;
        endColor.a = 0.0f;

        m_LineRender.endColor = endColor;
    }

    private void ProcessTouchpadDown()
    {
        if (!m_CurrentObject) return;

        Interactable interactable = m_CurrentObject.GetComponent<Interactable>();
        interactable.Pressed();
    }


}
