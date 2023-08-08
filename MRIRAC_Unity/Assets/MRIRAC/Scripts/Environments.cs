using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Mrirac;

public class Environments : MonoBehaviour
{
    [SerializeField]
    private string hologramObstacleTopic;

    // ROS Connector
    ROSConnection ros;

    [SerializeField]
    private GameObject robot;

    MeshFilter[] hologramObstacleMeshFilters;

    [SerializeField]
    private GameObject EnvironmentsObject;

    [SerializeField]
    private Vector3 DistanceFromRobotArm;

    [SerializeField]
    private GameObject StartAndGoalEnvironmentObjects;

    [SerializeField]
    private GameObject StartPositionPrefab;

    [SerializeField]
    private Vector3 StartPositionCoordinates;

     [SerializeField]
    private GameObject GoalPositionPrefab;

    [SerializeField]
    private Vector3 GoalPositionCoordinates;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<MeshObstaclesMsg>(hologramObstacleTopic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PublishHologramMeshes()
    {
        hologramObstacleMeshFilters = EnvironmentsObject.GetComponentsInChildren<MeshFilter>();
        MeshObstacleMsg[] hologramObstacleMsgs = new MeshObstacleMsg[hologramObstacleMeshFilters.Length];

        int obstacleIdx = 0;
        foreach (var hologramFilter in hologramObstacleMeshFilters)
        {
            MeshObstacleMsg meshObstacleMsg = new MeshObstacleMsg
            {
                mesh = MriracUtilities.MeshToMeshMsg(hologramFilter.mesh, hologramFilter.gameObject),
                mesh_pose = MriracUtilities.PoseInRobotFrameMsg(robot, hologramFilter.gameObject),
                name = hologramFilter.gameObject.name
            };

            hologramObstacleMsgs[obstacleIdx++] = meshObstacleMsg;

        }

        ros.Publish(hologramObstacleTopic, new MeshObstaclesMsg(hologramObstacleMsgs));
    }

    public void CreateStartAndGoalPosition()
    {
        GameObject StartPosition = Instantiate(StartPositionPrefab, StartAndGoalEnvironmentObjects.transform, false);
        StartPosition.name = "StartPosition";
        StartPosition.transform.localPosition = StartPositionCoordinates;

        GameObject GoalPosition = Instantiate(GoalPositionPrefab, StartAndGoalEnvironmentObjects.transform, false);
        GoalPosition.name = "GoalPosition";
        GoalPosition.transform.localPosition = GoalPositionCoordinates;

    }

    public void DestroyStartAndGoalPosition()
    {
        foreach (Transform objectTransform in StartAndGoalEnvironmentObjects.transform)
        {
            Destroy(objectTransform.gameObject);
        }
    }

    public void ActivateEnvironment(GameObject PrefabEnvironment)
    {

        foreach (Transform environmentTransform in EnvironmentsObject.transform)
        {
            Destroy(environmentTransform.gameObject);
        }

        GameObject Environment = Instantiate(PrefabEnvironment, EnvironmentsObject.transform, false);
        Environment.name = "Environment";
        Environment.transform.localPosition = DistanceFromRobotArm;

        PublishHologramMeshes();
    }

    public void DestroyEnvironment()
    {
        foreach (Transform environmentTransform in EnvironmentsObject.transform)
        {
            Destroy(environmentTransform.gameObject);
        }
    }
}
