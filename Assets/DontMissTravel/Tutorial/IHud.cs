using DontMissTravel.Data;

namespace DontMissTravel.Tutorial
{
    public interface IHud
    {
        void ChangeGateState(GateState gateState, string text);
        void SetGateWillOpenTime(float time);
        void SetGateWillClose(float inTime);
        void PrepareTutorialInfo();
    }
}