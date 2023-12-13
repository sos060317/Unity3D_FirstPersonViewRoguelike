using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorNode : Node
{
    private Node structure1;
    private Node structure2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePositionOfStructure2 = CheckPositionStructure2AgainstStructure1();
        switch (relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomInRelationUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomInRelationUpOrDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomInRelationRightOrLeft(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomInRelationRightOrLeft(this.structure2, this.structure1);
                break;
        }
    }

    private void ProcessRoomInRelationRightOrLeft(Node structure1, Node structure2)
    {
        Node leftStructure = null;
        List<Node> leftStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeafes(structure1);
        Node rightStructure = null;
        List<Node> rightStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeafes(structure2);

        var sortedLeftStructure = leftStructureChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();
        if (sortedLeftStructure.Count == 1)
        {
            leftStructure = sortedLeftStructure[0];
        }
        else
        {
            int maxX = sortedLeftStructure[0].TopRightAreaCorner.x;
            sortedLeftStructure = sortedLeftStructure.Where(children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftStructure.Count);
            leftStructure = sortedLeftStructure[index];
        }

        var possibleNeighboursInRightStructureList = rightStructureChildren.Where(
            child => GetValidYForNeighourLeftRight(
                leftStructure.TopRightAreaCorner,
                leftStructure.BottomRightAreaCorner,
                child.TopLeftAreaCorner,
                child.BottomLeftAreaCorner
                ) != -1
            ).ToList();
        if (possibleNeighboursInRightStructureList.Count <= 0)
        {
            rightStructure = structure2;
        }
        else
        {
            rightStructure = possibleNeighboursInRightStructureList[0];
        }
        int y = GetValidYForNeighourLeftRight(leftStructure.TopLeftAreaCorner, leftStructure.BottomRightAreaCorner,
            rightStructure.TopLeftAreaCorner,
            rightStructure.BottomRightAreaCorner);
    }

    private int GetValidYForNeighourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall)
                ).y;
        }
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        return -1;
    }

    private void ProcessRoomInRelationUpOrDown(Node structure1, Node structure2)
    {
        throw new NotImplementedException();
    }

    private RelativePosition CheckPositionStructure2AgainstStructure1()
    {
        Vector2 middlePointStructure1Temp = ((Vector2)structure1.TopRightAreaCorner + structure1.BottomLeftAreaCorner) / 2;
        Vector2 middlePointStructure2Temp = ((Vector2)structure2.TopRightAreaCorner + structure2.BottomLeftAreaCorner) / 2;
        float angle = CalculateAngle(middlePointStructure1Temp, middlePointStructure2Temp);
        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if (angle > 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if (angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 middlePointStructure1Temp, Vector2 middlePointStructure2Temp)
    {
        return Mathf.Atan2(middlePointStructure2Temp.y - middlePointStructure1Temp.y,
            middlePointStructure2Temp.x - middlePointStructure1Temp.x) * Mathf.Rad2Deg;
    }
}