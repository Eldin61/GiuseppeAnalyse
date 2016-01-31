using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.Xna.Framework;

namespace EntryPoint

{
    public class Node
    {
        public Vector2 root { get; set; }
        public Node left { get; set; }
        public Node right { get; set; }
    }

    class KDTree
    {
        public List<Vector2> PotentialBuildingsInRange;

        public KDTree()
        {
            PotentialBuildingsInRange = new List<Vector2>();
        }


        public Node MakeTree(List<Vector2> points, int depth)
        {

            if (points.Count == 0)
            {
                return null;
            }

            Vector2 middle;
            var left = new List<Vector2>();
            var right = new List<Vector2>();

            if (depth % 2 == 0)
            {
                middle = SortByCoordinate(points, p => p.X, left, right);
            }
            else
            {
                middle = SortByCoordinate(points, p => p.Y, left, right);
            }


            return new Node
            {
                root = middle,
                left = MakeTree(left, depth + 1),
                right = MakeTree(right, depth + 1)
            };
        }

        private static Vector2 SortByCoordinate(List<Vector2> points, Func<Vector2, float> coordinate, List<Vector2> left, List<Vector2> right)
        {
            points = points.OrderBy(coordinate).ToList();
            var middle = points[points.Count / 2];
            points.Remove(middle);
            foreach (var item in points)
            {
                if (coordinate(item) <= coordinate(middle))
                {
                    left.Add(item);
                }
                else if (coordinate(item) > coordinate(middle))
                {
                    right.Add(item);
                }
            }
            return middle;
        }

        public void RangeSearch(Vector2 house, Node rootNode, float maxDistance, int depth, Func<Vector2, float>[] coordinates)
        {
            if (rootNode == null)
            {
                return;
            }
            var rootVector = coordinates[depth % 2](rootNode.root);
            var houseVector = coordinates[depth % 2](house);

            if (rootVector < (houseVector - maxDistance))
            {
                RangeSearch(house, rootNode.right, maxDistance, depth + 1, coordinates);
            }
            else if (rootVector > (houseVector + maxDistance))
            {
                RangeSearch(house, rootNode.left, maxDistance, depth + 1, coordinates);
            }
            else if ((rootVector >= (houseVector - maxDistance)) && (rootVector <= (houseVector + maxDistance)))
            {
                PotentialBuildingsInRange.Add(rootNode.root);
                RangeSearch(house, rootNode.left, maxDistance, depth + 1, coordinates);
                RangeSearch(house, rootNode.right, maxDistance, depth + 1, coordinates);
            }
        }
    }
}
