using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ControllerLogger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;




    void Start()
    {
        if (leftController == null || rightController == null)
        {
            Debug.LogError("ControllerLogger: One or both controller references are not set.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsControllerInitialized())
        {
            return;
        }

        Vector3 leftPosition = leftController.transform.position;
        Vector3 leftRotationEuler = leftController.transform.eulerAngles;
        Quaternion leftRotationQuaternion = leftController.transform.rotation;
        Vector3 rightPosition = rightController.transform.position;
        Vector3 rightRotationEuler = rightController.transform.eulerAngles;
        Quaternion rightRotationQuaternion = rightController.transform.rotation;

        Debug.Log($"Left Controller  global Position: {leftPosition}");
        Debug.Log($"Left Controller  global Rotation (Euler): {leftRotationEuler}");
        Debug.Log($"Left Controller  global Rotation (Quaternion): {leftRotationQuaternion}");
        Debug.Log($"Right Controller  global Position: {rightPosition}");
        Debug.Log($"Right Controller  global Rotation (Euler): {rightRotationEuler}");
        Debug.Log($"Right Controller  global Rotation (Quaternion): {rightRotationQuaternion}");
        //-- retreive local position and rotation 

        Vector3 leftLocalPosition = leftController.transform.localPosition;
        Vector3 leftLocalRotationEuler = leftController.transform.localEulerAngles;
        Quaternion leftLocalRotationQuaternion = leftController.transform.localRotation;
        Vector3 rightLocalPosition = rightController.transform.localPosition;
        Vector3 rightLocalRotationEuler = rightController.transform.localEulerAngles;
        Quaternion rightLocalRotationQuaternion = rightController.transform.localRotation;

        Debug.Log($"Left Controller  local Position: {leftLocalPosition}");
        Debug.Log($"Left Controller  local Rotation (Euler): {leftLocalRotationEuler}");
        Debug.Log($"Left Controller  local Rotation (Quaternion): {leftLocalRotationQuaternion}");
        Debug.Log($"Right Controller  local Position: {rightLocalPosition}");
        Debug.Log($"Right Controller  local Rotation (Euler): {rightLocalRotationEuler}");
        Debug.Log($"Right Controller  local Rotation (Quaternion): {rightLocalRotationQuaternion}");

    }

    private bool IsControllerInitialized()
    {
        return leftController != null && rightController != null;
    }
}
