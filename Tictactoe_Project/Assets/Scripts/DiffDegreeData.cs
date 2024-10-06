using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffDegreeData 
{
    public int diffDegree=0;
   private static DiffDegreeData instance=new DiffDegreeData();
   private DiffDegreeData(){}
   public static DiffDegreeData Instance=>instance;
}
