using System.Collections.Generic;
using UnityEngine;

namespace Maps
{
    [CreateAssetMenu(menuName = "Scriptable Objects/StageEncountDatabase")]
    public class StageEncountDatabase : ScriptableObject
    {
        [SerializeReference,SubclassSelector]
        public List<StageEncount> stageEncountList = new();
    }
}

