using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor; // sensor_msgs/JointState

/// <summary>
/// Subscribes to /joint_states and (optionally) drives a robot built with ArticulationBody.
/// Map Unity joint names to ArticulationBodies in Inspector.
/// </summary>
public class RosJointStateSubscriber : MonoBehaviour
{
    [Header("ROS")]
    [SerializeField] string jointStatesTopic = "/joint_states";

    [Header("Optional: Drive a Rig")]
    [Tooltip("Map joint names (from ROS) to the corresponding ArticulationBody in Unity.")]
    public List<JointBinding> jointBindings = new();

    // Latest joint values by name
    Dictionary<string, double> latestPositions = new();
    Dictionary<string, double> latestVelocities = new();
    Dictionary<string, double> latestEfforts = new();

    // Quick lookup to avoid per-frame list scans
    Dictionary<string, ArticulationBody> bindingLookup = new();

    ROSConnection ros;

    [System.Serializable]
    public class JointBinding
    {
        public string rosJointName;            // Must match names in JointStateMsg.name[]
        public ArticulationBody articulation;  // Target joint drive in Unity
        [Tooltip("Degrees if your Unity rig expects degrees; leave false if radians.")]
        public bool convertRadiansToDegrees = false;
        [Tooltip("Which drive to set on the ArticulationBody.")]
        public DriveTarget driveTarget = DriveTarget.X;

        public enum DriveTarget { X, Y, Z }
    }

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<JointStateMsg>(jointStatesTopic, OnJointState);

        foreach (var b in jointBindings)
        {
            if (b != null && b.articulation != null && !bindingLookup.ContainsKey(b.rosJointName))
                bindingLookup.Add(b.rosJointName, b.articulation);
        }
    }

    void OnJointState(JointStateMsg msg)
    {
        Debug.Log($"Received JointState with {msg.name.Length} joints");

        for (int i = 0; i < msg.name.Length; i++)
        {
            string joint = msg.name[i];
            double pos = (i < msg.position.Length) ? msg.position[i] : double.NaN;

            Debug.Log($"{joint}: position={pos}");
            latestPositions[joint] = pos;
        }
    }

    void Update()
    {
        // Drive the Unity rig (optional). If you only need the values, remove this block.
        foreach (var b in jointBindings)
        {
            if (b == null || b.articulation == null || string.IsNullOrEmpty(b.rosJointName))
                continue;

            if (!latestPositions.TryGetValue(b.rosJointName, out double pos))
                continue;

            // Convert if your Unity setup expects degrees
            float target = b.convertRadiansToDegrees ? Mathf.Rad2Deg * (float)pos : (float)pos;

            var drive = b.articulation.xDrive; // default to X, we may swap below
            switch (b.driveTarget)
            {
                case JointBinding.DriveTarget.X:
                    drive = b.articulation.xDrive;
                    drive.target = target;
                    b.articulation.xDrive = drive;
                    break;

                case JointBinding.DriveTarget.Y:
                    drive = b.articulation.yDrive;
                    drive.target = target;
                    b.articulation.yDrive = drive;
                    break;

                case JointBinding.DriveTarget.Z:
                    drive = b.articulation.zDrive;
                    drive.target = target;
                    b.articulation.zDrive = drive;
                    break;
            }
        }
    }

    // Public getters if other scripts need the latest joint values
    public bool TryGetJointPosition(string jointName, out double position) =>
        latestPositions.TryGetValue(jointName, out position);

    public bool TryGetJointVelocity(string jointName, out double velocity) =>
        latestVelocities.TryGetValue(jointName, out velocity);

    public bool TryGetJointEffort(string jointName, out double effort) =>
        latestEfforts.TryGetValue(jointName, out effort);
}
