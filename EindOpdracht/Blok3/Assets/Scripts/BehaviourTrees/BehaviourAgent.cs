using UnityEngine;

public class BehaviourAgent : MonoBehaviour
{
    [SerializeField] private float updateInterval = 1;
    [SerializeField] private CompositeNode startNode;

    private BaseNode.state st = BaseNode.state.succes;
    
    private void Update()
    {
        if (Time.frameCount % updateInterval == 0) {
            if (st == BaseNode.state.running)
            {
                st = startNode.childStatus = startNode.runningNode.Tick();
            }
            else
            {
                st = startNode.Tick();
            }
        }

        //Debug.Log("The current status at the end of this tick is: " + st.ToString());
    }
}
