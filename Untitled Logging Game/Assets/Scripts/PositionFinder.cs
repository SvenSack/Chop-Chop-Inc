using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PositionFinder
{
    public abstract Vector3 GetObjectPosition();
}


public class FindPositionThroughRandomNumberGeneration : PositionFinder
{
    private static int maxIteration = 8;

    int lengthCount;
    int widthCount;
    float TreePerTerrainWidth;
    float TreePerTerrainLength;


    Vector3 position;

    GameObject[] objectArray;
    //int treeInWidthCount;
    //int treeInLengthCount;
    int currentI;

    float minimumDistance;

    DebugDrawerInterface debugDrawerInterface;

    public FindPositionThroughRandomNumberGeneration(Vector3 position, int lengthCount, int widthCount, float TreePerTerrainWidth
        , float TreePerTerrainLength, GameObject[] objectArray, int currentObjCount, float minimumDistance)
    {

        this.position = position;

        this.lengthCount = lengthCount;
        this.widthCount = widthCount;



        this.objectArray = objectArray;
        this.TreePerTerrainWidth = TreePerTerrainWidth;
        this.TreePerTerrainLength =TreePerTerrainLength;
        this.currentI = currentObjCount;
        this.minimumDistance = minimumDistance;
    }

    public override Vector3 GetObjectPosition()
    {
        bool foundValidPosition = true;
        int i = 0;

        do
        {
            Debug.Log("iterating");
            foundValidPosition = true;

            Vector3 generatedRandomPosition = GetRandomPositionInSquare();

            //get object directly on the left
            int objectIndexOnLeft = currentI - 1;
            if (currentI - 1 >= 0)
            {

                Debug.Log("objectArray[objectIndexOnLeft] != null " + objectArray[objectIndexOnLeft] != null);
                if (objectArray[objectIndexOnLeft] != null &&
                    !isGivenPositionFarEnough(generatedRandomPosition, objectArray[objectIndexOnLeft].transform.position))
                {
                    foundValidPosition = false;
                }
            }

            //get object one row before
            int objectIndexOneRowBefore = currentI - lengthCount;
            if (objectIndexOneRowBefore >= 0)
            {
                if (objectArray[objectIndexOneRowBefore] != null &&
                    !isGivenPositionFarEnough(generatedRandomPosition, objectArray[objectIndexOneRowBefore].transform.position))
                {
                    foundValidPosition = false;
                }
            }

            //get object one row before-1
            int objectIndexOneRowBeforeMinusOne = currentI - lengthCount - 1;
            if (objectIndexOneRowBeforeMinusOne >= 0)
            {
                if (objectArray[objectIndexOneRowBeforeMinusOne] != null &&
                    !isGivenPositionFarEnough(generatedRandomPosition, objectArray[objectIndexOneRowBeforeMinusOne].transform.position))
                {
                    foundValidPosition = false;
                }
            }

            int objectIndexOneRowBeforePlusOne = currentI - lengthCount + 1;
            if (objectIndexOneRowBeforePlusOne >= 0 && objectIndexOneRowBeforePlusOne < objectArray.Length)
            {
                if (objectArray[objectIndexOneRowBeforePlusOne] != null &&
                    !isGivenPositionFarEnough(generatedRandomPosition, objectArray[objectIndexOneRowBeforePlusOne].transform.position))
                {
                    foundValidPosition = false;
                }
            }

            i++;

            if (foundValidPosition)
            {
                Debug.Log("valid Position found");
                return generatedRandomPosition;
            }
        }
        while (!foundValidPosition && i < maxIteration);




        return GetRandomPositionInSquare();
    }


    private Vector3 GetRandomPositionInSquare()
    {
        Vector3 finalPosition = position;

        float widthAddition = Random.Range(0, TreePerTerrainWidth);
        float lengthAddition = Random.Range(0, TreePerTerrainLength);

        Vector3 right = Vector3.right * widthAddition;
        Vector3 forward = Vector3.forward * lengthAddition;

        finalPosition += right + forward;




        return finalPosition;
    }


    private bool isGivenPositionFarEnough(Vector3 position, Vector3 objectPosition)
    {

        return Vector3.Distance(objectPosition, position) > minimumDistance;
    }
}
