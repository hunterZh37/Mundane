using UnityEngine;
using RosMessageTypes.Std;           // for std_msgs/String
using Unity.Robotics.ROSTCPConnector; // core connector

public class ROSPublisherExample : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/unity_test";

    void Start()
    {
        ros = ROSConnection.instance;
        // Optionally register topic with QoS settings
        ros.RegisterPublisher<StringMsg>(topicName);
    }

    void Update()
    {
        // Example: send once per second
        if (Time.frameCount % 60 == 0)
        {
            StringMsg msg = new StringMsg("Hello from Unity!");
            ros.Publish(topicName, msg);
        }
    }
}
