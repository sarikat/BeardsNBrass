using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nkg;
using UnityEngine.Profiling;

public class NavMeshManager : MonoBehaviour
{
    static NavMeshManager main;
    NodeMap NavMap;

    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(this);
        }
        if (main == null)
        {
            main = this;
        }
        NavMap = GetComponent<NodeMap>();
    }


    public static List<Vector3> NavBetween(Vector3 start_pos, Vector3 end_pos)
    {
        return NavBetween(start_pos, end_pos, -1);
    }

    public static List<Vector3> NavBetween(Vector3 start_pos, Vector3 end_pos, int search_depth)
    {
        var path = new List<Vector3>();

        Vector2 start = main.NavMap.GetNearestNode(VUtils.Vec3ToVec2(start_pos));
        Vector2 end = main.NavMap.GetNearestNode(VUtils.Vec3ToVec2(end_pos));

        if (start == NodeMap.p_null || end == NodeMap.p_null)
        {
            Debug.Log("Out of Boundaries");
            return path;
        }

        if (start.Equals(end))
        {
            path.Add(VUtils.Vec2ToVec3(end));
            return path;
        }

        var open = new PriorityQueue<AStarNode>(AStarNodeComparator);

        // This hash function is arbitrary. However it must always return nonnegative values.
        var closed = new Dictionary<Vector2, AStarNode>();
        Vector2[] successors;

        open.Add(new AStarNode { coords = start, f = 0F, g = 0F });
        AStarNode finalNode = null;
        int searched_nodes = 0;

        while (open.Count > 0 && finalNode == null)
        {
            searched_nodes++;
            AStarNode cur = open.Pop();
            Vector2 curCoords = cur.coords;

            // Generate successors and set parents to cur
            successors = main.NavMap.GetNeighbors(curCoords);

            // For each successor:
            foreach (Vector2 s in successors)
            {
                // If goal, stop

                // Debug.DrawLine(
                //     VUtils.Vec2ToVec3(s),
                //     VUtils.Vec2ToVec3(curCoords),
                //     Color.magenta, 
                //     0, 
                //     false
                // );
                AStarNode next = new AStarNode { coords = s, parent = cur, g = cur.g + Vector2.Distance(s, curCoords) };
                if (next.coords.Equals(end))
                {
                    finalNode = next;
                    break;
                }
                if (Vector2.Distance(next.coords, end) < 0.3f)
                {
                    Debug.Log(next.coords);
                }

                next.h = Vector2.Distance(next.coords, end);
                next.f = next.g + next.h;

                // If the tile is already in open, only keep the lower f
                Profiler.BeginSample("open find");
                (int existingIndex, AStarNode existingOpen) = open.FindIndex(next);
                Profiler.EndSample();

                if (existingIndex >= 0)
                {
                    if (next.f < existingOpen.f)
                    {
                        open.ChangeKey(existingIndex, next);
                    }
                    continue;
                }

                // If noxel is already in closed with a lower f, skip
                if (closed.ContainsKey(next.coords))
                {
                    AStarNode c = closed[next.coords];
                    if (next.f < c.f)
                    {
                        closed.Remove(next.coords);
                    }
                    else
                    {
                        continue;
                    }
                }

                // Add node to the open list
                open.Add(next);
            }

            closed.Add(cur.coords, cur);
            if (search_depth > 0 && searched_nodes >= search_depth)
            {
                break;
            }
        }

        if (finalNode == null)
        {
            // Path not found
            Debug.Log("No path found");
            // Debug.Log("Nodes searched: " + searched_nodes.ToString());
            return path;
        }

        // Debug.Log("PATH FOUND!");

        // Walk back along the path starting from final and store it in the return list
        AStarNode node = finalNode;
        while (!node.coords.Equals(start))
        {
            path.Add(VUtils.Vec2ToVec3(node.coords));
            node = node.parent;
        }

        path.Reverse();
        // Debug.Log("Nodes searched: " + searched_nodes.ToString());
        return path;
    }

    public static void DrawPath(List<Vector3> path)
    {
        if (path.Count > 0)
        {
            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            a.transform.position = path[0];
            a.transform.localScale = Vector3.one * 0.2f;
            GameObject b = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            b.transform.position = path[path.Count - 1];
            b.transform.localScale = Vector3.one * 0.2f;
            Destroy(a, 0.1f);
            Destroy(b, 0.1f);
            for (int i = 0; i < path.Count - 1; ++i)
            {
                Debug.DrawLine(
                    path[i],
                    path[i + 1],
                    Color.green,
                    0,
                    false
                );
            }
        }
    }

    private static int AStarNodeComparator(AStarNode l, AStarNode r)
    {
        float lf = l.f;
        float rf = r.f;
        if (lf < rf) return 1;
        if (lf > rf) return -1;
        // Use g as tiebreaker.
        // If f is the same, g is more reliable because g is true, h is a guess
        float lg = l.g;
        float rg = r.g;
        if (lg > rg) return 1;
        if (lg < rg) return -1;
        return 0;
    }

    public class AStarNode : System.IEquatable<AStarNode>
    {
        public Vector2 coords;
        public AStarNode parent;
        public float g; // cost so far
        public float h; // heuristic estimated remaining cost
        public float f; // = g + h, estimated total cost

        public bool Equals(AStarNode node)
        {
            return coords == node.coords;
        }

        public override bool Equals(object obj)
        {
            return Equals((AStarNode)obj);
        }

        public override int GetHashCode()
        {
            return (int)((coords.x + 1000) + (coords.y + 1000) * 10000);
        }
    }



}
