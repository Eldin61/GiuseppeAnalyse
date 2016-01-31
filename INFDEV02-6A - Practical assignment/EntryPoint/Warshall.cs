using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EntryPoint
{
    class Warshall
    {
        public List<Vector2> roadToBuilding = new List<Vector2>();

        public int[,] CreateAdjMatrix(Dictionary<Vector2, int> adjList2, List<Tuple<Vector2, Vector2>> roads)
        {
            var adj = new int[adjList2.Count, adjList2.Count];

            foreach (var road in roads)
            {
                adj[adjList2[road.Item1], adjList2[road.Item2]] = (int)Vector2.Distance(road.Item1, road.Item2);
            }
            for (var u = 0; u < adj.GetLength(0); u++) // Vertices
            {
                for (var v = 0; v < adj.GetLength(0); v++) //Edge
                {
                    if (adj[u, v] == 0 && u != v)
                    {
                        adj[u, v] = 5000000; //Infinity
                    }
                }
            }
            return adj;
        }

        private int[,] MatrixOfPredecessors(int[,] matrix)
        {
            var loopCount = matrix.GetLength(0);
            var next = new int[loopCount, loopCount];

            for (var u = 0; u < loopCount; u++) // Vertices
            {
                for (var v = 0; v < loopCount; v++) //Edge
                {
                    if (matrix[u, v] != 5000000 && u != v)
                    {
                        next[u, v] = u;
                    }
                    else
                    {
                        next[u, v] = -1;
                    }
                }
            }
            return next;
        }


    }
}

