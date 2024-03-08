using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    [Header("던전 설정")]
    public int dungeonWidth;  // 던전 너비
    public int dungeonLength; // 던전 길이
    public int roomWidthMin;  // 던전 룸 최소 너비
    public int roomLengthMin; // 던전 룸 최소 길이
    public int maxIterations; // 던전을 자를 반복 횟수(BSP알고리즘 반복 횟수)
    public int corridorWidth; // 던전 룸을 이어주는 통로의 너비

    [Header("던전 속성")]
    public Material material; // 던전 구성 재질

    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerModifier;
    [Range(0, 2)]
    public int roomOffset;

    public GameObject wallVertical, wallHorizontal;
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPositon;
    List<Vector3Int> possibleWallHorizontalPositon;
    List<Vector3Int> possibleWallVerticalPosition;

    private void Start()
    {
        //던전 생성 함수
        CreateDungeon();
    }

    // 던전 생성 함수
    public void CreateDungeon()
    {
        DestroyAllChildren();
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        
        // 던전 계산기(던전 쪼개기, 룸만들기)
        var listOfRooms = generator.CalculateDungeon(maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset,
            corridorWidth);

        // 벽 정보 초기화
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPositon = new List<Vector3Int>();
        possibleWallHorizontalPositon = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        // 던전 바닥 생성
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }

        // 던전 벽(여러개)생성
        CreateWalls(wallParent);
    }

    // 던전 벽(여러개)생성
    private void CreateWalls(GameObject wallParent)
    {
        // 가로 벽
        foreach(var wallPosition in possibleWallHorizontalPositon)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }

        // 세로 벽
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical); 
        }
    }

    // 벽(한개) 생성
    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
    }

    // 바닥 메쉬 생성
    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV,
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor 
            = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;

        BoxCollider boxCollider = dungeonFloor.GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(topRightCorner.x - bottomLeftCorner.x, 0.1f, topRightCorner.y - bottomLeftCorner.y);
        boxCollider.center = new Vector3((topRightCorner.x + bottomLeftCorner.x) / 2, 0.05f, (topRightCorner.y + bottomLeftCorner.y) / 2);

        dungeonFloor.transform.parent = transform;

        for(int row = (int)bottomLeftV.x; row <= (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPositon, possibleDoorHorizontalPositon);
        }
        for (int row = (int)topLeftV.x; row <= (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPositon, possibleDoorHorizontalPositon);
        }
        for(int col = (int)bottomLeftV.z; col<= (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col <= (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}
