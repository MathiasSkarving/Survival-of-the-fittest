using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CowAI : MonoBehaviour
{
    public float moveSpeed;
    public float maxWalkDst;
    public float minWalkDst;
    public int minWaitTime;
    public int maxWaitTime;

    private bool taskDone;
    private bool justStarted;

    private Vector3 targetPosition;
    private int task;
    private float counter;
    private int countGoal;

    void Start()
    {
        newTask();
    }

    void Update()
    {
        if (taskDone)
        {
            newTask();
        }
        doTask();
    }

    private void doTask()
    {
        switch (task)
        {
            case 0:
                randomWalk();
                break;
            case 1:
                doNothing();
                break;
        }
    }

    public void killCow()
    {
        Destroy(gameObject);
    }


    private void newTask()
    {
        task = UnityEngine.Random.Range(0, 2); // Randomly choose between 0 and 1 (exclusive)
        taskDone = false;
        justStarted = true;
    }

    private void randomWalk()
    {
        if (justStarted)
        {
            Vector2 xzOffSet = new Vector2(
                GameManager.random4Interval(minWalkDst, maxWalkDst, -minWalkDst, -maxWalkDst),
                GameManager.random4Interval(minWalkDst, maxWalkDst, -minWalkDst, -maxWalkDst)
            );

            while (
                    xzOffSet.x + transform.position.x < -1200
                    || xzOffSet.x + transform.position.x > 1200
                    || xzOffSet.y + transform.position.z > 1200
                    || xzOffSet.y + transform.position.z < -1200
                    || GameManager.getTerrain(
                        xzOffSet.x + transform.position.x,
                        xzOffSet.y + transform.position.z
                    ) == "Water"
                    || GameManager.getTerrain(
                        xzOffSet.x + transform.position.x,
                        xzOffSet.y + transform.position.z
                    ) == "Deep Water"
            )
            {
                xzOffSet = new Vector2(
                    GameManager.random4Interval(minWalkDst, maxWalkDst, -minWalkDst, -maxWalkDst),
                    GameManager.random4Interval(minWalkDst, maxWalkDst, -minWalkDst, -maxWalkDst)
                );
            }
            Vector3 offSet = new Vector3(
                xzOffSet.x,
                GameManager.getHeight(
                    xzOffSet.x + transform.position.x,
                    xzOffSet.y + transform.position.z
                ) - transform.position.y,
                xzOffSet.y
            );

            targetPosition = transform.position + offSet;

            justStarted = false;
        }
        if (WalkTo(targetPosition))
        {
            taskDone = true;
        }
    }

    private void doNothing()
    {
        if (justStarted)
        {
            countGoal = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            counter = 0;
            justStarted = false;
        }

        counter += Time.deltaTime;
        if (counter >= countGoal)
        {
            taskDone = true;
        }
    }


    private bool WalkTo(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        if (Vector3.Magnitude(dir) < 1)
        {
            return true;
        }

        else if (
            GameManager.getTerrain(
                transform.position.x + dir.x / dir.magnitude * moveSpeed * Time.deltaTime,
                transform.position.z + dir.z / dir.magnitude * moveSpeed * Time.deltaTime
            ) == "Water"
        )
        {
            return true;
        }

        else
        {
            dir.y = 0;
            dir.Normalize();
            transform.position += dir * moveSpeed * Time.deltaTime;
            Vector3 newPos = new Vector3(
                transform.position.x,
                GameManager.getHeight(transform.position.x, transform.position.z),
                transform.position.z
            );

            transform.position = newPos;
            transform.LookAt(newPos + moveSpeed * Time.deltaTime * dir);
            return false;
        }
    }
}


