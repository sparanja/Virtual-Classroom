using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvent : MonoBehaviour
{
    #region Events
    public static UnityAction OnTouchpadUp = null;
    public static UnityAction OnTouchpadDown = null;
    public static UnityAction<OVRInput.Controller, GameObject> OnControllerSource = null;
    #endregion

    #region Anchors
    public GameObject m_LeftAnchor;
    public GameObject m_RightAnchor;
    public GameObject m_HeadAnchor;
    #endregion

    #region Input
    private Dictionary<OVRInput.Controller, GameObject> m_ControllerSets = null;
    private OVRInput.Controller m_InputSource = OVRInput.Controller.None;
    private OVRInput.Controller m_Controller = OVRInput.Controller.None;
    private bool m_InputActive = true;
    #endregion

    void Awake()
    {
        Debug.Log("PlayerEvent Awake");
        OVRManager.HMDMounted += PlayerFound;
        OVRManager.HMDUnmounted += PlayerLost;

        m_ControllerSets = CreateControllerSets();
    }

    void OnDestroy()
    {
        Debug.Log("PlayerEvent Destroy");
        OVRManager.HMDMounted -= PlayerFound;
        OVRManager.HMDUnmounted -= PlayerLost;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //check for active input
        if (!m_InputActive) return;
        //check if a controller exists
        CheckForController();
        //check for input source
        CheckInputSource();
        //check for actual input
        Input();
    }

    private void CheckForController()
    {
        Debug.Log("PlayerEvent CheckForControllervvvvvvvv");
        OVRInput.Controller controllerCheck = m_Controller;
        //left remote
        if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
        {
            Debug.Log("ControllerConnected left remote");
            controllerCheck = OVRInput.Controller.LTrackedRemote;
        }

        //right remote
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
        {
            Debug.Log("ControllerConnected right remote");
            controllerCheck = OVRInput.Controller.RTrackedRemote;
        }

        //Headset
        if (!OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote) &&
            !OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
        {
            Debug.Log("ControllerConnected touchpad");
            controllerCheck = OVRInput.Controller.Touchpad;
        }

        //Update
        m_Controller = UpdateSource(controllerCheck, m_Controller);
        Debug.Log("PlayerEvent CheckForController^^^^^^^^");
    }

    private void CheckInputSource()
    {
        Debug.Log("PlayerEvent CheckInputSourcevvvvvvvv");
        OVRInput.Controller controllerCheck = m_Controller;
        
        //Update
        m_InputSource = UpdateSource(OVRInput.GetActiveController(), m_InputSource);
        Debug.Log("PlayerEvent CheckInputSource^^^^^^^^");
    }

    private void Input()
    {
        // Touchpad down
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
        {
            Debug.Log("PlayerEvent Input Button.PrimaryTouchPad Down");
            if (OnTouchpadDown != null)
                OnTouchpadDown();
        }

        // Touchpad up
        if(OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad))
        {
            Debug.Log("PlayerEvent Input Button.PrimaryTouchPad Up");
            if (OnTouchpadUp != null)
                OnTouchpadUp();
        }
    }

    private OVRInput.Controller UpdateSource(OVRInput.Controller check, OVRInput.Controller previous)
    {
        Debug.Log("PlayerEvent UpdateSourcevvvvvvvv");
        // If values are the same, return
        Debug.Log("check == previous?"+ (check == previous).ToString());
        if (check == previous)
            return previous;
        // Get controller object
        GameObject controllerObject = null;
        m_ControllerSets.TryGetValue(check, out controllerObject);
        // If no controller, set to the head
        if (controllerObject == null)
            controllerObject = m_HeadAnchor;
        // Send out event
        if (OnControllerSource != null)
            OnControllerSource(check, controllerObject);
        // Return new value
        Debug.Log("controllerObject == null?" + (controllerObject == null).ToString());
        Debug.Log("OnControllerSource != null?" + (OnControllerSource != null).ToString());
        Debug.Log("PlayerEvent UpdateSource^^^^^^^^");
        return check;
    }

    private void PlayerFound()
    {
        m_InputActive = true;
    }

    private void PlayerLost()
    {
        m_InputActive = false;
    }

    private Dictionary<OVRInput.Controller, GameObject> CreateControllerSets()
    {
        Dictionary<OVRInput.Controller, GameObject> newSets = new Dictionary<OVRInput.Controller, GameObject>()
        {
            {OVRInput.Controller.LTrackedRemote, m_LeftAnchor },
            {OVRInput.Controller.RTrackedRemote, m_RightAnchor },
            {OVRInput.Controller.LTouch, m_LeftAnchor },
            {OVRInput.Controller.RTouch,m_RightAnchor },
            {OVRInput.Controller.Touchpad,m_HeadAnchor }
        };
        return newSets;
    }
}
