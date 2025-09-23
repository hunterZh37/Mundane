using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry; // PoseStamped

public class EETargetPublisher : MonoBehaviour
{
    [Header("ROS")]
    public string topic = "/ee_target";
    public string frameId = "base_link"; // robot base frame

    [Header("Transforms")]
    public Transform controller;          // XR controller pose in Unity
    public Transform baseFromUnity;       // Empty GameObject representing T_base_from_unity

    ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        
        ros.RegisterPublisher<PoseStampedMsg>(topic);
    }

    void Update()
    {
        if (!controller || !baseFromUnity) return;

        // Pose in Unity world
        Vector3 p_u = controller.position;
        Quaternion q_u = controller.rotation;

        // Convert Unity world into robot base via baseFromUnity
        // world_u --(baseFromUnity)--> base frame
        Vector3 p_b = baseFromUnity.TransformPoint(p_u);
        Quaternion q_b = baseFromUnity.rotation * q_u;

        // Convert handedness (Unity->ROS REP-103 mapping)
        Vector3 p_ros = new Vector3(+p_b.z, -p_b.x, +p_b.y);
        Quaternion q_ros = new Quaternion(+q_b.z, -q_b.x, +q_b.y, q_b.w);


        var msg = new PoseStampedMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg
            {
                frame_id = frameId,
               // stamp = ros.RosSocket.GetTimeStamp()
            },
            pose = new PoseMsg
            {
                position = new PointMsg(p_ros.x, p_ros.y, p_ros.z),
               // orientation = new QuaternionMsg(q_ros.x, q_ros.y, q_ros.z, q_ros.w)
                orientation = new QuaternionMsg(0, 0, 0, 1)
            }
        };

        ros.Publish(topic, msg);
    }
}
