using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntryPoint
{
#if WINDOWS || LINUX
  public static class Program
  {
    [STAThread]
    static void Main()
    {
      var fullscreen = false;
      read_input:
      switch (Microsoft.VisualBasic.Interaction.InputBox("Which assignment shall run next? (1, 2, 3, 4, or q for quit)", "Choose assignment", VirtualCity.GetInitialValue()))
      {
        case "1":
          using (var game = VirtualCity.RunAssignment1(SortSpecialBuildingsByDistance, fullscreen))
            game.Run();
          break;
        case "2":
          using (var game = VirtualCity.RunAssignment2(FindSpecialBuildingsWithinDistanceFromHouse, fullscreen))
            game.Run();
          break;
        case "3":
          using (var game = VirtualCity.RunAssignment3(FindRoute, fullscreen))
            game.Run();
          break;
        case "4":
          using (var game = VirtualCity.RunAssignment4(FindRoutesToAll, fullscreen))
            game.Run();
          break;
        case "q":
          return;
      }
      goto read_input;
    }

        public static float[] MergeSort(float[] list)
        {
            // If list size is 0 (empty) or 1, consider it sorted and return it
            // (using less than or equal prevents infinite recursion for a zero length array).
            if (list.Length <= 1)
            {
                return list;
            }

            // Else list size is > 1, so split the list into two sublists.
            int middleIndex = (list.Length) / 2;
            float[] left = new float[middleIndex];
            float[] right = new float[list.Length - middleIndex];

            Array.Copy(list, left, middleIndex);
            Array.Copy(list, middleIndex, right, 0, right.Length);

            // Recursively call MergeSort() to further split each sublist
            // until sublist size is 1.
            left = MergeSort(left);
            right = MergeSort(right);

            // Merge the sublists returned from prior calls to MergeSort()
            // and return the resulting merged sublist.
            return Merge(left, right);
        }

        public static float[] Merge(float[] left, float[] right)
        {
            // Convert the input arrays to lists, which gives more flexibility 
            // and the option to resize the arrays dynamically.
            List<float> leftList = left.OfType<float>().ToList();
            List<float> rightList = right.OfType<float>().ToList();
            List<float> resultList = new List<float>();

            // While the sublist are not empty merge them repeatedly to produce new sublists 
            // until there is only 1 sublist remaining. This will be the sorted list.
            while (leftList.Count > 0 || rightList.Count > 0)
            {
                if (leftList.Count > 0 && rightList.Count > 0)
                {
                    // Compare the 2 lists, append the smaller element to the result list
                    // and remove it from the original list.
                    if (leftList[0] <= rightList[0])
                    {
                        resultList.Add(leftList[0]);
                        leftList.RemoveAt(0);
                    }

                    else
                    {
                        resultList.Add(rightList[0]);
                        rightList.RemoveAt(0);
                    }
                }

                else if (leftList.Count > 0)
                {
                    resultList.Add(leftList[0]);
                    leftList.RemoveAt(0);
                }

                else if (rightList.Count > 0)
                {
                    resultList.Add(rightList[0]);
                    rightList.RemoveAt(0);
                }
            }

            // Convert the resulting list back to a static array
            float[] result = resultList.ToArray();
            return result;
        }

        private static IEnumerable<Vector2> SortSpecialBuildingsByDistance(Vector2 house, IEnumerable<Vector2> specialBuildings)
        {

            int lengthlist = specialBuildings.ToArray().Length;
            float[] list = new float[lengthlist];

            for (int i = 0; i < lengthlist; i++)
            {
                list[i] = Vector2.Distance(house, specialBuildings.ToArray()[i]);
            }

            list = MergeSort(list);
            // Print the sorted list
            for (int index = 0; index < list.Length; index++)
            {
                Console.WriteLine("element[{0}] = {1}", index, list[index]);
            }

            return specialBuildings.OrderBy(v => Vector2.Distance(v, house));
        }

        private static IEnumerable<IEnumerable<Vector2>> FindSpecialBuildingsWithinDistanceFromHouse(
      IEnumerable<Vector2> specialBuildings, 
      IEnumerable<Tuple<Vector2, float>> housesAndDistances)
    {
            var specialBuildingsInRange = new List<List<Vector2>>();
            var buildingList = housesAndDistances.ToList();

            var a = new KDTree();
            var b = a.MakeTree(specialBuildings.ToList(), 0);

            foreach (var building in buildingList)
            {
                var specialBuildingsInBox = new List<Vector2>();

                a.RangeSearch(building.Item1, b, building.Item2, 0, new Func<Vector2, float>[] { p => p.X, p => p.Y });

                specialBuildingsInBox.AddRange(
                    from x in a.PotentialBuildingsInRange
                    where Vector2.Distance(building.Item1, x) <= building.Item2
                    select x);

                specialBuildingsInRange.Add(specialBuildingsInBox);
            }

            Console.WriteLine(specialBuildingsInRange.Count);
            return specialBuildingsInRange;

        }

    private static IEnumerable<Tuple<Vector2, Vector2>> FindRoute(Vector2 startingBuilding, 
      Vector2 destinationBuilding, IEnumerable<Tuple<Vector2, Vector2>> roads)
    {
      var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
      List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
      var prevRoad = startingRoad;
      for (int i = 0; i < 30; i++)
      {
        prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, destinationBuilding)).First());
        fakeBestPath.Add(prevRoad);
      }
      return fakeBestPath;
    }

    private static IEnumerable<IEnumerable<Tuple<Vector2, Vector2>>> FindRoutesToAll(Vector2 startingBuilding, 
      IEnumerable<Vector2> destinationBuildings, IEnumerable<Tuple<Vector2, Vector2>> roads)
    {
      List<List<Tuple<Vector2, Vector2>>> result = new List<List<Tuple<Vector2, Vector2>>>();
      foreach (var d in destinationBuildings)
      {
        var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
        List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
        var prevRoad = startingRoad;
        for (int i = 0; i < 30; i++)
        {
          prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, d)).First());
          fakeBestPath.Add(prevRoad);
        }
        result.Add(fakeBestPath);
      }
      return result;
    }
  }
#endif
}
