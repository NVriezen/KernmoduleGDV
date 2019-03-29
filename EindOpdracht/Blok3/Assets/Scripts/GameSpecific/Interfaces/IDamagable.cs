using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    bool ReceiveDamage(float damagePower);
    void OnDeath();
    float GetHealth();
    bool RecoverHealth();
}
