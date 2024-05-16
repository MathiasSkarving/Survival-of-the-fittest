using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static MapGenerator mapGenerator;
    public SpawnManager spawnManager;
    private TreeGenerator treeGenerator;
    private StoneGenerator stoneGenerator;
    public float timeScale = 1;

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        treeGenerator = FindObjectOfType<TreeGenerator>();
        stoneGenerator = FindObjectOfType<StoneGenerator>();
        int seed = Mathf.RoundToInt(UnityEngine.Random.value * 100000);
        mapGenerator.GenerateMap(seed);
        treeGenerator.GenerateTrees(seed);
        stoneGenerator.Generatestones(seed);
        spawnManager.Initialize();
    }

    void Update()
    {
        Time.timeScale = timeScale;
    }

    public static float getHeight(float x, float y)
    {
        Vector3[] vertices = mapGenerator.meshData.vertices; //får verticies fra mappet
        int meshSimplificationIncrement = (int)(vertices[1].x - vertices[0].x); //finder afstanden mellem verticies
        float xDiff = x / 10 - vertices[0].x; //finder afstanden fra øverste venstre hjørne i x
        float yDiff = vertices[0].z - y / 10; //finder afstanden fra øverste venstre hjørne i y
        float xOffset = xDiff % meshSimplificationIncrement; //finder afstanden fra punkts verticy som er oppe og til venstre
        float yOffset = yDiff % meshSimplificationIncrement; //finder det samme i y retningen
        int lastValidX = Mathf.RoundToInt((xDiff - xOffset) / meshSimplificationIncrement); //finder den sidste valid verteci i x retning
        int lastValidY = Mathf.RoundToInt((yDiff - yOffset) / meshSimplificationIncrement); // finder det samme for y
        float lastHeight = vertices[lastValidX + lastValidY * (1 + 240 / meshSimplificationIncrement)].y; //finder højden på denne vertecy

        float nextXHeight = 0; //laver variabler til næste højder
        float nextYHeight = 0;

        if (x >= 1200 && y <= -1200)
        {
            return lastHeight; //hvis det er den sidste vertecy skal højden i punktet være last height
        }
        else if (x >= 1200)
        {
            nextYHeight = vertices[lastValidX + (lastValidY + 1) * (1 + 240 / meshSimplificationIncrement)].y; // hvis den er på x kanten findes den næste som højden under
            return Mathf.Lerp(lastHeight, nextYHeight, yOffset / meshSimplificationIncrement); //eftersom linjen mellem 2 vertecies er linær kan vi lerp
        }
        else if (y <= -1200)
        {
            //det samme som før bare i y retningen
            nextXHeight = vertices[lastValidX + 1 + lastValidY * (1 + 240 / meshSimplificationIncrement)].y;
            return Mathf.Lerp(lastHeight, nextXHeight, xOffset / meshSimplificationIncrement);
        }


        nextXHeight = vertices[lastValidX + 1 + lastValidY * (1 + 240 / meshSimplificationIncrement)].y; //forsøger at sætte den næste højde til den næste vertecy
        nextYHeight = vertices[lastValidX + (lastValidY + 1) * (1 + 240 / meshSimplificationIncrement)].y; //forsøger at sætte den næste højde i y retningen



        float nextDiagonalheight = vertices[lastValidX + 1 + (lastValidY + 1) * (1 + 240 / meshSimplificationIncrement)].y; //finder den næste diagonale højde
        Vector3 r1 = new(meshSimplificationIncrement * 10, nextDiagonalheight - lastHeight, meshSimplificationIncrement * 10);//laver den ene retningsvektor
        Vector3 r2; //laver den anden retningsvektor
        if (xOffset > yOffset) //tjekker om den er i den øverste eller nederste trekant
        {
            r2 = new(meshSimplificationIncrement * 10, nextXHeight - lastHeight, 0); //sætter den anden retningsvektor
        }
        else
        {
            r2 = new(0, nextYHeight - lastHeight, meshSimplificationIncrement * 10); //sætter den anden retingsvektor
        }
        Vector3 normal = Vector3.Cross(r1, r2); //finder normalvektoren til planen
        float height = -(normal.x * xOffset * 10 + normal.z * yOffset * 10) / normal.y + lastHeight; // finder højden ud fra denne plan


        return height; //returnere denne højde
    }


    public static bool validSpawn(float posX, float posY)
    {
        while (posX <= 1200 && posX >= -1200 && posY >= -1200 && posY <= 1200)
        {
            return true;
        }
        return false;
    }


    public static String getTerrain(float x, float z)
    {
        float[,] noiseMap = mapGenerator.getNoiseMap();
        float height = noiseMap[(int)x / 10 + 120, -(int)z / 10 + 120];
        for (int i = 0; i < mapGenerator.regions.Length; i++)
        {
            if (height <= mapGenerator.regions[i].height)
            {
                return mapGenerator.regions[i].name;
            }
        }
        return "getTerrain fuuucked up";
    }
    public static Vector3 getNormal(float x, float y)
    {
        Vector3[] vertices = mapGenerator.meshData.vertices;
        int meshSimplificationIncrement = (int)(vertices[1].x - vertices[0].x);
        float xDiff = x / 10 - vertices[0].x;
        float yDiff = vertices[0].z - y / 10;
        float xOffset = xDiff % meshSimplificationIncrement;
        float yOffset = yDiff % meshSimplificationIncrement;
        int lastValidX = Mathf.RoundToInt((xDiff - xOffset) / meshSimplificationIncrement);
        int lastValidY = Mathf.RoundToInt((yDiff - yOffset) / meshSimplificationIncrement);
        float lastHeight = vertices[lastValidX + lastValidY * (1 + 240 / meshSimplificationIncrement)].y;

        float nextXHeight = 0;
        float nextYHeight = 0;

        try
        {
            nextXHeight = vertices[lastValidX + 1 + lastValidY * (1 + 240 / meshSimplificationIncrement)].y;
            nextYHeight = vertices[lastValidX + (lastValidY + 1) * (1 + 240 / meshSimplificationIncrement)].y;
        }
        catch (Exception e)
        {
            if (x == 1200 && y == -1200)
            {
                return Vector3.up;
            }
            else if (x == 1200)
            {
                return Vector3.up;
            }
            else if (y == -1200)
            {
                return Vector3.up;
            }
            else
            {
                Debug.Log("something is fuuuucked");
                Debug.Log(e.Message);
            }

        }


        float nextDiagonalheight = vertices[lastValidX + 1 + (lastValidY + 1) * (1 + 240 / meshSimplificationIncrement)].y;
        Vector3 r1 = new(meshSimplificationIncrement * 10, nextDiagonalheight - lastHeight, meshSimplificationIncrement * 10);
        Vector3 r2;
        if (xOffset > yOffset)
        {
            r2 = new(meshSimplificationIncrement * 10, nextXHeight - lastHeight, 0);
        }
        else
        {
            r2 = new(0, nextYHeight - lastHeight, meshSimplificationIncrement * 10);
        }
        Vector3 normal = Vector3.Cross(r1, r2);


        return normal;
    }

    public static float random4Interval(float min1, float max1, float min2, float max2)
    {
        float randomValue;
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            randomValue = UnityEngine.Random.Range(min1, max1);
        }
        else
        {
            randomValue = UnityEngine.Random.Range(min2, max2);
        }
        return randomValue;
    }
}
