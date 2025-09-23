// JointStatePrinter.cs
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class JointStatePrinter : MonoBehaviour
{
    [SerializeField] string topic = "/unity_joint_angles";

    ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<JointStateMsg>(topic, OnJointState);
        Debug.Log($"[JointStatePrinter] Subscribed to {topic}");
    }

    void OnJointState(JointStateMsg msg)
    {
        // Example: print a compact line
        // (Avoid spamming every frame in production)
        string line = $"[{msg.header.stamp.sec}.{msg.header.stamp.nanosec:D9}] ";
        for (int i = 0; i < msg.name.Length; i++)
        {
            line += $"{msg.name[i]}={msg.position[i]:F3} ";
        }
        Debug.Log(line);
    }
}
