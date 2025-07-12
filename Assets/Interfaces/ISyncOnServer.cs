using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISyncOnServer
{
    bool CheckAuthority(); // Who can sync?
    void ForceSync(); // Force syncing even when this object is not the owner
    void StartSyncing();
    void StopSyncing();
    bool IsSyncing();
}
