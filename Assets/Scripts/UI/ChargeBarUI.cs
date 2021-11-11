using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ChargeBarUI : MonoBehaviour
{
   private float maxRad;
   [SerializeField]
   private Disc chargeBar;

   private void Start()
   {
      maxRad = chargeBar.AngRadiansEnd;
      chargeBar.AngRadiansEnd = 0;
   }

   public void OnChargeUpdate(float charge)
   {
      chargeBar.AngRadiansEnd = Mathf.Lerp(0, maxRad, charge);
   }
}
