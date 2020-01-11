using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class NavNode
{
    public int connections;
    public Vector2 n = NodeMap.p_null;
    public Vector2 ne = NodeMap.p_null;
    public Vector2 e = NodeMap.p_null;
    public Vector2 se = NodeMap.p_null;
    public Vector2 s = NodeMap.p_null;
    public Vector2 sw = NodeMap.p_null;
    public Vector2 w = NodeMap.p_null;
    public Vector2 nw = NodeMap.p_null;
}

public class NodeMap : MonoBehaviour
{
    // Ground MUST:
    // - be a rectangle
    // - contain the entire map (make sure there are ground tiles under wall tiles)
    public GameObject environment;
    private int Height, Width;
    Vector3 mapCorner;
    public int Granularity;
    public float Leeway;
    public bool DrawDebugMap = true;

    Dictionary<Vector2, NavNode> NodeDict = new Dictionary<Vector2, NavNode>();
    Vector2[] raw_directions = new Vector2[] {
        Vector2.up,
        Vector2.up + Vector2.right,
        Vector2.right,
        Vector2.right + Vector2.down,
        Vector2.down,
        Vector2.down + Vector2.left,
        Vector2.left,
        Vector2.left + Vector2.up
    };

    public static Vector2 p_null = new Vector2(float.MinValue, float.MinValue);
    Vector2 ToCenterOffset;


    private void Awake()
    {
        if (environment == null)
        {
            Debug.LogError("NodeMap is missing a reference to the environment.");
        }
        CalcMapBounds();
        ToCenterOffset = new Vector2(mapCorner.x, mapCorner.y);
        Build();

        // DrawMap();
        // DrawNode(100);
    }

    private void CalcMapBounds()
    {
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity;
        float maxY = Mathf.NegativeInfinity;
        foreach (Transform child in environment.transform)
        {
            var tilemap = child.GetComponent<Tilemap>();
            if (tilemap == null)
            {
                continue;
            }

            var min = tilemap.cellBounds.min;
            var max = tilemap.cellBounds.max;

            if (min.x < minX)
            {
                minX = min.x;
            }
            if (min.y < minY)
            {
                minY = min.y;
            }
            if (max.x > maxX)
            {
                maxX = max.x;
            }
            if (max.y > maxY)
            {
                maxY = max.y;
            }
        }
        Height = (int)(maxY - minY);
        Width = (int)(maxX - minX);
        mapCorner = new Vector3(minX, minY, 0);
    }

    private void Update()
    {
        if (DrawDebugMap)
        {
            // DrawMap();
        }
    }

    public Vector2 GetNearestNode(Vector2 pos)
    {
        Vector2 nearest_grid_point = new Vector2(
            Mathf.Round(pos.x * (Granularity * 2)) / (Granularity * 2),
            Mathf.Round(pos.y * (Granularity * 2)) / (Granularity * 2)
        );

        if (NodeDict.ContainsKey(nearest_grid_point))
        {
            return nearest_grid_point;
        }

        Vector2[] search_dirs = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

        foreach (Vector2 dir in search_dirs)
        {
            if (NodeDict.ContainsKey(nearest_grid_point + dir * (1f / (2 * Granularity))))
            {
                return nearest_grid_point + dir * (1f / (2 * Granularity));
            }
        }
        return p_null;
    }

    public Vector2[] GetNeighbors(Vector2 pos)
    {
        List<Vector2> neighbors = new List<Vector2>();
        if (NodeDict.ContainsKey(pos))
        {
            NavNode n = NodeDict[pos];
            for (int i = 0; i < 8; ++i)
            {
                if (GetDir(n, i) != p_null)
                {
                    neighbors.Add(GetDir(n, i));
                }
            }
        }

        return neighbors.ToArray();
    }

    public void Build()
    {
        GetBaseNodes();
        MakeConnections();
        TrimDeadends();
    }

    public void DrawMap()
    {
        Color c = Color.blue;
        foreach (KeyValuePair<Vector2, NavNode> entry in NodeDict)
        {
            for (int i = 0; i < 8; ++i)
            {
                if (GetDir(entry.Value, i) != p_null)
                {
                    Debug.DrawLine(
                        VUtils.Vec2ToVec3(entry.Key),
                        VUtils.Vec2ToVec3(GetDir(entry.Value, i)),
                        c,
                        0,
                        false
                    );
                }
            }
        }
    }

