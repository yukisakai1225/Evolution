using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class ListExtention{
  public static T LoopElementAt<T>(this IList<T> list, int index)
  {
      if (list.Count == 0) throw new ArgumentException("要素数が0のためアクセスできません");

      // 分配
      index %= list.Count;

      // 正の値にずらす
      if (index < 0)
      {
          index += list.Count;
      }

      T target = list[index];

      return target;
  }
}
