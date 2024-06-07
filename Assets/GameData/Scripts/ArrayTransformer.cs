using System;
using UnityEngine;

namespace PCTC.Scripts
{
    public static class ArrayTransformer
    {
        public static T[] Flatten<T>(T[,] arr)
        {
            int rows0 = arr.GetLength(0);
            int rows1 = arr.GetLength(1);
            T[] arrFlattened = new T[rows0 * rows1];
            for (int j = 0; j < rows1; j++)
            {
                for (int i = 0; i < rows0; i++)
                {
                    var test = arr[i, j];
                    arrFlattened[i + j * rows0] = arr[i, j];
                }
            }
            return arrFlattened;
        }

        public static T[,] Expand<T>(T[] arr, int rows0 = 8)
        {
            int length = arr.GetLength(0);
            int rows1 = length / rows0;
            T[,] arrExpanded = new T[rows0, rows1];
            for (int j = 0; j < rows1; j++)
            {
                for (int i = 0; i < rows0; i++)
                {
                    arrExpanded[i, j] = arr[i + j * rows0];
                }
            }
            return arrExpanded;
        }
    }
}
