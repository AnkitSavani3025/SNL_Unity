using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNL_3PLUS_MGP {
    public class ParticalEffectMatirialLoader : MonoBehaviour {
        Renderer myParticalsRendarer;
        Material myMatirial;

        private void Start(){
            myParticalsRendarer = GetComponent<Renderer>();
            myMatirial = myParticalsRendarer.material;
            myMatirial =
                MGPSDK.AllResourceContainer.instance.GetParticleMatirial(
                    myParticalsRendarer.material.name.Split(' ')[0]);
            if (myMatirial == null){
                Debug.Log("SNL || ParticalEffectMatirialLoader || Particle Material Not Found ==> " +
                          myParticalsRendarer.material.name.Split(' ')[0]);
            }

            myParticalsRendarer.material = myMatirial;
            myParticalsRendarer.material.shader = Shader.Find(myParticalsRendarer.material.shader.name);
        }
    }
}