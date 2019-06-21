using UnityEngine;

public class BehaviourAgent : MonoBehaviour
{
    [SerializeField] private float _updateInterval = 1;
    [SerializeField] private CompositeNode _startNode;

    private BaseNode.state st = BaseNode.state.succes;

    //BehaviourAgent(CompositeNode startnode, float updateInterval = 1)
    //{
    //    _startNode = startnode;
    //    _updateInterval = updateInterval;
    //}

    private void Update()
    {
        if (Time.frameCount % _updateInterval == 0) {
            if (st == BaseNode.state.running)
            {
                st = _startNode.childStatus = _startNode.runningNode.Tick();
            }
            else
            {
                st = _startNode.Tick();
            }
        }

        //Debug.Log("The current status at the end of this tick is: " + st.ToString());
    }
}
