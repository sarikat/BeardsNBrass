// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Battleaxe : Weapon
// {
//     float startAngle, destAngle;

//     BoxCollider2D box2d;
//     // Start is called before the first frame update
//     void Start()
//     {
//         box2d = GetComponent<BoxCollider2D>();
//         box2d.enabled = false;

//         transform.localPosition = new Vector3(0, 1, 0);
//         destAngle = startAngle = 0f;
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

//     public override void PrivateBeginAttack(Vector2 dir)
//     {
//         destAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
//         box2d.enabled = true;

//         StartCoroutine(AttackCoroutine());
//     }

//     public override void PrivateContinueAttack()
//     {

//     }

//     public override void PrivateFinishAttack()
//     {

//     }

//     IEnumerator AttackCoroutine()
//     {
//         float deltaRot = destAngle - startAngle;
//         while (!Mathf.Approximately(deltaRot, 0))
//         {
//             yield return null;
//             float increment = Mathf.Min(120f * TimeManager.deltaTime, Mathf.Abs(deltaRot));
//             increment *= Mathf.Sign(deltaRot);
//             deltaRot -= increment;

//             transform.RotateAround(transform.parent.position, -Vector3.forward, increment);
//         }
//         yield return null;
//         startAngle = destAngle;
//         box2d.enabled = false;
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (box2d.enabled && collision.tag == "Enemy")
//         {
//             // Debug.Log("HeavyHit");
//             collision.GetComponent<Health>()?.TakeDamage(25);
//         }
//     }
// }
