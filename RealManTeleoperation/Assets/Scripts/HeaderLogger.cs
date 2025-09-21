using UnityEngine;

public class HeaderLogger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameObject head;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsHeadInitialized())
        {
            return;
        }

        Vector3 headPosition = head.transform.position;
        Vector3 headRotationEuler = head.transform.eulerAngles;
        Quaternion headRotationQuaternion = head.transform.rotation;

        Debug.Log($"Head global Position: {headPosition}");
        Debug.Log($"Head global Rotation (Euler): {headRotationEuler}");
        Debug.Log($"Head global Rotation (Quaternion): {headRotationQuaternion}");

        //-- retreive local position and rotation 

        Vector3 headLocalPosition = head.transform.localPosition;
        Vector3 headLocalRotationEuler = head.transform.localEulerAngles;
        Quaternion headLocalRotationQuaternion = head.transform.localRotation;

        Debug.Log($"Head local Position: {headLocalPosition}");
        Debug.Log($"Head local Rotation (Euler): {headLocalRotationEuler}");
        Debug.Log($"Head local Rotation (Quaternion): {headLocalRotationQuaternion}");
    }

    // Util Functions
    private bool IsHeadInitialized()
    {
        if (head == null)
        {
            Debug.LogError("HeaderLogger: Head reference is not set.");
            return false;
        }
        return true;
    }
}
