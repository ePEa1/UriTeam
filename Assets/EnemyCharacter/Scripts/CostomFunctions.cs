using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ePEaCostomFunction
{
    public static class CostomFunctions
    {
        //최대값
        public static float Max(float n1, float n2)
        {
            if (n1 > n2)
                return n1;
            else return n2;
        }

        //최소값
        public static float Min(float n1, float n2)
        {
            if (n1 > n2)
                return n2;
            else return n1;
        }
    }
}

