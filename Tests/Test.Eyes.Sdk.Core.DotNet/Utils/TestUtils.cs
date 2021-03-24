using System.Collections.Generic;

namespace Applitools.Tests.Utils
{
    internal partial class TestUtils
    {
        public static List<object[]> GeneratePermutationsList(List<List<object>> lists)
        {
            List<object[]> result = new List<object[]>();
            GeneratePermutations_(lists, result, 0, null);
            return result;
        }

        public static object[][] GeneratePermutations(List<List<object>> lists)
        {
            List<object[]> result = GeneratePermutationsList(lists);
            return result.ToArray();
        }

        public static object[][] GeneratePermutations(params List<object>[] lists)
        {
            return GeneratePermutations(new List<List<object>>(lists));
        }

        public static object[][] GeneratePermutations(params object[][] arrays)
        {
            List<object> lists = new List<object>();
            foreach (object[] array in arrays)
            {
                lists.Add(new List<object>(array));
            }
            return GeneratePermutations(lists);
        }


        private static void GeneratePermutations_(List<List<object>> lists, List<object[]> result, int depth, List<object> permutation)
        {
            if (depth == lists.Count)
            {
                if (permutation != null)
                {
                    result.Add(permutation.ToArray());
                }
                return;
            }

            List<object> listInCurrentDepth = lists[depth];
            foreach (object newItem in listInCurrentDepth)
            {
                if (permutation == null || depth == 0)
                {
                    permutation = new List<object>();
                }

                permutation.Add(newItem);
                GeneratePermutations_(lists, result, depth + 1, permutation);
                permutation.Remove(permutation.Count - 1);
            }
        }
    }
}
