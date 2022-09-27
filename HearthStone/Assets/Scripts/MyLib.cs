using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyLib
{
    public class Exception
    {
        #region[배열 범위초과 검사]
        public static bool IndexOutRange<T>(int x, int y, T[,] array)
        {
            if (x >= array.GetLength(0) || x < 0 || y >= array.GetLength(1) || y < 0)
                return false;
            return true;
        }

        public static bool IndexOutRange<T>(Vector2Int v, T[,] array)
        {
            return IndexOutRange<T>(v.x, v.y, array);
        }

        public static bool IndexOutRange<T>(int a, List<T> array)
        {
            if (array == null || a >= array.Count || a < 0)
                return false;
            return true;
        }

        public static bool IndexOutRange<T>(int a, T[] array)
        {
            if (a >= array.GetLength(0) || a < 0)
                return false;
            return true;
        }
        #endregion
    }

    public class AreaCheck
    {
        #region[범위 내부인지 검사]
        public static bool RectIn(Vector2 pos,Rect rect)
        {
            if (rect.x > pos.x || rect.x + rect.width < pos.x || rect.y < pos.y || rect.y - rect.height > pos.y)
                return false;
            return true;
        }

        public static bool RectIn(Vector2 pos, RectInt rect)
        {
            return RectIn(pos, new Rect(rect.x, rect.y, rect.width, rect.height));
        }
        #endregion
    }

    public class Algorithm
    {
        #region[Next_Permutation]
        public static bool Next_Permutation<T>(List<T> list) where T : IComparable
        {
            Action<int, int> Swap = (int idx1, int idx2) => { T temp = list[idx1]; list[idx1] = list[idx2]; list[idx2] = temp; };
            int a = 0, b = 0, p = 0; //p : pivot
            for (int i = list.Count - 2; i >= 0; --i)
                if (list[i].CompareTo(list[i + 1]) < 0)
                {
                    a = i;
                    p = i + 1;

                    for (int j = list.Count - 1; j >= 0; --j)
                        if (list[a].CompareTo(list[j]) < 0)
                        {
                            b = j;
                            break;
                        }

                    Swap(a, b);

                    for (int j = 0; j < (list.Count - p) / 2; j++)
                        Swap(j + p, list.Count - j - 1);

                    return true;
                }
            // 이미 순서대로 정렬되어 있음 => 역순으로 뒤집
            for (int i = 0; i < list.Count / 2; i++)
                Swap(i, list.Count - i - 1);
            return false;
        }
        #endregion

        #region[Swap]
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        #endregion

        #region[Shuffle]
        public static void Shuffle<T>(ref List<T> list)
        {
            //list에 있는 데이터를 섞는다.
            //랜덤하게 인데스(a,b) 두개를 정하고
            //해당 인덱스에 해당하는 값을 교환한다.
            //해당 과정을 list길이*10번 만큼 반복한다.
            for (int i = 0; i < list.Count * 10; i++)
            {
                int a = UnityEngine.Random.Range(0, list.Count);
                int b = UnityEngine.Random.Range(0, list.Count);

                T temp = list[a];
                list[a] = list[b];
                list[b] = temp;
            }
        }
        #endregion

        #region[CreateRandomList]
        public static List<int> CreateRandomList(int n, int m)
        {
            //1~n에서 겹치지 않는 m개의 수를 가져온다.
            int[] tree = new int[n + 1];
            List<int> temp = new List<int>();

            int Sum(int i)
            {
                int ans = 0;
                while (i > 0)
                {
                    ans += tree[i];
                    i -= (i & -i);
                }
                return ans;
            }

            void Update(int i, int num)
            {
                while (i <= n)
                {
                    tree[i] += num;
                    i += (i & -i);
                }
            }

            for (int i = 1; i <= n; i++)
                Update(i, 1);

            for (int i = n; i > n - m; i--)
            {
                int rand = UnityEngine.Random.Range(0, i);

                int left = 1;
                int right = i;
                while (left < right)
                {
                    int mid = (left + right) / 2;
                    if (Sum(mid) >= rand)
                        right = mid;
                    else
                        left = mid + 1;
                }
                temp.Add(right);
                Update(right, -1);
            }

            return temp;
        }
        #endregion
    }
}