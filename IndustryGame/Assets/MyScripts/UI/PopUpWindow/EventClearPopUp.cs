using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EventClearPopUp : MonoBehaviour
{
    public class Data : IPopUpWindow
    {
        private readonly MainEvent mainEvent;
        public Data(MainEvent mainEvent)
        {
            this.mainEvent = mainEvent;
        }
        public void Generate()
        {
            GameObject window = Instantiate(Resources.Load<GameObject>("UI/PopUpWindow/EventClearPopUp"));
            EventClearPopUp script = window.GetComponent<EventClearPopUp>();
            script.eventFinishDescriptionText.text = mainEvent.descriptionAfterFinish;
            script.baseRewardText.text = mainEvent.contribution.ToString();
            script.wildReservedText.text = mainEvent.WildReservated.ToString();
            script.manMadeEnvReservedText.text = mainEvent.MamMadeEnvReservated.ToString();
            script.totalReward.text = mainEvent.TotalReward.ToString();
        }
    }
    public Image eventFinishImage;
    public Text eventFinishDescriptionText;
    public Text baseRewardText;
    public Text wildReservedText;
    public Text manMadeEnvReservedText;
    public Text totalReward;
    public void CloseWindow()
    {
        Destroy(gameObject);
        PopUpCanvas.SetWindowExists(false);
        PopUpCanvas.ShowPopUpWindowStack();
    }
}
