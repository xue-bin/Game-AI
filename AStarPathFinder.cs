using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AStarPathFinder : GreedyPathFinder
{
    public static int nodesOpened = 0;

    //ASSIGNMENT 2: EDIT BELOW THIS LINE, IMPLEMENT A*
    public override Vector3[] CalculatePath(GraphNode startNode, GraphNode goalNode)
    {
        nodesOpened = 0;

        AStarNode start = new AStarNode(null, startNode, Heuristic(startNode, goalNode));
        float gScore = 0;

        PriorityQueue<AStarNode> openSet = new PriorityQueue<AStarNode>();
        List<GraphNode> closedSet = new List<GraphNode>();

        openSet.Enqueue(start);
        Dictionary<string, float> gScores = new Dictionary<string, float>();

        int attempts = 0;
        while (openSet.Count() > 0 && attempts < 10000)
        {
            attempts += 1;
            AStarNode currNode = openSet.Dequeue();

            //Did we find the goal?
            if (currNode.Location == goalNode.Location)
            {
                Debug.Log("CHECKED " + nodesOpened + " NODES");//Don't delete this line
                //Reconstruct the path?
                return ReconstructPath(start, currNode);
            }
            closedSet.Add(currNode.GetGraphNode());
            //Check each neighbor
            foreach (GraphNode neighbor in currNode.GraphNode.Neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }
                gScore = currNode.GetGScore() + ObstacleHandler.Instance.GridSize;
                int temp = 0;
                for (int i = 0; i < openSet.Count(); i += 1)
                {
                    if (openSet.Data[i].GetGraphNode().Location == neighbor.Location)
                    {
                        temp = 1;
                    }
                }

                if (temp == 0)
                {
                    AStarNode aStarNeighbor = new AStarNode(currNode, neighbor, Heuristic(neighbor, goalNode));
                    openSet.Enqueue(aStarNeighbor);
                    gScores[neighbor.Location.ToString()] = gScore;
                }
                else if (gScore < gScores[neighbor.Location.ToString()])
                {
                    
                    for (int i = 0; i < openSet.Count(); i += 1)
                    {
                        if (openSet.Data[i].GetGraphNode().Location == neighbor.Location)
                        {
                            AStarNode tempNode;
                            tempNode = openSet.Data[i];
                            openSet.Remove(tempNode);
                        }
                    }
                    
                    AStarNode aStarNeighbor = new AStarNode(currNode, neighbor, Heuristic(neighbor, goalNode));
                    openSet.Enqueue(aStarNeighbor);
                    gScores[neighbor.Location.ToString()] = gScore;
                }
                    
                
            }
        }
        Debug.Log("CHECKED "+ nodesOpened + " NODES");//Don't delete this line
        return null;
    }
    //ASSIGNMENT 2: EDIT ABOVE THIS LINE, IMPLEMENT A*

    //EXTRA CREDIT ASSIGNMENT 2 EDIT BELOW THIS LINE
    public float Heuristic(GraphNode currNode, GraphNode goalNode)
    {
        return (Mathf.Abs(currNode.Location.x - goalNode.Location.x) + Mathf.Abs(currNode.Location.y - goalNode.Location.y));
        //1
        //float dx = currNode.Location.x - goalNode.Location.x;
        //float dy = currNode.Location.y - goalNode.Location.y;
        //return (Math.Sqrt((dx * dx + dy * dy)));
        //2
        //float dx = Math.Abs(currNode.Location.x - goalNode.Location.x);
        //float dy = Math.Abs(currNode.Location.y - goalNode.Location.y);
        //float diag = Math.Min(dx, dy);
        //float straight = dx + dy;
        //return (diag + (straight - 2 * diag));

        //EXTRA CREDIT ASSIGNMENT 2 EDIT ABOVE THIS LINE
    }
    //Code for reconstructing the path, don't edit this.
    private Vector3[] ReconstructPath(AStarNode startNode, AStarNode currNode)
    {
        List<Vector3> backwardsPath = new List<Vector3>();

        while (currNode != startNode)
        {
            backwardsPath.Add(currNode.Location);
            currNode = currNode.Parent;
        }
        backwardsPath.Reverse();

        return backwardsPath.ToArray();
    }
}



