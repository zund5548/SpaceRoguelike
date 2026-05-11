using UnityEngine;
using TMPro;
using Maps;
public class StageNodeBanner : MonoBehaviour
{
    public TextMeshProUGUI _stageNameDisplay;
    public TextMeshProUGUI _stageTypeDisplay;
    public TextMeshProUGUI _stageDescriptionDisplay;
    public void SetBanner(StageNode stageNode)
    {
        _stageNameDisplay.text = stageNode.stageName;
        _stageTypeDisplay.text = stageNode.GetStageTypeStr();   
    }
}
