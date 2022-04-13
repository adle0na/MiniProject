using System.Collections;
using _Test.LDG.Script;
using UnityEngine;

namespace TestLDG
{
    public class TestPlayer : MonoBehaviour,IAttackAble
    {
        /*public Vector3 P1;
        public Vector3 P2;

        private void Start()
        {
            StartCoroutine(TestPlayerMove());
        }

        IEnumerator TestPlayerMove()
        {
            while (true)
            {
                while (transform.position != P2)
                {
                    transform.position = Vector3.Lerp(transform.position, P2, .5f * Time.fixedDeltaTime);
                    yield return new WaitForFixedUpdate();
                    if(Vector3.Distance(transform.position,P2)<1)
                        break;
                }
                yield return new WaitForSecondsRealtime(2);
                
                while (transform.position !=  P1)
                {
                    transform.position = Vector3.Lerp(transform.position, P1, .5f * Time.fixedDeltaTime);
                    yield return new WaitForFixedUpdate();
                    if(Vector3.Distance(transform.position,P1)<1)
                        break;
                }
                yield return new WaitForSecondsRealtime(2);

                
            }

        }*/

        public void TakeDamage(int damage)
        {
            Debug.Log($"{damage}만큼 아프다");
        }
    }
    
}