    public void DrawNode(int freq)
    {
        Color c = Color.blue;
        int j = 0;
        foreach (KeyValuePair<Vector2, NavNode> entry in NodeDict)
        {
            if (j % freq == 0)
            {
                Vector3 pos = VUtils.Vec2ToVec3(entry.Key);
                Debug.DrawLine(pos, pos + Vector3.right * 0.1f, c, 20, false);
                // for (int i = 0; i < 8; ++i)
                // {
                //     if (GetDir(entry.Value, i) != p_null)
                //     {
                //         Debug.DrawLine(
                //             VUtils.Vec2ToVec3(entry.Key),
                //             VUtils.Vec2ToVec3(GetDir(entry.Value, i)),
                //             c,
                //             10,
                //             false
                //         );
                //     }
                // }
            }
            ++j;
        }
    }

    void TrimDeadends()
    {
        var deadends = new List<Vector2>();
        foreach (KeyValuePair<Vector2, NavNode> entry in NodeDict)
        {
            if (entry.Value.connections < 3)
            {
                deadends.Add(entry.Key);
            }
        }

        foreach (var key in deadends)
        {
            RemoveNode(key);
        }
    }

    void RemoveNode(Vector2 node_key)
    {
        NavNode node = NodeDict[node_key];
        for (int i = 0; i < 8; ++i)
        {
            // If a node has a connection in a given direction
            if (GetDir(node, i) != p_null)
            {
                //Get that neighbor, and remove this node from it...
                Debug.Log(GetDir(node, i));
                GetDir(NodeDict[GetDir(node, i)], i + 4) = p_null;
                NodeDict[GetDir(node, i)].connections--;
                //Then remove the connection in this node itself
                GetDir(node, i) = p_null;
                node.connections--;
            }
        }
        NodeDict.Remove(node_key);
    }

    void GetBaseNodes()
    {
        for (float x = 0; x < Width; x += 1f / Granularity)
        {
            for (float y = 0, j = 0; y < Height; y += 1f / (2 * Granularity), j++)
            {
                float offset = 0;
                if (j % 2 == 0)
                {
                    offset = 1f / (2 * Granularity);
                }

                Vector2 pos = new Vector2(transform.position.x + x + offset, transform.position.y + y);
                pos += ToCenterOffset;
                if (Physics2D.OverlapCircle(pos, Leeway, LayerMask.GetMask("Nav")))
                {
                    if (!Physics2D.OverlapCircle(pos, Leeway, LayerMask.GetMask("Impassables", "Hazards")))
                    {
                        NodeDict.Add(pos, new NavNode());
                    }
                }
            }
        }

        DrawNode(1);
    }

    void MakeConnections()
    {
        foreach (KeyValuePair<Vector2, NavNode> entry in NodeDict)
        {
            for (int i = 0; i < 8; ++i)
            {
                if (GetDir(entry.Value, i) == p_null)
                {
                    Vector2 offset = raw_directions[i];
                    offset *= i % 2 == 1 ? (1f / (2 * Granularity)) : (1f / Granularity);
                    Vector2 look_up = entry.Key + offset;
                    if (NodeDict.ContainsKey(look_up))
                    {
                        GetDir(NodeDict[look_up], i + 4) = entry.Key;
                        NodeDict[look_up].connections++;
                        GetDir(entry.Value, i) = look_up;
                        entry.Value.connections++;
                    }
                }
            }
        }
    }

    ref Vector2 GetDir(NavNode node, int direction)
    {
        direction %= 8;
        if (direction == 0)
            return ref node.n;
        if (direction == 1)
            return ref node.ne;
        if (direction == 2)
            return ref node.e;
        if (direction == 3)
            return ref node.se;
        if (direction == 4)
            return ref node.s;
        if (direction == 5)
            return ref node.sw;
        if (direction == 6)
            return ref node.w;
        return ref node.nw;
    }

    void PrintNode(NavNode node)
    {
        Debug.Log("----------------");
        for (int i = 0; i < 8; ++i)
        {
            Debug.Log(GetDir(node, i));
        }
        Debug.Log("----------------");
    }

    /// <summary>
    /// Returns a tuple of the lower-left corner and the upper-right corner of the environment.
    /// Use like: (Vector3 minCorner, Vector3 maxCorner) = nodeMap.GetMapBounds();
    /// 
    /// Make sure to call AFTER Start()
    /// </summary>
    /// <returns></returns>
    public (Vector3, Vector3) GetMapBounds()
    {
        float maxX = mapCorner.x + Width;
        float maxY = mapCorner.y + Height;
        return (mapCorner, new Vector3(maxX, maxY, 0));
    }

    public Func<NavMeshManager.AStarNode, bool> CreateComparer(Vector2 other)
    {
        return (NavMeshManager.AStarNode t) => CoordsEqual(t.coords, other);
    }

    public bool CoordsEqual(Vector2 one, Vector2 two)
    {
        float hashedOne = one.x - mapCorner.x + (one.y - mapCorner.y) * Width;
        float hashedTwo = two.x - mapCorner.x + (two.y - mapCorner.y) * Width;
        return Mathf.Approximately(hashedOne, hashedTwo);
    }
}
