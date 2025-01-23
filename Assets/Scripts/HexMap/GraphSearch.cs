using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is the BFS algorithm used to find the movement cost based movement range, as well as constructing the path.
/// </summary>
public class GraphSearch
{
    public static BfsResult BfsGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        Dictionary<Vector3Int, Vector3Int?> visited = new(); // <node, parent>
        Dictionary<Vector3Int, int> costSoFar = new(); // <node, cost>
        Queue<Vector3Int> frontier = new();
        
        frontier.Enqueue(startPoint);
        costSoFar.Add(startPoint, 0);
        visited.Add(startPoint, null);

        while (frontier.Count > 0)
        {
            Vector3Int currentNode = frontier.Dequeue();
            foreach (var neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;

                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;

                if (newCost > movementPoints) continue;
                
                if (visited.TryAdd(neighbourPosition, currentNode))
                {
                    costSoFar[neighbourPosition] = newCost;
                    frontier.Enqueue(neighbourPosition);
                }
                else if (costSoFar[neighbourPosition] > newCost)
                {
                    costSoFar[neighbourPosition] = newCost;
                    visited[neighbourPosition] = currentNode;
                }
            }
        }

        return new BfsResult
        {
            Visited = visited,
            StartPoint = startPoint
        };
    }
    
    public struct BfsResult
    {
        public Dictionary<Vector3Int, Vector3Int?> Visited;
        public Vector3Int StartPoint;

        public List<Vector3Int> GetPath(Vector3Int destination)
        {
            if (Visited.ContainsKey(destination) == false)
                return new List<Vector3Int>();
            return GeneratePathBfs(destination, Visited);
        }
    
        public bool IsHexPositionInRange(Vector3Int hexPosition)
        {
            return Visited.ContainsKey(hexPosition);
        }
    
        public IEnumerable<Vector3Int> GetHexPositionsInRange()
            => Visited.Keys;
    }

    public static List<Vector3Int> GeneratePathBfs(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> visited)
    {
        List<Vector3Int> path = new();
        path.Add(current);
        while (visited[current] != null)
        {
            path.Add(visited[current].Value);
            current = visited[current].Value;
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
}


