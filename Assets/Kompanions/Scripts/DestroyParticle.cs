using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kompanions
{
    public class DestroyParticle : MonoBehaviour
    {
        // Start is called before the first frame update
        IEnumerator Start()
        {
            while(true)
            {
                yield return new WaitForSeconds(2);
                if (transform.parent == null)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}