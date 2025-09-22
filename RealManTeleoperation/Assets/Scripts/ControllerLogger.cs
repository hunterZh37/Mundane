using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

public class ControllerLogger : MonoBehaviour
{
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;

    // Topic names
    [SerializeField] private string leftPoseTopic = "/unity/controllers/left_pose_world";
    [SerializeField] private string rightPoseTopic = "/unity/controllers/right_pose_world";
    [SerializeField] private string leftEulerTopic = "/unity/controllers/left_euler_world";
    [SerializeField] private string rightEulerTopic = "/unity/controllers/right_euler_world";
    [SerializeField] private string logTopic = "/unity/controllers/log_text";

    private ROSConnection ros;

    void Start()
    {
        if (leftController == null || rightController == null)
        {
            Debug.LogError("ControllerLoggerRos: controller references are not set");
            enabled = false;
            return;
        }

        ros = ROSConnection.instance;

        // Register publishers
        ros.RegisterPublisher<PoseMsg>(leftPoseTopic);
        ros.RegisterPublisher<PoseMsg>(rightPoseTopic);
        ros.RegisterPublisher<Float32MultiArrayMsg>(leftEulerTopic);
        ros.RegisterPublisher<Float32MultiArrayMsg>(rightEulerTopic);
        ros.RegisterPublisher<StringMsg>(logTopic);
    }

    void Update()
    {
        if (ros == null) return;

        // World transforms
        Vector3 lp = leftController.transform.position;
        Quaternion lq = leftController.transform.rotation;
        Vector3 le = leftController.transform.eulerAngles;

        Vector3 rp = rightController.transform.position;
        Quaternion rq = rightController.transform.rotation;
        Vector3 re = rightController.transform.eulerAngles;

        // Local transforms
        Vector3 llp = leftController.transform.localPosition;
        Quaternion llq = leftController.transform.localRotation;
        Vector3 lle = leftController.transform.localEulerAngles;

        Vector3 rlp = rightController.transform.localPosition;
        Quaternion rlq = rightController.transform.localRotation;
        Vector3 rle = rightController.transform.localEulerAngles;

        // Keep your existing Unity logs
        // Debug.Log($"Left Controller  global Position: {lp}");
        // Debug.Log($"Left Controller  global Rotation (Euler): {le}");
        // Debug.Log($"Left Controller  global Rotation (Quaternion): {lq}");
        // Debug.Log($"Right Controller  global Position: {rp}");
        // Debug.Log($"Right Controller  global Rotation (Euler): {re}");
        // Debug.Log($"Right Controller  global Rotation (Quaternion): {rq}");

        // Debug.Log($"Left Controller  local Position: {llp}");
        // Debug.Log($"Left Controller  local Rotation (Euler): {lle}");
        // Debug.Log($"Left Controller  local Rotation (Quaternion): {llq}");
        // Debug.Log($"Right Controller  local Position: {rlp}");
        // Debug.Log($"Right Controller  local Rotation (Euler): {rle}");
        // Debug.Log($"Right Controller  local Rotation (Quaternion): {rlq}");

        // 1. Publish Pose in world space
        var leftPose = new PoseMsg(
            new PointMsg(lp.x, lp.y, lp.z),
            new QuaternionMsg(lq.x, lq.y, lq.z, lq.w)
        );
        var rightPose = new PoseMsg(
            new PointMsg(rp.x, rp.y, rp.z),
            new QuaternionMsg(rq.x, rq.y, rq.z, rq.w)
        );
        ros.Publish(leftPoseTopic, leftPose);
        ros.Publish(rightPoseTopic, rightPose);

        // 2. Publish Euler in world space
        var leftEuler = new Float32MultiArrayMsg
        {
            data = new float[] { le.x, le.y, le.z }
        };
        var rightEuler = new Float32MultiArrayMsg
        {
            data = new float[] { re.x, re.y, re.z }
        };
        ros.Publish(leftEulerTopic, leftEuler);
        ros.Publish(rightEulerTopic, rightEuler);

        // 3. Publish a single combined log line as String
        string logLine =
            $"L global P:{lp} E:{le} Q:{lq} | R global P:{rp} E:{re} Q:{rq} | " +
            $"L local P:{llp} E:{lle} Q:{llq} | R local P:{rlp} E:{rle} Q:{rlq}";
        ros.Publish(logTopic, new StringMsg(logLine));
    }
}
