using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    [Header("던전 설정")]
    public int dungeonWidth;  //던전 너비
    public int dungeonLength; //던전 길이
    public int roomWidthMin;  //던전 룸 최소 너비
    public int roomLengthMin; //던전 룸 최소 길이
    public int maxIterations; //던전을 자를 반복 횟수(BSP알고리즘 반복 횟수)
    public int corridorWidth; //던전 룸을 이어주는 통로의 너비

    [Header("던전 재질")]
    public Material material;

    [Range(-10.0f, 10.3f)]
    public float roomBottomCornerModifier;
    [Range(-10.7f, 10.0f)]
    public float roomTopCornerModifier;
    [Range(0, 2)]
    public int roomOffset;

    private void Start()
    {
        //던전 생성 함수
        CreateDungeon();
    }
    /// <summary>
    /// 던전 생성 함수
    /// </summary>
    private void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateDungeon(maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset,
            corridorWidth);
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
    }

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

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
    }
}
