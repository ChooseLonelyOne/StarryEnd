using System;
using System.Collections.Generic;
using UnityEngine;


public class RotateObstacle : MonoBehaviour
{
    [SerializeField] List<ObstacleRotate> listRotate;
    [SerializeField] List<ObstacleUpAndDown> listUpAndDown;
    [SerializeField] List<ObstacleLeftAndRight> listLeftAndRight;

    private void Start()
    {
        foreach (ObstacleRotate spikeee in listRotate)
        {
            spikeee.spike.transform.eulerAngles = new Vector3(0, 0, spikeee.startAngle);
        }

        foreach (ObstacleUpAndDown spikeee in listUpAndDown)
        {
            spikeee.startDistance = spikeee.spike.transform.position.y;
        }

        foreach (ObstacleLeftAndRight spikeee in listLeftAndRight)
        {
            spikeee.startDistance = spikeee.spike.transform.position.x;
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        Rotate();
        UpAndDown();
        LeftAndRight();
    }

    private void FixedUpdate()
    {
    }

    private float time;
    private void Rotate()
    {
        foreach (ObstacleRotate spikeee in listRotate)
        {
            if (spikeee.full != 0)
            {
                float phase = Mathf.Sin(time / (spikeee.angle));
                spikeee.spike.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, phase * (spikeee.full / 2)));
            }

            if (spikeee.full == 0)
            {
                spikeee.spike.transform.Rotate(0, 0, spikeee.angle / -2);
            }
        }
    }

    private float offsetY;
    private void UpAndDown()
    {
        foreach (ObstacleUpAndDown spikeee in listUpAndDown)
        {
            Transform trans = spikeee.spike.transform;
            offsetY = spikeee.distance * Mathf.Sin(time * spikeee.speed);

            trans.position = new Vector3(trans.position.x, (spikeee.startDistance + offsetY), 0);
        }
    }

    private float offsetX;
    private void LeftAndRight()
    {
        foreach (ObstacleLeftAndRight spikeee in listLeftAndRight)
        {
            Transform trans = spikeee.spike.transform;
            offsetX = spikeee.distance * Mathf.Sin(time * spikeee.speed);

            trans.position = new Vector3((spikeee.startDistance + offsetX), trans.position.y, 0);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < listUpAndDown.Count; i++)
        {
            Transform transY = listUpAndDown[i].spike.transform;
            SpriteRenderer rendY = transY.GetComponent<SpriteRenderer>();
            Gizmos.color = Color.red;
            Gizmos.DrawLine
                (
                new Vector3(transY.position.x, transY.position.y + listUpAndDown[i].distance + (rendY.size.y / 2), 0), 
                new Vector3(transY.position.x, transY.position.y - listUpAndDown[i].distance - (rendY.size.y / 2), 0)
                );
        }

        for (int i = 0; i < listLeftAndRight.Count; i++)
        {
            Transform transX = listLeftAndRight[i].spike.transform;
            SpriteRenderer rendX = transX.GetComponent<SpriteRenderer>();
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine
                (
                new Vector3(transX.position.x + listLeftAndRight[i].distance + (rendX.size.x / 2), transX.position.y, 0),
                new Vector3(transX.position.x - listLeftAndRight[i].distance - (rendX.size.x / 2), transX.position.y, 0)
                );
        }
    }
}

[Serializable]
public class ObstacleRotate
{
    public GameObject spike;
    public float angle;
    public float startAngle;
    public float full;
}

[Serializable]
public class ObstacleUpAndDown
{
    public GameObject spike;
    public float speed; 
    public float distance;
    public float startDistance;
}

[Serializable]
public class ObstacleLeftAndRight
{
    public GameObject spike;
    public float speed;
    public float distance;
    public float startDistance;
}



