using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [System.Serializable]
    public class BodyPart
    {
        public SkinnedMeshRenderer meshRenderer;
    }

    public class CharacterHook : MonoBehaviour
    {
        public BodyPart body;

        public void Init(ClothItem clothItem)
        {
            body.meshRenderer.sharedMesh = clothItem.mesh;
            body.meshRenderer.material = clothItem.material;
        }
    }
}

