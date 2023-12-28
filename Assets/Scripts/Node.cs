using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    private List<Node> childrenNodeList;

    public List<Node> ChildrenNodeList { get => childrenNodeList; }

    public bool Visted { get; set; }

    public Vector2Int BottomLeftAreaCorner { get; set; }

    public Vector2Int BottomRightAreaCorner { get; set; }

    public Vector2Int TopRightAreaCorner { get; set; }

    public Vector2Int TopLeftAreaCorner { get; set; }

    public int TreeLayerIndex { get; set; }

    public Node Parent { get; set; }

    public Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        this.Parent = parentNode;
        //parentNode?.AddChild(this);
        if(parentNode != null) // 최상위 노드인지를 검사, 아니면 실행
        {
            parentNode.AddChild(this);
        }

    }

    public void AddChild(Node node)
    {
        childrenNodeList.Add(node);
    }

    public void RemoveChild(Node node)
    {
        childrenNodeList.Remove(node);
    }
}