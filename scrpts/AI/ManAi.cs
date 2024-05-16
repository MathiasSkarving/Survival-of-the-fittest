using System;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ManAi : MonoBehaviour
{
    private int damage = 10;
    public float moveSpeed;
    public float maxWalkDst;
    public float minWalkDst;
    public int minWaitTime;
    public int maxWaitTime;
    private bool taskDone;
    private bool justStarted;
    private Vector3 targetPosition;
    public bool warMode = false;
    private int task;
    private float counter;
    private int countGoal;
    public int food = 0;
    public int wood = 0;
    public int stone = 0;
    private CowAI closestCow;
    private GameObject stammeparent;
    private GameObject[] otherman;
    private GameObject[] friendlyman;
    private Vector3[] friendlymanpos;
    private ManAi[] manaiscript;

    private StammeAI stammeparentscript;
    private float DistToEnemy;
    private float OldDistToEnemy;
    private int closestenemy;
    private Vector3 closestenemypos;
    private GameObject[] enemyman;
    private ManAi enemyscript;

    private Vector3 fwd;
    private RaycastHit hit;
    public float health = 100;

    private GameObject[] trees;

    int closestTree;

    private GameObject[] stones;

    int closestStone = 0;

    public String taskDoing;
    void Start()
    {
        stammeparent = transform.parent.gameObject;
        stammeparentscript = stammeparent.GetComponent<StammeAI>();
        newTask();
    }

    void Update()
    {
        if (taskDone)
        {
            newTask();
        }
        if (health <= 0)
        {
            killself();
        }

        doTask();
    }

    private void doTask()
    {
        searchotherstamme();

        switch (task)
        {
            case 0:
                randomWalk();
                taskDoing = "random walk";
                break;
            case 1:
                doNothing();
                taskDoing = "do nothing";
                break;
            case 2:
                killCow();
                taskDoing = "kill cow";
                break;
            case 3:
                chopTree();
                taskDoing = "chopping a tree";
                break;
            case 4:
                mineStone();
                taskDoing = "mining stone";
                break;
            case 5:
                gotowarnomercy();
                taskDoing = "adding Rescources";
                break;
            case 6:
                addResources();
                taskDoing = "adding Rescources";
                break;
        }
    }
    private void newTask()
    {
        if (warMode == true)
        {
            task = 5;
        }
        else if (food >= 1 || wood >= 1 || stone >= 1)
        {
            task = 6;
        }
        else
        {
            task = UnityEngine.Random.Range(0, 5);
            taskDone = false;
            justStarted = true;
        }
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
                !GameManager.validSpawn(
                    xzOffSet.x + transform.position.x,
                    xzOffSet.y + transform.position.z
                )
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

    public void setwarmode(bool warmodeIn)
    {
        warMode = warmodeIn;
    }

    private void chopTree()
    {
        if (justStarted)
        {
            trees = GameObject.FindGameObjectsWithTag("Tree");
            float oldtreedist = float.MaxValue;

            for (int i = 0; i < trees.Length; i++)
            {
                float treedist = Vector3.Distance(this.transform.position, trees[i].transform.position);

                if (treedist < oldtreedist)
                {
                    closestTree = i;
                    oldtreedist = treedist;
                }
            }
            justStarted = false;
        }

        if (trees[closestTree] != null)
        {
            if (WalkTo(trees[closestTree].transform.position))
            {
                TreeAI treescript = trees[closestTree].GetComponent<TreeAI>();
                if (treescript.removeHP(damage * Time.deltaTime))
                {
                    taskDone = true;
                    wood += 1;
                }

            }
        }
        else
        {
            taskDone = true;
        }
    }

    private void mineStone()
    {
        if (justStarted)
        {
            stones = GameObject.FindGameObjectsWithTag("Stone");
            float oldstonedist = float.MaxValue;

            for (int i = 0; i < stones.Length; i++)
            {
                float stonedist = Vector3.Distance(this.transform.position, stones[i].transform.position);

                if (stonedist < oldstonedist)
                {
                    closestStone = i;
                    oldstonedist = stonedist;
                }
            }
            justStarted = false;
        }

        if (stones[closestStone] != null)
        {
            if (WalkTo(stones[closestStone].transform.position))
            {
                StoneAI stonescript = stones[closestStone].GetComponent<StoneAI>();
                if (stonescript.RemoveHP(damage * Time.deltaTime))
                {
                    taskDone = true;
                    stone += 1;
                }
            }
        }
        else
        {
            taskDone = true;
        }
    }

    private void killCow()
    {
        CowAI[] cows = FindObjectsOfType<CowAI>();
        if (cows.Length <= 0)
        {
            taskDone = true;
            return;
        }
        float closestDist = 99999;
        for (int i = 0; i < cows.Length; i++)
        {
            float distToCow = Vector3.Distance(transform.position, cows[i].transform.position);
            if (distToCow < closestDist)
            {
                closestCow = cows[i];
                closestDist = distToCow;
            }
        }

        if (closestCow != null)
        {
            if (WalkTo(closestCow.transform.position))
            {
                closestCow.killCow();
                food++;
                taskDone = true;
            }
        }
        else
        {
            taskDone = true;
        }
    }

    private void addResources()
    {
        if (WalkTo(stammeparent.transform.position))
        {
            stammeparentscript.AddResources(food, wood, stone);
            food = 0;
            wood = 0;
            stone = 0;
            taskDone = true;
        }
    }

    private void searchotherstamme()
    {
        otherman = GameObject.FindGameObjectsWithTag("Man");
        enemyman = new GameObject[otherman.Length];

        for (int i = 0; i < otherman.Length; i++)
        {
            if (otherman[i] != null)
            {
                int layerMask = 1 << 8;
                layerMask = ~layerMask;
                if (otherman[i].transform.parent.gameObject != transform.parent.gameObject)
                {
                    enemyman[i] = otherman[i];

                    if (enemyman[i] != null)
                    {
                        fwd = transform.position - enemyman[i].transform.position;

                        if (Physics.Raycast(transform.position + Vector3.up * 3, fwd, out hit, fwd.magnitude, layerMask))
                        {
                            if (hit.collider.gameObject.tag != "Mesh" && fwd.magnitude < 600)
                            {
                                warMode = true;
                            }
                        }
                    }
                }
            }
        }
    }


    private void gotowarnomercy()
    {
        otherman = GameObject.FindGameObjectsWithTag("Man");
        OldDistToEnemy = float.MaxValue;
        friendlyman = new GameObject[otherman.Length];
        manaiscript = new ManAi[otherman.Length];
        friendlymanpos = new Vector3[otherman.Length];

        for (int i = 0; i < otherman.Length; i++)
        {
            if (otherman[i].transform.parent.gameObject == transform.parent.gameObject)
            {
                friendlyman[i] = otherman[i];
                manaiscript[i] = friendlyman[i].GetComponent<ManAi>();
                friendlymanpos[i] = otherman[i].transform.position;
                float distToFriend = (friendlymanpos[i] - this.transform.position).magnitude;
                if (distToFriend < 600)
                {
                    if (manaiscript[i] != null)
                    {
                        manaiscript[i].setwarmode(true);
                    }
                }
            }
        }

        searchotherstamme();
        if (enemyman.Length > 0 && enemyman != null)
        {
            for (int i = 0; i < enemyman.Length; i++)
            {
                if (enemyman[i] != null)
                {
                    DistToEnemy = (this.transform.position - enemyman[i].transform.position).magnitude;
                    if (DistToEnemy < OldDistToEnemy)
                    {
                        closestenemy = i;
                        closestenemypos = enemyman[closestenemy].transform.position;
                        OldDistToEnemy = DistToEnemy;
                    }
                }
            }

            if (enemyman[closestenemy] != null)
            {
                if (WalkTo(closestenemypos) && DistToEnemy < 8000)
                {
                    enemyscript = enemyman[closestenemy].GetComponent<ManAi>();

                    if (enemyscript.Hurt(damage * Time.deltaTime) == true)
                    {
                        taskDone = true;
                    }
                }
            }
            else
            {
                taskDone = true;
            }
        }
    }

    private void killself()
    {
        stammeparentscript.RemoveMan(this);
        Destroy(this.gameObject);
    }

    public Boolean Hurt(float damage)
    {
        health -= damage;


        if (health <= 0)
        {
            killself();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddHealth(int amount)
    {
        health += amount;
    }

    public void AddDamage(int amount)
    {
        damage += amount;
    }

    private bool WalkTo(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        if (Vector3.Magnitude(dir) < 10)
        {
            return true;
        }
        /*
        else if (
            GameManager.getTerrain(
                transform.position.x + dir.x / dir.magnitude * moveSpeed * Time.deltaTime,
                transform.position.z + dir.z / dir.magnitude * moveSpeed * Time.deltaTime
            ) == "Water"
        )
        {
        }
        */
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
