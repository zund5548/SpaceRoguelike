using System.Collections.Generic;
using UnityEngine;

namespace Maps
{
    [CreateAssetMenu(menuName = "Database/StageEncountDatabase")]
    public class StageEncountDatabase : ScriptableObject
    {
        [SerializeReference,SubclassSelector]
        public List<StageEncount> stageEncountList = new();
    }
}

